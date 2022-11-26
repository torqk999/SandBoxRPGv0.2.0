using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using RPGconstants;

//Interactions
public interface Interaction
{
    public void Interact();
    public InteractData GetInteractData();
}
[Serializable]
public class InteractData
{
    public TriggerType Type;
    public string Name;
}
public class CharacterData : InteractData
{
    public Character myCharacter;

    public float HealthCurrent;
    public float HealthMax;

    public CharacterData(Character character)
    {
        Name = character.Sheet.Name;
        myCharacter = character;

        HealthCurrent = character.CurrentStats.Stats[(int)RawStat.HEALTH];
        HealthMax = character.MaximumStatValues.Stats[(int)RawStat.HEALTH];
    }
}
public class LootData : InteractData
{
    public float Weight;
    public Quality Quality;
}


public enum ClassType
{
    WARRIOR,
    MAGE,
    ROGUE
}
public enum ValueType
{
    NONE,
    PERC_CURR,
    PERC_MAX,
    PERC_MISS,
    FLAT
}
public enum CCstatus
{
    DEAD,
    IMMOBILE,
    UN_ARMED,
    SILENCED,
    FEARED,
    CHARMED,
    DRUNK,
    STUNNED
}


[Serializable]
public struct CCstateReflection
{
    public bool DEAD;
    public bool IMMOBILE;
    public bool UN_ARMED;
    public bool SILENCED;
    public bool FEARED;
    public bool CHARMED;
    public bool DRUNK;
    public bool STUNNED;

