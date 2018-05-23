using UnityEngine;
using System.Collections.Generic;
using Common;
using GameState;
using EC.Common;

namespace EC.Sound
{
    /// <summary>
    /// 音乐管理器
    /// </summary>
    /// <typeparam name="MusicManager"></typeparam>
	public class MusicManager : ManagerBehaviourBase<MusicManager>
    {
        private const string VOLUMEKEY = "GameMusicVolume";
        private float _musicVolume;
        /// <summary>
        /// 音乐频道
        /// </summary>
        /// <returns></returns>
        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                if (value != _musicVolume && value >= 0 && value <= 1)
                {
                    _musicVolume = value;
                    PlayerPrefs.SetFloat(VOLUMEKEY, value);
                    for (int i = 0; i < _musicChannels.Count; i++)
                    {
                        _musicChannels[i].SetVolume(value);
                    }
                }
            }
        }

        private MusicChannel _backgroundMusic;
        private List<MusicChannel> _musicChannels;
        /// <summary>
        /// 音乐信道
        /// </summary>
        /// <returns></returns>
        public List<MusicChannel> MusicChannels { get { return _musicChannels; } }

        #region Singleton
        protected override void SingletonAwake()
        {
            _musicVolume = PlayerPrefs.GetFloat(VOLUMEKEY, 1);

            _musicChannels = new List<MusicChannel>();
            _initialized = true;
        }

        protected override void SingletonDestroy()
        {
            for (int i = 0; i < _musicChannels.Count; i++)
            {
                _musicChannels[i].Stop();
            }
        }
        #endregion

        private void Update()
        {
            for (int i = 0; i < _musicChannels.Count; i++)
            {
                if (!_musicChannels[i].AudioSource)
                {
                    _musicChannels.RemoveAt(i);
                    i--;
                    continue;
                }
                if (_musicChannels[i].FadeInfo != null)
                {
                    if (_musicChannels[i].FadeInfo.OnFade(_musicChannels[i].AudioSource))
                    {
                        if (_musicChannels[i].FadeInfo.FadeType == MusicFadeType.FadeOut)
                        {
                            _musicChannels[i].Stop();
                            _musicChannels.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建音乐频道
        /// </summary>
        /// <returns></returns>
        public MusicChannel CreateAudioChannel()
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = _musicVolume;
            audioSource.spatialBlend = 0;
            var musicChannel = new MusicChannel(audioSource);
            _musicChannels.Add(musicChannel);
            return musicChannel;
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void PlayMusic(string audioPath)
        {
            AudioClip audioClip = AssetBundles.DataLoader.Load<AudioClip>(audioPath);
            if (audioClip != null)
            {
                PlayMusic(audioClip);
            }
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="audioClip">音频</param>
        public void PlayMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.Play(audioClip);
        }

        /// <summary>
        /// 停止音乐
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void StopMusic(string audioPath)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip != null && c.AudioClip.name == audioPath);
            if (musicChannel != null) musicChannel.Stop();
        }

        /// <summary>
        /// 停止音乐
        /// </summary>
        /// <param name="audioClip">音频</param>
        public void StopMusic(AudioClip audioClip)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip == audioClip);
            if (musicChannel != null)
            {
                musicChannel.Stop();
                _musicChannels.Remove(musicChannel);
            }
        }

        /// <summary>
        /// 音乐渐入
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void FadeInMusic(string audioPath)
        {
            AudioClip audioClip = AssetBundles.DataLoader.Load<AudioClip>(audioPath);
            if (audioClip != null)
            {
                FadeInMusic(audioClip);
            }
        }

        /// <summary>
        /// 音乐渐入
        /// </summary>
        /// <param name="audioClip">音频</param>
        public void FadeInMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeIn, _musicVolume);
        }

        /// <summary>
        /// 音乐渐出
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void FadeOutMusic(string audioPath)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip != null && c.AudioClip.name == audioPath);
            if (musicChannel != null) musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeOut, 0);
        }

        /// <summary>
        /// 音乐渐出
        /// </summary>
        /// <param name="audioClip">音频</param>
        public void FadeOutMusic(AudioClip audioClip)
        {
            var musicChannel = MusicChannels.Find(c => c.AudioClip == audioClip);
            if (musicChannel != null) musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeOut, 0);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void PlayBackgroundMusic(string audioPath)
        {
            AudioClip audioClip = AssetBundles.DataLoader.Load<AudioClip>(audioPath);
            if (audioClip != null)
            {
                PlayBackgroundMusic(audioClip);
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="audioClip">音频</param>
        public void PlayBackgroundMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.Play(audioClip);
            StopBackgroundMusic();
            _backgroundMusic = musicChannel;
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.Stop();
                _musicChannels.Remove(_backgroundMusic);
                _backgroundMusic = null;
            }
        }

        /// <summary>
        /// 背景音乐渐入
        /// </summary>
        /// <param name="audioPath">音频地址</param>
        public void FadeInBackgroundMusic(string audioPath)
        {
            AudioClip audioClip = AssetBundles.DataLoader.Load<AudioClip>(audioPath);
            if (audioClip != null)
            {
                FadeInBackgroundMusic(audioClip);
            }
        }

        /// <summary>
        /// 背景音乐渐入
        /// </summary>
        /// <param name="audioClip">音频</param>
        public void FadeInBackgroundMusic(AudioClip audioClip)
        {
            var musicChannel = CreateAudioChannel();
            musicChannel.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeIn, _musicVolume);
            FadeOutBackgroundMusic();
            _backgroundMusic = musicChannel;
        }

        /// <summary>
        /// 背景音乐渐出
        /// </summary>
        public void FadeOutBackgroundMusic()
        {
            if (_backgroundMusic != null)
            {
                _backgroundMusic.FadeInfo = new MusicFadeInfo(MusicFadeType.FadeOut, 0);
                _backgroundMusic = null;
            }
        }
    }

    /// <summary>
    /// 音乐频道
    /// </summary>
    public class MusicChannel
    {
        protected AudioSource _audioSource;
        /// <summary>
        /// 音源
        /// </summary>
        /// <returns></returns>
        public virtual AudioSource AudioSource { get { return _audioSource; } }
        /// <summary>
        /// 音频
        /// </summary>
        /// <returns></returns>
        public virtual AudioClip AudioClip { get { return _audioSource ? _audioSource.clip : null; } }

        /// <summary>
        /// 音乐渐变信息
        /// </summary>
        /// <returns></returns>
        public MusicFadeInfo FadeInfo { get; set; }

        public MusicChannel(AudioSource audioSource)
        {
            audioSource.loop = true;
            this._audioSource = audioSource;
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="audioClip"></param>
        public void Play(AudioClip audioClip)
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            _audioSource.Stop();
            _audioSource.clip = null;
            Object.Destroy(_audioSource);
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
        /// 设置音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            if (FadeInfo != null && FadeInfo.FadeType == MusicFadeType.FadeIn)
            {
                FadeInfo.FadeEndVolume = volume;
            }
            else
            {
                _audioSource.volume = volume;
            }
        }

    }

    /// <summary>
    /// 音乐渐变信息
    /// </summary>
    public class MusicFadeInfo
    {
        private float _fadeSpeed;
        /// <summary>
        /// 渐变速度
        /// </summary>
        /// <returns></returns>
        public float FadeSpeed { get { return _fadeSpeed; } set { _fadeSpeed = value; } }
        private float _fadeEndVolume;
        /// <summary>
        /// 渐变结束音量
        /// </summary>
        /// <returns></returns>
        public float FadeEndVolume { get { return _fadeEndVolume; } set { _fadeEndVolume = value; } }
        private MusicFadeType _fadeType;
        /// <summary>
        /// 渐变类型
        /// </summary>
        /// <returns></returns>
        public MusicFadeType FadeType { get { return _fadeType; } }


        public MusicFadeInfo(MusicFadeType fadeType, float fadeEndVolume)
        {
            this._fadeEndVolume = fadeEndVolume;
            this._fadeType = fadeType;
            this._fadeSpeed = 1;
        }

        /// <summary>
        /// 渐变
        /// </summary>
        /// <param name="audioSource">音源</param>
        /// <returns></returns>
        public bool OnFade(AudioSource audioSource)
        {
            if (_fadeType == MusicFadeType.FadeIn)
            {
                float fadeVolume = audioSource.volume + Time.deltaTime * _fadeSpeed;
                audioSource.volume = Mathf.Max(fadeVolume, _fadeEndVolume);
                if (audioSource.volume <= _fadeEndVolume) { return true; }
            }
            else if (_fadeType == MusicFadeType.FadeOut)
            {
                float fadeVolume = audioSource.volume - Time.deltaTime * _fadeSpeed;
                audioSource.volume = Mathf.Min(fadeVolume, _fadeEndVolume);
                if (audioSource.volume >= _fadeEndVolume) { return true; }
            }
            return false;
        }
    }

    /// <summary>
    /// 渐变类型
    /// </summary>
    public enum MusicFadeType
    {
        FadeIn,     //渐入
        FadeOut,    //渐出
    }
}