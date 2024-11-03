using CodeBase.Grid;
using CodeBase.Services.ObjectSnap;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class BootstrapComponents : MonoBehaviour
    {
        [SerializeField] private GridHandler _gridHandler;
        [SerializeField] private ObjectSnapHandler _objectSnapperHandler;
        
        private void Start()
        {
            InitializeGrid();
            InitializeObjectSnapper();
        }

        private void InitializeGrid() => _gridHandler.Initialize();

        private void InitializeObjectSnapper()
        {
            var gridMediator = new GridMediator(_gridHandler);
            _objectSnapperHandler.Initialize(gridMediator);
        }
    }
}
