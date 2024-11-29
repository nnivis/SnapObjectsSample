using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0.Snap
{
    [RequireComponent((typeof(BoxCollider)))]
    public class SnapObject : MonoBehaviour
    {
        public event Action<bool> OnSnapStatus;
        public event Action<Collider> OnSnap;
        public event Action<Collider, bool> OnUnsnap;

        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private float snapDistance = 2f;
        [SerializeField] private float searchRadius = 5f;

        private RelativeRotator _rotator;
        private Camera _mainCamera;
        private Collider _myCollider;
        private Collider _currentTarget;

        private InputHandler _inputHandler;
        private SnapHandler _snapHandler;
        private IntersectionResolver _intersectionResolver;
        private TargetFinder _targetFinder;

        private bool _isSnapped;

        public void Initialize(RelativeRotator relativeRotator)
        {
            _rotator = relativeRotator;
            _mainCamera = Camera.main;
            _myCollider = GetComponent<Collider>();

            _inputHandler = new InputHandler(_mainCamera, transform);
            _snapHandler = new SnapHandler(transform, _myCollider, _rotator, snapDistance);
            _intersectionResolver = new IntersectionResolver(transform, _myCollider);
            _targetFinder = new TargetFinder(transform, targetLayerMask, searchRadius, _myCollider);

            _snapHandler.OnSnapStatus += HandleSnapStatus;
            _snapHandler.OnSnap += RaiseSnapEvent;
            _snapHandler.OnUnsnap += RaiseUnsnapEvent;
            _snapHandler.SetSnapStatus(false);
        }

        private void HandleSnapStatus(bool isSnapped)
        {
            _isSnapped = isSnapped;
            OnSnapStatus?.Invoke(isSnapped);
        }

        private void Update()
        {
            if (_inputHandler.IsDragging)
            {
                if (!_isSnapped)
                {
                    _inputHandler.MoveObject();
                }
                else
                {
                    if (_currentTarget != null)
                    {
                        Vector3 cursorWorldPos = _inputHandler.GetMouseWorldPos();
                        float distanceToTarget = Vector3.Distance(cursorWorldPos, _currentTarget.transform.position);
                        
                        if (distanceToTarget > _snapHandler.GetUnsnapThreshold())
                        {
                            _snapHandler.Unsnap();
                        }
                        else
                        {
                            _inputHandler.MoveObject();
                            _snapHandler.HandleSnap();
                            
                        }
                    }
                }

                _currentTarget = _targetFinder.FindClosestTarget();
                _snapHandler.UpdateCurrentTarget(_currentTarget);
                _snapHandler.HandleSnap();
                _intersectionResolver.UpdateCurrentTarget(_currentTarget);
            }
            else
            {
                CheckIntersection();
            }
        }

        private void CheckIntersection() => _intersectionResolver.CheckIntersection();

        public void UpdatePosition()
        {
            _currentTarget = _targetFinder.FindClosestTarget();

            _snapHandler.UpdateCurrentTarget(_currentTarget);
            _snapHandler.HandleSnap();

            _intersectionResolver.UpdateCurrentTarget(_currentTarget);
        }

        private void OnMouseDown() => _inputHandler.OnMouseDown();
        private void OnMouseUp() => _inputHandler.OnMouseUp();

        private void RaiseUnsnapEvent(Collider collider, bool wasSuccessful) =>
            OnUnsnap?.Invoke(collider, wasSuccessful);

        private void RaiseSnapEvent(Collider collider) => OnSnap?.Invoke(collider);
    }

    public class IntersectionResolver
    {
        private readonly Collider _myCollider;
        private readonly Transform _transform;
        private Collider _currentTarget;

        public IntersectionResolver(Transform transform, Collider myCollider)
        {
            _transform = transform;
            _myCollider = myCollider;
        }

        public void UpdateCurrentTarget(Collider currentTarget)
        {
            _currentTarget = currentTarget;
        }

        public void CheckIntersection()
        {
            if (_currentTarget != null && CheckOverlap())
            {
                ResolveOverlap();
            }
        }

        private bool CheckOverlap()
        {
            return _currentTarget != null && _myCollider.bounds.Intersects(_currentTarget.bounds);
        }

        private void ResolveOverlap()
        {
            if (_currentTarget == null) return;

            bool isOverlap = Physics.ComputePenetration(
                _myCollider, _transform.position, _transform.rotation,
                _currentTarget, _currentTarget.transform.position, _currentTarget.transform.rotation,
                out var direction, out var distance);

            if (isOverlap)
            {
                _transform.position += direction * distance;
            }
        }
    }

    public class TargetFinder
    {
        private Transform _transform;
        private LayerMask _targetLayerMask;
        private float _searchRadius;
        private Collider _myCollider;

        public TargetFinder(Transform transform, LayerMask targetLayerMask, float searchRadius, Collider myCollider)
        {
            _transform = transform;
            _targetLayerMask = targetLayerMask;
            _searchRadius = searchRadius;
            _myCollider = myCollider;
        }

        public Collider FindClosestTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position, _searchRadius, _targetLayerMask);
            float closestDistance = float.MaxValue;
            Collider closestCollider = null;

            foreach (var collider in colliders)
            {
                if (collider == _myCollider) continue;

                float distance = Vector3.Distance(_transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }

            return closestCollider;
        }
    }
}