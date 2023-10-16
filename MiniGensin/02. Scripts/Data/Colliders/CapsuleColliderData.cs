using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleColliderData
{
    public CapsuleCollider collider { get; private set; }
    public Vector3 colliderCenterInLocalSpace { get; private set; }

    public void Initialize(GameObject gameObject)
    {
        // it means it's already initialized
        if (collider != null)
        {
            return;
        }
        collider = gameObject.GetComponent<CapsuleCollider>();
        /*
        For our Capsule Center in local space we have two ways of doing it.
        1. "transform.InverseTransformPoint()" method
            to transform a world space position to a local space position.
        However, the "Capsule Collider" already contains a "center" variable in local space.
        */
        UpdateColliderData();

    }

    public void UpdateColliderData()
    {
        colliderCenterInLocalSpace = collider.center;
    }
}
