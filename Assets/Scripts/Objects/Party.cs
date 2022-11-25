using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public string Name;
    public GameState GameState;
    public List<Character> Members;
    public List<Character> Foes;
    public RootPanel PartyLoot;
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
        PartyLoot = new RootPanel(CharacterMath.PARTY_INVENTORY_MAX, GameState.UIman.Inventories);
    }
}