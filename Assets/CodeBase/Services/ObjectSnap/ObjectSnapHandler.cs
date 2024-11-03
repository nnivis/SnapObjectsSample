using System.Collections.Generic;
using CodeBase.Grid;
using UnityEngine;

namespace CodeBase.Services.ObjectSnap
{
    public class ObjectSnapHandler : MonoBehaviour
    {
        [SerializeField] private List<ObjectSnapper> _objectSnappers;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _snapRadius = 1.5f;

        public void Initialize(GridMediator gridMediator)
        {
            foreach (var objectSnapper in _objectSnappers)
            {
                objectSnapper.Initialize(gridMediator, _layerMask, _snapRadius);
            }
        }
    }
}
