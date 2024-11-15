using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    [RequireComponent(typeof(ObjectMagnetization))]
    [RequireComponent(typeof(MovableGridObject))]
    public class ObjectSnapController : MonoBehaviour
    {
        [SerializeField] private ObjectMagnetization _objectMagnetization;
        [SerializeField] private MovableGridObject _movableGridObject;
        private bool _isMagnetized;

        private void Awake()
        {
            _objectMagnetization = GetComponent<ObjectMagnetization>();
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

        public void UpdateCenterPosition() => _movableGridObject.UpdateCenterPosition();
    }
}