using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Race
{
    HUMAN,
    ORC,
    DWARF,
    TROLL,
    GNOME,
    ///
    HIGHELF,
    DARKELF,
    BLOODELF,
    ///
    LIKIN,
    REPTILIAN,
    AVIAN,
    CEPHILOPOD
}
public enum Faction
{
    GOODIES,
    BADDIES
}

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "ScriptableObjects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    public string Name;
    public Sprite Portrait;

    public EXPpackage CurrentEXP;
    public EXPpackage NextLevelEXP;
    public LVLpackage Level;

    public Race Race;
    public Faction Faction;

    public void Clone(CharacterSheet target)
    {
        Name = target.Name;
        Portrait = target.Portrait;
        CurrentEXP = target.CurrentEXP;
        NextLevelEXP = target.NextLevelEXP;
        Level = target.Level;
        Race = target.Race;
        Level = target.Level;
    }

    /*== GOOD ==
     * =MAINSTATS
     * health
     * stamina
     * mana
     * speed
     * 
     * =SIDESTATS
     * evade
     * block
     * parry
     * resistance
     * CoolDownBar
     * level scaling
     * race modifiers
     * 
     * == MAYBE ==
     * phase? ghost?
     */
}