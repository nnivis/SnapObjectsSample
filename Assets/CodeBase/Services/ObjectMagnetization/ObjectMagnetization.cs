using System;
using UnityEngine;

namespace CodeBase.Services.ObjectMagnetization
{
    public class ObjectMagnetization : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        [SerializeField] private float detectionRadius = 5f;

        private Vector3 _screenPoint;
        private Vector3 _offset;
        private Camera _camera;
        private bool _isDragging;
        private Collider _collider;
        private Collider _nearestCollider;
        private bool _isAttached;
        
        private bool _isXConditionMet;
        private bool _isZConditionMet;

        private void Awake()
        {
            _camera = Camera.main;
            _collider = GetComponent<Collider>();
            _isDragging = false;
        }

        private void Update()
        {
            FindNearestObject();
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
                Vector3 nearestCenter = _nearestCollider.bounds.center;
                Vector3 nearestExtents = _nearestCollider.bounds.extents;

                if (isColliding && !_isAttached)
                {
                    _isAttached = true;
                }

                if (_isAttached)
                {
                    bool movedAlongX = false;
                    bool movedAlongZ = false;

                    if (curPosition.x > nearestCenter.x + nearestExtents.x)
                    {
                        float currentObjectWidth = _collider.bounds.size.x;
                        float newPositionX = nearestCenter.x + nearestExtents.x + currentObjectWidth / 2;

                        curPosition.x = newPositionX;
                        movedAlongX = true; 
                    }

                    if (!movedAlongX && curPosition.z > nearestCenter.z + nearestExtents.z)
                    {
                        float currentObjectWidth = _collider.bounds.size.z;
                        float newPositionZ = nearestCenter.z + nearestExtents.z + currentObjectWidth / 2;

                        curPosition.z = newPositionZ;
                        movedAlongZ = true;  
                    }

                    if (movedAlongX || movedAlongZ)
                    {
                        FixedMove(curPosition);
                    }
                }

            }
            else
            {
                transform.position = curPosition;
                _isAttached = false; 
            }
        }

        private void FixedMove(Vector3 newPosition)
        {
            transform.position = newPosition;
        }
        private void FindNearestObject()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, _layer);
            float closestDistance = Mathf.Infinity;
            _nearestCollider = null;

            foreach (var col in colliders)
            {
                if (col == _collider) continue;

                float distance = Vector3.Distance(transform.position, col.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _nearestCollider = col;
                }
            }
        }
    }
}