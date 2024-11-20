using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    [RequireComponent(typeof(SnapObject))]
    public class SnapRotationObject : MonoBehaviour
    {
        public event Action<Collider> OnChangeNativeRotation;

        private SnapObject _snapObject;
        private ObjectRotation.ObjectRotation _objectRotation;
        private Quaternion _nativeRotation;

        private bool _isSnapped;

        private void OnEnable()
        {
            _snapObject = GetComponent<SnapObject>();

            SubscribeToObjectRotationEvents();
            SetNativeRotation();

            _snapObject.OnSnap += HandleSnap;
            _snapObject.OnUnsnap += HandleUnsnap;
        }

        private void SubscribeToObjectRotationEvents()
        {
            _objectRotation = GetComponent<ObjectRotation.ObjectRotation>();

            if (_objectRotation != null)
                _objectRotation.OnChangeNativeRotation += ApplyTargetObjectRotation;
        }

        private void OnDisable()
        {
            _snapObject.OnSnap -= HandleSnap;
            _snapObject.OnUnsnap -= HandleUnsnap;
        }

        private void HandleSnap(Collider target)
        {
            if (_isSnapped) return;
            _isSnapped = true;

            ApplyTargetObjectRotation(target);
            OnChangeNativeRotation?.Invoke(target);
        }

        private void HandleUnsnap(Collider target)
        {
            if (!_isSnapped) return;
            _isSnapped = false;

            ApplyNativeRotation();
            OnChangeNativeRotation?.Invoke(GetComponent<BoxCollider>());
        }

        private void ApplyTargetObjectRotation(Collider target) => transform.rotation = target.transform.rotation;

        private void ApplyTargetObjectRotation(Quaternion targetRotation)
        {
            transform.rotation = targetRotation;
            SetNativeRotation();
        }

        private void ApplyNativeRotation() => transform.rotation = _nativeRotation;

        private void SetNativeRotation() => _nativeRotation = transform.rotation;
    }
}