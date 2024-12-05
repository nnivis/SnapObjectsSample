using UnityEditor;
using UnityEngine;

namespace CodeBase.Services.LightCalibration
{
    public class ShaderPropertyInspector : MonoBehaviour
    {
        public Material material;

        void Start()
        {
            if (material == null)
            {
                Debug.LogError("Material is not assigned!");
                return;
            }

            Shader shader = material.shader;
            Debug.Log($"Shader: {shader.name}");

            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                string propertyName = ShaderUtil.GetPropertyName(shader, i);
                ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(shader, i);

                Debug.Log($"Property: {propertyName}, Type: {propertyType}");
            }
        }
    }
}