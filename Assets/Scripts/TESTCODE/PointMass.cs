using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointMass
{
    public string Name;
    public float Weight;
    public PointMass Parent;
    public Transform MassTransform;
    public Transform CameraSnapPoint;
}
