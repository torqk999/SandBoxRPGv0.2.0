using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public string Name;
    public List<Character> Members;
    public List<Character> Foes;
    public Inventory PartyLoot;
    public Formation Formation;
    public Faction Faction;
    public int CurrentMemberIndex;

    public void SetupParty(GameState state, Faction faction, string name = "", Formation formation = null)
    {
        if (state == null)
        {
            Debug.Log("SetupParty failed!");
            return;
        }

        Faction = faction;
        Name = name == "" ? Faction.ToString() : name;
        Members = new List<Character>();
        Foes = new List<Character>();
        GameObject lootObject = state.UIman.GenerateInventoryPanel($"LootBag: {Name}");
        PartyLoot = lootObject.AddComponent<Inventory>();
        PartyLoot.SetupInventory(state, CharacterMath.PARTY_INVENTORY_MAX, Name);

         //= new Inventory(state, $"LootBag: {Name}");
    }
}