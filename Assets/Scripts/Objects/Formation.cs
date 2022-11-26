using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FormationType
{
    CIRCLE,
    ROW,
    VAN,
    SPEAR
}

[System.Serializable]
public class Formation
{
    /* row
     * circle
     * vanguard
     * spearhead
     */
    public Party Parent;

    public FormationType Type;
    public bool bIsTightFormation;
    public float Displacement;

    public void PositionParty(Transform location)
    {
        if (Parent.MemberSheets.Count == 0) // Am I a joke to you?
            return;

        if (location == null)
            location = ((CharacterSheet)Parent.MemberSheets[0]).Posession.Root;

        if (Parent.MemberSheets.Count == 1) // -__-
        {
            ((CharacterSheet)Parent.MemberSheets[0]).Posession.Root.position = location.position;
            return;
        }

        if (Parent.MemberSheets.Count == 2) // "Hold my hand"
        {
            Vector3 uno = ((CharacterSheet)Parent.MemberSheets[0]).Posession.Root.position;
            Vector3 dos = ((CharacterSheet)Parent.MemberSheets[1]).Posession.Root.position;

            uno.x += Displacement / 2;
            dos.x -= Displacement / 2;

            ((CharacterSheet)Parent.MemberSheets[0]).Posession.Root.position = uno;
            ((CharacterSheet)Parent.MemberSheets[1]).Posession.Root.position = dos;
            return;
        }

        switch (Type) // More than two papples
        {
            case FormationType.CIRCLE:
                CircleFormation(location.position);
                break;
        }
    }
    void CircleFormation(Vector3 position)
    {
        float angleA = (2 * Mathf.PI) / Parent.MemberSheets.Count;
        float angleB = (Mathf.PI - angleA) / 2;
        float sideB = Displacement * (Mathf.Sin(angleB) / Mathf.Sin(angleA));

        angleA *= GlobalConstants.RAD_2_DEG;

        for(int i = 0; i < Parent.MemberSheets.Count; i++)
        {
            ((CharacterSheet)Parent.MemberSheets[i]).Posession.Root.position = position;
            ((CharacterSheet)Parent.MemberSheets[i]).Posession.Root.Rotate(0, i * angleA, 0);
            ((CharacterSheet)Parent.MemberSheets[i]).Posession.Root.position += ((CharacterSheet)Parent.MemberSheets[i]).Posession.Root.forward * sideB;
            //Debug.Log($"{Parent.Members[i].Source.name} : {Parent.Members[i].Source.position}");
        }
    }
}
