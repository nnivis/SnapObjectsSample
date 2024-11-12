using UnityEngine;

namespace CodeBase.Services.Grid_Cell
{
    public class GridMaker : MonoBehaviour
    {
        public GameObject GridContainer => _gridContainer;
        [SerializeField] private Vector3Int _matrix = new Vector3Int(5, 1, 5);
        [SerializeField] private GameObject _prefab;
        [SerializeField] private Renderer _bounds;
        private bool _scalePrefabToFitBounds = true;
        private float _spacing = 0.1f;
        private bool _destroyBounds = true;
        private Transform _boundingParent;
        private GameObject _gridContainer;

        public void CreateGrid()
        {
            if (_prefab == null || _bounds == null)
            {
                return;
            }

            _boundingParent = _bounds.transform.parent;
            Quaternion boundsRotation = _bounds.transform.rotation;

            _bounds.transform.parent = null;
            _bounds.transform.localEulerAngles = Vector3.zero;
            Vector3 max = _bounds.bounds.size;
            Vector3 centerposition = new Vector3(-max.x * .5f, -max.y * .5f, -max.z * .5f);

            float x_increment = max.x / _matrix.x;
            float y_increment = max.y / _matrix.y;
            float z_increment = max.z / _matrix.z;

            _gridContainer = new GameObject(this.name + "_container");

            Vector3 scale = Vector3.one;

            if (_scalePrefabToFitBounds)
            {
                float x = _prefab.transform.localScale.x * x_increment;
                if (x > _spacing)
                    x = x - _spacing;

                float y = _prefab.transform.localScale.y * y_increment;
                if (y > _spacing)
                    y = y - _spacing;


                float z = _prefab.transform.localScale.z * z_increment;
                if (z > _spacing)
                    z = z - _spacing;

                scale = new Vector3(x, y, z);
            }
        
            for (int z = 0; z < _matrix.z; z++)
            {
                for (int x = 0; x < _matrix.x; x++)
                {
                    for (int y = 0; y < _matrix.y; y++)
                    {
                        Vector3 p = new Vector3(x_increment * x, y_increment * y, z_increment * z) + centerposition;
                        GameObject go = Instantiate(_prefab, p, Quaternion.identity, _gridContainer.transform);
                        go.transform.localScale = scale;
                        go.name = _prefab.name + "_" + x.ToString() + y.ToString() + z.ToString();

                        Renderer r = go.GetComponent<Renderer>() as Renderer;
                        go.transform.localPosition += r.bounds.extents;
                    }
                }
            }
        
            _bounds.transform.rotation = boundsRotation;
        
            _gridContainer.transform.position = _bounds.transform.position;
            _gridContainer.transform.rotation = _bounds.transform.rotation;
        }

        public void DestroyBound()
        {
            if (_destroyBounds)
            {
                Destroy(_bounds.gameObject);
            }
            else
            {
                _bounds.transform.parent = _boundingParent;
            }
        }
    }
}