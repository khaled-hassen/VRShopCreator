using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Player.Controllers.Scripts
{
    public class GrabBehaviorManager : MonoBehaviour
    {
        [SerializeField] private TeleportationProvider teleportationProvider;
        [SerializeField] private TeleportationManager teleportationManager;
        [SerializeField] private ActionBasedController teleportationController;

        [SerializeField] private ActionBasedContinuousMoveProvider continuousMoveProvider;
        [SerializeField] private ActionBasedContinuousTurnProvider continuousTurnProvider;

        [SerializeField] private XRDirectInteractor leftDirectInteractor;
        [SerializeField] private XRRayInteractor leftRayInteractor;

        [SerializeField] private XRDirectInteractor rightDirectInteractor;
        [SerializeField] private XRRayInteractor rightRayInteractor;


        // Start is called before the first frame update
        private void Start()
        {
            leftDirectInteractor.selectEntered.AddListener(OnLeftGrabEntered);
            leftDirectInteractor.selectExited.AddListener(OnLeftGrabExited);
            leftRayInteractor.selectEntered.AddListener(OnLeftGrabEntered);
            leftRayInteractor.selectExited.AddListener(OnLeftGrabExited);

            rightDirectInteractor.selectEntered.AddListener(OnRightGrabEntered);
            rightDirectInteractor.selectExited.AddListener(OnRightGrabExited);
            rightRayInteractor.selectEntered.AddListener(OnRightGrabEntered);
            rightRayInteractor.selectExited.AddListener(OnRightGrabExited);
        }

        private void OnLeftGrabEntered(SelectEnterEventArgs args)
        {
            continuousMoveProvider.enabled = false;
            continuousTurnProvider.enabled = false;
        }

        private void OnLeftGrabExited(SelectExitEventArgs args)
        {
            continuousMoveProvider.enabled = true;
            continuousTurnProvider.enabled = true;
        }

        private void OnRightGrabEntered(SelectEnterEventArgs args)
        {
            teleportationProvider.enabled = false;
            teleportationController.enabled = false;
            teleportationManager.isTeleportationDisabled = true;
        }

        private void OnRightGrabExited(SelectExitEventArgs args)
        {
            teleportationProvider.enabled = true;
            teleportationController.enabled = true;
            teleportationManager.isTeleportationDisabled = false;
        }
    }
}