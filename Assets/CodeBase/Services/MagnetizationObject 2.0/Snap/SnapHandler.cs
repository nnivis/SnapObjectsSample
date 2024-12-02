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
                Vector3[] myCorners = GetColliderCorners(_myCollider);
                Vector3[] targetCorners = GetColliderCorners(_currentTarget);

                if (myCorners == null || targetCorners == null) return;

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
                    cornerOffset.y = 0f;
                    _transform.position += cornerOffset;

                    if (!_isCornerSnapped)
                    {
                       // Debug.Log("Привязка к углу");
                        OnSnap?.Invoke(_currentTarget);
                        SetSnapStatus(true);
                        _isCornerSnapped = true;
                        _previousTarget = _currentTarget;
                    }

                    return;
                }
                else if (_isCornerSnapped && minDistance > _cornerUnsnapThreshold)
                {
                    //Debug.Log("Отвязка от угла");
                    _isCornerSnapped = false;
                }

                if (!_isCornerSnapped)
                {
                    Vector3 myClosestPoint = _myCollider.ClosestPoint(_currentTarget.transform.position);
                    Vector3 targetClosestPoint = _currentTarget.ClosestPoint(myClosestPoint);
                    Vector3 offset = targetClosestPoint - myClosestPoint;

                    offset.y = 0f;

                    float snapThreshold = _snapDistance;
                    float unsnapThreshold = _snapDistance * F;

                    if (offset.magnitude < snapThreshold)
                    {
                        bool isOverlap = Physics.ComputePenetration(
                            _myCollider, _transform.position + offset, _transform.rotation,
                            _currentTarget, _currentTarget.transform.position, _currentTarget.transform.rotation,
                            out Vector3 separationDirection, out float separationDistance);

                        if (isOverlap)
                        {
                            separationDirection.y = 0f;
                            offset += separationDirection * separationDistance;
                        }

                        _transform.position += offset;

                        if (_previousTarget != _currentTarget ||
                            (_previousTarget == _currentTarget && offset.magnitude < snapThreshold))
                        {
                            //   Debug.Log("Движение вдоль объекта с устранением пересечения");
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
                //Debug.Log("Отсоединение от объекта");
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

        private Vector3[] GetColliderCorners(Collider collider)
        {
            Vector3[] corners = new Vector3[8];

            MeshCollider meshCollider = collider as MeshCollider;
            BoxCollider boxCollider = collider as BoxCollider;

            if (meshCollider != null)
            {
                Mesh mesh = meshCollider.sharedMesh;
                if (mesh == null)
                {
                    return null;
                }

                Bounds meshBounds = mesh.bounds;
                Vector3 center = meshBounds.center;
                Vector3 extents = meshBounds.extents;

                Vector3[] localCorners = new Vector3[8];
                localCorners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
                localCorners[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
                localCorners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
                localCorners[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
                localCorners[4] = center + new Vector3(-extents.x, extents.y, -extents.z);
                localCorners[5] = center + new Vector3(extents.x, extents.y, -extents.z);
                localCorners[6] = center + new Vector3(extents.x, extents.y, extents.z);
                localCorners[7] = center + new Vector3(-extents.x, extents.y, extents.z);

                for (int i = 0; i < 8; i++)
                {
                    corners[i] = collider.transform.TransformPoint(localCorners[i]);
                }
            }

            return corners;
        }
    }
}