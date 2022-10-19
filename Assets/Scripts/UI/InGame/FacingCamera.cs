using UnityEngine;

namespace RPG.UI.InGame
{
    public class FacingCamera : MonoBehaviour
    {
        private Camera mainCam = null;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = mainCam.transform.forward;
        }
    }
}
