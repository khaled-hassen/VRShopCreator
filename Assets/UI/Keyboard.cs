using System;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Keyboard : MonoBehaviour
    {
        private readonly float _distance = 0.5f;
        private readonly float _verticalOffset = -0.2f;
        private TMP_InputField _inputField;
        private Transform _positionSource;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onSelect.AddListener(_ => OpenKeyboard());
            if (Camera.main != null) _positionSource = Camera.main.transform;
        }

        private void OpenKeyboard()
        {
            NonNativeKeyboard.Instance.Close();

            NonNativeKeyboard.Instance.InputField = _inputField;
            NonNativeKeyboard.Instance.PresentKeyboard(_inputField.text);

            var direction = _positionSource.forward;
            direction.y = 0;
            direction.Normalize();

            var targetPosition = _positionSource.position + direction * _distance + Vector3.up * _verticalOffset;
            NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition);
            SetCursorIndicatorAlpha(1);
            NonNativeKeyboard.Instance.OnClosed += CloseKeyboard;
        }

        private void CloseKeyboard(object _, EventArgs __)
        {
            SetCursorIndicatorAlpha(0);
            NonNativeKeyboard.Instance.OnClosed -= CloseKeyboard;
        }

        public void SetCursorIndicatorAlpha(float alpha)
        {
            _inputField.customCaretColor = true;
            var color = _inputField.caretColor;
            color.a = alpha;
            _inputField.caretColor = color;
        }
    }
}