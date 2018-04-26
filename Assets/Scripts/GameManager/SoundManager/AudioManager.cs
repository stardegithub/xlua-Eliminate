using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameManager
{
    public class AudioManager : GameManagerBase<AudioManager>
    {
        private const string VOLUMEKEY = "GameAudioVolume";
        private float audioVolume;
        public float AudioVolume
        {
            get
            {
                return audioVolume;
            }
            set
            {
                if (value != audioVolume && value >= 0 && value <= 1)
                {
                    audioVolume = value;
                    PlayerPrefs.SetFloat(VOLUMEKEY, value);
                    for (int i = 0; i < audioChannels.Count; i++)
                    {
                        audioChannels[i].AudioSource.volume = audioVolume;
                    }
                }
            }
        }

        private AudioListener audioListener;
        public AudioListener AudioListener { get { return audioListener; } }

        private List<AudioChannel> audioChannels;
        public List<AudioChannel> AudioChannels { get { return audioChannels; } }

        private Dictionary<string, AudioClip> audioClipCachePool;
        public Dictionary<string, AudioClip> AudioClipCachePool { get { return audioClipCachePool; } }

        #region Singleton
        protected override void SingletonAwake()
        {
            audioVolume = PlayerPrefs.GetFloat(VOLUMEKEY, 1);

            CreateAudioListener();
            CreateAudioChannels(10);
            audioClipCachePool = new Dictionary<string, AudioClip>();
            initialized = true;
        }

        protected override void SingletonDestroy()
        {
            StopAllAudio();
            ClearCachePool();
        }
        #endregion

        private void CreateAudioListener()
        {
            GameObject go = new GameObject("AudioListener");
            go.transform.SetParent(transform, false);
            audioListener = go.AddComponent<AudioListener>();
        }

        public void CreateAudioChannels(int channelCount, bool is3D = false)
        {
            if (audioChannels == null)
            {
                audioChannels = new List<AudioChannel>();
                GameObject go = new GameObject("AudioChannel");
                go.transform.SetParent(transform, false);
            }

            GameObject channelObject = transform.Find("AudioChannel").gameObject;
            for (int i = 0; i < channelCount; i++)
            {
                if (i >= audioChannels.Count)
                {
                    audioChannels.Add(new AudioChannel(channelObject.AddComponent<AudioSource>()));
                }
                audioChannels[i].AudioSource.playOnAwake = false;
                audioChannels[i].AudioSource.volume = audioVolume;
                audioChannels[i].AudioSource.spatialBlend = is3D ? 1 : 0;
            }
            for (int i = channelCount; i < audioChannels.Count; i++)
            {
                audioChannels[i].Destroy();
                audioChannels.RemoveAt(i);
            }
        }

        public void PushAudioCache(string audioPath, Action<bool, string, AudioClip> callBack)
        {
            StartCoroutine(AssetBundles.DataLoader.LoadAsync<AudioClip>(audioPath, (success, path, assetBundleType, audioClip) =>
            {
                if (success)
                {
                    audioClipCachePool.Add(path, audioClip);
                }
                callBack(success, path, audioClip);
            }));
        }

        public void PushAudioCaches(string[] audioPaths, Action<bool, string, AudioClip> callBack)
        {
            foreach (var audioPath in audioPaths)
            {
                PushAudioCache(audioPath, callBack);
            }
        }

        public void PlayAudio(string audioPath)
        {
            if (string.IsNullOrEmpty(audioPath))
            {
                return;
            }
            AudioClip audioClip;
            if (!audioClipCachePool.TryGetValue(audioPath, out audioClip))
            {
                audioClip = AssetBundles.DataLoader.Load<AudioClip>(audioPath);
                if (audioClip != null)
                {
                    audioClipCachePool.Add(audioPath, audioClip);
                }
            }

            if (audioClip != null)
            {
                PlayAudio(audioClip);
            }
        }

        public void PlayAudio(AudioClip audioClip)
        {
            PlayAudio(new AudioInfo(audioClip));
        }

        public void PlayAudio(AudioInfo audioInfo)
        {
            AudioChannel audioChannel = null;
            if (audioInfo.Group > 0)
            {
                audioChannel = audioChannels.Find(c => !c.Empty && c.AudioInfo.Group == audioInfo.Group && c.AudioInfo.Priority >= audioInfo.Priority);
            }

            if (audioChannel == null)
            {
                audioChannel = audioChannels.Find(c => c.Empty);
            }

            if (audioChannel != null)
            {
                audioChannel.Play(audioInfo);
            }
        }

        public void StopAllAudio()
        {
            for (int i = 0; i < audioChannels.Count; i++)
            {
                audioChannels[i].Stop();
            }
        }

        public void ClearCachePool()
        {
            StopAllAudio();
            foreach (var element in audioClipCachePool)
            {
                Resources.UnloadAsset(element.Value);
            }
            audioClipCachePool.Clear();
        }
    }

    public class AudioChannel
    {
        protected AudioInfo audioInfo;
        public virtual AudioInfo AudioInfo { get { return audioInfo; } }

        protected AudioSource audioSource;
        public virtual AudioSource AudioSource { get { return audioSource; } }

        public virtual bool Empty { get { return !audioSource.isPlaying || audioInfo == null || !audioInfo.AudioClip; } }

        public AudioChannel(AudioSource audioSource)
        {
            this.audioSource = audioSource;
        }

        public void Play(AudioInfo audioInfo)
        {
            audioSource.Stop();
            this.audioInfo = audioInfo;
            audioSource.loop = audioInfo.Loop;
            audioSource.priority = audioInfo.Priority;
            audioSource.clip = audioInfo.AudioClip;
            audioSource.Play();
        }

        public void Stop()
        {
            audioSource.Stop();
            audioInfo = null;
        }

        public void Pause()
        {
            audioSource.Pause();
        }

        public void UnPause()
        {
            audioSource.UnPause();
        }

        public void Destroy()
        {
            Object.Destroy(audioSource);
        }
    }

    public class AudioInfo
    {
        protected bool loop;
        public virtual bool Loop { get { return loop; } }

        protected int group;
        public virtual int Group { get { return group; } }

        protected int priority;
        public virtual int Priority { get { return priority; } }

        protected AudioClip audioClip;
        public virtual AudioClip AudioClip { get { return audioClip; } }

        public AudioInfo(AudioClip audioClip)
        {
            this.loop = false;
            this.group = 0;
            this.priority = 128;
            this.audioClip = audioClip;
        }

        public AudioInfo(bool loop, int group, int priority, AudioClip audioClip)
        {
            this.loop = loop;
            this.group = group;
            this.priority = priority;
            this.audioClip = audioClip;
        }
    }
}