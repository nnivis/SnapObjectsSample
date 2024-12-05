using System;
using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationHoldPanel : MonoBehaviour
    {
        public event Action OnHoldLeftButton;
        public event Action OnHoldRightButton;

        private bool _isHoldingLeft;
        private bool _isHoldingRight;

        private void Update()
        {
            if (_isHoldingLeft)
                OnHoldLeftButton?.Invoke();


            if (_isHoldingRight)
                OnHoldRightButton?.Invoke();
        }

        public void StartHoldingLeft() => _isHoldingLeft = true;
        public void StopHoldingLeft() => _isHoldingLeft = false;
        public void StartHoldingRight() => _isHoldingRight = true;
        public void StopHoldingRight() => _isHoldingRight = false;
    }
}