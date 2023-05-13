using Gooyes.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubic.Audio
{
    internal class AudioPlayer : Singleton<AudioPlayer>, IPausable
    {
        [SerializeField] private AudioSource _otherSource;
        private float _backgroundVolume;
        [SerializeField] private float _divideVolumeByOnPause;
        [SerializeField] private SoundConfig[] _soundConfigs;
        private Dictionary<SoundType, SoundConfig> _soundMap;
        private Dictionary<SoundType, float> _volumeMap;
        public bool Muted { get; private set; }

        private void Start()
        {
            #region Sound Map
            _soundMap = new Dictionary<SoundType, SoundConfig>();
            _volumeMap = new Dictionary<SoundType, float>();
            for (int i = 0; i < _soundConfigs.Length; i++)
            {
                SoundType type = _soundConfigs[i].type;
                if (_soundMap.ContainsKey(type))
                    throw new Exception("Duplicate in sound configs");
                _soundMap[type] = _soundConfigs[i];
                _volumeMap[type] = _soundMap[type].source.volume;
                if (_soundConfigs[i].clip == null)
                    throw new Exception($"No clip for sound of type {type}");
                if (_soundConfigs[i].source != null)
                    _soundConfigs[i].source.Stop();
            }

            if (_soundMap.Count != Enum.GetNames(typeof(SoundType)).Length)
                throw new Exception("Clips count doesn't match sound types count");
            #endregion

            _otherSource.Stop();
            _backgroundVolume = _soundMap[SoundType.Background].source.volume;

            Mute(PlayerPrefs.GetInt("Mute", 0) == 1);

            if (_divideVolumeByOnPause <= 0)
                throw new Exception("Division number can't be zero or lower");
        }

        public void Mute(bool noSound)
        {
            Muted = noSound;
            PlayerPrefs.SetInt("Mute", noSound ? 1 : 0);
            foreach (var kvp in _soundMap)
            {
                kvp.Value.source.volume = noSound ? 0 : _volumeMap[kvp.Key];
                if (!MainController.Instance.IsRunning.Value && !noSound)
                    _soundMap[SoundType.Background].source.volume = _backgroundVolume / _divideVolumeByOnPause;
            }
        }

        public void Play(SoundType sound, float? turnOffInSeconds = null, AudioSource source = null)
        {
            if (source == null) source = _soundMap[sound].source;
            if (source == null) source = _otherSource;
            source.Stop();
            source.clip = _soundMap[sound].clip;
            source.Play();
            if (turnOffInSeconds.HasValue) StartCoroutine(Stop(turnOffInSeconds.Value, source));
        }
        public void Stop(SoundType sound, AudioSource source = null)
        {
            if (source == null) source = _soundMap[sound].source;
            if (source == null) source = _otherSource;
            source.Stop();
        }
        private IEnumerator Stop(float delay, AudioSource source)
        {
            float timer = 0;
            if (delay > 0)
            {
                while (timer < delay)
                {
                    yield return null;
                    timer += Time.deltaTime;
                }
            }
            source.Stop();
        }

        public void OnStartByUser()
        {
            Play(SoundType.Background);
        }

        public void OnPause()
        {
            if (Muted) return;
            foreach (SoundConfig sound in _soundMap.Values)
            {
                if (sound.source != null && sound.type != SoundType.Background)
                    sound.source.Stop();
            }
            _otherSource.Stop();
            _soundMap[SoundType.Background].source.volume = _backgroundVolume / _divideVolumeByOnPause;
        }

        public void OnContinue()
        {
            if (Muted) return;
            _soundMap[SoundType.Background].source.volume = _backgroundVolume;
        }

        public void OnRestart()
        {
            /*foreach (SoundConfig sound in _soundMap.Values)
            {
                if (sound.source != null)
                    sound.source.Stop();
            }
            _otherSource.Stop();
            _soundMap[SoundType.Background].source.volume = _backgroundVolume / _divideVolumeByOnPause;
            _soundMap[SoundType.Background].source.Play();*/
        }

        [Serializable]
        private struct SoundConfig
        {
            public SoundType type;
            public AudioClip clip;
            public AudioSource source;
        }
    }

    [Serializable]
    public enum SoundType
    {
        Background,
        ButtonTap,
        Stack
    }
}
