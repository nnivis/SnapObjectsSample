using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationHandler : MonoBehaviour
    {
        [SerializeField] private ObjectRotationPanel _objectRotationPanel;
        [SerializeField] private ObjectRotation _objectRotation;
        private ObjectRotation _activeObject;

        public void Initialize()
        {
            _objectRotationPanel.OnClickedLeftButton += ApplyLeftRotation;
            _objectRotationPanel.OnClickedRightButton += ApplyRightRotation;
            
            _activeObject = _objectRotation;
        }
        private void ApplyRightRotation() => _activeObject.ApplyRotateRight();
        private void ApplyLeftRotation() => _activeObject.ApplyRotateLeft();
    }
}
