using UnityEngine;

namespace CodeBase.Services.LightCalibration
{
    public class LightEmissiveController : MonoBehaviour
    {
        [SerializeField] private GameObject prefabExample; // delete this

        private LightCalibrationComponent _lightCalibration;
        private EmissiveCalibrationComponent _emissiveCalibration;

        private void Start()
        {
            _lightCalibration = new LightCalibrationComponent();
            _emissiveCalibration = new EmissiveCalibrationComponent();

            CalibrateModel(); // delete this
        }

        public void CalibrateModel() // CalibrateModel(GameObject prefabExample)
        {
            _lightCalibration.CalibrationLight(prefabExample);
            _emissiveCalibration.CalibrationEmissive(prefabExample);
        }
    }

    public class LightCalibrationComponent
    {
        private const float SpotLightCoefficient = 17.3f;
        private const float PointLightCoefficient = 170.75f;

        private const float ScalingFactor = 0.1f;
        private const float BaseRange = 1.5f;

        public void CalibrationLight(GameObject prefabLight)
        {
            if (prefabLight == null)
            {
                Debug.LogError("PrefabLightExample is not assigned!");
                return;
            }

            Light[] childLights = prefabLight.GetComponentsInChildren<Light>();

            foreach (Light light in childLights)
            {
                switch (light.type)
                {
                    case LightType.Spot:
                        CalibrateSpotLight(light);
                        break;

                    case LightType.Point:
                        CalibratePointLight(light);
                        break;
                    default:
                        Debug.LogWarning($"Unsupported light type: {light.type}");
                        break;
                }
            }
        }

        private void CalibrateSpotLight(Light light)
        {
            light.intensity /= SpotLightCoefficient;

            float rangeFactor = light.transform.localScale.z * 10f;
            light.range = rangeFactor;
        }

        private void CalibratePointLight(Light light)
        {
            light.intensity /= PointLightCoefficient;

            float range = BaseRange + Mathf.Sqrt(light.intensity) * ScalingFactor;
            light.range = range;
        }
    }

    public class EmissiveCalibrationComponent
    {
        private const float F = 2.0f; // Параметр для настройки интенсивности света
        private const string Emissivefactor = "emissiveFactor";
        private const string Emissivetexture = "emissiveTexture";

        public void CalibrationEmissive(GameObject prefabEmissive)
        {
            if (prefabEmissive == null) return;

            Renderer[] renders = prefabEmissive.GetComponentsInChildren<Renderer>();

            foreach (var render in renders)
            {
                foreach (var material in render.materials)
                {
                    if (material.HasProperty(Emissivefactor))
                    {
                        Color emissiveFactor = material.GetColor(Emissivefactor);
                        material.SetColor(Emissivefactor, emissiveFactor * F);
                    }

                    if (material.HasProperty(Emissivetexture))
                    {
                        Texture emissiveTexture = material.GetTexture(Emissivetexture);
                        if (emissiveTexture == null)
                        {
                            Texture2D dummyTexture = new Texture2D(1, 1);
                            dummyTexture.SetPixel(0, 0, Color.white);
                            dummyTexture.Apply();
                            material.SetTexture(Emissivetexture, dummyTexture);
                        }
                    }
                }
            }
        }
    }
}