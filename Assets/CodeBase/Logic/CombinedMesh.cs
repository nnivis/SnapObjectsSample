using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CombinedMeshes : MonoBehaviour
{
    void Start()
    {
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            CombineMeshes();
        }
    }

    void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length + skinnedMeshRenderers.Length];
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<BlendShape> blendShapes = new List<BlendShape>();

        // Обработка MeshFilter
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            // Смещаем индексы вершин для каждого подмеша
            int vertexOffset = vertices.Count;
            vertices.AddRange(combine[i].mesh.vertices);
            foreach (int index in combine[i].mesh.GetIndices(0))
            {
                indices.Add(index + vertexOffset);
            }
        }

        // Обработка SkinnedMeshRenderer
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            SkinnedMeshRenderer skinnedRenderer = skinnedMeshRenderers[i];
            if (skinnedRenderer.sharedMesh != null)
            {
                combine[meshFilters.Length + i].mesh = skinnedRenderer.sharedMesh;
                combine[meshFilters.Length + i].transform = skinnedRenderer.transform.localToWorldMatrix;

                int vertexOffset = vertices.Count;
                vertices.AddRange(combine[meshFilters.Length + i].mesh.vertices);
                foreach (int index in combine[meshFilters.Length + i].mesh.GetIndices(0))
                {
                    indices.Add(index + vertexOffset);
                }

                // Сохраняем информацию о весах и индексах вершин
                for (int j = 0; j < skinnedRenderer.sharedMesh.blendShapeCount; j++)
                {
                    blendShapes.Add(new BlendShape
                    {
                        name = skinnedRenderer.sharedMesh.GetBlendShapeName(j),
                        weight = skinnedRenderer.GetBlendShapeWeight(j)
                    });
                }
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        combinedMesh.RecalculateNormals();

        // Обработка BlendShapes (при наличии)
        if (blendShapes.Count > 0)
        {
            foreach (var blendShape in blendShapes)
            {
                combinedMesh.AddBlendShapeFrame(blendShape.name, blendShape.weight, vertices.ToArray(), null, null);
            }
        }

        gameObject.AddComponent<MeshFilter>();
        gameObject.GetComponent<MeshFilter>().mesh = combinedMesh;

        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }

        gameObject.GetComponent<MeshCollider>().sharedMesh = combinedMesh;
    }

    private class BlendShape
    {
        public string name;
        public float weight;
    }

    // RePlay animator
    private Animation animationComponent;
    public string currentAnimation;
    public float currentAnimationTime;

    void OnDisable()
    {
        // Сохраняем текущее состояние анимации
        if (transform.GetChild(0).TryGetComponent(out animationComponent) && animationComponent.isPlaying)
        {
            AnimationState state = animationComponent[animationComponent.clip.name];
            currentAnimation = state.clip.name;
            currentAnimationTime = state.time;
        }
    }

    void OnEnable()
    {
        // Восстанавливаем анимацию
        if (!string.IsNullOrEmpty(currentAnimation) && animationComponent != null)
        {
            if (currentAnimation != "Static")
            {
                animationComponent.Stop();
                animationComponent[currentAnimation].time = currentAnimationTime;
                animationComponent[currentAnimation].wrapMode = WrapMode.Loop;
                animationComponent.Play(currentAnimation);
                animationComponent.clip = animationComponent[currentAnimation].clip;
                animationComponent.Sample();
            }
            else
            {
                animationComponent.Stop();
            }
        }
    }
}