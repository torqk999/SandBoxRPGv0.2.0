using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Set var Px = player.MapPos.X;
Set var Py ...
Set var Tx = target.MapPos.X;
Set var Ty ...

Set var Dx = Tx - Px;
Set var Dy = Ty - Py;
Set var Mag = ((Dx * Dx) + (Dy * Dy)) ^ 0.5;

////////

float Square(float x, float y)
{
    return ((Dx * Dx) + (Dy * Dy)) ^ 0.5;
}

/////////

Set var Mag = MathScript.Square(Tx - Px, Ty - Py);


#x#
xsx  = 10
#x#

x#x
#s#  = 14
x#x

x y

s l

(s * 14) + ((l - s) * 10)

*



                *

*/

public enum TriggerType
{
    NONE,
    DOOR,
    CONTAINER,
    LOOTBAG,
    HAZARD,
    CHARACTER
}

public class PawnTriggerVolume : MonoBehaviour
{
    public Character Parent;
    //public bool bHasAtleastOneTrigger;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I'm Triggered! SJW!!!");

        if (Parent == null ||
            Parent.Source.gameObject == other.gameObject)
            return;

        Debug.Log("I have a parent");

        switch(other.tag)
        {
            case GlobalConstants.TAG_MOB: 
            case GlobalConstants.TAG_CHARACTER:
                Debug.Log("Character detected!");
                Character targetChar = other.gameObject.GetComponent<Character>();
                if (targetChar == null)
                    return;
                Debug.Log($"{targetChar.name} added!");
                Parent.CurrentInteractions.Add(targetChar);
                break;

            case GlobalConstants.TAG_LOOT:
                Debug.Log("Loot detected!");
                GenericContainer targetContainer = other.gameObject.GetComponent<GenericContainer>();
                if (targetContainer == null)
                    return;
                Debug.Log($"{targetContainer} added!");
                Parent.CurrentInteractions.Add(targetContainer);
                break;
        }
        Debug.Log("Interaction added!");
        Parent.bTriggerStateChange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (Parent == null ||
            Parent.Source.gameObject == other.gameObject)
            return;

        switch(other.tag)
        {
            case GlobalConstants.TAG_CHARACTER:
                Character targetChar = other.gameObject.GetComponent<Character>();
                if (targetChar == null)
                    return;
                Parent.CurrentInteractions.Remove(targetChar);
                break;

            case GlobalConstants.TAG_LOOT:
                GenericContainer targetContainer = other.gameObject.GetComponent<GenericContainer>();
                if (targetContainer == null)
                    return;
                Parent.CurrentInteractions.Remove(targetContainer);
                break;
        }
        Debug.Log("Interaction removed!");
        Parent.bTriggerStateChange = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Parent.Source.position;
    }
}
