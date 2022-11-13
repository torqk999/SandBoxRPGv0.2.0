using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public List<Character> Members;
    public List<Character> Foes;
    public Inventory PartyLoot;
    public Formation Formation;
    public Faction Faction;
    //public Character CurrentTargetMember;
    public int CurrentMemberIndex;

    public Party()
    {
        Members = new List<Character>();
        Foes = new List<Character>();
        PartyLoot = new Inventory();
    }
}