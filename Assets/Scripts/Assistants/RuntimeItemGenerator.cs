using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeItemGenerator : MonoBehaviour
{
    public bool SPAWN;
    public int STACK_COUNT;
    public GameState GameState;
    public Character Character;
    public ItemObject Item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!SPAWN)
            return;

        SPAWN = false;

        if (Character == null || Item == null)
            return;

        RootOptions options = new RootOptions(ref GameState.ROOT_SO_INDEX, GameState.UIman.Inventories, Character.Slots.Inventory.ReturnEmptyIndex());
        Character.Slots.Inventory.VirtualParent.PushItemIntoOccupants((ItemObject)Item.GenerateRootObject(options));
    }
}
