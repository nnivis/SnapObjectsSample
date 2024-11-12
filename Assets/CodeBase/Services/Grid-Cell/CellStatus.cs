using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CellStatus : MonoBehaviour
    {
        public bool IsOccupied { get; private set; }
        public Color occupiedColor = Color.yellow;
        public string otherTag = "Furniture";

        private Color _originalSurfaceColor;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _originalSurfaceColor = _meshRenderer.material.color;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsOccupied && other.tag == otherTag)
            {
                SetOccupied(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsOccupied && other.tag == otherTag)
            {
                SetOccupied(false);
            }
        }

        public void SetOccupied(bool occupied)
        {
            IsOccupied = occupied;
            _meshRenderer.material.color = occupied ? occupiedColor : _originalSurfaceColor;
        }
    }
}