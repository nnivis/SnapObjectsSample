using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    public class MovableGridObject : MonoBehaviour
    {
        [SerializeField] private float _snapRadius;
        [SerializeField] private LayerMask _gridLayerMask;
        [SerializeField] private GridPlacementHandler _gridPlacementHandler;

        private List<Transform> _currentCells = new List<Transform>();
        private bool _isDragging;

        public void HandleMouseInputForGrid()
        {
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.transform == transform)
                {
                    _isDragging = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                SnapToNearestCell();
            }
        }

        private void SnapToNearestCell()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _gridLayerMask))
            {
                if (_gridPlacementHandler.AreCellsOccupied(hitInfo.point))
                {
                    return;
                }

                Transform nearestCell = _gridPlacementHandler.GetNearestAvailableCell(hitInfo.point, _snapRadius);

                if (nearestCell != null)
                {
                    FreeCurrentCells();

                    bool canOccupy = _gridPlacementHandler.OccupyCell(nearestCell, this.gameObject);

                    if (canOccupy)
                    {
                        _currentCells = _gridPlacementHandler.GetCellsToOccupy(nearestCell.position);
                    }
                }
            }
        }

        public void FreeCurrentCells()
        {
            foreach (Transform cell in _currentCells)
            {
                _gridPlacementHandler.FreeCell(cell);
            }

            _currentCells.Clear();
        }

        public void ResetDraggingState()
        {
            _isDragging = false;
            FreeCurrentCells();
        }

        private void OnDrawGizmos()
        {
            Collider objectCollider = GetComponent<Collider>();
            if (objectCollider != null)
            {
                Bounds objectBounds = objectCollider.bounds;
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(objectBounds.center, objectBounds.size);
            }
            else
            {
                Renderer objectRenderer = GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    Bounds objectBounds = objectRenderer.bounds;
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(objectBounds.center, objectBounds.size);
                }
            }
        }

        public void UpdateCenterPosition()
        {
            if (_currentCells.Any())
                _gridPlacementHandler.ApplyXZPositionOffset();
        }
    }
}