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
        if (Parent.Members.Count == 0) // Am I a joke to you?
            return;

        if (location == null)
            location = Parent.Members[0].Root;

        if (Parent.Members.Count == 1) // -__-
        {
            Parent.Members[0].Root.position = location.position;
            return;
        }

        if (Parent.Members.Count == 2) // "Hold my hand"
        {
            Vector3 uno = Parent.Members[0].Root.position;
            Vector3 dos = Parent.Members[1].Root.position;

            uno.x += Displacement / 2;
            dos.x -= Displacement / 2;

            Parent.Members[0].Root.position = uno;
            Parent.Members[1].Root.position = dos;
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
        float angleA = (2 * Mathf.PI) / Parent.Members.Count;
        float angleB = (Mathf.PI - angleA) / 2;
        float sideB = Displacement * (Mathf.Sin(angleB) / Mathf.Sin(angleA));

        angleA *= GlobalConstants.RAD_2_DEG;

        for(int i = 0; i < Parent.Members.Count; i++)
        {
            Parent.Members[i].Root.position = position;
            Parent.Members[i].Root.Rotate(0, i * angleA, 0);
            Parent.Members[i].Root.position += Parent.Members[i].Root.forward * sideB;
            //Debug.Log($"{Parent.Members[i].Source.name} : {Parent.Members[i].Source.position}");
        }
    }
}
