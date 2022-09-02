using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //Debug.Log("I'm Triggered! SJW!!!");

        if (Parent == null ||
            Parent.Source.gameObject == other.gameObject)
            return;

        //Debug.Log("I have a parent");

        switch(other.tag)
        {
            case GlobalConstants.TAG_MOB: 
            case GlobalConstants.TAG_CHARACTER:
                Debug.Log("Character detected!");
                Character targetChar = other.gameObject.GetComponent<Character>();
                if (targetChar == null)
                    return;
                Parent.CurrentInteractions.Add(targetChar);
                break;

            case GlobalConstants.TAG_LOOT:
                Debug.Log("Loot detected!");
                GenericContainer targetContainer = other.gameObject.GetComponent<GenericContainer>();
                if (targetContainer == null)
                    return;
                Parent.CurrentInteractions.Add(targetContainer);
                break;
        }
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
