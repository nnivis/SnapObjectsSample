using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationHandler : MonoBehaviour
    {
        [SerializeField] private ObjectRotationPanel _objectRotationPanel;
        private ObjectRotation _activeObject;

        public void Initialize()
        {
            _objectRotationPanel.OnClickedLeftButton += ApplyLeftRotation;
            _objectRotationPanel.OnClickedRightButton += ApplyRightRotation;
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
                        _activeObject = clickedObject;
                    }
                }
            }
        }
        
        private void ApplyRightRotation()
        {
            if (_activeObject != null)
                _activeObject.ApplyRotateRight();
        }

        private void ApplyLeftRotation()
        {
            if (_activeObject != null)
                _activeObject.ApplyRotateLeft();
        }
    }
}