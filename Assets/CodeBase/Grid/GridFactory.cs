using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Grid
{
    [CreateAssetMenu(fileName = "GridFactory", menuName = "Grid/GridFactory")]
    public class GridFactory : ScriptableObject
    {
        [SerializeField] private GridConfig _config;
        public float GetCellSize() => _config.CellSize;
        public GridCell[,] GetGridCells(out List<GridCell> availableCells, Transform parent)
        {
            GridCell[,] gridCells = new GridCell[_config.Width, _config.Height];
            availableCells = new List<GridCell>();

            for (int i = 0; i < _config.Width; i++)
            {
                for (int j = 0; j < _config.Height; j++)
                {
                    Vector3 cellCenter = new Vector3(i , 0, j ) * _config.CellSize + parent.position;

                    gridCells[i, j] = new GridCell
                    {
                        occupied = false,
                        centerPosition = cellCenter,
                        visualRepresentation = VisualizeCell(i, j, parent),
                    };
                    availableCells.Add(gridCells[i, j]);
                }
            }
            return gridCells;
        }
        private GameObject VisualizeCell(int i, int j, Transform parent)
        {
            var cell = Instantiate(_config.CellPrefab, new Vector3(i, 0, j) * _config.CellSize + parent.position, Quaternion.identity);
            cell.transform.SetParent(parent);
            return cell;
        }
    }
}