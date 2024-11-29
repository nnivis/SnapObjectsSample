using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0.Snap
{
    public class SnapHandler
    {
        private const float F = 1.5f;

        public event Action<bool> OnSnapStatus;
        public event Action<Collider> OnSnap;
        public event Action<Collider, bool> OnUnsnap;

        private readonly Transform _transform;
        private readonly Collider _myCollider;
        private readonly RelativeRotator _rotator;
        private readonly float _snapDistance;
        private Collider _currentTarget;
        private Collider _previousTarget;

        private bool _isCornerSnapped;
        private float _cornerSnapThreshold = 0.1f;
        private float _cornerUnsnapThreshold = 0.1f;

        public SnapHandler(Transform transform, Collider myCollider, RelativeRotator rotator, float snapDistance)
        {
            _transform = transform;
            _myCollider = myCollider;
            _rotator = rotator;
            _snapDistance = snapDistance;
        }

        public void UpdateCurrentTarget(Collider currentTarget)
        {
            _currentTarget = currentTarget;
        }

        public void HandleSnap()
        {
            if (_currentTarget != null && _myCollider != null)
            {
                Vector3 myClosestPoint = _myCollider.ClosestPoint(_currentTarget.transform.position);
                Vector3 targetClosestPoint = _currentTarget.ClosestPoint(myClosestPoint);
                Vector3 offset = targetClosestPoint - myClosestPoint;

                float snapThreshold = _snapDistance;
                float unsnapThreshold = _snapDistance * F;

                BoxCollider myBoxCollider = _myCollider as BoxCollider;
                BoxCollider targetBoxCollider = _currentTarget as BoxCollider;

                if (myBoxCollider != null && targetBoxCollider != null)
                {
                    Vector3[] myCorners = GetColliderCorners(myBoxCollider);
                    Vector3[] targetCorners = GetColliderCorners(targetBoxCollider);

                    float minDistance = float.MaxValue;
                    Vector3 myClosestCorner = Vector3.zero;
                    Vector3 targetClosestCorner = Vector3.zero;

                    foreach (var myCorner in myCorners)
                    {
                        foreach (var targetCorner in targetCorners)
                        {
                            float distance = Vector3.Distance(myCorner, targetCorner);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                myClosestCorner = myCorner;
                                targetClosestCorner = targetCorner;
                            }
                        }
                    }

                    if (minDistance < _cornerSnapThreshold)
                    {
                        Vector3 cornerOffset = targetClosestCorner - myClosestCorner;
                        _transform.position += cornerOffset;

                        if (!_isCornerSnapped)
                        {
                            //Debug.Log("Привязка к углу");
                            OnSnap?.Invoke(_currentTarget);
                            SetSnapStatus(true);
                            _isCornerSnapped = true;
                            _previousTarget = _currentTarget;
                        }
                        return;
                    }
                    else if (_isCornerSnapped && minDistance > _cornerUnsnapThreshold)
                    {
                        //Debug.Log("Отвязка от угла, но остаемся привязанными к объекту");
                        _isCornerSnapped = false;
                    }
                }

                if (offset.magnitude < snapThreshold && !_isCornerSnapped)
                {
                  //  _transform.position += offset;

                    if (_previousTarget != _currentTarget ||
                        (_previousTarget == _currentTarget && offset.magnitude < _snapDistance))
                    {
                        _transform.position += offset;
                        
                      //  Debug.Log("Привязка вдоль объекта");
                        OnSnap?.Invoke(_currentTarget);
                        SetSnapStatus(true);
                        _previousTarget = _currentTarget;
                    }
                }
                else if (offset.magnitude > unsnapThreshold)
                {
                    Unsnap();
                }
            }
            else if (_previousTarget != null)
            {
                Unsnap();
            }
        }

        public void Unsnap()
        {
            if (_previousTarget != null)
            {
                //Debug.Log("Отвязка от объекта");
                OnUnsnap?.Invoke(_previousTarget, _rotator.IsRotatorActive);
                SetSnapStatus(false);
                _previousTarget = null;
            }
        }

        public float GetUnsnapThreshold()
        {
            return _snapDistance * F; 
        }

        public void SetSnapStatus(bool isSnapped) => OnSnapStatus?.Invoke(isSnapped);

        private Vector3[] GetColliderCorners(BoxCollider boxCollider)
        {
            Vector3[] corners = new Vector3[8];

            Transform t = boxCollider.transform;
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size * 0.5f;

            Vector3[] localCorners = new Vector3[8];
            localCorners[0] = center + new Vector3(-size.x, -size.y, -size.z);
            localCorners[1] = center + new Vector3(size.x, -size.y, -size.z);
            localCorners[2] = center + new Vector3(size.x, -size.y, size.z);
            localCorners[3] = center + new Vector3(-size.x, -size.y, size.z);
            localCorners[4] = center + new Vector3(-size.x, size.y, -size.z);
            localCorners[5] = center + new Vector3(size.x, size.y, -size.z);
            localCorners[6] = center + new Vector3(size.x, size.y, size.z);
            localCorners[7] = center + new Vector3(-size.x, size.y, size.z);

            for (int i = 0; i < 8; i++)
            {
                corners[i] = t.TransformPoint(localCorners[i]);
            }

            return corners;
        }
    }
}