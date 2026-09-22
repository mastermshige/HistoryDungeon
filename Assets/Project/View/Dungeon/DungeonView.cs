using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HistoryDungeon.View
{
    /// <summary>
    /// ダンジョンの出題画面（仮素材：色つきの四角と文字）。表示と入力の受け取りだけを行い、正誤の判断はしない。
    /// 見た目はすべてこのクラスの中に閉じているので、素材を差し替えるときもここだけ直せばよい。
    /// </summary>
    public sealed class DungeonView : MonoBehaviour
    {
        private const float ShadeAlpha = 0.35f;

        private static readonly Color BubbleColor = new Color(0.97f, 0.95f, 0.88f);
        private static readonly Color ButtonColor = new Color(0.30f, 0.45f, 0.75f);
        private static readonly Color CorrectTextColor = new Color(0.55f, 0.95f, 0.55f);
        private static readonly Color IncorrectTextColor = new Color(1.00f, 0.55f, 0.50f);
        private static readonly Color NeutralTextColor = Color.white;
        private static readonly Color DarkTextColor = new Color(0.12f, 0.10f, 0.10f);

        private UiBuilder _builder;
        private RectTransform _characterBox;
        private Image _characterImage;
        private IReadOnlyList<Sprite> _characterSprites;
        private TextMeshProUGUI _characterName;
        private TextMeshProUGUI _prompt;
        private TextMeshProUGUI _feedback;
        private ChoiceButtonView[] _choiceButtons;

        public event Action<int> ChoiceSelected;

        public void Initialize(TMP_FontAsset font, SpriteBank sprites, int choiceCount)
        {
            if (_builder != null)
                throw new InvalidOperationException("すでに初期化されています。");

            _builder = new UiBuilder(font);
            _characterSprites = sprites.DungeonCharacters;
            UiBuilder.EnsureEventSystem();
            BuildLayout(sprites, choiceCount);
        }

        /// <param name="characterNumber">どのキャラの絵を出すか。絵の枚数を超えたら最初に戻って使い回す。</param>
        public void ShowQuestion(string characterName, string prompt, IReadOnlyList<string> choices, int characterNumber)
        {
            _characterImage.sprite = _characterSprites[characterNumber % _characterSprites.Count];
            _characterName.text = characterName;
            _prompt.text = prompt;
            SetFeedback(string.Empty, NeutralTextColor);

            for (var i = 0; i < _choiceButtons.Length; i++)
            {
                _choiceButtons[i].SetText(choices[i]);
                _choiceButtons[i].SetInteractable(true);
            }

            _characterBox.DOKill();
            _characterBox.localScale = Vector3.one * 0.6f;
            _characterBox.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
        }

        public void ShowCorrect()
        {
            SetFeedback("正解じゃ！ お見事！", CorrectTextColor);
        }

        public void ShowIncorrect(string correctAnswer, string explanation)
        {
            SetFeedback($"ざんねん！ 正解は「{correctAnswer}」\n{explanation}\nもう一度こたえてみるのじゃ！", IncorrectTextColor);
            _feedback.transform.DOKill(true);
            _feedback.transform.DOShakePosition(0.4f, new Vector3(16f, 0f, 0f), 20, 90f, false, true);
        }

        public void ShowEraCleared(string message)
        {
            _characterName.text = string.Empty;
            _prompt.text = message;
            SetFeedback(string.Empty, NeutralTextColor);
            SetChoicesInteractable(false);
        }

        public void SetChoicesInteractable(bool interactable)
        {
            foreach (var button in _choiceButtons)
                button.SetInteractable(interactable);
        }

        private void SetFeedback(string text, Color color)
        {
            _feedback.text = text;
            _feedback.color = color;
        }

        private void OnChoiceClicked(int index)
        {
            ChoiceSelected?.Invoke(index);
        }

        private void OnDestroy()
        {
            if (_characterBox != null)
                _characterBox.DOKill();
            if (_feedback != null)
                _feedback.transform.DOKill();

            if (_choiceButtons == null)
                return;

            foreach (var button in _choiceButtons)
            {
                if (button != null)
                    button.Clicked -= OnChoiceClicked;
            }
        }

        private void BuildLayout(SpriteBank sprites, int choiceCount)
        {
            var root = _builder.CreateCanvas(transform, "DungeonCanvas", 0);

            DungeonBackdrop.Build(_builder, root, sprites, ShadeAlpha);

            _characterBox = _builder.CreatePanel("Character", root, Color.clear);
            UiBuilder.Place(_characterBox, new Vector2(-700f, 250f), new Vector2(360f, 360f));
            var characterSprite = _builder.CreateSprite("CharacterSprite", _characterBox, _characterSprites[0]);
            UiBuilder.Place(characterSprite, new Vector2(0f, 30f), new Vector2(288f, 288f));
            _characterImage = characterSprite.GetComponent<Image>();
            _characterName = _builder.CreateText("CharacterName", _characterBox, 36f, NeutralTextColor, TextAlignmentOptions.Center);
            UiBuilder.Place(_characterName.rectTransform, new Vector2(0f, -150f), new Vector2(360f, 60f));

            var bubble = _builder.CreatePanel("Bubble", root, BubbleColor);
            UiBuilder.Place(bubble, new Vector2(200f, 250f), new Vector2(1100f, 360f));
            _prompt = _builder.CreateText("Prompt", bubble, 44f, DarkTextColor, TextAlignmentOptions.Center);
            UiBuilder.Stretch(_prompt.rectTransform, 30f);

            _choiceButtons = new ChoiceButtonView[choiceCount];
            for (var i = 0; i < choiceCount; i++)
            {
                var column = i % 2 == 0 ? -430f : 430f;
                var row = -80f - (i / 2) * 160f;
                _choiceButtons[i] = CreateChoiceButton(i, root, new Vector2(column, row));
                _choiceButtons[i].Clicked += OnChoiceClicked;
            }

            _feedback = _builder.CreateText("Feedback", root, 34f, NeutralTextColor, TextAlignmentOptions.Center);
            UiBuilder.Place(_feedback.rectTransform, new Vector2(0f, -410f), new Vector2(1700f, 240f));
        }

        private ChoiceButtonView CreateChoiceButton(int index, RectTransform parent, Vector2 position)
        {
            var button = _builder.CreateButton(
                $"Choice{index}", parent, ButtonColor, string.Empty, 44f, position, new Vector2(820f, 130f), out var label);

            var view = button.gameObject.AddComponent<ChoiceButtonView>();
            view.Initialize(index, button, label);
            return view;
        }
    }
}
