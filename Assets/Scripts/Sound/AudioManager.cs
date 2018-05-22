using System;
using System.Collections.Generic;
using UnityEngine;
using GameState;
using EC.Common;

namespace EC.Sound
{
    /// <summary>
    /// 声音管理器
    /// </summary>
    /// <typeparam name="AudioManager"></typeparam>
    public class AudioManager : GameManagerBase<AudioManager>
    {
        private const string VOLUMEKEY = "GameAudioVolume";
        private float _audioVolume;
        /// <summary>
        /// 音频音量
        /// </summary>
        /// <returns></returns>
        public float AudioVolume
        {
            get
            {
                return _audioVolume;
            }
            set
            {
                if (value != _audioVolume && value >= 0 && value <= 1)
                {
                    _audioVolume = value;
                    PlayerPrefs.SetFloat(VOLUMEKEY, value);
                    for (int i = 0; i < _audioChannels.Count; i++)
                    {
                        _audioChannels[i].AudioSource.volume = _audioVolume;
                    }
                }
            }
        }

        private AudioListener _audioListener;
        /// <summary>
        /// 音频监听器;
        /// </summary>
        /// <returns></returns>
        public AudioListener AudioListener { get { return _audioListener; } }

        private List<AudioChannel> _audioChannels;
        /// <summary>
        /// 声音频道
        /// </summary>
        /// <returns></returns>
        public List<AudioChannel> AudioChannels { get { return _audioChannels; } }

        private Dictionary<string, AudioClip> audioClipCachePool;
        /// <summary>
        /// 音频缓存池
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, AudioClip> AudioClipCachePool { get { return audioClipCachePool; } }

        #region Singleton
        protected override void SingletonAwake()
        {
            _audioVolume = PlayerPrefs.GetFloat(VOLUMEKEY, 1);

            CreateAudioListener();
            CreateAudioChannels(10);
            audioClipCachePool = new Dictionary<string, AudioClip>();
            _initialized = true;
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
            _audioListener = go.AddComponent<AudioListener>();
        }

        /// <summary>
        /// 创建声音频道
        /// </summary>
        /// <param name="channelCount"></param>
        /// <param name="is3D"></param>
        public void CreateAudioChannels(int channelCount, bool is3D = false)
        {
            if (_audioChannels == null)
            {
                _audioChannels = new List<AudioChannel>();
                GameObject go = new GameObject("AudioChannel");
                go.transform.SetParent(transform, false);
            }

            GameObject channelObject = transform.Find("AudioChannel").gameObject;
            for (int i = 0; i < channelCount; i++)
            {
                if (i >= _audioChannels.Count)
                {
                    _audioChannels.Add(new AudioChannel(channelObject.AddComponent<AudioSource>()));
                }
                _audioChannels[i].AudioSource.playOnAwake = false;
                _audioChannels[i].AudioSource.volume = _audioVolume;
                _audioChannels[i].AudioSource.spatialBlend = is3D ? 1 : 0;
            }
            for (int i = channelCount; i < _audioChannels.Count; i++)
            {
                _audioChannels[i].Destroy();
                _audioChannels.RemoveAt(i);
            }
        }

        /// <summary>
        /// 推入音频缓存
        /// </summary>
        /// <param name="audioPath"></param>
        /// <param name="callBack"></param>
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

        /// <summary>
        /// 推入多个音频缓存
        /// </summary>
        /// <param name="audioPaths"></param>
        /// <param name="callBack"></param>
        public void PushAudioCaches(string[] audioPaths, Action<bool, string, AudioClip> callBack)
        {
            foreach (var audioPath in audioPaths)
            {
                PushAudioCache(audioPath, callBack);
            }
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioPath">声音地址</param>
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

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioClip">声音片段</param>
        public void PlayAudio(AudioClip audioClip)
        {
            PlayAudio(new AudioInfo(audioClip));
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audioInfo"></param>
        public void PlayAudio(AudioInfo audioInfo)
        {
            AudioChannel audioChannel = null;
            if (audioInfo.Group > 0)
            {
                audioChannel = _audioChannels.Find(c => !c.Empty && c.AudioInfo.Group == audioInfo.Group && c.AudioInfo.Priority >= audioInfo.Priority);
            }

            if (audioChannel == null)
            {
                audioChannel = _audioChannels.Find(c => c.Empty);
            }

            if (audioChannel != null)
            {
                audioChannel.Play(audioInfo);
            }
        }

        public void StopAllAudio()
        {
            for (int i = 0; i < _audioChannels.Count; i++)
            {
                _audioChannels[i].Stop();
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

    /// <summary>
    /// 音频信道
    /// </summary>
    public class AudioChannel
    {
        protected AudioInfo _audioInfo;
        /// <summary>
        /// 声音信息
        /// </summary>
        /// <returns></returns>
        public virtual AudioInfo AudioInfo { get { return _audioInfo; } }

        protected AudioSource _audioSource;
        /// <summary>
        /// 音源
        /// </summary>
        /// <returns></returns>
        public virtual AudioSource AudioSource { get { return _audioSource; } }

        public virtual bool Empty { get { return !_audioSource.isPlaying || _audioInfo == null || !_audioInfo.AudioClip; } }

        public AudioChannel(AudioSource audioSource)
        {
            this._audioSource = audioSource;
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="audioInfo"></param>
        public void Play(AudioInfo audioInfo)
        {
            _audioSource.Stop();
            this._audioInfo = audioInfo;
            _audioSource.loop = audioInfo.Loop;
            _audioSource.priority = audioInfo.Priority;
            _audioSource.clip = audioInfo.AudioClip;
            _audioSource.Play();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            _audioSource.Stop();
            _audioInfo = null;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            _audioSource.Pause();
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        public void UnPause()
        {
            _audioSource.UnPause();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Destroy()
        {
			UnityEngine.Object.Destroy(_audioSource);
        }
    }

    /// <summary>
    /// 音频信息
    /// </summary>
    public class AudioInfo
    {
        protected bool _loop;
        /// <summary>
        /// 循环
        /// </summary>
        /// <returns></returns>
        public virtual bool Loop { get { return _loop; } }

        protected int _group;
        /// <summary>
        /// 组
        /// </summary>
        /// <returns></returns>
        public virtual int Group { get { return _group; } }

        protected int _priority;
        /// <summary>
        /// 优先级
        /// </summary>
        /// <returns></returns>
        public virtual int Priority { get { return _priority; } }

        protected AudioClip _audioClip;
        /// <summary>
        /// 音频
        /// </summary>
        /// <returns></returns>
        public virtual AudioClip AudioClip { get { return _audioClip; } }

        public AudioInfo(AudioClip audioClip)
        {
            this._loop = false;
            this._group = 0;
            this._priority = 128;
            this._audioClip = audioClip;
        }

        public AudioInfo(bool loop, int group, int priority, AudioClip audioClip)
        {
            this._loop = loop;
            this._group = group;
            this._priority = priority;
            this._audioClip = audioClip;
        }
    }
}