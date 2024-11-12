using UnityEngine;
using System.Collections.Generic;

namespace CodeBase.Services.Grid_Cell
{
    public class GridPlacementHandler : MonoBehaviour
    {
        [SerializeField] private GridHandler _gridHandler;

        public void Initialize(GridHandler gridHandler)
        {
            _gridHandler = gridHandler;
        }

        public bool AreCellsOccupied(Vector3 position)
        {
            Transform[] cells = _gridHandler.GetCellsInRange(position, 1);

            foreach (Transform cell in cells)
            {
                CellStatus tileStatus = cell.GetComponent<CellStatus>();
                if (tileStatus != null && tileStatus.IsOccupied)
                {
                    return true;
                }
            }

            return false;
        }

        public bool OccupyCell(Transform cell, GameObject objectToPlace)
        {
            List<Transform> cellsToOccupy = GetCellsToOccupy(cell.position);

            foreach (Transform cellTransform in cellsToOccupy)
            {
                CellStatus tileStatus = cellTransform.GetComponent<CellStatus>();
                if (tileStatus != null && tileStatus.IsOccupied)
                {
                    return false;
                }
            }

            foreach (Transform cellTransform in cellsToOccupy)
            {
                CellStatus tileStatus = cellTransform.GetComponent<CellStatus>();
                if (tileStatus != null)
                {
                    tileStatus.SetOccupied(true);
                }

                AdjustObjectPosition(objectToPlace, cellTransform);
            }

            return true;
        }

        private void AdjustObjectPosition(GameObject objectToPlace, Transform cellTransform)
        {
            Vector3 newPosition = cellTransform.position;
            Collider objectCollider = objectToPlace.GetComponent<Collider>();
            Bounds objectBounds;

            if (objectCollider != null)
            {
                objectBounds = objectCollider.bounds;
            }
            else
            {
                Renderer objectRenderer = objectToPlace.GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    objectBounds = objectRenderer.bounds;
                }
                else
                {
                    return;
                }
            }

            if (cellTransform.CompareTag("Floor"))
            {
                float offsetToGround = objectBounds.min.y - objectToPlace.transform.position.y;
                newPosition.y = cellTransform.position.y - offsetToGround;
            }

            if (cellTransform.CompareTag("Wall"))
            {
                Vector3 wallNormal = cellTransform.forward;

                if (Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x))
                {
                    float offsetZ = objectBounds.min.z - objectToPlace.transform.position.z;
                    newPosition.z = cellTransform.position.z + Mathf.Sign(wallNormal.z) * objectBounds.extents.z -
                                    offsetZ;
                    newPosition.z += Mathf.Sign(wallNormal.z) * objectBounds.extents.z;
                }
                else
                {
                    float offsetX = objectBounds.max.x - objectToPlace.transform.position.x;
                    newPosition.x = cellTransform.position.x - Mathf.Sign(wallNormal.x) * objectBounds.extents.x -
                                    offsetX;
                    newPosition.x += Mathf.Sign(wallNormal.x) * objectBounds.extents.x;
                }


                float offsetToGroundY = objectBounds.min.y - objectToPlace.transform.position.y;
                newPosition.y = cellTransform.position.y - offsetToGroundY;
            }

            objectToPlace.transform.position = newPosition;
        }

        public List<Transform> GetCellsToOccupy(Vector3 position)
        {
            List<Transform> cellsToOccupy = new List<Transform>();

            Transform centralCell = _gridHandler.GetCellAtPosition(position);
            if (centralCell != null)
            {
                cellsToOccupy.Add(centralCell);
            }

            return cellsToOccupy;
        }

        public Transform GetNearestAvailableCell(Vector3 hitInfoPoint, float snapRadius)
        {
            Transform nearestCell = null;
            float minDistance = Mathf.Infinity;

            foreach (Transform cell in _gridHandler.GetAllCells())
            {
                float distance = Vector3.Distance(hitInfoPoint, cell.position);
                if (distance <= snapRadius)
                {
                    CellStatus tileStatus = cell.GetComponent<CellStatus>();
                    if (tileStatus != null && !tileStatus.IsOccupied && distance < minDistance)
                    {
                        nearestCell = cell;
                        minDistance = distance;
                    }
                }
            }

            return nearestCell;
        }

        public void FreeCell(Transform cell)
        {
            CellStatus tileStatus = cell.GetComponent<CellStatus>();
            if (tileStatus != null)
            {
                tileStatus.SetOccupied(false);
            }
        }
    }
}