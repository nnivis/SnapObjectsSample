using TMPro;
using UnityEngine;

namespace CodeBase.UI.OrientationImageChecker
{
    public class AccelerationRotation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private RectTransform _rectTransform;

        private void Start() => _rectTransform = GetComponent<RectTransform>();

        private void FixedUpdate()
        {
            Vector3 acceleration = Input.acceleration;
            float zAngle = Mathf.Atan2(acceleration.x, -acceleration.y) * Mathf.Rad2Deg;
            zAngle = NormalizeAngle(zAngle);

            if (zAngle is > 300 and <= 360)
            {
                zAngle = 0;
            }
            
            if (zAngle is > 155 and < 170)
            {
                zAngle = 0;
            }

            float snappedRotation = SnapToNearest90Degree(zAngle);
            _rectTransform.localRotation = Quaternion.Euler(0, 0, snappedRotation);

            if (_text != null)
            {
                _text.text = $"Accelerometer Z Angle: {zAngle:F2}°\nSnapped Rotation (Z): {snappedRotation}°";
            }
        }

        private float SnapToNearest90Degree(float angle)
        {
            float[] possibleAngles = {0, 90, 180, 270};
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