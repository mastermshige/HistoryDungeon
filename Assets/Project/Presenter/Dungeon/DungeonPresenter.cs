using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HistoryDungeon.Logic;
using HistoryDungeon.View;

namespace HistoryDungeon.Presenter
{
    /// <summary>EraQuizLogic の通知を DungeonView の表示に変換し、View の入力を Logic に伝える橋渡し。</summary>
    public sealed class DungeonPresenter : IDisposable
    {
        private const float CorrectDisplaySeconds = 1.2f;
        private const float EraClearedDisplaySeconds = 2.5f;
        private readonly EraQuizLogic _logic;
        private readonly DungeonView _view;
        private readonly SoundPlayerView _sound;
        private readonly MistakeLog _mistakes;
        private readonly string _clearedMessage;
        private readonly int _characterOffset;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private bool _isWaitingForNext;

        /// <summary>時代クリアの表示を終えたことを知らせる（教室へ戻る合図として使う）。</summary>
        public event Action Finished;

        /// <param name="mistakes">不正解を記録する先。復習中のように記録しない場合は null。</param>
        /// <param name="clearedMessage">すべて正解したときに見せるメッセージ。</param>
        /// <param name="characterOffset">キャラの絵の選び始め。時代ごとにずらすと、時代をまたいでも顔ぶれが変わる。</param>
        public DungeonPresenter(
            EraQuizLogic logic, DungeonView view, SoundPlayerView sound, MistakeLog mistakes, string clearedMessage,
            int characterOffset)
        {
            _logic = logic ?? throw new ArgumentNullException(nameof(logic));
            _view = view != null ? view : throw new ArgumentNullException(nameof(view));
            _sound = sound != null ? sound : throw new ArgumentNullException(nameof(sound));
            _mistakes = mistakes;
            _characterOffset = characterOffset;
            _clearedMessage = clearedMessage ?? throw new ArgumentNullException(nameof(clearedMessage));

            _view.ChoiceSelected += OnChoiceSelected;
            _logic.QuestionPresented += OnQuestionPresented;
            _logic.AnswerJudged += OnAnswerJudged;
            _logic.EraCleared += OnEraCleared;
        }

        public void Dispose()
        {
            _cancellation.Cancel();
            _cancellation.Dispose();

            if (_view != null)
                _view.ChoiceSelected -= OnChoiceSelected;
            _logic.QuestionPresented -= OnQuestionPresented;
            _logic.AnswerJudged -= OnAnswerJudged;
            _logic.EraCleared -= OnEraCleared;
        }

        private void OnChoiceSelected(int index)
        {
            _logic.Answer(index);
        }

        private void OnQuestionPresented(QuestionPresentation presentation)
        {
            if (!_isWaitingForNext)
            {
                _view.ShowQuestion(
                presentation.CharacterName, presentation.Prompt, presentation.Choices, _characterOffset + presentation.QuestionIndex);
                return;
            }

            ShowQuestionAfterCorrectAsync(presentation, _cancellation.Token).Forget();
        }

        private void OnAnswerJudged(AnswerResult result)
        {
            if (!result.IsCorrect)
            {
                _mistakes?.Record(_logic.GetQuestion(result.QuestionIndex));
                _view.ShowIncorrect(result.CorrectAnswer, result.Explanation);
                _sound.PlaySe(SeCue.Incorrect);
                return;
            }

            _isWaitingForNext = true;
            _view.SetChoicesInteractable(false);
            _view.ShowCorrect();
            _sound.PlaySe(SeCue.Correct);
        }

        private void OnEraCleared()
        {
            ShowEraClearedAsync(_cancellation.Token).Forget();
        }

        private async UniTaskVoid ShowQuestionAfterCorrectAsync(QuestionPresentation presentation, CancellationToken token)
        {
            var canceled = await UniTask.Delay(TimeSpan.FromSeconds(CorrectDisplaySeconds), cancellationToken: token)
                .SuppressCancellationThrow();
            if (canceled)
                return;

            _isWaitingForNext = false;
            _view.ShowQuestion(
                presentation.CharacterName, presentation.Prompt, presentation.Choices, _characterOffset + presentation.QuestionIndex);
        }

        private async UniTaskVoid ShowEraClearedAsync(CancellationToken token)
        {
            var canceled = await UniTask.Delay(TimeSpan.FromSeconds(CorrectDisplaySeconds), cancellationToken: token)
                .SuppressCancellationThrow();
            if (canceled)
                return;

            _view.ShowEraCleared(_clearedMessage);
            _sound.PlaySe(SeCue.EraCleared);

            canceled = await UniTask.Delay(TimeSpan.FromSeconds(EraClearedDisplaySeconds), cancellationToken: token)
                .SuppressCancellationThrow();
            if (canceled)
                return;

            Finished?.Invoke();
        }
    }
}
