using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    [RequireComponent(typeof(BoxCollider))]
    public class SnapObject : MonoBehaviour
    {
        public event Action<Collider> OnSnap;
        public event Action<Collider> OnUnsnap;

        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float snapDistance = 1f;
        [SerializeField] private float searchRadius = 5f;
        [SerializeField] private float cornerSnapDistance = 0.2f;

        private Camera _mainCamera;
        private Vector3 _offset;
        private bool _isDragging;
        private Collider _myCollider;
        private Collider _currentTarget;
        private Collider _previousTarget;
        private bool _isSnappedToCorner = false;
        private bool _isSnappedToSide = false;

        private void Start()
        {
            _mainCamera = Camera.main;
            _myCollider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (_isDragging)
            {
                MoveObject();

                FindClosestTarget();
                CheckCornerSnapping();
                HandleSnap();
            }
            else
            {
                CheckIntersection();
            }

            // DebugSnappingState();
        }

        private void OnMouseDown()
        {
            _isDragging = true;
            _offset = transform.position - GetMouseWorldPos();

            if (_isSnappedToCorner || _isSnappedToSide)
            {
                OnUnsnap?.Invoke(_currentTarget);
            }

            _isSnappedToCorner = false;
            _isSnappedToSide = false;
        }


        private void OnMouseUp()
        {
            _isDragging = false;

            if (_isSnappedToCorner || _isSnappedToSide)
            {
                OnSnap?.Invoke(_currentTarget);
            }
            else
            {
                OnUnsnap?.Invoke(_currentTarget);
            }
        }


        private void MoveObject()
        {
            Vector3 newPosition = GetMouseWorldPos() + _offset;
            newPosition.y = transform.position.y;

            if (_isSnappedToCorner)
            {
                Vector3 closestCorner = GetClosestCorner(_currentTarget as BoxCollider);
                Vector3 direction = newPosition - closestCorner;
                float distance = direction.magnitude;

                if (distance > cornerSnapDistance * 1.1f)
                {
                    _isSnappedToCorner = false;
                }
                else
                {
                    newPosition = closestCorner + direction.normalized * Mathf.Min(distance, snapDistance);
                }
            }

            transform.position = newPosition;
        }

        private Vector3 GetClosestCorner(BoxCollider boxCollider)
        {
            Vector3[] corners = GetBoxColliderCorners(boxCollider);
            Vector3 closestCorner = corners[0];
            float closestDistance = Vector3.Distance(transform.position, corners[0]);

            for (int i = 1; i < corners.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, corners[i]);
                if (distance < closestDistance)
                {
                    closestCorner = corners[i];
                    closestDistance = distance;
                }
            }

            return closestCorner;
        }


        private void HandleSnap()
        {
            if (_isSnappedToCorner) return;

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

                    if (!_isSnappedToSide)
                    {
                        _isSnappedToSide = true;
                        OnSnap?.Invoke(_currentTarget);
                    }
                }
                else if (offset.magnitude > unsnapThreshold && _isSnappedToSide)
                {
                    _isSnappedToSide = false;
                    OnUnsnap?.Invoke(_currentTarget);
                }
            }
            else if (_isSnappedToSide)
            {
                _isSnappedToSide = false;
                OnUnsnap?.Invoke(_currentTarget);
            }
        }

        private void CheckCornerSnapping()
        {
            if (_currentTarget != null)
            {
                Vector3[] myCorners = GetBoxColliderCorners(_myCollider as BoxCollider);
                Vector3[] targetCorners = GetBoxColliderCorners(_currentTarget as BoxCollider);

                foreach (var myCorner in myCorners)
                {
                    foreach (var targetCorner in targetCorners)
                    {
                        float distance = Vector3.Distance(myCorner, targetCorner);
                        if (distance < cornerSnapDistance)
                        {
                            Vector3 offset = targetCorner - myCorner;
                            transform.position += offset;

                            if (!_isSnappedToCorner)
                            {
                                _isSnappedToCorner = true;
                                _isSnappedToSide = false;
                                OnSnap?.Invoke(_currentTarget);
                            }

                            return;
                        }
                    }
                }

                if (_isSnappedToCorner)
                {
                    float maxDistance = 0f;
                    foreach (var myCorner in myCorners)
                    {
                        foreach (var targetCorner in targetCorners)
                        {
                            float distance = Vector3.Distance(myCorner, targetCorner);
                            if (distance > maxDistance)
                            {
                                maxDistance = distance;
                            }
                        }
                    }

                    if (maxDistance > cornerSnapDistance * 1.1f)
                    {
                        _isSnappedToCorner = false;
                        OnUnsnap?.Invoke(_currentTarget);
                    }
                }
            }
        }


        private Vector3 GetMouseWorldPos()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = _mainCamera.WorldToScreenPoint(transform.position).z;
            return _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }

        private Vector3[] GetBoxColliderCorners(BoxCollider boxCollider)
        {
            Vector3[] corners = new Vector3[8];
            Transform t = boxCollider.transform;
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size * 0.5f;

            Vector3[] localCorners = new Vector3[]
            {
                new Vector3(-size.x, -size.y, -size.z),
                new Vector3(-size.x, -size.y, size.z),
                new Vector3(-size.x, size.y, -size.z),
                new Vector3(-size.x, size.y, size.z),
                new Vector3(size.x, -size.y, -size.z),
                new Vector3(size.x, -size.y, size.z),
                new Vector3(size.x, size.y, -size.z),
                new Vector3(size.x, size.y, size.z)
            };

            for (int i = 0; i < localCorners.Length; i++)
            {
                corners[i] = t.TransformPoint(center + localCorners[i]);
            }

            return corners;
        }

        private void CheckIntersection()
        {
            if (_currentTarget != null && CheckOverlap())
            {
                if (!_isSnappedToCorner && !_isSnappedToSide)
                {
                    ResolveOverlap();
                }
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

        /*private void DebugSnappingState()
        {
            Debug.Log($"Dragging: {_isDragging}, SnappedToCorner: {_isSnappedToCorner}, SnappedToSide: {_isSnappedToSide}, CurrentTarget: {_currentTarget}");
        }*/
    }
}