using CodeBase.Services.MagnetizationObject_2._0;
using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationHandler : MonoBehaviour
    {
        [SerializeField] private ObjectRotationPanel clickRotationPanel;
        [SerializeField] private ObjectRotationHoldPanel holdRotationPanel;
        [SerializeField] private bool _useHoldLogic;
        private ObjectRotation _activeObject;
        private ObjectRotationMediator _objectRotationMediator;
        private SnapObject _currentSnapObject;

        private void OnEnable()
        {
            _useHoldLogic = true;

            holdRotationPanel.OnHoldLeftButton += ApplyLeftHoldRotation;
            holdRotationPanel.OnHoldRightButton += ApplyRightHoldRotation;

            clickRotationPanel.OnClickedLeftButton += ApplyLeftRotation;
            clickRotationPanel.OnClickedRightButton += ApplyRightRotation;
        }

        private void OnDestroy()
        {
            holdRotationPanel.OnHoldLeftButton += ApplyLeftHoldRotation;
            holdRotationPanel.OnHoldRightButton += ApplyRightHoldRotation;

            clickRotationPanel.OnClickedLeftButton += ApplyLeftRotation;
            clickRotationPanel.OnClickedRightButton += ApplyRightRotation;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    ObjectRotation clickedObject = hit.collider.GetComponent<ObjectRotation>();

                    if (clickedObject != null || _activeObject == null)
                    {
                        UpdateActiveObject(clickedObject);
                    }
                }
            }
        }

        private void UpdateActiveObject(ObjectRotation newActiveObject)
        {
            if (_currentSnapObject != null)
            {
                _currentSnapObject.OnSnapStatus -= HandleSnapStatus;
                _currentSnapObject = null;
            }

            _activeObject = newActiveObject;
            if (_activeObject != null)
            {
                SnapObject snapObject = _activeObject.GetComponent<SnapObject>();
                if (snapObject != null)
                {
                    _currentSnapObject = snapObject;
                    _currentSnapObject.OnSnapStatus += HandleSnapStatus;
                }
            }
        }

        private void HandleSnapStatus(bool isSnapped) => _useHoldLogic = isSnapped;

        private void ApplyNewPositionForObject() => _currentSnapObject.UpdatePosition();

        private void ApplyRightRotation()
        {
            if (_activeObject != null && !_useHoldLogic)
            {
                _activeObject.SetRotationLogic(_useHoldLogic);
                _activeObject.ApplyRotateRight();
                ApplyNewPositionForObject();
            }
        }

        private void ApplyLeftRotation()
        {
            if (_activeObject != null && !_useHoldLogic)
            {
                _activeObject.SetRotationLogic(_useHoldLogic);
                _activeObject.ApplyRotateLeft();
                ApplyNewPositionForObject();
            }
        }

        private void ApplyRightHoldRotation()
        {
            if (_activeObject != null && _useHoldLogic)
            {
                _activeObject.SetRotationLogic(_useHoldLogic);
                _activeObject.ApplyRotateHoldRight();
            }
        }

        private void ApplyLeftHoldRotation()
        {
            if (_activeObject != null && _useHoldLogic)
            {
                _activeObject.SetRotationLogic(_useHoldLogic);
                _activeObject.ApplyRotateHoldLeft();
            }
        }

        /*private void NotifyObjectRotation()
        {
            _objectRotationMediator.NotifyObjectRotationChange(_activeObject);
        }*/
    }
}