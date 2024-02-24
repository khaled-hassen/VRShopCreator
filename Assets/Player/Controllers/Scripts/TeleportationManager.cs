using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Player.Controllers.Scripts
{
    public class TeleportationManager : MonoBehaviour
    {
        [SerializeField] private InputActionProperty activateAction;
        [SerializeField] private InputActionProperty cancelAction;
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private TeleportationProvider teleportationProvider;

        private bool _isTeleporting;
        private XRInteractorLineVisual _rayInteractorLineVisual;

        public bool IsTeleportationDisabled { get; set; }

        // Start is called before the first frame update
        private void Start()
        {
            _rayInteractorLineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();

            activateAction.action.Enable();
            activateAction.action.performed += OnTeleportActivate;

            cancelAction.action.Enable();
            cancelAction.action.performed += OnTeleportCancel;

            teleportationProvider.endLocomotion += OnTeleportationEnded;

            ToggleRayInteractor(false);
        }

        private void ToggleRayInteractor(bool value)
        {
            rayInteractor.enabled = value;
            _rayInteractorLineVisual.enabled = value;
        }

        private void OnTeleportActivate(InputAction.CallbackContext ctx)
        {
            if (IsTeleportationDisabled) return;
            ToggleRayInteractor(true);
            _isTeleporting = true;
        }

        private void OnTeleportCancel(InputAction.CallbackContext ctx)
        {
            if (IsTeleportationDisabled) return;
            if (!_isTeleporting) return;
            ToggleRayInteractor(false);
            _isTeleporting = false;
        }

        private void OnTeleportationEnded(LocomotionSystem system)
        {
            if (IsTeleportationDisabled) return;
            _isTeleporting = false;
            ToggleRayInteractor(false);
        }
    }
}