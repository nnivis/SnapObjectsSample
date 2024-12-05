using System;
using CodeBase.Services.MagnetizationObject_2._0;
using CodeBase.Services.MagnetizationObject_2._0.Snap;
using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotation : MonoBehaviour
    {
        public event Action<Quaternion> OnChangeNativeRotation;
        
        private const float RotationSpeed = 170.0f;
        
        private SnapRotationObject _snapRotationObject;
        private Transform _objectTransform;
        private Quaternion _targetRotation;
        private bool _useHoldLogic;

        private void Start()
        {
            _objectTransform = GetComponent<Transform>();
            _targetRotation = _objectTransform.rotation;
            SubscribeToObjectRotationEvents();
        }

        public void SetRotationLogic(bool useHoldLogic) => _useHoldLogic = useHoldLogic;

        private void SubscribeToObjectRotationEvents()
        {
            _snapRotationObject = GetComponent<SnapRotationObject>();

            if (_snapRotationObject != null)
            {
                _snapRotationObject.OnChangeNativeRotation += ApplyTargetObjectRotation;
                _snapRotationObject.OnChangeNativeQuaternionRotation += ApplyTargetObjectQuaternionRotation;
            }
        }

        private void Update()
        {
            if (_useHoldLogic)
                return;

            _objectTransform.localRotation = Quaternion.RotateTowards(
                _objectTransform.localRotation,
                _targetRotation,
                Time.deltaTime * RotationSpeed
            );
        }


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

        public void ApplyRotateHoldRight()
        {
            _objectTransform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
            _targetRotation = _objectTransform.rotation;
            
            NotifyNativeRotationChanged();
        }

        
        public void ApplyRotateHoldLeft()
        {
            _objectTransform.Rotate(Vector3.up, -RotationSpeed * Time.deltaTime);
            _targetRotation = _objectTransform.rotation;
            
            NotifyNativeRotationChanged();
        }

        private void ApplyTargetObjectRotation(Collider target) => _targetRotation = target.transform.rotation;

        private void ApplyTargetObjectQuaternionRotation(Quaternion quaternion) => _targetRotation = quaternion;

        private void NotifyNativeRotationChanged() => OnChangeNativeRotation?.Invoke(_targetRotation);
        
    }
}