using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    public class RelativeRotator : MonoBehaviour
    {
        public bool IsRotatorActive => _isActive;
        
        [SerializeField] private RelativeRotatorView relativeRotatorView;
        private bool _isActive = false;
        public void Start()
        {
            relativeRotatorView.OnClickedButton += UpdateRotatorStatus;
            UpdateView();
        }
        
        public void OnDisable() => relativeRotatorView.OnClickedButton -= UpdateRotatorStatus;

        private void UpdateRotatorStatus()
        {
            _isActive = !_isActive;
            UpdateView();
        }

        private void UpdateView() => relativeRotatorView.UpdateView(_isActive);
    }
}