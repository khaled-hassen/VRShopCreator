using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace AssetPopup
{
    public class AssetPopupInteractor : MonoBehaviour
    {
        [SerializeField] private InputActionProperty menuInputAction;
        [SerializeField] private XRRayInteractor rayInteractor;

        private void Awake() => menuInputAction.action.performed += ToggleMenu;

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (!rayInteractor.TryGetCurrent3DRaycastHit(out var hit)) return;

            var hitObject = hit.collider.gameObject;
            var assetPopup = hitObject.GetComponent<AssetPopup>();
            if (assetPopup is not null) assetPopup.TogglePopup();
        }
    }
}