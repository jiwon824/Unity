using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DefaultColliderData
{
    // The "Height" of our Character Model is known through its "Mesh Renderer" "bounds.size" variable.
    [field: SerializeField] public float height { get; private set; } = 1.8f;
    // The "CenterY" is simply half of that "Height"
    [field: SerializeField] public float centerY { get; private set; } = 0.9f;
    // "Radius" is something we've defined ourselves before.
    [field: SerializeField] public float radius { get; private set; } = 0.2f;

}
