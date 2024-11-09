using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Grid
{
    public class GridHandler : MonoBehaviour
    {
        [SerializeField] private GridFactory _factory;
        [SerializeField] private Transform _parent;
        private GridCell[,] _gridCells;
        private float _cellSize;
        private List<GridCell> _availableCells;

        public void Initialize() => CreateGrid();

        public GridCell GetNearestAvailableCell(Vector3 position, float maxDistance)
        {
            float closerDistance = maxDistance;
            GridCell nearestCell = null;

            foreach (var availableCell in _availableCells)
            {
                if (!availableCell.occupied)
                {
                    float distance = Vector3.Distance(position, availableCell.centerPosition);

                    if (distance < closerDistance)
                    {
                        closerDistance = distance;
                        nearestCell = availableCell;
                    }
                }
            }
            return nearestCell;
        }

        public bool IsCellUnderCursorOccupied(Vector3 position)
        {
            foreach (var cell in _gridCells)
            {
                if (Vector3.Distance(position, cell.centerPosition) < _cellSize / 2)
                {
                    return cell.occupied;
                }
            }
            return false;
        }

        public void OccupyCell(GridCell cell, GameObject containedObject)
        {
            cell.occupied = true;
            _availableCells.Remove(cell);
            UpdatePositionObject(cell, containedObject);
        }

        private static void UpdatePositionObject(GridCell cell, GameObject containedObject)
        {
            float cellHeight = cell.visualRepresentation.GetComponent<Renderer>().bounds.size.y;
            float objectHeight = containedObject.GetComponent<Renderer>().bounds.size.y;
            containedObject.transform.position = cell.centerPosition + new Vector3(0, (cellHeight / 2 + objectHeight / 2), 0);
            
            containedObject.transform.SetParent(cell.visualRepresentation.transform);
        }

        public void FreeCell(GridCell cell)
        {
            if (cell != null)
            {
                cell.occupied = false;
                if (!_availableCells.Contains(cell))
                    _availableCells.Add(cell);
            }
        }
        private void CreateGrid()
        {
            _gridCells = _factory.GetGridCells(out _availableCells, _parent);
            _cellSize = _factory.GetCellSize();
        }
    }
}