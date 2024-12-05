using System;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0.Rotator
{
    public class RelativeRotator : MonoBehaviour
    {
        public bool IsRotatorActive => _isActive;
        public event Action<bool> OnRotatorStatusChange;
        
        [SerializeField] private RelativeRotatorView relativeRotatorView;
        private bool _isActive;
        public void Start()
        {
            relativeRotatorView.OnClickedButton += UpdateRotatorStatus;
            UpdateView();
            OnRotatorStatusChange?.Invoke(!_isActive);
        }
        
        public void OnDisable() => relativeRotatorView.OnClickedButton -= UpdateRotatorStatus;

        private void UpdateRotatorStatus()
        {
            _isActive = !_isActive;
            UpdateView();
            
            OnRotatorStatusChange?.Invoke(!_isActive);
        }

        private void UpdateView() => relativeRotatorView.UpdateView(_isActive);
    }
}