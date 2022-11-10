using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "PassiveEffect", menuName = "ScriptableObjects/Effect/Passive")]
/*public class PassiveEffect : BaseEffect
{
    [Header("Passive Properties")]
    public int EquipID;

    public PassiveEffect(string name, CCstatus status, Sprite sprite = null) // Hard indefinite CC creation (ez death)
    {
        Name = name;
        Sprite = sprite;
        Action = EffectAction.CROWD_CONTROL;
        TargetCCstatus = status;
    }

    public override void CloneEffect(BaseEffect effect, int equipId = -1, float amp = 1, bool inject = true)
    {
        base.CloneEffect(effect, equipId = -1, amp = 1, inject = true);

        if (!(effect is PassiveEffect))
            return;

        EquipID = equipId;
    }
}*/
