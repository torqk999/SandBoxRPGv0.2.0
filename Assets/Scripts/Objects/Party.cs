using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public List<Character> Members;
    public Inventory PartyLoot;
    public Formation Formation;
    public Faction Faction;
    public int CurrentMemberIndex;

    public Party()
    {
        Members = new List<Character>();
        PartyLoot = new Inventory();
    }
}