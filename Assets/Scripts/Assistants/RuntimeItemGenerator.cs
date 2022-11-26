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

        RootOptions options = new RootOptions(GameState, Item, ref GameState.ROOT_SO_INDEX, Character.Slots.Inventory, GameState.UIman.Inventories.FindClosestEmptyIndex(0));
        GameState.UIman.Inventories.PushItemIntoOccupants((ItemObject)Item.GenerateRootObject(options));
    }
}
