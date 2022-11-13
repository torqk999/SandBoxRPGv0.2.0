using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConstants
{
    public const float TIME_BLIP = 0.5f;
    public const float TIME_SCALE = 0.005f;
    public const float RAD_2_DEG = 180 / Mathf.PI;
    public const int DECIMAL_PLACES = 2;
    public const int ACTION_KEY_COUNT = 2;
    //public const int TOTAL_MOUSE_BUTTONS = 7;

    public const int STR_BUILD_CAP = 512;
    public const int MAX_PATH_CYCLES = 1000;

    public const string ANIM_BOOL_WALK = "walking";
    public const string ANIM_VERT_WALK = "verticalMove";
    public const string ANIM_HORZ_WALK = "horizontalMove";

    public const string TAG_GROUND = "GROUND";
    public const string TAG_CHARACTER = "CHARACTER";
    public const string TAG_MOB = "MOB";
    public const string TAG_TRIGGER = "TRIGGER";
    public const string TAG_LOOT = "LOOT";

    public enum AttacksAnims
    {
        SWING = 1,
        BOW = 5,
    }
}

public static class CharacterMath
{
    public static float GeneratePotency(ref StringBuilder debug, CharacterSheet sheet = null, Equipment equip = null)
    {
        try
        {
            int school = equip == null ? (int)School.MONK : (int)equip.EquipSchool;
            debug.Append("0\n");
            float weaponLevelFactor = equip == null ? 0 : equip.EquipLevel;
            debug.Append("1\n");
            Race race = sheet == null ? Race.HUMAN : sheet.Race;
            debug.Append("2\n");
            int skillLevel = sheet == null ? 0 : sheet.SkillsLevels.Levels[school];
            debug.Append("3\n");
            int charLevel = sheet == null ? 0 : sheet.Level;
            debug.Append("4\n");

            debug.Append($"{charLevel * CharacterMath.CHAR_LEVEL_FACTOR}:" +
                $"{weaponLevelFactor * CharacterMath.WEP_LEVEL_FACTOR}:" +
                $"{skillLevel * CharacterMath.SKILL_MUL_LEVEL[school]}:" +
                $"{CharacterMath.SKILL_MUL_RACE[(int)race, school]}");

            return 1 +                                                                  // Base

            (((charLevel * CharacterMath.CHAR_LEVEL_FACTOR) +                           // Level

            (weaponLevelFactor * CharacterMath.WEP_LEVEL_FACTOR) +                      // Weapon

            (skillLevel * CharacterMath.SKILL_MUL_LEVEL[school])) *                     // Skill

            CharacterMath.SKILL_MUL_RACE[(int)race, school]);                           // Race
        }
        catch
        {
            return -1;
        }
    }

    public static readonly int STATS_CC_COUNT = Enum.GetNames(typeof(CCstatus)).Length;
    public static readonly int STATS_RAW_COUNT = Enum.GetNames(typeof(RawStat)).Length;
    public static readonly int STATS_CHAR_COUNT = Enum.GetNames(typeof(CharStat)).Length;
    public static readonly int STATS_ELEMENT_COUNT = Enum.GetNames(typeof(Element)).Length;
    public static readonly int STATS_SKILLS_COUNT = Enum.GetNames(typeof(School)).Length;
    public static readonly int EQUIP_SLOTS_COUNT = Enum.GetNames(typeof(EquipSlot)).Length;

    public const int RES_PRIME_DENOM = 100;
    public const int RING_SLOT_COUNT = 2;
    public const int PARTY_INVENTORY_MAX = 100;
    public const int ABILITY_SLOTS = 12;
    public const int TACTIC_SLOTS = 8;
    public const int LEVEL_CAP = 99;
    public const int BASE_EXP_CAP = 100;
    public const float LEVEL_EXP_CAP_MULTIPLIER = 1.1f;
    public const float CHAR_LEVEL_FACTOR = .2f;
    public const float WEP_LEVEL_FACTOR = .1f;
    public const float GLOBAL_COOLDOWN = .5f;

    public static readonly float[] BASE_REGEN =
    {
        0.0005f,
        0.01f,
        0.01f
    };
    public static readonly float[] SKILL_MUL_LEVEL = 
    {
        .5f,  // BERZERKER
        .5f,  // WARRIOR
        .5f,  // MONK
        .5f,  // RANGER
        .5f,  // ROGUE
        .5f,  // PYROMANCER
        .5f,  // GEOMANCER
        .5f,  // NECROMANCER
        .5f,  // AEROTHURGE
        .5f,  // HYDROSOPHIST
    };
    public static readonly float[,] SKILL_MUL_RACE =
    {
        // HUMAN
        {
            1,  // BERZERKER
            1,  // WARRIOR
            1,  // MONK
            1,  // RANGER
            1,  // ROGUE
            1,  // PYROMANCER
            1,  // GEOMANCER
            1,  // NECROMANCER
            1,  // AEROTHURGE
            1,  // HYDROSOPHIST
        },

        // ORC
        {
            1,      // BERZERKER
            1,      // WARRIOR
            1,      // MONK
            2,      // RANGER
            1,      // ROGUE
            1,      // PYROMANCER
            1.25f,  // GEOMANCER
            1,      // NECROMANCER
            1,      // AEROTHURGE
            .5f,    // HYDROSOPHIST
        }
    };
    public static readonly float[,] STAT_MUL_RACE =
    {
        // HUMAN
        {
        0.9f,   // HEALTH
        0.9f,   // STAMINA
        0.9f,   // MANA
        1,      // SPEED
        },

        // ORC
        {
        1.2f,   // HEALTH
        1.0f,   // STAMINA
        0.6f,   // MANA
        1.1f,   // SPEED
        }
    };
    public static readonly float[,] RES_MUL_RACE =
    {
        // HUMAN
        {
        1,      // PHYS
        1,      // FIRE
        1,      // WATER
        1,      // EARTH
        1,      // AIR
        1,      // POISON
        1       // HEALING
        },

        // ORC
        {
        1,      // PHYS
        1.2f,   // FIRE
        1,      // WATER
        1,      // EARTH
        1,      // AIR
        1,      // POISON
        1       // HEALING
        }
    };
    public static readonly float[] STAT_BASE =
    {
        100,
        100,
        100,
        6,
    };
    public static readonly float[] RES_BASE =
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };
    public static readonly float[] STAT_GROWTH =
    {
        5,
        1,
        1,
        0,
    };
    public static readonly float[] RES_GROWTH =
{
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };
}

