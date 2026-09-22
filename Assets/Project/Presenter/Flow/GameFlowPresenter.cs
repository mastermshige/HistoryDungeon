using System;
using HistoryDungeon.Logic;
using HistoryDungeon.View;

namespace HistoryDungeon.Presenter
{
    /// <summary>
    /// 教室シーンでの画面の切り替え役。GameProgressLogic の状態に合わせて、
    /// タイトル・黒板・エンディングのどれを見せるか、いつダンジョンのシーンへ移るかを決めて View に伝える。
    /// </summary>
    public sealed class GameFlowPresenter : IDisposable
    {
        private const string EndingTitle = "エンディング";
        private const string EndingBody = "すべての時代をクリアしたのじゃ！\nおめでとう！";
        private const string ReviewedEndingBody = "復習もばっちりじゃ！\nこれで年号は完璧じゃな！";

        private readonly GameProgressLogic _progress;
        private readonly TitleView _title;
        private readonly ClassroomView _classroom;
        private readonly EndingView _ending;
        private readonly SoundPlayerView _sound;
        private readonly Action<string> _loadScene;
        private readonly string _gameName;
        private readonly string _dungeonSceneName;
        private ClassroomPresenter _classroomPresenter;

        public GameFlowPresenter(
            GameProgressLogic progress,
            TitleView title,
            ClassroomView classroom,
            EndingView ending,
            SoundPlayerView sound,
            Action<string> loadScene,
            string gameName,
            string dungeonSceneName)
        {
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _title = title != null ? title : throw new ArgumentNullException(nameof(title));
            _classroom = classroom != null ? classroom : throw new ArgumentNullException(nameof(classroom));
            _ending = ending != null ? ending : throw new ArgumentNullException(nameof(ending));
            _sound = sound != null ? sound : throw new ArgumentNullException(nameof(sound));
            _loadScene = loadScene ?? throw new ArgumentNullException(nameof(loadScene));
            _gameName = gameName;
            _dungeonSceneName = dungeonSceneName;

            _title.StartRequested += OnStartRequested;
            _ending.ReturnRequested += OnReturnRequested;
            _ending.ReviewRequested += OnReviewRequested;
            _progress.PhaseChanged += OnPhaseChanged;
        }

        /// <summary>シーンを開いた時点の進行状況に合わせて最初の画面を出す。</summary>
        public void Start()
        {
            Apply(_progress.Phase);
        }

        public void Dispose()
        {
            DisposeClassroomPresenter();

            if (_title != null)
                _title.StartRequested -= OnStartRequested;
            if (_ending != null)
                _ending.ReturnRequested -= OnReturnRequested;
            if (_ending != null)
                _ending.ReviewRequested -= OnReviewRequested;
            _progress.PhaseChanged -= OnPhaseChanged;
        }

        private void OnStartRequested()
        {
            _sound.PlaySe(SeCue.ButtonStart);
            _progress.StartGame();
        }

        private void OnReturnRequested()
        {
            _sound.PlaySe(SeCue.ButtonStart);
            _progress.ReturnToTitle();
        }

        private void OnReviewRequested()
        {
            _sound.PlaySe(SeCue.ButtonStart);
            _progress.StartReview();
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            Apply(phase);
        }

        private void OnClassroomFinished()
        {
            _progress.FinishClassroom();
        }

        private void Apply(GamePhase phase)
        {
            DisposeClassroomPresenter();
            _title.Hide();
            _classroom.Hide();
            _ending.Hide();

            switch (phase)
            {
                case GamePhase.Title:
                    _title.Show(_gameName);
                    _sound.PlayBgm(BgmCue.Title);
                    break;
                case GamePhase.Classroom:
                    ShowClassroom();
                    _sound.PlayBgm(BgmCue.Classroom);
                    break;
                case GamePhase.Dungeon:
                    _loadScene(_dungeonSceneName);
                    break;
                case GamePhase.Ending:
                    ShowEnding();
                    _sound.PlayBgm(BgmCue.Ending);
                    break;
                case GamePhase.Review:
                    _loadScene(_dungeonSceneName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        private void ShowEnding()
        {
            var mistakeCount = _progress.Mistakes.Count;
            var reviewLabel = mistakeCount > 0 ? $"間違えた問題を復習する（{mistakeCount}問）" : null;
            _ending.Show(EndingTitle, _progress.HasReviewed ? ReviewedEndingBody : EndingBody, reviewLabel);
        }

        private void ShowClassroom()
        {
            var era = _progress.CurrentEra;
            _classroomPresenter = new ClassroomPresenter(new ExplanationLogic(era.ExplanationPages.Count), era, _classroom, _sound);
            _classroomPresenter.Finished += OnClassroomFinished;
            _classroomPresenter.Start();
            _classroom.Show();
        }

        private void DisposeClassroomPresenter()
        {
            if (_classroomPresenter == null)
                return;

            _classroomPresenter.Finished -= OnClassroomFinished;
            _classroomPresenter.Dispose();
            _classroomPresenter = null;
        }
    }
}
