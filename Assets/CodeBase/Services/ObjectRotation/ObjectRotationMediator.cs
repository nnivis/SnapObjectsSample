using CodeBase.Services.Grid_Cell;
using UnityEngine;

namespace CodeBase.Services.ObjectRotation
{
    public class ObjectRotationMediator
    {
        public void NotifyObjectRotationChange(ObjectRotation objectRotation)
        {
            GameObject gameObject = objectRotation.gameObject;
            ObjectSnapController objectSnapController = gameObject.GetComponent<ObjectSnapController>();

            objectSnapController.UpdateCenterPosition();
        }
    }
}