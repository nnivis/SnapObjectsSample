using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Services.BoundsUtility
{
    public static class BoundsUtility
    {
        private static readonly Dictionary<GameObject, Bounds> boundsCache = new Dictionary<GameObject, Bounds>();

        public static Bounds GetObjectBounds(GameObject targetGameObject)
        {
            if (boundsCache.TryGetValue(targetGameObject, out var cachedBounds))
            {
                return cachedBounds;
            }

            Bounds calculatedBounds = CalculateBounds(targetGameObject);
            boundsCache[targetGameObject] = calculatedBounds;
            return calculatedBounds;
        }

        public static void ClearCache() => boundsCache.Clear();

        private static Bounds CalculateBounds(GameObject targetGameObject)
        {
            Bounds? combinedBounds = null;

            foreach (var meshFilter in targetGameObject.GetComponentsInChildren<MeshFilter>())
            {
                var meshBounds = meshFilter.sharedMesh.bounds;
                var worldBounds = TransformBounds(meshBounds, meshFilter.transform.localToWorldMatrix);

                combinedBounds = combinedBounds.HasValue
                    ? CombineBounds(combinedBounds.Value, worldBounds)
                    : worldBounds;
            }

            return combinedBounds ?? new Bounds();
        }

        private static Bounds TransformBounds(Bounds bounds, Matrix4x4 matrix)
        {
            var center = matrix.MultiplyPoint3x4(bounds.center);
            var extents = matrix.MultiplyVector(bounds.extents);

            return new Bounds(center, extents * 2);
        }

        private static Bounds CombineBounds(Bounds bounds1, Bounds bounds2)
        {
            bounds1.Encapsulate(bounds2.min);
            bounds1.Encapsulate(bounds2.max);
            return bounds1;
        }
    }
}