    public bool Reflect(ref bool[] ccStates, bool inject = true)
    {
        try
        {
            if (inject)
            {
                ccStates[(int)CCstatus.DEAD] = DEAD;
                ccStates[(int)CCstatus.IMMOBILE] = IMMOBILE;
                ccStates[(int)CCstatus.UN_ARMED] = UN_ARMED;
                ccStates[(int)CCstatus.SILENCED] = SILENCED;
                ccStates[(int)CCstatus.FEARED] = FEARED;
                ccStates[(int)CCstatus.CHARMED] = CHARMED;
                ccStates[(int)CCstatus.DRUNK] = DRUNK;
                ccStates[(int)CCstatus.STUNNED] = STUNNED;
            }
            else
            {
                DEAD = ccStates[(int)CCstatus.DEAD];
                IMMOBILE = ccStates[(int)CCstatus.IMMOBILE];
                UN_ARMED = ccStates[(int)CCstatus.UN_ARMED];
                SILENCED = ccStates[(int)CCstatus.SILENCED];
                FEARED = ccStates[(int)CCstatus.FEARED];
                CHARMED = ccStates[(int)CCstatus.CHARMED];
                DRUNK = ccStates[(int)CCstatus.DRUNK];
                STUNNED = ccStates[(int)CCstatus.STUNNED];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
public enum CharStat
{
    STR,
    INT,
    DEX
}
[Serializable]
public struct CharStatPackageReflection
{
    ///public float STAMINA;
    ///public float HEALTH;
    ///public float MANA;
    ///public float SPEED;
    public int STR;
    public int INT;
    public int DEX;

    public bool Reflect(ref int[] rawStats, bool inject = true)
    {
        try
        {
            if (inject)
            {
                ///rawStats[(int)RawStat.STAMINA] = STAMINA;
                ///rawStats[(int)RawStat.HEALTH] = HEALTH;
                ///rawStats[(int)RawStat.MANA] = MANA;
                ///rawStats[(int)RawStat.SPEED] = SPEED;
                rawStats[(int)CharStat.STR] = STR;
                rawStats[(int)CharStat.INT] = INT;
                rawStats[(int)CharStat.DEX] = DEX;
            }
            else
            {
                ///STAMINA = rawStats[(int)RawStat.STAMINA];
                ///HEALTH = rawStats[(int)RawStat.HEALTH];
                ///MANA = rawStats[(int)RawStat.MANA];
                ///SPEED = rawStats[(int)RawStat.SPEED];
                STR = rawStats[(int)CharStat.STR];
                INT = rawStats[(int)CharStat.INT];
                DEX = rawStats[(int)CharStat.DEX];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
[Serializable]
public struct CharStatPackage
{
    public CharStatPackageReflection Reflection;
    public int[] Stats;

    public CharStatPackage(CharacterSheet sheet)
    {
        Stats = new int[CharacterMath.STATS_CHAR_COUNT];
        for (int i = 0; i < CharacterMath.STATS_CHAR_COUNT; i++)
        {

            // NOT YET IMPLEMENTED!!! //

            float stat = CharacterMath.STAT_BASE[i] +
                (CharacterMath.STAT_GROWTH[i] *
                CharacterMath.STAT_MUL_RACE[(int)sheet.Race, i] *
                sheet.CharacterLevel);

            Stats[i] = (int)stat;
        }
        Reflection = new CharStatPackageReflection();
        Reflection.Reflect(ref Stats, false);
    }

    public void Clone(CharStatPackage source)
    {
        if (source.Stats == null || source.Stats.Length == CharacterMath.STATS_CHAR_COUNT)
            return;

        Reflection = source.Reflection;
        Stats = new int[CharacterMath.STATS_CHAR_COUNT];
        for (int i = 0; i < CharacterMath.STATS_CHAR_COUNT; i++)
            Stats[i] = source.Stats[i];
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Stats.Length; i++)
            Stats[i] = (int)(Stats[i] * amp);
    }
    public void Initialize(bool inject = true)
    {
        Stats = new int[CharacterMath.STATS_CHAR_COUNT];
        Reflection.Reflect(ref Stats, inject);
    }
}
public enum RawStat
{
    HEALTH,
    STAMINA,
    MANA,
    SPEED
}
[Serializable]
public struct RawStatPackageReflection
{
    public float STAMINA;
    public float HEALTH;
    public float MANA;
    public float SPEED;
    ///public float STR;
    ///public float INT;
    ///public float DEX;

    public bool Reflect(ref float[] rawStats, bool inject = true)
    {
        try
        {
            if (inject)
            {
                rawStats[(int)RawStat.STAMINA] = STAMINA;
                rawStats[(int)RawStat.HEALTH] = HEALTH;
                rawStats[(int)RawStat.MANA] = MANA;
                rawStats[(int)RawStat.SPEED] = SPEED;
                ///rawStats[(int)RawStat.SPEED] = STR;
                ///rawStats[(int)RawStat.SPEED] = INT;
                ///rawStats[(int)RawStat.SPEED] = DEX;
            }
            else
            {
                STAMINA = rawStats[(int)RawStat.STAMINA];
                HEALTH = rawStats[(int)RawStat.HEALTH];
                MANA = rawStats[(int)RawStat.MANA];
                SPEED = rawStats[(int)RawStat.SPEED];
                ///STR = rawStats[(int)RawStat.STR];
                ///INT = rawStats[(int)RawStat.INT];
                ///DEX = rawStats[(int)RawStat.DEX];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
[Serializable]
public struct RawStatPackage
{
    public RawStatPackageReflection Reflection;

    [Header("NO TOUCHY!")]
    public float[] Stats;
    public float[] Amplification;

    public RawStatPackage(CharacterSheet sheet)
    {
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
        Amplification = new float[CharacterMath.STATS_RAW_COUNT];
        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
        {
            float stat = CharacterMath.STAT_BASE[i] +
                (CharacterMath.STAT_GROWTH[i] *
                CharacterMath.STAT_MUL_RACE[(int)sheet.Race, i] *
                sheet.CharacterLevel);

            Stats[i] = stat;
        }
        Reflection = new RawStatPackageReflection();
        Reflection.Reflect(ref Stats, false);
    }

    public bool Clone(RawStatPackage source)
    {
        if (source.Stats == null || source.Stats.Length != CharacterMath.STATS_RAW_COUNT)
            return false;

        Reflection = source.Reflection;
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
            Stats[i] = source.Stats[i];

        return true;
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Stats.Length; i++)
            Amplification[i] = Stats[i] * amp;
    }
    public void Initialize(bool inject = true)
    {
        Stats = new float[CharacterMath.STATS_RAW_COUNT];
        Reflection.Reflect(ref Stats, inject);
    }
}
public enum Element
{
    PHYSICAL,
    FIRE,
    WATER,
    EARTH,
    AIR,
    POISON,
    HEALING
}
[Serializable]
public struct ElementReflection
{
    public float PHYSICAL;
    public float FIRE;
    public float WATER;
    public float EARTH;
    public float AIR;
    public float POISON;
    public float HEALING;

    public bool Reflect(ref float[] elements, bool inject = true)
    {
        try
        {
            if (inject)
            {
                elements[(int)Element.PHYSICAL] = PHYSICAL;
                elements[(int)Element.FIRE] = FIRE;
                elements[(int)Element.WATER] = WATER;
                elements[(int)Element.EARTH] = EARTH;
                elements[(int)Element.AIR] = AIR;
                elements[(int)Element.POISON] = POISON;
                elements[(int)Element.HEALING] = HEALING;
            }
            else
            {
                PHYSICAL = elements[(int)Element.PHYSICAL];
                FIRE = elements[(int)Element.FIRE];
                WATER = elements[(int)Element.WATER];
                EARTH = elements[(int)Element.EARTH];
                AIR = elements[(int)Element.AIR];
                POISON = elements[(int)Element.POISON];
                HEALING = elements[(int)Element.HEALING];
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}
[Serializable]
public struct ElementPackage
{
    public ElementReflection Reflection;

    [Header("NO TOUCHY!")]
    public float[] Elements;
    public float[] Amplification;

    public ElementPackage(float healMagnitude)
    {
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Amplification = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Elements[(int)Element.HEALING] = healMagnitude;
        Reflection = new ElementReflection();
        Reflection.Reflect(ref Elements, false);
    }
    public ElementPackage(CharacterSheet sheet)
    {
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Amplification = new float[CharacterMath.STATS_ELEMENT_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            Elements[i] = CharacterMath.RES_MUL_RACE[(int)sheet.Race, i] * (CharacterMath.RES_BASE[i] +
                (CharacterMath.RES_GROWTH[i] * sheet.CharacterLevel));
        }
        Reflection = new ElementReflection();
        Reflection.Reflect(ref Elements, false);
    }

    public bool Clone(ElementPackage source)
    {
        if (source.Elements == null || source.Elements.Length != CharacterMath.STATS_ELEMENT_COUNT)
            return false;

        Reflection = source.Reflection;
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Amplification = new float[CharacterMath.STATS_ELEMENT_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            Elements[i] = source.Elements[i];

        return true;
    }
    public void Amplify(float amp)
    {
        for (int i = 0; i < Elements.Length; i++)
            Amplification[i] = Elements[i] * amp;
    }
    public void Initialize(bool inject = true)
    {
        Elements = new float[CharacterMath.STATS_ELEMENT_COUNT];
        Reflection.Reflect(ref Elements, inject);
    }
}
[Serializable]
public struct EXPreflection
{
    public float BERZERKER;
    public float WARRIOR;
    public float MONK;
    public float RANGER;
    public float ROGUE;
    public float PYROMANCER;
    public float GEOMANCER;
    public float NECROMANCER;
    public float AEROTHURGE;
    public float HYDROSOPHIST;

    public bool Reflect(ref float[] elements, bool inject = true)
    {
        try
        {
            if (inject)
            {
                elements[(int)School.BERZERKER] = BERZERKER;
                elements[(int)School.WARRIOR] = WARRIOR;
                elements[(int)School.MONK] = MONK;
                elements[(int)School.RANGER] = RANGER;
                elements[(int)School.ROGUE] = ROGUE;
                elements[(int)School.PYROMANCER] = PYROMANCER;
                elements[(int)School.GEOMANCER] = GEOMANCER;
                elements[(int)School.NECROMANCER] = NECROMANCER;
                elements[(int)School.AEROTHURGE] = AEROTHURGE;
                elements[(int)School.HYDROSOPHIST] = HYDROSOPHIST;
            }
            else
            {
                BERZERKER = elements[(int)School.BERZERKER];
                WARRIOR = elements[(int)School.WARRIOR];
                MONK = elements[(int)School.MONK];
                RANGER = elements[(int)School.RANGER];
                ROGUE = elements[(int)School.ROGUE];
                PYROMANCER = elements[(int)School.PYROMANCER];
                GEOMANCER = elements[(int)School.GEOMANCER];
                NECROMANCER = elements[(int)School.NECROMANCER];
                AEROTHURGE = elements[(int)School.AEROTHURGE];
                HYDROSOPHIST = elements[(int)School.HYDROSOPHIST];
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
[Serializable]
public struct EXPpackage
{
    public EXPreflection Reflection;
    public float[] Experience;

    public EXPpackage(int skillCount)
    {
        Experience = new float[skillCount];
        Reflection = new EXPreflection();
    }
    public bool Clone(EXPpackage source)
    {
        if (source.Experience == null || source.Experience.Length != CharacterMath.STATS_SKILLS_COUNT)
            return false;

        Reflection = source.Reflection;
        Experience = new float[CharacterMath.STATS_SKILLS_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            Experience[i] = source.Experience[i];

        return true;
    }
    public void Initialize(bool inject = true)
    {
        Experience = new float[CharacterMath.STATS_SKILLS_COUNT];
        Reflection.Reflect(ref Experience, inject);
    }
}
public enum School
{
    BERZERKER,
    WARRIOR,
    MONK,
    RANGER,
    ROGUE,
    PYROMANCER,
    GEOMANCER,
    NECROMANCER,
    AEROTHURGE,
    HYDROSOPHIST
}
[Serializable]
public struct LVLreflection
{
    public int BERZERKER;
    public int WARRIOR;
    public int MONK;
    public int RANGER;
    public int ROGUE;
    public int PYROMANCER;
    public int GEOMANCER;
    public int NECROMANCER;
    public int AEROTHURGE;
    public int HYDROSOPHIST;

    public bool Reflect(ref int[] elements, bool inject = true)
    {
        try
        {
            if (inject)
            {
                elements[(int)School.BERZERKER] = BERZERKER;
                elements[(int)School.WARRIOR] = WARRIOR;
                elements[(int)School.MONK] = MONK;
                elements[(int)School.RANGER] = RANGER;
                elements[(int)School.ROGUE] = ROGUE;
                elements[(int)School.PYROMANCER] = PYROMANCER;
                elements[(int)School.GEOMANCER] = GEOMANCER;
                elements[(int)School.NECROMANCER] = NECROMANCER;
                elements[(int)School.AEROTHURGE] = AEROTHURGE;
                elements[(int)School.HYDROSOPHIST] = HYDROSOPHIST;
            }
            else
            {
                BERZERKER = elements[(int)School.BERZERKER];
                WARRIOR = elements[(int)School.WARRIOR];
                MONK = elements[(int)School.MONK];
                RANGER = elements[(int)School.RANGER];
                ROGUE = elements[(int)School.ROGUE];
                PYROMANCER = elements[(int)School.PYROMANCER];
                GEOMANCER = elements[(int)School.GEOMANCER];
                NECROMANCER = elements[(int)School.NECROMANCER];
                AEROTHURGE = elements[(int)School.AEROTHURGE];
                HYDROSOPHIST = elements[(int)School.HYDROSOPHIST];
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
[Serializable]
public struct LVLpackage
{
    LVLreflection Reflection;
    public int[] Levels;

    public LVLpackage(int skillCount)
    {
        Levels = new int[skillCount];
        Reflection = new LVLreflection();
    }
    public bool Clone(LVLpackage source)
    {
        if (source.Levels == null || source.Levels.Length != CharacterMath.STATS_SKILLS_COUNT)
            return false;

        Reflection = source.Reflection;
        Levels = new int[CharacterMath.STATS_SKILLS_COUNT];
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            Levels[i] = source.Levels[i];

        return true;
    }
    public void Initialize(bool inject = true)
    {
        Levels = new int[CharacterMath.STATS_SKILLS_COUNT];
        Reflection.Reflect(ref Levels, inject);
    }
}
[Serializable]
public struct AbilityLogic
{
    public float CD_Timer;
    public float Cast_Timer;
    public Character SourceCharacter;
    public GameObject CastParticleInstance;
    //public GameObject CastTargetInstance;
    public List<Character> TargetBuffer;

    public void Clone(AbilityLogic source)
    {
        Debug.Log("cloning ability logic...");

        SourceCharacter = source.SourceCharacter;
        TargetBuffer = new List<Character>();

        CD_Timer = 0;
        Cast_Timer = 0;
    }
}
[Serializable]
public struct EffectLogic
{
    // Sourced from ability
    public EffectOptions Options;

    // States
    public bool IsProjectile;
    public bool ToggleActive;

    // Timers
    public float ProjectileTimer;
    public float PeriodTimer;
    public float DurationTimer;

    // Active object references
    public List<Character> TargetBuffer;

    // Psystem Instantiations
    public GameObject Condition;
    public GameObject Projectile;
    public GameObject Impact;

    public void Clone(EffectLogic source)
    {
        Options = source.Options;

        IsProjectile = false;
        ToggleActive = true;

        ProjectileTimer = 0;
        PeriodTimer = 0;
        DurationTimer = 0;

        TargetBuffer = new List<Character>();

        Condition = null;
        Projectile = null;
        Impact = null;
    }
}
[Serializable]
public struct EffectOptions
{
    public EffectType EffectType;
    public CharacterAbility Source;
    public Character Effected;

    public EffectOptions(CharacterAbility source, Character effected = null)
    {
        switch(source)
        {
            case PassiveAbility:
                EffectType = EffectType.PASSIVE;
                break;

            case ProcAbility:
                EffectType = EffectType.PROC;
                break;

            case ToggleAbility:
                EffectType = EffectType.TOGGLE;
                break;

            default:
                EffectType = EffectType.NONE;
                break;
        }

        Source = source;
        Effected = effected;
    }
}
[Serializable]
public struct InventoryOptions
{
}
[Serializable]
public struct ItemOptions
{
    public Quality Quality;
    public int Quantity;
    public int GoldValue;
    public int InventoryIndex;
    public int SlotIndex;

    public ItemOptions(Quality quality = default, int quantity = 0, int goldValue = 0, int inventoryIndex = -1, int slotIndex = -1)
    {
        Quality = quality;
        Quantity = quantity;
        GoldValue = goldValue;
        InventoryIndex = inventoryIndex;
        SlotIndex = slotIndex;
    }

    public ItemOptions(int inventoryIndex, int slotIndex)
    {
        Quality = default;
        Quantity = 1;
        GoldValue = 0;
        InventoryIndex = inventoryIndex;
        SlotIndex = slotIndex;
    }
}
[Serializable]
public struct RootLogic
{
    public RootOptions Options;
    public List<RootScriptObject> Clones;
    public RootUI Button;

    public void Copy(RootOptions options)
    {
        Options = options;
        Clones = new List<RootScriptObject>();
    }
}
[Serializable] 
public struct RootOptions
{
    public GameState GameState;
    public RootScriptObject Source;
    public List<RootScriptObject> HomePanel;
    //public Page
    //public bool IsProjectile;
    public bool IsClone;
    public int ID;
    public int Index;
    public int Quantity; // Migrate to ItemOptions
    
    public RootOptions(GameState state, RootScriptObject source, ref int id, List<RootScriptObject> panel = null, int index = -1, int quantity = 1, bool clone = false)//, bool projectile = false)
    {
        GameState = state;
        Source = source;
        //IsProjectile = projectile;
        IsClone = clone;
        HomePanel = panel;
        ID = id;
        Index = index;
        Quantity = quantity;
    }
}
