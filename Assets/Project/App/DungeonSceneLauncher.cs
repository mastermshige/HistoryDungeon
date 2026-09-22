using HistoryDungeon.Logic;
using HistoryDungeon.Presenter;
using HistoryDungeon.View;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HistoryDungeon.App
{
    /// <summary>
    /// ダンジョンシーンの組み立て入口（Composition Root）。現在の時代の問題を出題し、終わったら教室へ戻る。
    /// 復習フェーズのときは、間違えた問題だけを出題する。
    /// </summary>
    public sealed class DungeonSceneLauncher : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private SoundBank _soundBank;
        [SerializeField] private SpriteBank _spriteBank;

        private const string EraClearedMessage = "この時代の問題はすべて正解じゃ！\nよくがんばった！";
        private const string ReviewClearedMessage = "間違えた問題がすべて正解になったぞ！\nよく復習したのう！";

        // 時代が変わるたびにキャラの絵の選び始めをずらす（1時代の問題数より大きい値にして、時代内で重ならないようにする）
        private const int CharactersPerEraStep = 7;

        private DungeonPresenter _dungeonPresenter;
        private bool _isReview;

        private void Start()
        {
            var sound = new GameObject("SoundPlayerView").AddComponent<SoundPlayerView>();
            sound.Initialize(_soundBank);
            sound.PlayBgm(BgmCue.Dungeon);

            var progress = GameSession.Progress;
            _isReview = progress.Phase == GamePhase.Review;
            if (!_isReview)
                EnterDungeonPhase(progress);

            var questions = _isReview ? progress.Mistakes.Questions : progress.CurrentEra.Questions;
            var quiz = new EraQuizLogic(questions, max => Random.Range(0, max));

            var view = new GameObject("DungeonView").AddComponent<DungeonView>();
            view.Initialize(_font, _spriteBank, EraQuizLogic.ChoiceCount);

            _dungeonPresenter = new DungeonPresenter(
                quiz, view, sound,
                _isReview ? null : progress.Mistakes,
                _isReview ? ReviewClearedMessage : EraClearedMessage,
                _isReview ? 0 : progress.CurrentEraIndex * CharactersPerEraStep);
            _dungeonPresenter.Finished += OnDungeonFinished;
            quiz.Start();
        }

        private void OnDestroy()
        {
            if (_dungeonPresenter == null)
                return;

            _dungeonPresenter.Finished -= OnDungeonFinished;
            _dungeonPresenter.Dispose();
        }

        private void OnDungeonFinished()
        {
            if (_isReview)
                GameSession.Progress.FinishReview();
            else
                GameSession.Progress.FinishDungeon();
            SceneManager.LoadScene(GameSession.ClassroomSceneName);
        }

        /// <summary>ダンジョンシーンを単体で再生した場合（開発時）も動かせるよう、進行状況をダンジョンまで進めておく。</summary>
        private static void EnterDungeonPhase(GameProgressLogic progress)
        {
            if (progress.Phase == GamePhase.Ending)
                progress.ReturnToTitle();
            if (progress.Phase == GamePhase.Title)
                progress.StartGame();
            if (progress.Phase == GamePhase.Classroom)
                progress.FinishClassroom();
        }
    }
}
