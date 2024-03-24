using TMPro;
using UnityEngine;

namespace UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        public delegate void OnCloseWindowCallback();

        [SerializeField] protected TMP_FontAsset font;
        public event OnCloseWindowCallback OnCloseWindow;

        public void OnCloseWindowClick() => OnCloseWindow?.Invoke();
    }
}