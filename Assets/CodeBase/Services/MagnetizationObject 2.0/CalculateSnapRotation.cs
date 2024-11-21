using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    public class CalculateSnapRotation
    {
        public Quaternion ApplyTargetObjectRotation(Transform sourceTransform, Transform targetTransform)
        {
            // Шаг 1: Определяем оси текущего объекта (положительные и отрицательные)
            Vector3[] myAxes =
            {
                sourceTransform.up, -sourceTransform.up,
                sourceTransform.right, -sourceTransform.right,
                sourceTransform.forward, -sourceTransform.forward
            };

            // Шаг 2: Определяем оси целевого объекта (положительные и отрицательные)
            Vector3[] targetAxes =
            {
                targetTransform.up, -targetTransform.up,
                targetTransform.right, -targetTransform.right,
                targetTransform.forward, -targetTransform.forward
            };

            // Шаг 3: Находим пару осей с наименьшим углом между ними
            float smallestAngle = Mathf.Infinity;
            Vector3 myClosestAxis = Vector3.zero;
            Vector3 targetClosestAxis = Vector3.zero;

            foreach (var myAxis in myAxes)
            {
                foreach (var targetAxis in targetAxes)
                {
                    float angle = Vector3.Angle(myAxis, targetAxis);
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        myClosestAxis = myAxis;
                        targetClosestAxis = targetAxis;
                    }
                }
            }

            // Шаг 4: Вычисляем поворот для совмещения первых осей
            Quaternion rotation1 = Quaternion.FromToRotation(myClosestAxis, targetClosestAxis);
            Quaternion newRotation = rotation1 * sourceTransform.rotation;

            Vector3[] myAxesAfterFirstRotation =
            {
                rotation1 * sourceTransform.up, -(rotation1 * sourceTransform.up),
                rotation1 * sourceTransform.right, -(rotation1 * sourceTransform.right),
                rotation1 * sourceTransform.forward, -(rotation1 * sourceTransform.forward)
            };


            smallestAngle = Mathf.Infinity;
            Vector3 mySecondAxis = Vector3.zero;
            Vector3 targetSecondAxis = Vector3.zero;

            foreach (var myAxis in myAxesAfterFirstRotation)
            {
                if (Vector3.Equals(myAxis, rotation1 * myClosestAxis) ||
                    Vector3.Equals(myAxis, -(rotation1 * myClosestAxis)))
                    continue;

                foreach (var targetAxis in targetAxes)
                {
                    if (Vector3.Equals(targetAxis, targetClosestAxis) || Vector3.Equals(targetAxis, -targetClosestAxis))
                        continue;

                    float angle = Vector3.Angle(myAxis, targetAxis);
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        mySecondAxis = myAxis;
                        targetSecondAxis = targetAxis;
                    }
                }
            }

            // Шаг 7: Вычисляем второй поворот для совмещения вторых осей
            Quaternion rotation2 = Quaternion.FromToRotation(mySecondAxis, targetSecondAxis);

            newRotation = rotation2 * newRotation;

            return newRotation;
        }
    }
}