using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        public float followSpeed = 2f;
        [SerializeField] private Transform _followTarget;

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, _followTarget.position, followSpeed * Time.deltaTime);
        }
    }
}
