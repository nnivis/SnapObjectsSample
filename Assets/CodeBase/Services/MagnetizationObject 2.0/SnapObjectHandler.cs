using System.Collections.Generic;
using CodeBase.Services.MagnetizationObject_2._0.Snap;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    [RequireComponent(typeof(RelativeRotator))]
    public class SnapObjectHandler : MonoBehaviour
    {
        [SerializeField] private RelativeRotator rotator;
        [SerializeField] private List<SnapObject> snapObjects;

        private void Start() => snapObjects.ForEach(snapObject => snapObject.Initialize(rotator));
    }
}