using HistoryDungeon.Presenter;
using HistoryDungeon.View;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HistoryDungeon.App
{
    /// <summary>教室シーンの組み立て入口（Composition Root）。タイトル・黒板・エンディングの画面を用意して進行を任せる。</summary>
    public sealed class ClassroomSceneLauncher : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private SoundBank _soundBank;
        [SerializeField] private SpriteBank _spriteBank;

        private GameFlowPresenter _flow;

        private void Start()
        {
            var sound = new GameObject("SoundPlayerView").AddComponent<SoundPlayerView>();
            sound.Initialize(_soundBank);

            var title = new GameObject("TitleView").AddComponent<TitleView>();
            title.Initialize(_font, _spriteBank);
            var classroom = new GameObject("ClassroomView").AddComponent<ClassroomView>();
            classroom.Initialize(_font, _spriteBank);
            var ending = new GameObject("EndingView").AddComponent<EndingView>();
            ending.Initialize(_font, _spriteBank);

            _flow = new GameFlowPresenter(
                GameSession.Progress, title, classroom, ending, sound,
                SceneManager.LoadScene, GameSession.GameName, GameSession.DungeonSceneName);
            _flow.Start();
        }

        private void OnDestroy()
        {
            _flow?.Dispose();
        }
    }
}
