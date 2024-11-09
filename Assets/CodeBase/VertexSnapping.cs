using UnityEngine;

public class VertexSnapping : MonoBehaviour
{
    [SerializeField] private LayerMask _mask;
    private Vector3 nearestVertex;
    private Vector3 targetPosition;

    private void SnapToNearestVertex(Vector3 targetPosition)
    {
        float snapRange = 1.0f;
        nearestVertex = transform.position;
        float minDistance = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetPosition, snapRange, _mask);

        foreach (var collider in colliders)
        {
            MeshFilter filter = collider.GetComponent<MeshFilter>();

            if (filter != null)
            {
                Vector3[] vertices = filter.mesh.vertices;

                foreach (var vertex in vertices)
                {
                    Vector3 worldVertex = filter.transform.TransformPoint(vertex);
                    float distance = Vector3.Distance(worldVertex, targetPosition);

                    Debug.DrawLine(worldVertex, targetPosition, Color.red);

                    if (distance < minDistance && distance <= snapRange)
                    {
                        minDistance = distance;
                        nearestVertex = worldVertex;
                    }
                }
            }
        }

        if (minDistance < Mathf.Infinity)
        {
            nearestVertex.x = nearestVertex.x >= 0 ? Mathf.Ceil(nearestVertex.x) : Mathf.Floor(nearestVertex.x);
            nearestVertex.y = nearestVertex.y >= 0 ? Mathf.Ceil(nearestVertex.y) : Mathf.Floor(nearestVertex.y);
            nearestVertex.z = nearestVertex.z >= 0 ? Mathf.Ceil(nearestVertex.z) : Mathf.Floor(nearestVertex.z);

            transform.position = nearestVertex;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            targetPosition = transform.position;
            targetPosition.y -= 1;
            SnapToNearestVertex(targetPosition);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.1f); // Целевая позиция

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nearestVertex, 0.1f); // Ближайшая вершина

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, nearestVertex); // Линия до ближайшей вершины


        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null && filter.sharedMesh != null)
        {
            Vector3[] vertices = filter.sharedMesh.vertices;
            Gizmos.color = Color.yellow;
            foreach (var vertex in vertices)
            {
                Vector3 worldVertex = filter.transform.TransformPoint(vertex);
                Gizmos.DrawSphere(worldVertex, 0.05f);
            }
        }
    }
}