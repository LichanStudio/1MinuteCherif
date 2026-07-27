using System;
using UnityEngine;

[Serializable]
public class SkillContext
{
    public float Range;
    public float Time;
    public int Count;
    public GameObject PrefabZone;
    public Vector3 InitialPosition = Vector3.zero;
    public Vector3 TargetPosition = Vector3.zero;
}