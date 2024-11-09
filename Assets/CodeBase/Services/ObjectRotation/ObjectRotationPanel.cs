using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationPanel : MonoBehaviour
    {
        public event Action OnClickedLeftButton;
        public event Action OnClickedRightButton;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        private void OnEnable()
        {
            _leftButton.onClick.AddListener(OnClickLeftButton);
            _rightButton.onClick.AddListener(OnClickRightButton);
        }
        
        private void OnDisable()
        {
        _leftButton.onClick.RemoveListener(OnClickLeftButton);
        _rightButton.onClick.RemoveListener(OnClickRightButton);
        }

        private void OnClickRightButton() => OnClickedRightButton?.Invoke();

        private void OnClickLeftButton() => OnClickedLeftButton?.Invoke();
    }
}
