using UnityEngine;

namespace CodeBase.Services.ObjectMagnetization
{
    public class ObjectDrag : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;

        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Camera _camera;
        private bool _isDragging;
        private Collider _collider;

        private void Awake()
        {
            _camera = Camera.main;
            _collider = GetComponent<Collider>();
            _isDragging = false;
        }

        private void OnMouseDown()
        {
            var position = gameObject.transform.position;
            _screenPoint = _camera.WorldToScreenPoint(position);
            _offset = position -
                      _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                          _screenPoint.z));
            _isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (!_isDragging) return;

            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 curPosition = _camera.ScreenToWorldPoint(curScreenPoint) + _offset;
            curPosition.y = transform.position.y;

            if (IsIntersectingWithOtherBounds(curPosition))
            {
                return;
            }

            transform.position = curPosition;
        }

        private bool IsIntersectingWithOtherBounds(Vector3 targetPosition)
        {
            Collider[] hitColliders =
                Physics.OverlapBox(targetPosition, _collider.bounds.extents, Quaternion.identity, _layer);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider == _collider) continue;

                Bounds otherBounds = hitCollider.bounds;

                if (otherBounds.Intersects(_collider.bounds))
                {
                    return true;
                }
            }

            return false;
        }

        public void OnDrawGizmosSelected()
        {
            var r = GetComponent<Renderer>();
            if (r == null)
                return;
            var bounds = r.bounds;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }
    }
}