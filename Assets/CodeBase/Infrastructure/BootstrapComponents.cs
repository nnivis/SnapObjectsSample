using CodeBase.Grid;
using CodeBase.Services.ObjectRotation;
using CodeBase.Services.ObjectSnap;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class BootstrapComponents : MonoBehaviour
    {
        [SerializeField] private GridHandler _gridHandler;
        [SerializeField] private ObjectSnapHandler _objectSnapperHandler;
        [SerializeField] private ObjectRotationHandler _objectRotationHandler;

        private void Start()
        {
            InitializeGrid();
            InitializeObjectSnapper();
            InitializeObjectRotation();
        }
        
        private void InitializeGrid() => _gridHandler.Initialize();

        private void InitializeObjectSnapper()
        {
            var gridMediator = new GridMediator(_gridHandler);
            _objectSnapperHandler.Initialize(gridMediator);
        }

        private void InitializeObjectRotation() => _objectRotationHandler.Initialize();
    }
}