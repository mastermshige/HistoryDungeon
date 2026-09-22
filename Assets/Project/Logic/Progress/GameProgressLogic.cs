using System;
using System.Collections.Generic;

namespace HistoryDungeon.Logic
{
    public enum GamePhase
    {
        Title,
        Classroom,
        Dungeon,
        Ending,
        Review,
    }

    /// <summary>ゲーム全体の進行（タイトル → 教室 → ダンジョン → … → エンディング → 復習）を管理する。</summary>
    public sealed class GameProgressLogic
    {
        private readonly IReadOnlyList<Era> _eras;

        public GamePhase Phase { get; private set; } = GamePhase.Title;
        public int CurrentEraIndex { get; private set; }
        public int EraCount => _eras.Count;
        public Era CurrentEra => _eras[CurrentEraIndex];

        /// <summary>このプレイで間違えた問題。新しく始める（StartGame）・タイトルへ戻るたびに空にする。</summary>
        public MistakeLog Mistakes { get; } = new MistakeLog();

        /// <summary>このプレイで復習をやり遂げたか。新しく始める・タイトルへ戻るたびに戻す。</summary>
        public bool HasReviewed { get; private set; }

        public event Action<GamePhase> PhaseChanged;

        public GameProgressLogic(IReadOnlyList<Era> eras)
        {
            if (eras == null || eras.Count == 0)
                throw new ArgumentException("時代が1つもありません。", nameof(eras));

            _eras = eras;
        }

        public void StartGame()
        {
            RequirePhase(GamePhase.Title);
            CurrentEraIndex = 0;
            Mistakes.Clear();
            HasReviewed = false;
            ChangePhase(GamePhase.Classroom);
        }

        public void FinishClassroom()
        {
            RequirePhase(GamePhase.Classroom);
            ChangePhase(GamePhase.Dungeon);
        }

        public void FinishDungeon()
        {
            RequirePhase(GamePhase.Dungeon);

            if (CurrentEraIndex + 1 < _eras.Count)
            {
                CurrentEraIndex++;
                ChangePhase(GamePhase.Classroom);
                return;
            }

            ChangePhase(GamePhase.Ending);
        }

        /// <summary>エンディングから、間違えた問題の復習へ進む。間違いが1つも無ければ例外。</summary>
        public void StartReview()
        {
            RequirePhase(GamePhase.Ending);
            if (Mistakes.Count == 0)
                throw new InvalidOperationException("復習する問題がありません。");

            ChangePhase(GamePhase.Review);
        }

        /// <summary>復習をやり遂げたので記録を空にして、エンディングへ戻る。</summary>
        public void FinishReview()
        {
            RequirePhase(GamePhase.Review);
            Mistakes.Clear();
            HasReviewed = true;
            ChangePhase(GamePhase.Ending);
        }

        public void ReturnToTitle()
        {
            RequirePhase(GamePhase.Ending);
            CurrentEraIndex = 0;
            Mistakes.Clear();
            HasReviewed = false;
            ChangePhase(GamePhase.Title);
        }

        private void RequirePhase(GamePhase expected)
        {
            if (Phase != expected)
                throw new InvalidOperationException($"{expected} のときだけ実行できます（現在: {Phase}）。");
        }

        private void ChangePhase(GamePhase next)
        {
            Phase = next;
            PhaseChanged?.Invoke(next);
        }
    }
}
