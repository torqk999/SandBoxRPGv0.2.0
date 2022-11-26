using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public string Name;
    public GameState GameState;
    public List<RootScriptObject> MemberSheets;
    public List<Character> Foes;
    public List<RootScriptObject> PartyLoot;
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
        MemberSheets = new List<RootScriptObject>();//new RootPanel(0, GameState.UIman.Parties);
        Foes = new List<Character>();
        PartyLoot = new List<RootScriptObject>(CharacterMath.PARTY_INVENTORY_MAX);
        for (int i = 0; i < PartyLoot.Capacity; i++)
            PartyLoot.Add(null);
    }
}