using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    public class GridHandler : MonoBehaviour
{
    public GridMaker[] grids;
    private List<CellStatus> _cell = new List<CellStatus>();

    private void Start()
    {
        foreach (GridMaker gridMaker in grids)
        {
            gridMaker.CreateGrid();
            InitializeGridCells(gridMaker);
            gridMaker.DestroyBound();
        }
    }

    private void InitializeGridCells(GridMaker gridMaker)
    {
        Transform gridContainer = gridMaker.GridContainer.transform;

        if (gridContainer != null)
        {
            foreach (Transform cell in gridContainer)
            {
                CellStatus tileStatus = cell.GetComponent<CellStatus>();
                if (tileStatus != null)
                {
                    _cell.Add(tileStatus);
                }
            }
        }
    }

    public Transform[] GetAllCells()
    {
        List<Transform> cellTransforms = new List<Transform>();
        foreach (CellStatus tile in _cell)
        {
            cellTransforms.Add(tile.transform);
        }

        return cellTransforms.ToArray();
    }

    public Transform[] GetCellsInRange(Vector3 position, float range)
    {
        List<Transform> cellsInRange = new List<Transform>();

        foreach (CellStatus tile in _cell)
        {
            Vector3 cellPosition = tile.transform.position;

            if (Vector3.Distance(position, cellPosition) <= range)
            {
                cellsInRange.Add(tile.transform);
            }
        }

        return cellsInRange.ToArray();
    }

    public Transform GetCellAtPosition(Vector3 position)
    {
        Transform closestCell = null;
        float minDistance = Mathf.Infinity;

        foreach (CellStatus tile in _cell)
        {
            float distance = Vector3.Distance(position, tile.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCell = tile.transform;
            }
        }

        return closestCell;
    }
}

}