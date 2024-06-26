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

        public bool isTeleportationDisabled { get; set; }
        
        private void Awake()
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
            if (isTeleportationDisabled) return;
            ToggleRayInteractor(true);
            _isTeleporting = true;
        }

        private void OnTeleportCancel(InputAction.CallbackContext ctx)
        {
            if (isTeleportationDisabled) return;
            if (!_isTeleporting) return;
            ToggleRayInteractor(false);
            _isTeleporting = false;
        }

        private void OnTeleportationEnded(LocomotionSystem system)
        {
            if (isTeleportationDisabled) return;
            _isTeleporting = false;
            ToggleRayInteractor(false);
        }
    }
}