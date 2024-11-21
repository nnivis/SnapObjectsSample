using System;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    public class RelativeRotatorView : MonoBehaviour
    {
        public event Action OnClickedButton;

        [SerializeField] private Button button;
        [SerializeField] private RectTransform rectActiveView;
        private void OnEnable() => button.onClick.AddListener(OnClickButton);
        private void OnDisable() => button.onClick.RemoveListener(OnClickButton);
        public void UpdateView(bool isActive) => rectActiveView.gameObject.SetActive(isActive);
        private void OnClickButton() => OnClickedButton?.Invoke();
    }
}