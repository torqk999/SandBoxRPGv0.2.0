using System;
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
    public const int TOTAL_MOUSE_BUTTONS = 7;

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
    public static readonly int STATS_CC_COUNT = Enum.GetNames(typeof(CCstatus)).Length;
    public static readonly int STATS_RAW_COUNT = Enum.GetNames(typeof(RawStat)).Length;
    public static readonly int STATS_ELEMENT_COUNT = Enum.GetNames(typeof(Element)).Length;
    public static readonly int STATS_SKILLS_COUNT = Enum.GetNames(typeof(SkillType)).Length;
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

    public static readonly float[] BASE_REGEN =
    {
        0.0005f,
        0.01f,
        0.01f
    };

    public static readonly float[] SKILL_MUL_LEVEL = 
    {
        .5f,  // UN ARMED
        .5f,  // LIGHT
        .5f,  // MEDIUM
        .5f,  // HEAVY
        .5f,  // ONE HAND
        .5f,  // OFF HAND
        .5f,  // TWO HAND
        .5f,  // SHIELD
        .5f,  // RANGED
        .5f,  // MAGIC
    };
    public static readonly float[,] SKILL_MUL_RACE =
    {
        // HUMAN
        {
        1,  // UN ARMED
        1,  // LIGHT
        1,  // MEDIUM
        1,  // HEAVY
        1,  // ONE HAND
        1,  // OFF HAND
        1,  // TWO HAND
        1,  // SHIELD
        1,  // RANGED
        1,  // MAGIC
        },

        // ORC
        {
        1,      // UN ARMED
        1,      // LIGHT
        1,      // MEDIUM
        2,      // HEAVY
        1,      // ONE HAND
        1,      // OFF HAND
        1.25f,  // TWO HAND
        1,      // SHIELD
        1,      // RANGED
        .5f,    // MAGIC
        }
    };
    public static readonly float[,] RAW_MUL_RACE =
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
    public static readonly float[] RAW_BASE =
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
    public static readonly float[] RAW_GROWTH =
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

    /*PHYSICAL;
    public float FIRE;
    public float WATER;
    public float EARTH;
    public float AIR;
    public float POISON;
    public float HEALING;
     */
}

