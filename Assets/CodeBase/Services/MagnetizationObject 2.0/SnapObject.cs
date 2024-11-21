using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    [RequireComponent((typeof(BoxCollider)))]
    public class SnapObject : MonoBehaviour
    {
        public event Action<bool> OnSnapStatus;
        public event Action<Collider> OnSnap;
        public event Action<Collider, bool> OnUnsnap;

        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float snapDistance = 1f;
        [SerializeField] private float searchRadius = 5f;

        private RelativeRotator _rotator;
        private Camera _mainCamera;
        private Vector3 _offset;
        private bool _isDragging;
        private Collider _myCollider;
        private Collider _currentTarget;
        private Collider _previousTarget;

        public void Initialize(RelativeRotator relativeRotator)
        {
            _rotator = relativeRotator;
            _mainCamera = Camera.main;
            _myCollider = GetComponent<Collider>();
            DisableSnapStatus();
        }

        private void Update()
        {
            if (_isDragging)
            {
                MoveObject();
                FindClosestTarget();
                HandleSnap();
            }
            else
            {
                CheckIntersection();
            }
        }

        public void UpdatePosition() => HandleSnap();

        private void OnMouseDown()
        {
            _isDragging = true;
            _offset = transform.position - GetMouseWorldPos();
        }

        private void OnMouseUp() => _isDragging = false;

        private void MoveObject()
        {
            Vector3 newPosition = GetMouseWorldPos() + _offset;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }

        private void HandleSnap()
        {
            if (_currentTarget != null && _myCollider != null)
            {
                Vector3 myClosestPoint = _myCollider.ClosestPoint(_currentTarget.transform.position);
                Vector3 targetClosestPoint = _currentTarget.ClosestPoint(myClosestPoint);
                Vector3 offset = targetClosestPoint - myClosestPoint;

                float snapThreshold = snapDistance;
                float unsnapThreshold = snapDistance * 1.1f;

                if (offset.magnitude < snapThreshold)
                {
                    transform.position += offset;

                    if (_previousTarget != _currentTarget ||
                        _previousTarget == _currentTarget && offset.magnitude < snapDistance)
                    {
                        OnSnap?.Invoke(_currentTarget);
                        EnableSnapStatus();
                        _previousTarget = _currentTarget;
                    }
                }
                else if (offset.magnitude > unsnapThreshold)
                {
                    if (_previousTarget != null)
                    {
                        OnUnsnap?.Invoke(_previousTarget, _rotator.IsRotatorActive);
                        DisableSnapStatus();
                        _previousTarget = null;
                    }
                }
            }
            else if (_previousTarget != null)
            {
                OnUnsnap?.Invoke(_previousTarget, _rotator.IsRotatorActive);
                DisableSnapStatus();
                _previousTarget = null;
            }
        }

        private void DisableSnapStatus() => OnSnapStatus?.Invoke(true);
        private void EnableSnapStatus() => OnSnapStatus?.Invoke(false);

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = _mainCamera.WorldToScreenPoint(transform.position).z;
            return _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }

        private void CheckIntersection()
        {
            if (_currentTarget != null && CheckOverlap())
            {
                ResolveOverlap();
            }
        }

        private bool CheckOverlap() => _currentTarget != null && _myCollider.bounds.Intersects(_currentTarget.bounds);

        private void ResolveOverlap()
        {
            if (_currentTarget == null) return;

            bool isOverlap = Physics.ComputePenetration(
                _myCollider, transform.position, transform.rotation,
                _currentTarget, _currentTarget.transform.position, _currentTarget.transform.rotation,
                out var direction, out var distance);

            if (isOverlap)
            {
                transform.position += direction * distance;
            }
        }

        private void FindClosestTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, targetLayerMask);
            float closestDistance = float.MaxValue;
            Collider closestCollider = null;

            foreach (var collider in colliders)
            {
                if (collider == _myCollider) continue;

                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
            _currentTarget = closestCollider;
        }
    }
}