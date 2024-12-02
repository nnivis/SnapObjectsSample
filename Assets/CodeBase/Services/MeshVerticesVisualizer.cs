using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeshVerticesVisualizer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
            return;

        Vector3[] corners = GetBoundsCorners(collider);

        Gizmos.color = Color.green;
        foreach (var corner in corners)
        {
            Gizmos.DrawSphere(corner, 0.05f);
        }
    }

    private Vector3[] GetBoundsCorners(Collider collider)
    {
        Vector3[] corners = new Vector3[8];

        if (collider is MeshCollider meshCollider)
        {
            Mesh mesh = meshCollider.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("MeshCollider не имеет присвоенного меша.");
                return null;
            }

            Bounds meshBounds = mesh.bounds;

            Vector3 center = meshBounds.center;
            Vector3 size = meshBounds.size * 0.5f;

            Vector3[] localCorners = new Vector3[8];
            localCorners[0] = center + new Vector3(-size.x, -size.y, -size.z);
            localCorners[1] = center + new Vector3(size.x, -size.y, -size.z);
            localCorners[2] = center + new Vector3(size.x, -size.y, size.z);
            localCorners[3] = center + new Vector3(-size.x, -size.y, size.z);
            localCorners[4] = center + new Vector3(-size.x, size.y, -size.z);
            localCorners[5] = center + new Vector3(size.x, size.y, -size.z);
            localCorners[6] = center + new Vector3(size.x, size.y, size.z);
            localCorners[7] = center + new Vector3(-size.x, size.y, size.z);

            for (int i = 0; i < 8; i++)
            {
                corners[i] = collider.transform.TransformPoint(localCorners[i]);
            }
        }
        else
        {
          
            Debug.LogWarning("Коллайдер типа " + collider.GetType() + " не поддерживается.");
            return null;
        }

        return corners;
    }
}