using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HistoryDungeon.View
{
    /// <summary>
    /// タイトル画面。ダンジョンを背景に、題名と先生・キャラの行列を見せる。
    /// 「はじめる」が押されたことを知らせるだけで、その後どうするかは判断しない。
    /// </summary>
    public sealed class TitleView : MonoBehaviour
    {
        private static readonly Color TitleColor = new Color(1f, 0.85f, 0.35f);
        private static readonly Color SubtitleColor = new Color(0.95f, 0.92f, 0.85f);
        private static readonly Color ButtonColor = new Color(0.90f, 0.55f, 0.15f);

        private const float CastY = -330f;
        private const float CastSize = 200f;
        private const float ShadeAlpha = 0.45f;

        private RectTransform _root;
        private CastRow _cast;
        private TextMeshProUGUI _gameName;
        private RectTransform _startButtonRect;
        private Button _startButton;

        public event Action StartRequested;

        public void Initialize(TMP_FontAsset font, SpriteBank sprites)
        {
            if (_root != null)
                throw new InvalidOperationException("すでに初期化されています。");

            var builder = new UiBuilder(font);
            UiBuilder.EnsureEventSystem();
            _root = builder.CreateCanvas(transform, "TitleCanvas", 10);

            DungeonBackdrop.Build(builder, _root, sprites, ShadeAlpha);
            _cast = new CastRow(builder, _root, sprites, CastY, CastSize);

            _gameName = builder.CreateText("GameName", _root, 130f, TitleColor, TextAlignmentOptions.Center);
            UiBuilder.Place(_gameName.rectTransform, new Vector2(0f, 220f), new Vector2(1800f, 220f));

            var subtitle = builder.CreateText("Subtitle", _root, 48f, SubtitleColor, TextAlignmentOptions.Center);
            UiBuilder.Place(subtitle.rectTransform, new Vector2(0f, 60f), new Vector2(1700f, 100f));
            subtitle.text = "年号をおぼえて、ダンジョンを進め！";

            _startButton = builder.CreateButton(
                "StartButton", _root, ButtonColor, "はじめる", 60f, new Vector2(0f, -110f), new Vector2(560f, 150f), out _);
            _startButtonRect = (RectTransform)_startButton.transform;
            _startButton.onClick.AddListener(OnStartClicked);
        }

        public void Show(string gameName)
        {
            _gameName.text = gameName;
            _root.gameObject.SetActive(true);
            StartAnimations();
        }

        public void Hide()
        {
            StopAnimations();
            _root.gameObject.SetActive(false);
        }

        /// <summary>キャラを上下にゆらし、「はじめる」ボタンをゆっくり脈打たせる。</summary>
        private void StartAnimations()
        {
            StopAnimations();
            _cast.StartBobbing();
            _startButtonRect.DOScale(1.06f, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        private void StopAnimations()
        {
            _cast?.StopBobbing();

            if (_startButtonRect == null)
                return;

            _startButtonRect.DOKill();
            _startButtonRect.localScale = Vector3.one;
        }

        private void OnStartClicked()
        {
            StartRequested?.Invoke();
        }

        private void OnDestroy()
        {
            StopAnimations();

            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartClicked);
        }
    }
}
