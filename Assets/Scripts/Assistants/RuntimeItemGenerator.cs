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

        switch(Item)
        {
            case OneHand:
                Debug.Log("OneHand");
                Character.Inventory.PushItemIntoInventory(new OneHandWrapper((OneHand)Item, GameState.EQUIPMENT_INDEX, true));
                break;

            case OffHand:
                Debug.Log("OffHand");
                Character.Inventory.PushItemIntoInventory(new OffHandWrapper((OffHand)Item, GameState.EQUIPMENT_INDEX, true));
                break;

            case TwoHand:
                Debug.Log("TwoHand");
                Character.Inventory.PushItemIntoInventory(new TwoHandWrapper((TwoHand)Item, GameState.EQUIPMENT_INDEX, true));
                break;

            case Ring:
                Debug.Log("Ring");
                Character.Inventory.PushItemIntoInventory(new RingWrapper((Ring)Item, GameState.EQUIPMENT_INDEX, true));
                break;

            case Wearable:
                Debug.Log("Wearable");
                Character.Inventory.PushItemIntoInventory(new WearableWrapper((Wearable)Item, GameState.EQUIPMENT_INDEX, true));
                break;

            case Stackable:
                Debug.Log("Stackable");
                Character.Inventory.PushItemIntoInventory(new StackableWrapper((Stackable)Item), STACK_COUNT);
                break;

            case ItemObject:
                Debug.Log("ItemObject");
                //Character.Inventory.PushItemIntoInventory(new ItemWrapper(Item);
                break;
        }

        if (Item is Equipment)
            GameState.EQUIPMENT_INDEX++;
    }
}
