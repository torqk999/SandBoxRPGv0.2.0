using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThreatType
{
    Stat_Relative,
    Stat_Missing,
    Res,
    CC,
    Immunity,
    Proximity
}
public enum ThreatRamp
{
    flat,
    percent
}
public struct ThreatCondition
{
    public bool RampUp;
    public float RampValue;
    public float ThreatValue;

    public ThreatType Type;
    public ThreatRamp Ramp;
}

public class ThreatSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
