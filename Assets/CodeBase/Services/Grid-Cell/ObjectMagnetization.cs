using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    public class ObjectMagnetization : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;

        private Vector3[] _magnetPoints;
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Vector3 _nativeRotation;

        private Camera _camera;

        private bool _isDragging;
        private bool _isAttached;

        private Collider _collider;
        private Collider _nearestCollider;

        private void Awake()
        {
            _camera = Camera.main;
            _collider = GetComponent<MeshCollider>();
            _isDragging = false;
            _magnetPoints = new Vector3[0];
        }

        public bool IsAttached() => _isAttached;

        public void HandleMouseInputForObject() => HandleMouseInput();

        private void HandleMouseInput()
        {
            if (_nearestCollider != null)
            {
                GenerateMagnetPoints(_nearestCollider);
            }
        }

        private void OnMouseDown()
        {
            var position = gameObject.transform.position;
            _screenPoint = _camera.WorldToScreenPoint(position);
            _offset = position -
                      _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                          _screenPoint.z));
            _isDragging = true;
            _isAttached = false;

            SetNativeRotate();
            Debug.Log($"Update Rotation {_nativeRotation}");
        }


        private void OnMouseDrag()
        {
            if (!_isDragging) return;

            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint) + _offset;

            Collider[] colliders =
                Physics.OverlapBox(curPosition, _collider.bounds.extents, Quaternion.identity, _layer);
            bool isColliding = false;

            foreach (var collider in colliders)
            {
                if (collider != _collider)
                {
                    isColliding = true;
                    _nearestCollider = collider;
                    break;
                }
            }

            if (_nearestCollider != null)
            {
                Vector3 nearestMagnetPoint = FindNearestMagnetPoint(curPosition);

                if (isColliding && !_isAttached)
                {
                    _isAttached = true;
                }

                if (_isAttached)
                {
                    float distanceToMagnetPoint = Vector3.Distance(transform.position, nearestMagnetPoint);
                    float cursorDistanceFromMagnetPoint = Vector3.Distance(curPosition, nearestMagnetPoint);

                    float movementRadius = 5.0f;
                    float detachmentThreshold = 7.0f;

                    if (cursorDistanceFromMagnetPoint > detachmentThreshold)
                    {
                        _isAttached = false;
                    }
                    else if (distanceToMagnetPoint <= movementRadius)
                    {
                        FixedMove(nearestMagnetPoint);
                        FixedRotation(_nearestCollider);
                    }
                }
                else
                {
                    transform.position = curPosition;
                }
            }
            else
            {
                _isAttached = false;
            }
        }

        private void FixedMove(Vector3 nearestMagnetPoint)
        {
            if (_nearestCollider == null) return;

            Bounds targetBounds = _nearestCollider.bounds;
            Bounds objectBounds = _collider.bounds;

            Vector3 newPosition = nearestMagnetPoint;

            if (Mathf.Abs(nearestMagnetPoint.x - targetBounds.center.x) < (targetBounds.extents.x))
            {
                newPosition.x = targetBounds.center.x;
            }
            else
            {
                if (nearestMagnetPoint.x > targetBounds.center.x)
                {
                    newPosition.x = targetBounds.max.x + objectBounds.extents.x;
                }
                else
                {
                    newPosition.x = targetBounds.min.x - objectBounds.extents.x;
                }
            }

            if (Mathf.Abs(nearestMagnetPoint.z - targetBounds.center.z) < (targetBounds.extents.z))
            {
                newPosition.z = targetBounds.center.z;
            }
            else
            {
                if (nearestMagnetPoint.z > targetBounds.center.z)
                {
                    newPosition.z = targetBounds.max.z + objectBounds.extents.z;
                }
                else
                {
                    newPosition.z = targetBounds.min.z - objectBounds.extents.z;
                }
            }

            newPosition.y = transform.position.y;
            transform.position = newPosition;

            if (objectBounds.Intersects(targetBounds))
            {
                //  Debug.LogWarning("Объект пересекает границы, дополнительная корректировка может быть необходима.");
            }
        }

        private void FixedRotation(Collider nearestCollider)
        {
            if (nearestCollider == null) return;

            transform.rotation = nearestCollider.transform.rotation;
        }

        private void ApplyNativeRotation() => transform.eulerAngles = _nativeRotation;

        private void SetNativeRotate() => _nativeRotation = transform.eulerAngles;

        private void GenerateMagnetPoints(Collider targetCollider)
        {
            if (targetCollider == null)
            {
                return;
            }

            Bounds bounds;
            MeshCollider meshCollider = targetCollider as MeshCollider;
            if (meshCollider != null && meshCollider.sharedMesh != null)
            {
                bounds = meshCollider.bounds;
            }
            else
            {
                Renderer objectRenderer = targetCollider.GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    bounds = objectRenderer.bounds;
                }
                else
                {
                    return;
                }
            }

            _magnetPoints = new Vector3[8];

            _magnetPoints[0] = new Vector3(bounds.min.x, bounds.center.y, bounds.center.z); // Центр левой стороны
            _magnetPoints[1] = new Vector3(bounds.max.x, bounds.center.y, bounds.center.z); // Центр правой стороны
            _magnetPoints[2] = new Vector3(bounds.center.x, bounds.center.y, bounds.min.z); // Центр передней стороны
            _magnetPoints[3] = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z); // Центр задней стороны

          //  _magnetPoints[4] = new Vector3(bounds.min.x, bounds.center.y, bounds.min.z); // Левый передний угол
          //  _magnetPoints[5] = new Vector3(bounds.max.x, bounds.center.y, bounds.min.z); // Правый передний угол
          //  _magnetPoints[6] = new Vector3(bounds.min.x, bounds.center.y, bounds.max.z); // Левый задний угол
         //   _magnetPoints[7] = new Vector3(bounds.max.x, bounds.center.y, bounds.max.z); // Правый задний угол


            // _magnetPoints[8] = new Vector3(bounds.min.x, bounds.max.y, bounds.center.z); // Вверх левой стороны
            //  _magnetPoints[9] = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z); // Вверх правой стороны
            //   _magnetPoints[10] = new Vector3(bounds.center.x, bounds.max.y, bounds.min.z); // Вверх передней стороны
            //  _magnetPoints[11] = new Vector3(bounds.center.x, bounds.max.y, bounds.max.z); // Вверх задней стороны

            //   _magnetPoints[12] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z); // Вверхний Левый передний угол
            //  _magnetPoints[13] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z); // Вверхний Правый передний угол
            // _magnetPoints[14] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z); // Вверхний Левый задний угол
            //  _magnetPoints[15] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z); // Вверхний Правый задний угол
        }


        private Vector3 FindNearestMagnetPoint(Vector3 currentPosition)
        {
            if (_magnetPoints.Length == 0)
                return currentPosition;

            float closestDistance = Mathf.Infinity;
            Vector3 nearestPoint = currentPosition;


            foreach (Vector3 point in _magnetPoints)
            {
                float distance = Vector3.Distance(currentPosition, point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestPoint = point;
                }
            }

            return nearestPoint;
        }

        public void DisableMagnetization()
        {
            _isDragging = false;
            _isAttached = false;
            _nearestCollider = null;
        }

        private void OnDrawGizmos()
        {
            if (_magnetPoints == null || _magnetPoints.Length == 0)
                return;
            Gizmos.color = Color.red;
            foreach (var point in _magnetPoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}