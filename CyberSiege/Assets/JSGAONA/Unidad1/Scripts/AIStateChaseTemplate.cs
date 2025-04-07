using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChaseTemplate", menuName = "CyberSiege/Behavior/Behavior Id")]
public class AIStateChaseTemplate : AIStateIdle
{
    public float LeaveDistance = 10;
    [Range(0.1f, 5.0f)] public float MovementSpeedModifier = 1.0f;
}
