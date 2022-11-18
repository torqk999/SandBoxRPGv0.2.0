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
public class CharacterSheet : RootScriptObject
{
    public Character Posession;
    // Persistent
    public int CharacterLevel;
    public float CurrentEXP;
    public float NextLevelEXP;

    public Race Race;
    public Faction Faction;

    public EXPpackage CurrentSkillEXP;
    public EXPpackage NextLevelSkillEXP;
    public LVLpackage SkillsLevels;

    public CharacterAbility[] InnateAbilities;
    //public BaseEffect[] InnatePassives;

    public override void Copy(RootScriptObject source, RootOptions options)
    {
        if (!(source is CharacterSheet))
            return;
        CharacterSheet sheet = (CharacterSheet)source;

        Race = sheet.Race;
        Faction = sheet.Faction;

        CharacterLevel = sheet.CharacterLevel;
        CurrentEXP = sheet.CurrentEXP;
        NextLevelEXP = sheet.NextLevelEXP;

        CurrentSkillEXP.Clone(sheet.CurrentSkillEXP);
        NextLevelSkillEXP.Clone(sheet.NextLevelSkillEXP);
        SkillsLevels.Clone(sheet.SkillsLevels);

        if (sheet.InnateAbilities != null)
        {
            InnateAbilities = new CharacterAbility[sheet.InnateAbilities.Length];
            for (int i = 0; i < sheet.InnateAbilities.Length; i++)
                InnateAbilities[i] = sheet.InnateAbilities[i];
        }
    }

    public void Initialize(bool fresh = true)
    {
        if (fresh)
        {
            CharacterLevel = 0;
            CurrentEXP = 0;
            NextLevelEXP = 0;
        }

        CurrentSkillEXP.Initialize(!fresh);
        NextLevelSkillEXP.Initialize(!fresh);
        SkillsLevels.Initialize(!fresh);// = new LVLpackage(CharacterMath.STATS_SKILLS_COUNT);
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