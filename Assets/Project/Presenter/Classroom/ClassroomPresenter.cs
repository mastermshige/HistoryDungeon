using System;
using System.Linq;
using HistoryDungeon.Logic;
using HistoryDungeon.View;

namespace HistoryDungeon.Presenter
{
    /// <summary>ExplanationLogic の通知を黒板の表示に変換し、画面のクリックを Logic の「次へ」に伝える橋渡し。</summary>
    public sealed class ClassroomPresenter : IDisposable
    {
        private readonly ExplanationLogic _logic;
        private readonly Era _era;
        private readonly ClassroomView _view;
        private readonly SoundPlayerView _sound;

        /// <summary>解説を最後まで読み終えた（ダンジョンへ移る合図）。</summary>
        public event Action Finished;

        public ClassroomPresenter(ExplanationLogic logic, Era era, ClassroomView view, SoundPlayerView sound)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _era = era ?? throw new ArgumentNullException(nameof(era));
            _view = view != null ? view : throw new ArgumentNullException(nameof(view));
            _sound = sound != null ? sound : throw new ArgumentNullException(nameof(sound));

            _view.AdvanceRequested += OnAdvanceRequested;
            _logic.PageChanged += OnPageChanged;
            _logic.Finished += OnFinished;
        }

        public void Start()
        {
            var timelineLines = _era.Timeline.Select(entry => $"{entry.Year}　{entry.Event}").ToArray();
            _view.ShowEra(_era.Name, timelineLines);
            ShowPage(_logic.CurrentPageIndex);
        }

        public void Dispose()
        {
            if (_view != null)
                _view.AdvanceRequested -= OnAdvanceRequested;
            _logic.PageChanged -= OnPageChanged;
            _logic.Finished -= OnFinished;
        }

        private void OnAdvanceRequested()
        {
            if (_logic.IsFinished)
                return;

            _sound.PlaySe(SeCue.PageTurn);
            _logic.Advance();
        }

        private void OnPageChanged(int pageIndex)
        {
            ShowPage(pageIndex);
        }

        private void OnFinished()
        {
            Finished?.Invoke();
        }

        private void ShowPage(int pageIndex)
        {
            var pageCount = _era.ExplanationPages.Count;
            var action = _logic.IsLastPage ? "クリックでダンジョンへ！" : "クリックでつぎへ";
            _view.ShowPage(_era.ExplanationPages[pageIndex], $"{action}（{pageIndex + 1}/{pageCount}）");
        }
    }
}
