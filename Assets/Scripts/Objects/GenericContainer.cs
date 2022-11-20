using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericContainer : MonoBehaviour,  Interaction
{
    public GameState GameState;
    public InteractData InteractData;
    public SlotPage Inventory;
    //public Inventory Inventory;
    public bool bIsLocked;
    public string TriggerTag;

    public bool bRequestViewing;

    public void Interact()
    {
        GameState.InteractWithContainer(this);
    }

    public InteractData GetInteractData()
    {
        return InteractData;
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
