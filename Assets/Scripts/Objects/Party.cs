using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public string Name;
    public GameState GameState;
    public List<Character> Members;
    public List<Character> Foes;
    public Page PartyLoot;
    public Formation Formation;
    public Faction Faction;
    public int CurrentMemberIndex;

    public void SetupParty(GameState state, Faction faction, string name = "")
    {
        if (state == null)
        {
            Debug.Log("SetupParty failed!");
            return;
        }

        GameState = state;
        Faction = faction;
        Name = name == "" ? Faction.ToString() : name;
        Members = new List<Character>();
        Foes = new List<Character>();
        PartyLoot = GameState.UIman.GeneratePage(SlotPageType.INVENTORY);

        /*GameObject lootObject = state.UIman.GenerateInventoryPanel(state.UIman.InventoriesContent, $"LootBag: {Name}");
        PartyLoot = lootObject.GetComponent<SlotPage>();
        PartyLoot.SetupInventory(state, CharacterMath.PARTY_INVENTORY_MAX, Name);

         = new Inventory(state, $"LootBag: {Name}");*/
    }
}