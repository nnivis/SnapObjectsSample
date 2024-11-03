using UnityEngine;

namespace CodeBase.Grid
{
    [CreateAssetMenu(fileName = "GridData", menuName = "Grid/GridData")]
    public class GridConfig : ScriptableObject
    {
        [SerializeField] private float _cellSize;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private GameObject _cellPrefab;

        public float CellSize => _cellSize;
        public int Width => _width;
        public int Height => _height;
        public GameObject CellPrefab => _cellPrefab;
    }
}