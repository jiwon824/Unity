using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CapsuleColliderUtility
{
    public CapsuleColliderData capsuleColliderData { get; private set; }
    [field: SerializeField] public DefaultColliderData defaultColliderData { get; private set; }
    [field: SerializeField] public SlopData slopData { get; private set; }

    public void Initialize(GameObject gameObject)
    {
        if (capsuleColliderData != null) return;
        capsuleColliderData = new CapsuleColliderData();

        capsuleColliderData.Initialize(gameObject);
    }

    public void CalculateCapsuleColliderDimensions()
    {
        SetCapsuleColliderRadius(defaultColliderData.radius);
        // we need to multiply this with our step height percentage, which goes from "0" to "1", where "1" is "100%".
        // So, multiply the Height by "* (1f - SlopeData.StepHeightPercentage)".
        // We add the "1f -" here because the "step height percentage" is the "percentage" we want to remove,
        // so removing "0.25", or "25%", means that our new "Height" should be "75%" of its default "Height",
        SetCapsuleColliderHeight(defaultColliderData.height * (1f-slopData.stepHeightPercentage));

        RecalculateCapsuleColliderCenter();
        
        float halfColliderHeight = capsuleColliderData.collider.height / 2f;
        if (halfColliderHeight < capsuleColliderData.collider.radius)
        {
            SetCapsuleColliderRadius(halfColliderHeight);
        }
        capsuleColliderData.UpdateColliderData();
    }


    public void SetCapsuleColliderRadius(float radius)
    {
        capsuleColliderData.collider.radius = radius;
    }
    public void SetCapsuleColliderHeight(float height)
    {
        capsuleColliderData.collider.height = height;
    }
    public void RecalculateCapsuleColliderCenter()
    {
        float colliderHeightDifference = defaultColliderData.height - capsuleColliderData.collider.height;

        // we're making it go up half of our Height, which is the same as adding our Top Height difference to the bottom.
        // Note that this "Collider.center" is measured in local space,
        // so we can assign our new center without transforming it into World Space, which if you ever need, you can do it by using the "transform.TransformPoint" method.
        Vector3 newColliderCenter = new Vector3(0f, defaultColliderData.centerY + (colliderHeightDifference / 2f), 0f);
        capsuleColliderData.collider.center = newColliderCenter;
    }
}
