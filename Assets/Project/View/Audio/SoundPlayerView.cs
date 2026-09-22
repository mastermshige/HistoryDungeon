using System;
using UnityEngine;

namespace HistoryDungeon.View
{
    /// <summary>
    /// 効果音とBGMを再生する窓口。どの音をいつ鳴らすかは決めず、頼まれた種類の音を SoundBank から取り出して鳴らすだけ。
    /// </summary>
    public sealed class SoundPlayerView : MonoBehaviour
    {
        private SoundBank _bank;
        private AudioSource _bgmSource;
        private AudioSource _seSource;

        public void Initialize(SoundBank bank)
        {
            if (_bank != null)
                throw new InvalidOperationException("すでに初期化されています。");

            _bank = bank != null ? bank : throw new ArgumentNullException(nameof(bank));

            // AudioListener が無いと音が出ないため、シーンに無ければここに付ける。
            if (FindFirstObjectByType<AudioListener>() == null)
                gameObject.AddComponent<AudioListener>();

            _bgmSource = CreateSource("Bgm", loop: true);
            _seSource = CreateSource("Se", loop: false);
        }

        public void PlaySe(SeCue cue)
        {
            _seSource.PlayOneShot(_bank.GetSe(cue), _bank.SeVolume);
        }

        public void PlayBgm(BgmCue cue)
        {
            var clip = _bank.GetBgm(cue);
            if (_bgmSource.clip == clip && _bgmSource.isPlaying)
                return;

            _bgmSource.clip = clip;
            _bgmSource.volume = _bank.BgmVolume;
            _bgmSource.Play();
        }

        public void StopBgm()
        {
            _bgmSource.Stop();
        }

        private AudioSource CreateSource(string sourceName, bool loop)
        {
            var child = new GameObject(sourceName);
            child.transform.SetParent(transform, false);
            var source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            return source;
        }
    }
}
