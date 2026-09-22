using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HistoryDungeon.View
{
    /// <summary>
    /// 教室の画面（仮素材）。黒板に時代名・解説・年表を表示し、画面のクリックを「次へ」として知らせるだけ。
    /// どのページを出すか、いつダンジョンへ移るかは判断しない。
    /// </summary>
    public sealed class ClassroomView : MonoBehaviour
    {
        private const float FloorTileSize = 96f;

        private static readonly Color RoomColor = new Color(0.82f, 0.72f, 0.55f);
        private static readonly Color FrameColor = new Color(0.45f, 0.30f, 0.15f);
        private static readonly Color BoardColor = new Color(0.10f, 0.28f, 0.20f);
        private static readonly Color ChalkColor = new Color(0.97f, 0.97f, 0.92f);
        private static readonly Color TimelineColor = new Color(1.00f, 0.92f, 0.55f);
        private static readonly Color DarkTextColor = new Color(0.25f, 0.15f, 0.08f);

        private RectTransform _root;
        private Button _clickArea;
        private TextMeshProUGUI _eraName;
        private TextMeshProUGUI _pageText;
        private TextMeshProUGUI _timeline;
        private TextMeshProUGUI _hint;

        public event Action AdvanceRequested;

        public void Initialize(TMP_FontAsset font, SpriteBank sprites)
        {
            if (_root != null)
                throw new InvalidOperationException("すでに初期化されています。");

            var builder = new UiBuilder(font);
            UiBuilder.EnsureEventSystem();
            _root = builder.CreateCanvas(transform, "ClassroomCanvas", 0);

            var room = builder.CreatePanel("Room", _root, RoomColor);
            UiBuilder.Stretch(room);
            builder.TileSprite(room, sprites.ClassroomFloor, FloorTileSize);
            room.GetComponent<Image>().raycastTarget = true;
            _clickArea = room.gameObject.AddComponent<Button>();
            _clickArea.transition = Selectable.Transition.None;
            _clickArea.targetGraphic = room.GetComponent<Image>();
            _clickArea.onClick.AddListener(OnClicked);

            var frame = builder.CreatePanel("BoardFrame", _root, FrameColor);
            UiBuilder.Place(frame, new Vector2(0f, 90f), new Vector2(1700f, 840f));
            var board = builder.CreatePanel("Board", _root, BoardColor);
            UiBuilder.Place(board, new Vector2(0f, 90f), new Vector2(1640f, 780f));

            _eraName = builder.CreateText("EraName", _root, 72f, ChalkColor, TextAlignmentOptions.Center);
            UiBuilder.Place(_eraName.rectTransform, new Vector2(0f, 420f), new Vector2(1500f, 110f));

            _pageText = builder.CreateText("PageText", _root, 46f, ChalkColor, TextAlignmentOptions.TopLeft);
            UiBuilder.Place(_pageText.rectTransform, new Vector2(-340f, 100f), new Vector2(900f, 520f));

            _timeline = builder.CreateText("Timeline", _root, 36f, TimelineColor, TextAlignmentOptions.TopLeft);
            UiBuilder.Place(_timeline.rectTransform, new Vector2(500f, 60f), new Vector2(520f, 600f));
            _timeline.enableAutoSizing = true;
            _timeline.fontSizeMin = 24f;
            _timeline.fontSizeMax = 36f;

            var teacher = builder.CreateSprite("Teacher", _root, sprites.Teacher);
            UiBuilder.Place(teacher, new Vector2(-780f, -420f), new Vector2(240f, 240f));

            _hint = builder.CreateText("Hint", _root, 40f, DarkTextColor, TextAlignmentOptions.Center);
            UiBuilder.Place(_hint.rectTransform, new Vector2(200f, -420f), new Vector2(1300f, 100f));
        }

        public void ShowEra(string eraName, IReadOnlyList<string> timelineLines)
        {
            _eraName.text = eraName;
            _timeline.text = "【年表】\n" + string.Join("\n", timelineLines);
        }

        public void ShowPage(string text, string hint)
        {
            _pageText.text = text;
            _hint.text = hint;
        }

        public void Show()
        {
            _root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }

        private void OnClicked()
        {
            AdvanceRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (_clickArea != null)
                _clickArea.onClick.RemoveListener(OnClicked);
        }
    }
}
