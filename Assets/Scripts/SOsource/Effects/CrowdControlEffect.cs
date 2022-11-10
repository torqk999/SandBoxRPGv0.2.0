using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdControl", menuName = "ScriptableObjects/Effects/CrowdControl")]
public class CrowdControlEffect : BaseEffect
{
    [Header("CC Properties")]
    public CCstatus TargetCCstatus;

    public CrowdControlEffect(string name, CCstatus status, Sprite sprite = null) // Hard indefinite CC creation (ez death)
    {
        Name = name;
        Sprite = sprite;
        //Action = EffectAction.CROWD_CONTROL;
        TargetCCstatus = status;
    }
}
