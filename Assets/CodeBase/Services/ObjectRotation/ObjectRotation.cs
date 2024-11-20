using System;
using CodeBase.Services.MagnetizationObject_2._0;
using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotation : MonoBehaviour
    {
        public event Action<Quaternion> OnChangeNativeRotation;

        private SnapRotationObject _snapRotationObject;
        private Transform _objectTransform;
        private float _rotationSpeed = 170.0f;
        private Quaternion _targetRotation;

        private void Start()
        {
            _objectTransform = GetComponent<Transform>();
            _targetRotation = _objectTransform.rotation;
            SubscribeToObjectRotationEvents();
        }

        private void SubscribeToObjectRotationEvents()
        {
            _snapRotationObject = GetComponent<SnapRotationObject>();

            if (_snapRotationObject != null)
                _snapRotationObject.OnChangeNativeRotation += ApplyTargetObjectRotation;
        }

        private void Update() =>
            _objectTransform.localRotation = Quaternion.RotateTowards(_objectTransform.localRotation,
                _targetRotation, Time.deltaTime * _rotationSpeed);

        public void ApplyRotateLeft()
        {
            _targetRotation *= Quaternion.Euler(0, -45, 0);
            NotifyNativeRotationChanged();
        }

        public void ApplyRotateRight()
        {
            _targetRotation *= Quaternion.Euler(0, 45, 0);
            NotifyNativeRotationChanged();
        }

        private void ApplyTargetObjectRotation(Collider target) => _targetRotation = target.transform.rotation;

        private void NotifyNativeRotationChanged() => OnChangeNativeRotation?.Invoke(_targetRotation);

        
        #region Commit

        //private void Update() =>
        //  _objectTransform.localRotation = _targetRotation;
        //public void ApplyRotateLeft() => _objectTransform.localRotation *= Quaternion.Euler(0, -45, 0);

        // public void ApplyRotateRight() => _objectTransform.localRotation *= Quaternion.Euler(0, 45, 0);

        #endregion
    }
}