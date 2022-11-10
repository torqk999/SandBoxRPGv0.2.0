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
    CEPHILOPOD,
    ///
    ELEMENTAL,
    GOLEM,
    SPRITE
}
public enum Faction
{
    GOODIES,
    BADDIES
}

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "ScriptableObjects/CharacterSheet")]
public class CharacterSheet : ScriptableObject
{
    public Sprite Portrait;
    public string Name;
    public int Level;
    public float CurrentEXP;
    public float NextLevelEXP;

    public Race Race;
    public Faction Faction;

    public EXPpackage CurrentSkillEXP;
    public EXPpackage NextLevelSkillEXP;
    public LVLpackage SkillsLevels;

    public CharacterAbility[] InnateAbilities;
    public BaseEffect[] InnatePassives;

    public void Clone(CharacterSheet target)
    {
        Name = target.Name;
        Level = target.Level;
        CurrentEXP = target.CurrentEXP;
        NextLevelEXP = target.NextLevelEXP;

        Portrait = target.Portrait;
        Race = target.Race;
        Faction = target.Faction;

        CurrentSkillEXP = target.CurrentSkillEXP;
        NextLevelSkillEXP = target.NextLevelSkillEXP;
        SkillsLevels = target.SkillsLevels;
    }

    public void Fresh()
    {
        Level = 0;
        CurrentSkillEXP = new EXPpackage(CharacterMath.STATS_SKILLS_COUNT);
        NextLevelSkillEXP = new EXPpackage(CharacterMath.STATS_SKILLS_COUNT);
        SkillsLevels = new LVLpackage(CharacterMath.STATS_SKILLS_COUNT);
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