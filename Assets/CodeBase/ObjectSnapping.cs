using UnityEngine;

namespace CodeBase
{
    public class ObjectSnapping : MonoBehaviour
    {
        public MeshCollider colliderTarget;
        public MeshCollider colliderElement1;
        public float snapDistance = 1;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                SnapToNearestBounds();
            }
        }

        private void SnapToNearestBounds()
        {
            Bounds boundsElement1 = colliderElement1.bounds;
            Bounds boundsTarget = colliderTarget.bounds;

            Vector3 closePoint = boundsElement1.ClosestPoint(boundsTarget.center);
            Vector3 targetClosePoint = boundsTarget.ClosestPoint(boundsElement1.center);
            Vector3 offset = targetClosePoint - closePoint;

            if (offset.magnitude < snapDistance)
            {
                transform.position += offset;
            }
        }

        public void OnDrawGizmosSelected()
        {
            var r = GetComponent<Renderer>();
            if (r == null)
                return;
            var bounds = r.bounds;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
        }
    }
}