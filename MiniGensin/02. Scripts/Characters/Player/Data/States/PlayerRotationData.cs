using System;
using UnityEngine;

[Serializable]
public class PlayerRotationData 
{
    [field:SerializeField] public Vector3 targetRotationReachTime { get; private set; }
}
