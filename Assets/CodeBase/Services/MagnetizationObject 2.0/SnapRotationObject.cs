using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    [RequireComponent(typeof(SnapObject))]
    public class SnapRotationObject : MonoBehaviour
    {
        public event Action<Collider> OnChangeNativeRotation;
        public event Action<Quaternion> OnChangeNativeQuaternionRotation;

        private SnapObject _snapObject;
        private ObjectRotation.ObjectRotation _objectRotation;
        private Quaternion _nativeRotation;
        private CalculateSnapRotation _calculateSnapRotation;

        private bool _isSnapped;

        private void OnEnable()
        {
            _snapObject = GetComponent<SnapObject>();
            _calculateSnapRotation = new CalculateSnapRotation();

            SubscribeToObjectRotationEvents();
            SetNativeRotation();

            _snapObject.OnSnap += HandleSnap;
            _snapObject.OnUnsnap += HandleUnsnap;
        }

        private void OnDisable()
        {
            _snapObject.OnSnap -= HandleSnap;
            _snapObject.OnUnsnap -= HandleUnsnap;
        }

        private void SubscribeToObjectRotationEvents()
        {
            _objectRotation = GetComponent<ObjectRotation.ObjectRotation>();

            if (_objectRotation != null)
                _objectRotation.OnChangeNativeRotation += ApplyTargetObjectRotation;
        }

        private void HandleSnap(Collider target)
        {
            if (_isSnapped) return;
            _isSnapped = true;

            var rotation = ApplyTargetObjectRotation(target);
            OnChangeNativeQuaternionRotation?.Invoke(rotation);
        }

        private void HandleUnsnap(Collider target, bool isRotatorActive)
        {
            if (!_isSnapped) return;
            _isSnapped = false;

            if (!isRotatorActive)
            {
                ApplyNativeRotation();
                OnChangeNativeRotation?.Invoke(GetComponent<BoxCollider>());
            }
            else
            {
                var rotation = ApplyTargetObjectRotation(target);
                OnChangeNativeQuaternionRotation?.Invoke(rotation);
            }
        }


        private void ApplyTargetObjectRotation(Quaternion targetRotation)
        {
            transform.rotation = targetRotation;
            SetNativeRotation();
        }

        private void ApplyNativeRotation() => transform.rotation = _nativeRotation;

        private void SetNativeRotation() => _nativeRotation = transform.rotation;

        private Quaternion ApplyTargetObjectRotation(Collider target)
        {
            Vector3[] myAxes =
            {
                transform.up, -transform.up,
                transform.right, -transform.right,
                transform.forward, -transform.forward
            };

            Vector3[] targetAxes =
            {
                target.transform.up, -target.transform.up,
                target.transform.right, -target.transform.right,
                target.transform.forward, -target.transform.forward
            };

            float smallestAngle = Mathf.Infinity;
            Vector3 myClosestAxis = Vector3.zero;
            Vector3 targetClosestAxis = Vector3.zero;

            foreach (var myAxis in myAxes)
            {
                foreach (var targetAxis in targetAxes)
                {
                    float angle = Vector3.Angle(myAxis, targetAxis);
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        myClosestAxis = myAxis;
                        targetClosestAxis = targetAxis;
                    }
                }
            }

            Quaternion rotation1 = Quaternion.FromToRotation(myClosestAxis, targetClosestAxis);

            transform.rotation = rotation1 * transform.rotation;

            Vector3[] myAxesAfterFirstRotation =
            {
                transform.up, -transform.up,
                transform.right, -transform.right,
                transform.forward, -transform.forward
            };

            smallestAngle = Mathf.Infinity;
            Vector3 mySecondAxis = Vector3.zero;
            Vector3 targetSecondAxis = Vector3.zero;

            foreach (var myAxis in myAxesAfterFirstRotation)
            {
                if (myAxis == myClosestAxis || myAxis == -myClosestAxis) continue; 

                foreach (var targetAxis in targetAxes)
                {
                    if (targetAxis == targetClosestAxis || targetAxis == -targetClosestAxis)
                        continue; 

                    float angle = Vector3.Angle(myAxis, targetAxis);
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        mySecondAxis = myAxis;
                        targetSecondAxis = targetAxis;
                    }
                }
            }

            Quaternion rotation2 = Quaternion.FromToRotation(mySecondAxis, targetSecondAxis);
            transform.rotation = rotation2 * transform.rotation;

            return transform.rotation;
        }
    }
}