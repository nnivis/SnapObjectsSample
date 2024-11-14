using UnityEngine;
using System.Collections.Generic;

namespace CodeBase.Services.Grid_Cell
{
    public class GridPlacementHandler : MonoBehaviour
    {
        private GridHandler _gridHandler;
        private Vector3 _newPosition;
        private GameObject _objectToPlace;
        private Bounds _objectBounds;
        private Transform _cellTransform;

        private float _savedZPosition;
        private float _savedXPosition;

        public void Initialize(GridHandler gridHandler) => _gridHandler = gridHandler;

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

        private void AdjustObjectPosition(GameObject objectToPlace, Transform cellTransform, bool? _isStart = null)
        {
            _objectToPlace = objectToPlace;
            _cellTransform = cellTransform;
            _newPosition = _cellTransform.position;

            Collider objectCollider = objectToPlace.GetComponent<Collider>();
            if (objectCollider != null)
            {
                _objectBounds = objectCollider.bounds;
            }
            else
            {
                Renderer objectRenderer = objectToPlace.GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    _objectBounds = objectRenderer.bounds;
                }
                else
                {
                    return;
                }
            }


            if (cellTransform.CompareTag("Floor"))
            {
                float offsetToGroundY = _objectBounds.min.y - objectToPlace.transform.position.y;

                _newPosition.y = cellTransform.position.y - offsetToGroundY;

                Vector3 xzOffset = CalculateXZPositionOffset(objectToPlace, _objectBounds, cellTransform);
                _newPosition.x = xzOffset.x;
                _newPosition.z = xzOffset.z;
            }

            if (cellTransform.CompareTag("Wall"))
            {
                Vector3 wallNormal = cellTransform.forward;

                if (_isStart == false)
                {
                    if (Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x))
                        _newPosition.z = _savedZPosition;
                    else
                        _newPosition.x = _savedXPosition;
                }
                else
                {
                    if (Mathf.Abs(wallNormal.z) > Mathf.Abs(wallNormal.x))
                    {
                        float offsetZ = _objectBounds.min.z - objectToPlace.transform.position.z;
                        _newPosition.z = cellTransform.position.z + Mathf.Sign(wallNormal.z) * _objectBounds.extents.z -
                                         offsetZ;
                        _newPosition.z += Mathf.Sign(wallNormal.z) * _objectBounds.extents.z;

                        _savedZPosition = _newPosition.z;
                    }
                    else
                    {
                        float offsetX = _objectBounds.max.x - objectToPlace.transform.position.x;
                        _newPosition.x = cellTransform.position.x - Mathf.Sign(wallNormal.x) * _objectBounds.extents.x -
                                         offsetX;
                        _newPosition.x += Mathf.Sign(wallNormal.x) * _objectBounds.extents.x;

                        _savedXPosition = _newPosition.x;
                    }

                    float offsetToGroundY = _objectBounds.min.y - objectToPlace.transform.position.y;
                    _newPosition.y = cellTransform.position.y - offsetToGroundY;
                }
            }

            objectToPlace.transform.position = _newPosition;
        }

        public void ApplyXZPositionOffset()
        {
            AdjustObjectPosition(_objectToPlace, _cellTransform, false);
        }

        private Vector3 CalculateXZPositionOffset(GameObject objectToPlace, Bounds objectBounds,
            Transform cellTransform)
        {
            float offsetToGroundX = objectBounds.center.x - objectToPlace.transform.position.x;
            float offsetToGroundZ = objectBounds.center.z - objectToPlace.transform.position.z;
            Vector3 xzOffset = new Vector3(cellTransform.position.x - offsetToGroundX, 0,
                cellTransform.position.z - offsetToGroundZ);

            return xzOffset;
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