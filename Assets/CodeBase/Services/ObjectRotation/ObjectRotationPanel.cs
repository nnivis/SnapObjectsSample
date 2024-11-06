using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationPanel : MonoBehaviour
    {

        //TODO: Кнопки должны зажиматся +++ и обратно
        
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

        private void OnClickRightButton() => OnClickedLeftButton?.Invoke();

        private void OnClickLeftButton() => OnClickedRightButton?.Invoke();
    }
}
