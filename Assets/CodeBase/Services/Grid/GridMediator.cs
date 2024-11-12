using UnityEngine;

namespace CodeBase.Grid
{
    public class GridMediator
    {
        private readonly GridHandler _gridHandler;
        
        public GridMediator(GridHandler gridHandler) => _gridHandler = gridHandler;

        public GridCell GetNearestAvailableCell(Vector3 position, float maxDistance) =>
            _gridHandler.GetNearestAvailableCell(position, maxDistance);

        public void OccupyCell(GridCell cell, GameObject containedObject) => _gridHandler.OccupyCell(cell, containedObject);

        public void FreeCell(GridCell cell) => _gridHandler.FreeCell(cell);

        public bool IsCellUnderCursorOccupied(Vector3 position) => _gridHandler.IsCellUnderCursorOccupied(position);
    }
}