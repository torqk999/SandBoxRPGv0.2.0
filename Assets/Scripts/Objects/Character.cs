using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AnimatorType
{
    BI_PED,
    QUAD_PED,
    BIRD,
    FISH
}

public enum EquipSlot
{
    HEAD,
    CHEST,
    BELT,
    LEGS,
    BOOTS,
    NECK,
    MAIN,
    OFF,
    RING_L,
    RING_R
}

public class Character : Pawn, Interaction
{
    [Header("Character Defs")]
    [Header("==== CHARACTER CLASS ====")]
    public int ID;

    [Header("Animation")]
    public CharacterAssetPack Assets;
    public AnimatorPlus myAnim;
    public Animator Animator;
    public AnimatorType AnimType;
    public float IntentForward;
    public float IntentRight;
    public bool bIntent;

    [Header("Character Stats")]
    public StatPackage CurrentStats;
    public StatPackage MaximumStatValues;
    public ElementPackage Resistances;
    //public ElementPackage MaximumResistances; // ??

    public CharacterSheet Sheet;
    public List<CharacterAbility> Abilities;
    public List<Effect> Effects;

    public bool bIsAlive;
    public bool[] CCstatus;
    public float ChannelTimer;

    [Header("Character Logic")]
    public Character Target;
    public List<Character> TargettedBy;
    public Inventory Inventory;
    public GameObject CharacterCanvas;

    [Header("Character Slots")]
    public CharacterAbility[] AbilitySlots;
    public EquipWrapper[] EquipmentSlots;

    [Header("Interaction")]
    public Interaction CurrentTargetInteraction;
    public List<Interaction> CurrentProximityInteractions;
    public int InteractionCount;

    [Header("Debugging")]
    public DebugState DebugState;
    public bool bDebugMode;
    public bool bAssetUpdate;
    public bool bAssetTimer;
    public float AssetTimer;

    public void UpdateAbilites()
    {
        UpdateAbilitySlots();
        UpdateAbilityList();
    }
    public void InitializeCharacter()
    {
        InitializeCharacterSheet();
        InitializePassiveRegen();
    }

    #region INTERACTION
    public void RemoveInteraction(Interaction interact)
    {
        int index = CurrentProximityInteractions.FindIndex(x => x == interact);
        if (index == -1)
            return;

        ResolveCurrentTargetInteraction(index);
        CurrentProximityInteractions.Remove(interact);

    }
    void ResolveCurrentTargetInteraction(int index)
    {
        if (index < 0 || index >= CurrentProximityInteractions.Count || CurrentProximityInteractions.Count <= 1)
        {
            CurrentTargetInteraction = null;
            return;
        }

        index++;
        index = index >= CurrentProximityInteractions.Count ? 0 : index;

        CurrentTargetInteraction = CurrentProximityInteractions[index];
    }
    public InteractData GetInteractData()
    {
        return new CharacterData(this);
    }
    public void Interact()
    {
        GameState.pController.CurrentCharacter.Target = this;
    }
    #endregion

    #region INITIALIZERS
    void InitializeCharacterSheet()
    {
        if (Sheet == null)
            return;

        CurrentStats = new StatPackage(CharacterMath.STATS_RAW_COUNT);
        MaximumStatValues = new StatPackage(CharacterMath.STATS_RAW_COUNT);

        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
        {
            float stat = CharacterMath.RAW_BASE[i] +
                (CharacterMath.RAW_GROWTH[i] *
                CharacterMath.RAW_MUL_RACE[(int)Sheet.Race, i] *
                Sheet.Level);

            CurrentStats.Stats[i] = stat;
            MaximumStatValues.Stats[i] = stat;
        }

        Resistances = new ElementPackage(CharacterMath.STATS_ELEMENT_COUNT);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            Resistances.Elements[i] = CharacterMath.RES_BASE[i] +
                (CharacterMath.RES_GROWTH[i] *
                CharacterMath.RES_MUL_RACE[(int)Sheet.Race, i] *
                Sheet.Level);
        }

        Debug.Log($"{Sheet.Name}/{name} : {CurrentStats.Stats.Length} : {MaximumStatValues.Stats.Length}");

