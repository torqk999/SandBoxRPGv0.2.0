using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTriggerVolume : MonoBehaviour
{
    public GenericContainer parent;

    private void OnTriggerEnter(Collider other)
    {
        if (parent == null || other.tag != parent.TriggerTag)
            return;

        UpdateCharacterInteractions(other.gameObject.GetComponent<Character>(), true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (parent == null || other.tag != parent.TriggerTag)
            return;

        UpdateCharacterInteractions(other.gameObject.GetComponent<Character>(), false);
    }

    void UpdateCharacterInteractions(Character character, bool addToList)
    {
        if (character == null)
        {
            Debug.Log("wtf m8");
            return;
        }

        if (addToList)
            character.CurrentProximityInteractions.Add(parent);
        else
            character.CurrentProximityInteractions.Remove(parent);

        character.CurrentTargetInteraction = parent;

        //pawn.bTriggerStateChange = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
