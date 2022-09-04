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

    public const string ANIM_BOOL_WALK = "walking";

    public const string TAG_GROUND = "GROUND";
    public const string TAG_CHARACTER = "CHARACTER";
    public const string TAG_MOB = "MOB";
    public const string TAG_TRIGGER = "TRIGGER";
    public const string TAG_LOOT = "LOOT";
}

public static class CharacterMath
{
    public static readonly int PARTY_INVENTORY_MAX;
    public static readonly int STATS_RAW_COUNT = 4;
    //public static readonly int STATS_REGEN_COUNT = 3;
    public static readonly int STATS_ELEMENT_COUNT = 7;
    public static readonly int STATS_TOTAL_COUNT = STATS_RAW_COUNT + STATS_ELEMENT_COUNT;

    public static readonly int SKILLS_COUNT = 9;
    public static readonly int EQUIP_SLOTS = 10;
    public static readonly int ABILITY_SLOTS = 12;
    public static readonly int TACTIC_SLOTS = 8;
    public static readonly int LEVEL_CAP = 99;
    public static readonly int BASE_EXP_CAP = 100;
    public static readonly float LEVEL_EXP_CAP_MULTIPLIER = 1.1f;

    public static readonly float[] BASE_REGEN =
    {
        0.001f,
        0.01f,
        0.01f
    };
    public static readonly float[] SKILL_FACTOR =
    {
        .5f,  // LIGHT
        .5f,  // MEDIUM
        .5f,  // HEAVY
        .5f,  // ONE HAND
        .5f,  // TWO HAND
        .5f,  // SHIELD
        .5f,  // RANGED
        .5f,  // MAGIC
        .2f,  // CHAR LEVEL
        .1f,  // WEP LEVEL
    };
    public static readonly float[,] RACE_FACTOR =
    {
        // HUMAN
        {
        1,  // LIGHT
        1,  // MEDIUM
        1,  // HEAVY
        1,  // ONE HAND
        1,  // TWO HAND
        1,  // SHIELD
        1,  // RANGED
        1,  // MAGIC
        },

        // ORC
        {
        1,  // LIGHT
        1,  // MEDIUM
        2,  // HEAVY
        1,  // ONE HAND
        1,  // TWO HAND
        1,  // SHIELD
        1,  // RANGED
        .5f,  // MAGIC
        }
    };
    public static readonly float[,] RACE_STAT =
    {
        // HUMAN
        {
        0.9f,   // HEALTH
        0.9f,   // STAMINA
        0.9f,   // MANA
        1,      // SPEED

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
        1.2f,   // HEALTH
        1.0f,   // STAMINA
        0.6f,   // MANA
        1.1f,   // SPEED

        1,      // PHYS
        1,      // FIRE
        1,      // WATER
        1,      // EARTH
        1,      // AIR
        1,      // POISON
        1       // HEALING
        }
    };
    public static readonly float[] BASE_STAT =
    {
        100,
        100,
        100,
        6,

        0,
        0,
        0,
        0,
        0,
        0,
        0
    };
    public static readonly float[] LEVEL_STAT =
    {
        5,
        1,
        1,
        0,

        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    /*PHYSICAL;
    public float FIRE;
    public float WATER;
    public float EARTH;
    public float AIR;
    public float POISON;
    public float HEALING;
     */
}

