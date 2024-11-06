using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    // TODO: Сделать так, чтобы вращение происходило вокруг объекта. Нужно, чтобы центр объекта определялся автоматически, при старте
    public class ObjectRotation : MonoBehaviour
    {
        private Transform _objectTransform;
        private float _rotationSpeed = 100.0f;
        private Quaternion _targetRotation;

        void Start()
        {
            _objectTransform = GetComponent<Transform>();
            _targetRotation = _objectTransform.rotation;
        }
        public void ApplyRotateLeft() => _targetRotation *= Quaternion.Euler(0, 0, -90);
        public void ApplyRotateRight() => _targetRotation *= Quaternion.Euler(0, 0, 90);

        private void Update() =>
            _objectTransform.rotation = Quaternion.RotateTowards(_objectTransform.rotation,
                _targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }
}