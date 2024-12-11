using UnityEngine;

namespace CodeBase.Services.BoundsUtility
{
    public class BoundsExample : MonoBehaviour
    {
        [SerializeField] private GameObject targetObject;
        private Bounds _objectBounds;

        public void Start() => _objectBounds = BoundsUtility.GetObjectBounds(targetObject);

        private void OnDrawGizmos()
        {
            if (targetObject != null)
            {
                _objectBounds = BoundsUtility.GetObjectBounds(targetObject);

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_objectBounds.center, _objectBounds.size);
            }
        }
    }
}