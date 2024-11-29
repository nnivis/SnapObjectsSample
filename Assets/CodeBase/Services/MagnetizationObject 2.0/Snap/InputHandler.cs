using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0.Snap
{
    public class InputHandler
    {
        public bool IsDragging => _isDragging;

        private bool _isDragging;
        private Vector3 _offset;
        private readonly Camera _mainCamera;
        private readonly Transform _transform;

        public InputHandler(Camera mainCamera, Transform transform)
        {
            _mainCamera = mainCamera;
            _transform = transform;
        }

        public void OnMouseDown()
        {
            _isDragging = true;
            _offset = _transform.position - GetMouseWorldPos();
        }

        public void OnMouseUp() => _isDragging = false;

        public void MoveObject()
        {
            Vector3 newPosition = GetMouseWorldPos() + _offset;
            newPosition.y = _transform.position.y;
            _transform.position = newPosition;
        }

        public Vector3 GetMouseWorldPos()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = _mainCamera.WorldToScreenPoint(_transform.position).z;
            return _mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }
        
    }
}