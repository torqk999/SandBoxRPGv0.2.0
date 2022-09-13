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
    public int Level;
    public Sprite Portrait;
    public Race Race;
    public Faction Faction;

    public EXPpackage CurrentEXP;
    public EXPpackage NextLevelEXP;
    public LVLpackage Skills;
    
    public void Clone(CharacterSheet target)
    {
        Name = target.Name;
        Level = target.Level;
        Portrait = target.Portrait;
        Race = target.Race;
        //Faction = target.Faction;
        CurrentEXP = target.CurrentEXP;
        NextLevelEXP = target.NextLevelEXP;
        Skills = target.Skills;
    }

    public void Fresh()
    {
        Level = 0;
        CurrentEXP = new EXPpackage(CharacterMath.STATS_LEVELS_COUNT);
        NextLevelEXP = new EXPpackage(CharacterMath.STATS_LEVELS_COUNT);
        Skills = new LVLpackage(CharacterMath.STATS_LEVELS_COUNT);
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