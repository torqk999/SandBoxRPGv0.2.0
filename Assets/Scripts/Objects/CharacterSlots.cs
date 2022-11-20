using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSlots : MonoBehaviour
{
    public GameState GameState;
    public SlotPage Inventory;
    public SlotPage Equips;
    public SlotPage Rings;
    public SlotPage Abilities;

    public CharacterSlots(GameState state, GameObject invPrefab, GameObject equipPrefab, GameObject ringPrefab, GameObject abPrefab)
    {
        GameState = state;
        //Inventory = new SlotPage(CharacterMath.EQUIP_SLOTS_COUNT, parentContent, occupantContent);
        //Equips = new SlotPage(CharacterMath.EQUIP_SLOTS_COUNT, parentContent, occupantContent);
        //Rings = new SlotPage(CharacterMath.EQUIP_SLOTS_COUNT, parentContent, occupantContent);
        //Abilities = new SlotPage(CharacterMath.EQUIP_SLOTS_COUNT, parentContent, occupantContent);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
