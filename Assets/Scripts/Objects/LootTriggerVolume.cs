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

        UpdatePawnInteractions(other.gameObject.GetComponent<Pawn>(), true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (parent == null || other.tag != parent.TriggerTag)
            return;

        UpdatePawnInteractions(other.gameObject.GetComponent<Pawn>(), false);
    }

    void UpdatePawnInteractions(Pawn pawn, bool addToList)
    {
        if (pawn == null)
        {
            Debug.Log("wtf m8");
            return;
        }

        if (addToList)
            pawn.CurrentInteractions.Add(parent);
        else
            pawn.CurrentInteractions.Remove(parent);

        pawn.bTriggerStateChange = true;
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
