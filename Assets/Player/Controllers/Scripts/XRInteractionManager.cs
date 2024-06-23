using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Player.Controllers.Scripts
{
    public class XRInteractionManager : MonoBehaviour
    {
        [SerializeField] private TeleportationProvider teleportationProvider;
        [SerializeField] private TeleportationManager teleportationManager;
        [SerializeField] private ActionBasedController teleportationController;

        [SerializeField] private ActionBasedContinuousMoveProvider continuousMoveProvider;
        [SerializeField] private ActionBasedContinuousTurnProvider continuousTurnProvider;

        public void DisableTeleportation()
        {
            teleportationProvider.enabled = false;
            teleportationController.enabled = false;
            teleportationManager.isTeleportationDisabled = true;
        }

        public void EnableTeleportation()
        {
            teleportationProvider.enabled = true;
            teleportationController.enabled = true;
            teleportationManager.isTeleportationDisabled = false;
        }

        public void DisableContinuousMovement()
        {
            continuousMoveProvider.enabled = false;
            continuousTurnProvider.enabled = false;
        }

        public void EnableContinuousMovement()
        {
            continuousMoveProvider.enabled = true;
            continuousTurnProvider.enabled = true;
        }
    }
}