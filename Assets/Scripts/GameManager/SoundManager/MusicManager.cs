using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    public class MusicManager : GameManagerBase<MusicManager>
    {
        private const string VOLUMEKEY = "GameMusicVolume";
        private float musicVolume;
        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                if (value != musicVolume && value >= 0 && value <= 1)
                {
                    musicVolume = value;
                    PlayerPrefs.SetFloat(VOLUMEKEY, value);
                    for (int i = 0; i < musicChannels.Count; i++)
                    {
                        musicChannels[i].SetVolume(value);
                    }
                }
            }
        }

        private MusicChannel backgroundMusic;
        private List<MusicChannel> musicChannels;
        public List<MusicChannel> MusicChannels { get { return musicChannels; } }

        #region Singleton
        protected override void SingletonAwake()
        {
            musicVolume = PlayerPrefs.GetFloat(VOLUMEKEY, 1);

            musicChannels = new List<MusicChannel>();
            initialized = true;
        }

        protected override void SingletonDestroy()
        {
            for (int i = 0; i < musicChannels.Count; i++)
            {
                musicChannels[i].Stop();
            }
        }
        #endregion

        private void Update()
        {
            for (int i = 0; i < musicChannels.Count; i++)
            {
                if (!musicChannels[i].AudioSource)
                {
                    musicChannels.RemoveAt(i);
                    i--;
                    continue;
                }
                if (musicChannels[i].FadeInfo != null)
                {
                    if (musicChannels[i].FadeInfo.OnFade(musicChannels[i].AudioSource))
                    {
                        if (musicChannels[i].FadeInfo.FadeType == MusicFadeType.FadeOut)
                        {
                            musicChannels[i].Stop();
                            musicChannels.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public MusicChannel CreateAudioChannel()
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = musicVolume;
            audioSource.spatialBlend = 0;
            var musicChannel = new MusicChannel(audioSource);
            musicChannels.Add(musicChannel);
            return musicChannel;
        }

        public void PlayMusic(string audioPath)
        {
            AudioClip audioClip = LoadAssetManager.Instance.LoadAsset<AudioClip>(audioPath);
            if (audioClip != null)
            {
                PlayMusic(audioClip);
            }
        }

        public void PlayMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.Play(audioClip);
        }

        public void StopMusic(string audioPath)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip != null && c.AudioClip.name == audioPath);
            if (musicChannel != null) musicChannel.Stop();
        }

        public void StopMusic(AudioClip audioClip)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip == audioClip);
            if (musicChannel != null)
            {
                musicChannel.Stop();
                musicChannels.Remove(musicChannel);
            }
        }

        public void FadeInMusic(string audioPath)
        {
            AudioClip audioClip = LoadAssetManager.Instance.LoadAsset<AudioClip>(audioPath);
            if (audioClip != null)
            {
                FadeInMusic(audioClip);
            }
        }

        public void FadeInMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeIn, musicVolume);
        }

        public void FadeOutMusic(string audioPath)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip != null && c.AudioClip.name == audioPath);
            if (musicChannel != null) musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeOut, 0);
        }

        public void FadeOutMusic(AudioClip audioClip)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip == audioClip);
            if (musicChannel != null) musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeOut, 0);
        }

        public void PlayBackgroundMusic(string audioPath)
        {
            AudioClip audioClip = LoadAssetManager.Instance.LoadAsset<AudioClip>(audioPath);
            if (audioClip != null)
            {
                PlayBackgroundMusic(audioClip);
            }
        }

        public void PlayBackgroundMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.Play(audioClip);
            StopBackgroundMusic();
            backgroundMusic = musicChannel;
        }

        public void StopBackgroundMusic()
        {
            if (backgroundMusic != null)
            {
                backgroundMusic.Stop();
                musicChannels.Remove(backgroundMusic);
                backgroundMusic = null;
            }
        }

        public void FadeInBackgroundMusic(string audioPath)
        {
            AudioClip audioClip = LoadAssetManager.Instance.LoadAsset<AudioClip>(audioPath);
            if (audioClip != null)
            {
                FadeInBackgroundMusic(audioClip);
            }
        }

        public void FadeInBackgroundMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeIn, musicVolume);
            FadeOutBackgroundMusic();
            backgroundMusic = musicChannel;
        }

        public void FadeOutBackgroundMusic()
        {
            if (backgroundMusic != null)
            {
                backgroundMusic.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeOut, 0);
                backgroundMusic = null;
            }
        }
    }

    public class MusicChannel
    {
        protected AudioSource audioSource;
        public virtual AudioSource AudioSource { get { return audioSource; } }
        public virtual AudioClip AudioClip { get { return audioSource ? audioSource.clip : null; } }


        public MusicFadeInfo FadeInfo { get; set; }

        public MusicChannel(AudioSource audioSource)
        {
            audioSource.loop = true;
            this.audioSource = audioSource;
        }

        public void Play(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void Stop()
        {
            audioSource.Stop();
            audioSource.clip = null;
            Object.Destroy(audioSource);
        }

        public void Pause()
        {
            audioSource.Pause();
        }

        public void UnPause()
        {
            audioSource.UnPause();
        }

        public void SetVolume(float volume)
        {
            if (FadeInfo != null && FadeInfo.FadeType == MusicFadeType.FadeIn)
            {
                FadeInfo.FadeEndVolume = volume;
            }
            else
            {
                audioSource.volume = volume;
            }
        }

    }

    public class MusicFadeInfo
    {
        private float fadeSpeed;
        public float FadeSpeed { get { return fadeSpeed; } set { fadeSpeed = value; } }
        private float fadeEndVolume;
        public float FadeEndVolume { get { return fadeEndVolume; } set { fadeEndVolume = value; } }
        private MusicFadeType fadeType;
        public MusicFadeType FadeType { get { return fadeType; } }


        public MusicFadeInfo(MusicFadeType fadeType, float fadeEndVolume)
        {
            this.fadeEndVolume = fadeEndVolume;
            this.fadeType = fadeType;
            this.fadeSpeed = 1;
        }

        public bool OnFade(AudioSource audioSource)
        {
            if (fadeType == MusicFadeType.FadeIn)
            {
                float fadeVolume = audioSource.volume + Time.deltaTime * fadeSpeed;
                audioSource.volume = Mathf.Max(fadeVolume, fadeEndVolume);
                if (audioSource.volume <= fadeEndVolume) { return true; }
            }
            else if (fadeType == MusicFadeType.FadeOut)
            {
                float fadeVolume = audioSource.volume - Time.deltaTime * fadeSpeed;
                audioSource.volume = Mathf.Min(fadeVolume, fadeEndVolume);
                if (audioSource.volume >= fadeEndVolume) { return true; }
            }
            return false;
        }
    }

    public enum MusicFadeType
    {
        FadeIn,
        FadeOut,
    }
}