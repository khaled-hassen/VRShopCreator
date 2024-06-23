using UnityEngine;
using UnityEngine.UI;

namespace AssetPopup
{
    public class AssetPopup : MonoBehaviour
    {
        private Button _button;
        private Camera _mainCamera;
        private GameObject _popup;
        private GameObject _popupPrefab;

        private void Awake() => _popupPrefab = Resources.Load<GameObject>("Popup");

        private void Update()
        {
            if (_popup is null || _mainCamera is null) return;
            _popup.transform.LookAt(_mainCamera.transform);
            _popup.transform.forward *= -1;
        }

        public void TogglePopup()
        {
            if (_popup is null) ShowPopup();
            else HidePopup();
        }

        private void ShowPopup()
        {
            _mainCamera = Camera.main;
            _popup = Instantiate(_popupPrefab, transform);
            _popup.transform.localPosition = new Vector3(0, 1.2f, 0);

            var buttonContainer = _popup.transform.Find("RemoveAsset");
            if (buttonContainer is null) return;
            _button = buttonContainer.GetComponent<Button>();
            if (_button is not null) _button.onClick.AddListener(RemoveFromStore);
        }

        private void HidePopup()
        {
            if (_popup is null) return;
            _button.onClick.RemoveListener(RemoveFromStore);
            _button = null;
            Destroy(_popup.gameObject);
            _popup = null;
        }

        public void RemoveFromStore()
        {
            HidePopup();
            Destroy(gameObject);
        }
    }
}