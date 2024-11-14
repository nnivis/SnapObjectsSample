using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotation : MonoBehaviour
    {
        private Transform _objectTransform;
        private float _rotationSpeed = 170.0f;
        private Quaternion _targetRotation;

        void Start()
        {
            _objectTransform = GetComponent<Transform>();
            _targetRotation = _objectTransform.rotation;
        }

        public void ApplyRotateLeft() => _targetRotation *= Quaternion.Euler(0, -45, 0);
        public void ApplyRotateRight() => _targetRotation *= Quaternion.Euler(0, 45, 0);

        private void Update() =>
            _objectTransform.localRotation = _targetRotation;

        //private void Update() =>

        //   _objectTransform.localRotation = Quaternion.RotateTowards(_objectTransform.localRotation,

        //  _targetRotation, Time.deltaTime * _rotationSpeed);
    }
}