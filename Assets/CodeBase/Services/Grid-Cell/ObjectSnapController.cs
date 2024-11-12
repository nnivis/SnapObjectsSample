using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    public class ObjectSnapController : MonoBehaviour
    {
        [SerializeField] private ObjectMagnetization _objectMagnetization;
        [SerializeField] private MovableGridObject _movableGridObject;

        private bool _isMagnetized = false;

        private void Awake()
        {
            if (_objectMagnetization == null)
                _objectMagnetization = GetComponent<ObjectMagnetization>();

            if (_movableGridObject == null)
                _movableGridObject = GetComponent<MovableGridObject>();
        }

        private void Update()
        {
            _objectMagnetization.HandleMouseInputForObject();
            bool wasMagnetized = _isMagnetized;
            _isMagnetized = _objectMagnetization.IsAttached();

            if (!wasMagnetized && _isMagnetized && _movableGridObject != null)
            {
                _movableGridObject.FreeCurrentCells();
            }

            if (wasMagnetized && !_isMagnetized)
            {
                _objectMagnetization.DisableMagnetization();
                _movableGridObject.ResetDraggingState();
            }

            if (!_isMagnetized)
            {
                _movableGridObject.HandleMouseInputForGrid();
            }
        }
    }
}