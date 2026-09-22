using System;

namespace HistoryDungeon.Logic
{
    /// <summary>教室の解説ページ送りを管理する。最後のページで送ると解説終了を通知する。</summary>
    public sealed class ExplanationLogic
    {
        private readonly int _pageCount;

        public int CurrentPageIndex { get; private set; }
        public bool IsLastPage => CurrentPageIndex == _pageCount - 1;
        public bool IsFinished { get; private set; }

        public event Action<int> PageChanged;
        public event Action Finished;

        public ExplanationLogic(int pageCount)
        {
            if (pageCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageCount), "解説ページは1ページ以上必要です。");

            _pageCount = pageCount;
        }

        public void Advance()
        {
            if (IsFinished)
                throw new InvalidOperationException("解説はすでに終了しています。");

            if (!IsLastPage)
            {
                CurrentPageIndex++;
                PageChanged?.Invoke(CurrentPageIndex);
                return;
            }

            IsFinished = true;
            Finished?.Invoke();
        }
    }
}
