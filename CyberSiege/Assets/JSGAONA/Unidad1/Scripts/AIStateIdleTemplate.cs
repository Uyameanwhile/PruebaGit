using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIdleTemplate", menuName = "CyberSiege/Behavior/Behavior Id")]
public abstract class AIStateIdleTemplate : ScriptableObject
{
    [Range(20f, 180)] public float ViewAngle = 60;
    public float DetectionDistance = 15;
    public float AutoAggroDistance = 5;
}
