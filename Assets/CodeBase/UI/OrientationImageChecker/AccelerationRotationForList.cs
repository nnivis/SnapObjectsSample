﻿using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.UI.OrientationImageChecker
{
    public class AccelerationRotationForList : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> rectTransforms;

        private void LateUpdate()
        {
            Vector3 acceleration = Input.acceleration;
            float zAngle = Mathf.Atan2(acceleration.x, -acceleration.y) * Mathf.Rad2Deg;
            zAngle = NormalizeAngle(zAngle);

            if (zAngle is > 300 and <= 360)
            {
                zAngle = 0;
            }

            float snappedRotation = SnapToNearest90Degree(zAngle);

            foreach (RectTransform rectTransform in rectTransforms)
            {
                if (rectTransform != null)
                {
                    rectTransform.localRotation = Quaternion.Euler(0, 0, snappedRotation);
                }
            }
        }

        private float SnapToNearest90Degree(float angle)
        {
            float[] possibleAngles = {0, 90, 0, 270};
            float closest = possibleAngles[0];
            float minDifference = Mathf.Abs(angle - closest);

            foreach (float possibleAngle in possibleAngles)
            {
                float difference = Mathf.Abs(angle - possibleAngle);
                if (difference < minDifference)
                {
                    closest = possibleAngle;
                    minDifference = difference;
                }
            }

            return closest;
        }

        private float NormalizeAngle(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }
    }
}