using CodeBase.Services.ObjectRotation;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class BootstrapComponents : MonoBehaviour
    {
        [SerializeField] private ObjectRotationHandler _objectRotationHandler;

        private void Start()
        {
             InitializeObjectRotation();
        }
        
      

        private void InitializeObjectRotation() => _objectRotationHandler.Initialize();
    }
}