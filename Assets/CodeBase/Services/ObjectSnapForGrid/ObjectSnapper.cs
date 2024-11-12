using CodeBase.Grid;
using UnityEngine;

namespace CodeBase.Services.ObjectSnapForGrid
{
    public class ObjectSnapper : MonoBehaviour
    {
        private float _snapRadius;
        private LayerMask _gridLayerMask;
        private GridMediator _gridMediator;

        private GridCell _currentCell;
        private Vector3 _currentHitPoint;
        private bool _isDragging;

        public void Initialize(GridMediator gridMediator, LayerMask layerMask, float snapRadius)
        {
            _gridMediator = gridMediator;
            _gridLayerMask = layerMask;
            _snapRadius = snapRadius;
        }

        private void Update()
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
                if (_gridMediator.IsCellUnderCursorOccupied(hitInfo.point))
                {
                    return; 
                }
                
                GridCell nearestCell = _gridMediator.GetNearestAvailableCell(hitInfo.point, _snapRadius);
                
                if (nearestCell != null)
                {
                    if (_currentCell != null)
                    {
                        _gridMediator.FreeCell(_currentCell);
                    }
                    _gridMediator.OccupyCell(nearestCell, this.gameObject);
                    _currentCell = nearestCell; 
                }
            }
        }
        
    }
}