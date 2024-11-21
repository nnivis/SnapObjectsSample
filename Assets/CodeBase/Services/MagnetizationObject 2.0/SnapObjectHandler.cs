using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Services.MagnetizationObject_2._0
{
    [RequireComponent(typeof(RelativeRotator))]
    public class SnapObjectHandler : MonoBehaviour
    {
        [SerializeField] private RelativeRotator _rotator;
        [SerializeField] private List<SnapObject> _snapObjects;

        private void Start()
        {
            foreach (SnapObject snapObject in _snapObjects)
            {
                snapObject.Initialize(_rotator);
            }
        }
    }
}