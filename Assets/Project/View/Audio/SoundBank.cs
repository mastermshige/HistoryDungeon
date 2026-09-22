using System;
using UnityEngine;

namespace HistoryDungeon.View
{
    /// <summary>鳴らす効果音の種類。</summary>
    public enum SeCue
    {
        ButtonStart,
        PageTurn,
        Correct,
        Incorrect,
        EraCleared,
    }

    /// <summary>流すBGMの種類。</summary>
    public enum BgmCue
    {
        Title,
        Classroom,
        Dungeon,
        Ending,
    }

    /// <summary>
    /// 効果音とBGMをまとめて持つ設定ファイル。音の追加・差し替えは、Inspector で対応する枠にクリップをドラッグするだけで済む。
    /// 個別のオブジェクトにはクリップを持たせない。
    /// </summary>
    [CreateAssetMenu(fileName = "SoundBank", menuName = "HistoryDungeon/Sound Bank")]
    public sealed class SoundBank : ScriptableObject
    {
        [Header("効果音")]
        [SerializeField] private AudioClip _seButtonStart;
        [SerializeField] private AudioClip _sePageTurn;
        [SerializeField] private AudioClip _seCorrect;
        [SerializeField] private AudioClip _seIncorrect;
        [SerializeField] private AudioClip _seEraCleared;

        [Header("BGM")]
        [SerializeField] private AudioClip _bgmTitle;
        [SerializeField] private AudioClip _bgmClassroom;
        [SerializeField] private AudioClip _bgmDungeon;
        [SerializeField] private AudioClip _bgmEnding;

        [Header("音量")]
        [SerializeField, Range(0f, 1f)] private float _seVolume = 0.8f;
        [SerializeField, Range(0f, 1f)] private float _bgmVolume = 0.35f;

        public float SeVolume => _seVolume;
        public float BgmVolume => _bgmVolume;

        public AudioClip GetSe(SeCue cue)
        {
            switch (cue)
            {
                case SeCue.ButtonStart: return Require(_seButtonStart, cue);
                case SeCue.PageTurn: return Require(_sePageTurn, cue);
                case SeCue.Correct: return Require(_seCorrect, cue);
                case SeCue.Incorrect: return Require(_seIncorrect, cue);
                case SeCue.EraCleared: return Require(_seEraCleared, cue);
                default: throw new ArgumentOutOfRangeException(nameof(cue), cue, null);
            }
        }

        public AudioClip GetBgm(BgmCue cue)
        {
            switch (cue)
            {
                case BgmCue.Title: return Require(_bgmTitle, cue);
                case BgmCue.Classroom: return Require(_bgmClassroom, cue);
                case BgmCue.Dungeon: return Require(_bgmDungeon, cue);
                case BgmCue.Ending: return Require(_bgmEnding, cue);
                default: throw new ArgumentOutOfRangeException(nameof(cue), cue, null);
            }
        }

        // 設定し忘れは握りつぶさず、どの枠が空かが分かるエラーにする。
        private AudioClip Require(AudioClip clip, Enum cue)
        {
            if (clip == null)
                throw new InvalidOperationException($"SoundBank の「{cue}」にクリップが設定されていません。");

            return clip;
        }
    }
}
