using UnityEngine;

namespace Cubic
{
    public class ThrowLine : MonoBehaviour
    {
        [SerializeField] private Transform _leftEdge;
        [SerializeField] private Transform _rightEdge;

        [SerializeField] private float _height = 0.5f;

        public Vector3 Center { get => new Vector3(0, _height, _leftEdge.position.z); }

        public Vector3 Clamp(Vector3 value)
        {
            value.x = Mathf.Clamp(value.x, _leftEdge.position.x, _rightEdge.position.x);
            value.y = _height;
            value.z = _leftEdge.position.z;
            return value;
        }

        private void OnCollisionEnter(Collision collision)
        {
            MainController.Instance.EndGame(GameEndReason.DeadZoneCuboid);
        }
    }
}