        UpdateAbilites();
    }
    void InitializePassiveRegen()
    {
        Effects = new List<Effect>();

        for (int i = 0; i < 3; i++)
        {
            float magnitude = (CharacterMath.BASE_REGEN[i] * CharacterMath.RAW_MUL_RACE[(int)Sheet.Race, i]) *
                (1 + CharacterMath.RAW_GROWTH[i]);

            //Debug.Log($"{Sheet.Name} : {(EffectTarget)i} : {magnitude}");
            Effects.Add(CreateRegen((EffectType)i, magnitude));
        }
    }
    Effect CreateRegen(EffectType targetStat, float magnitude)
    {
        Effect regen = (Effect)ScriptableObject.CreateInstance("Effect");
        regen.Name = $"{targetStat} REGEN";
        regen.Type = targetStat;
        regen.Duration = EffectDuration.PASSIVE;
        regen.ElementPack = new ElementPackage(CharacterMath.STATS_ELEMENT_COUNT);
        regen.ElementPack.Elements[(int)Element.HEALING] = magnitude;

        return regen;
    }

    #endregion

    #region ABILITIES

    void UpdateAbilitySlots()
    {
        List<int> equipIDs = new List<int>();
        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT; i++)
            if (EquipmentSlots[i] != null)
                equipIDs.Add(EquipmentSlots[i].ItemID);

        /*
        for  (int i = Abilities.Count - 1; i > -1; i--)
            if (equipIDs.FindIndex(x => x == Abilities[i].WeaponID) < 0)
                Abilities.RemoveAt(i);
        */

        for (int i = CharacterMath.ABILITY_SLOTS - 1; i > -1; i--)
            if (AbilitySlots[i] != null && equipIDs.FindIndex(x => x == AbilitySlots[i].WeaponID) < 0)
                AbilitySlots[i] = null;
    }
    void UpdateAbilityList()
    {
        Abilities.Clear();

        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT; i++)
        {
            if (EquipmentSlots[i] == null)
                continue;

            for(int j = 0; j < EquipmentSlots[i].Equip.EquipAbilites.Length; j++)
            {
                Abilities.Add(EquipmentSlots[i].Equip.EquipAbilites[j].EquipAbility(this, EquipmentSlots[i].Equip));
            }

        }
    }

    #endregion

    #region EQUIPPING
    public bool EquipSelection(int equipIndex, int inventoryIndex)
    {
        if (inventoryIndex == -1 && equipIndex != -1)
            return AttemptEquipRemoval(EquipmentSlots[equipIndex], equipIndex);

        if (inventoryIndex != -1 && equipIndex == -1)
        {
            if (Inventory.Items[inventoryIndex] == null || !(Inventory.Items[inventoryIndex] is EquipWrapper))
                return false;

            EquipWrapper equip = (EquipWrapper)Inventory.Items[inventoryIndex];

            if (equip is WearableWrapper)
            {
                WearableWrapper wear = (WearableWrapper)equip;
                //int slotNumber = (int)wear.Wear.Type;

                if (wear.Wear.Type != WearableType.RING)
                {
                    EquipWear(wear.Wear.Type, inventoryIndex);
                    return true;
                }
                EquipRing(inventoryIndex);
                return true;
            }


            if (equip is TwoHandWrapper)
                return EquipTwoHand(inventoryIndex);

            if (equip is OneHandWrapper)
                return EquipOneHand(inventoryIndex);

            if (equip is OffHandWrapper)
                return EquipOneHand(inventoryIndex, false);
        }

        Debug.Log("How did you get here? >.>");
        return false;
    }
    void EquipWear(WearableType type, int inventoryIndex)
    {
        int equipIndex = (int)type;

        if (EquipmentSlots[equipIndex] == null)
        {
            EquipmentSlots[equipIndex] =
                (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return;
        }

        EquipmentSlots[equipIndex] =
            (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[equipIndex], inventoryIndex);
    }
    bool EquipOneHand(int inventoryIndex, bool main = true)
    {
        int IND = main ? (int)EquipSlot.MAIN : (int)EquipSlot.OFF;
        int ind = main ? (int)EquipSlot.OFF : (int)EquipSlot.MAIN;

        if (EquipmentSlots[IND] == null)
        {
            EquipmentSlots[IND] = (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return true;
        }
        if (EquipmentSlots[IND] != null)
        {
            //if (Inventory.Items.Count == Inventory.MaxCount)
            //    return false;

            //EquipmentSlots[(int)EquipSlot.MAIN] = (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[(int)EquipSlot.MAIN], inventoryIndex);
            if (EquipmentSlots[IND] is TwoHandWrapper)
                EquipmentSlots[ind] = null;
            EquipmentSlots[IND] = (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[IND], inventoryIndex);
            return true;
        }
        
        return false;
    }
    bool EquipTwoHand(int inventoryIndex)
    {
        if (EquipmentSlots[(int)EquipSlot.MAIN] == null && EquipmentSlots[(int)EquipSlot.OFF] == null) // none occupied
        {
            EquipmentSlots[(int)EquipSlot.MAIN] = (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);

            EquipmentSlots[(int)EquipSlot.OFF] = EquipmentSlots[(int)EquipSlot.MAIN];
            return true;
        }
        if (EquipmentSlots[(int)EquipSlot.MAIN] != null && EquipmentSlots[(int)EquipSlot.OFF] != null) // both occupied
        {
            if (Inventory.Items.Count == Inventory.MaxCount)
                return false;
            EquipmentSlots[(int)EquipSlot.MAIN] =
                (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[(int)EquipSlot.MAIN], inventoryIndex);
            Inventory.PushItemIntoInventory(EquipmentSlots[(int)EquipSlot.OFF]);

            EquipmentSlots[(int)EquipSlot.OFF] = EquipmentSlots[(int)EquipSlot.MAIN];
            return true;
        }
        if (EquipmentSlots[(int)EquipSlot.MAIN] != null && EquipmentSlots[(int)EquipSlot.OFF] == null) // main occupied
        {
            EquipmentSlots[(int)EquipSlot.MAIN] =
                (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[(int)EquipSlot.MAIN], inventoryIndex);

            EquipmentSlots[(int)EquipSlot.OFF] = EquipmentSlots[(int)EquipSlot.MAIN];
            return true;
        }
        if (EquipmentSlots[(int)EquipSlot.MAIN] == null && EquipmentSlots[(int)EquipSlot.OFF] != null) // off occupied
        {
            EquipmentSlots[7] =
                (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[(int)EquipSlot.OFF], inventoryIndex);

            EquipmentSlots[(int)EquipSlot.MAIN] = EquipmentSlots[(int)EquipSlot.OFF];
            return true;
        }
        Debug.Log("How did you get here? >.>");
        return false;
    }
    void EquipRing(int inventoryIndex)
    {
        //int slot = (bLeftHand) ? 8 : 9;

        if (EquipmentSlots[(int)EquipSlot.RING_L] == null)
        {
            EquipmentSlots[(int)EquipSlot.RING_L] = (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return;
        }

        if (EquipmentSlots[(int)EquipSlot.RING_R] == null)
        {
            EquipmentSlots[(int)EquipSlot.RING_R] = (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return;
        }

        EquipmentSlots[(int)EquipSlot.RING_R] = (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[(int)EquipSlot.RING_R], inventoryIndex);
    }
    bool AttemptEquipRemoval(EquipWrapper equip, int equipIndex)
    {
        if (equip == null)
            return false;

        if (Inventory.PushItemIntoInventory(equip))
        {
            if (EquipmentSlots[equipIndex] is TwoHandWrapper)
            {
                EquipmentSlots[(int)EquipSlot.MAIN] = null;
                EquipmentSlots[(int)EquipSlot.OFF] = null;
            }
            else
                EquipmentSlots[equipIndex] = null;
            return true;
        }
        return false;
    }
    #endregion

    #region LOOTING
    public bool LootContainer(GenericContainer loot, int containerIndex, int inventoryIndex)
    {
        if (loot.Inventory.Items[containerIndex] is StackableWrapper &&
            Inventory.PushItemIntoStack((StackableWrapper)loot.Inventory.Items[containerIndex]))
        {
            loot.Inventory.RemoveIndexFromInventory(containerIndex);
            return true;
            //Debug.Log("found stackable!: " + Inventory.Items[containerIndex].Name);
        }

        if (Inventory.PushItemIntoInventory(loot.Inventory.Items[containerIndex]))
        {
            loot.Inventory.RemoveIndexFromInventory(containerIndex);
            return true;
        }
        return false;
    }
    #endregion

    #region UPDATES

    void UpdateLife() // Get a life...
    {
        for (int i = 0; i < 3; i++)
        {
            CurrentStats.Stats[i] = (CurrentStats.Stats[i] < 0) ? 0 : CurrentStats.Stats[i];
            CurrentStats.Stats[i] = (CurrentStats.Stats[i] > MaximumStatValues.Stats[i]) ? MaximumStatValues.Stats[i] : CurrentStats.Stats[i];
        }

        if (CurrentStats.Stats[(int)RawStat.HEALTH] == 0)
        {
            bIsAlive = false;
            DebugState = DebugState.DEAD;
            //Source.GetComponent<Collider>().enabled = false;

            if (RigidBody != null)
            {
                RigidBody.useGravity = false;
                RigidBody.velocity = Vector3.zero;
            }
        }
    }
    void UpdateCooldowns()
    {
        for (int i = 0; i < AbilitySlots.Length; i++)
            if (AbilitySlots[i] != null)
                AbilitySlots[i].UpdateCooldown();
    }
    /*void UpdateInteractData()
    {
        InteractData.HealthCurrent = CurrentStats.Stats[(int)RawStat.HEALTH];
        InteractData.HealthMax = MaximumStatValues.Stats[(int)RawStat.HEALTH];
    }*/
    void UpdateAssetTimer()
    {
        if (!bAssetTimer)
            return;
        AssetTimer -= GlobalConstants.TIME_SCALE;
        if (AssetTimer <= 0)
        {
            bAssetTimer = false;
            if (DebugState != DebugState.DEAD)
                DebugState = DebugState.DEFAULT;
            bAssetUpdate = true;
        }
    }
    void UpdateAssets()
    {
        if (!bDebugMode || Assets == null || !bAssetUpdate)
            return;

        switch (DebugState)
        {
            case DebugState.DEAD:
                if (Assets.Dead != null)
                    Source.gameObject.GetComponent<Renderer>().material = Assets.Dead;
                break;
            case DebugState.DEFAULT:
                if (Assets.Default != null)
                    Source.gameObject.GetComponent<Renderer>().material = Assets.Default;
                break;
            case DebugState.LOSS_H:
                if (Assets.HealthLoss != null)
                    Source.gameObject.GetComponent<Renderer>().material = Assets.HealthLoss;
                break;
            case DebugState.LOSS_M:
                if (Assets.ManaLoss != null)
                    Source.gameObject.GetComponent<Renderer>().material = Assets.ManaLoss;
                break;
            case DebugState.LOSS_S:
                if (Assets.StamLoss != null)
                    Source.gameObject.GetComponent<Renderer>().material = Assets.StamLoss;
                break;
        }

        AssetTimer = GlobalConstants.TIME_BLIP;
        bAssetTimer = true;
        bAssetUpdate = false;
    }
    public void UpdateAnimationIntents(float forward, float right)
    {
        IntentForward = forward;
        IntentRight = right;
        bIntent = true;
    }
    
    void UpdateAnimation()
    {
        if (Animator == null)
            return;
        if (!bIntent)
        {
            Animator.SetFloat(GlobalConstants.ANIM_HORZ_WALK, 0);
            Animator.SetFloat(GlobalConstants.ANIM_VERT_WALK, 0);
            return;
        }
        Animator.SetFloat(GlobalConstants.ANIM_HORZ_WALK, IntentRight);
        Animator.SetFloat(GlobalConstants.ANIM_VERT_WALK, IntentForward);
        bIntent = false;
    }
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldowns();

        UpdateAnimation();
        UpdateAssetTimer();
        UpdateAssets();
        UpdateLife();

        //UpdateInteractData();
    }
}
