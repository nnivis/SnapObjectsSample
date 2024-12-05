using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationPanel : MonoBehaviour
    {
        public event Action OnClickedLeftButton;
        public event Action OnClickedRightButton;
        
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;

        private void OnEnable()
        {
            leftButton.onClick.AddListener(OnClickLeftButton);
            rightButton.onClick.AddListener(OnClickRightButton);
        }
        
        private void OnDisable()
        {
        leftButton.onClick.RemoveListener(OnClickLeftButton);
        rightButton.onClick.RemoveListener(OnClickRightButton);
        }

        private void OnClickRightButton() => OnClickedRightButton?.Invoke();

        private void OnClickLeftButton() => OnClickedLeftButton?.Invoke();
    }
}
