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
        if (Parent == null)
            return;

        switch(other.tag)
        {
            case GlobalConstants.TAG_CHARACTER:
                Character targetChar = other.gameObject.GetComponent<Character>();
                if (targetChar == null)
                    return;
                Parent.CurrentInteractions.Add(targetChar);
                break;

            case GlobalConstants.TAG_LOOT:
                GenericContainer targetContainer = other.gameObject.GetComponent<GenericContainer>();
                if (targetContainer == null)
                    return;
                Parent.CurrentInteractions.Add(targetContainer);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Parent == null)
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
