using CodeBase.Services.Grid_Cell;
using CodeBase.Services.ObjectRotation;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class BootstrapComponents : MonoBehaviour
    {
        [SerializeField] private ObjectRotationHandler _objectRotationHandler;
        [SerializeField] private GridHandler _gridHandler;
        [SerializeField] private GridPlacementHandler _placementHandler;

        private void Start()
        {
             InitializeObjectRotation();
             InitializeGridHandler();
             InitializePlacement();
        }
        
        private void InitializeObjectRotation() => _objectRotationHandler.Initialize();
        private void InitializeGridHandler() => _gridHandler.Initialize();
        private void InitializePlacement() => _placementHandler.Initialize(_gridHandler);
    }
}