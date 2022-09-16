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
    NECK,
    PAULDRON,
    CHEST,
    BELT,
    GLOVES,
    MAIN,
    OFF,
    LEGS,
    BOOTS,
    
    //RING
    //RING_L,
    //RING_R
}

public class Character : Pawn, Interaction
{
    [Header("Character Defs")]
    [Header("==== CHARACTER CLASS ====")]
    public int ID;

    [Header("Animation")]
    public CharacterAssetPack Assets;
    public AnimatorPlus myAnim;
    public CharacterRender Animator;
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

    public bool bIsPaused;
    public bool bIsAlive;
    public bool bIsSpawn;
    public bool bTimedSpawn;
    public float SpawnTimer;
    public float ChannelTimer;

    public Character SpawnParent;

    [Header("Character Logic")]
    public Character CurrentTargetCharacter;
    public List<Character> TargettedBy;
    public Inventory Inventory;
    public GameObject CharacterCanvas;

    [Header("Character Slots")]
    public CharacterAbility[] AbilitySlots;
    public EquipWrapper[] EquipmentSlots;
    public RingWrapper[] RingSlots;

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
    public void SwapInteractions()
    {
        if (CurrentTargetCharacter == null &&
            CurrentTargetInteraction != null &&
            CurrentTargetInteraction is Character)
        {
            CurrentTargetCharacter = (Character)CurrentTargetInteraction;
            return;
        }
        
        if (CurrentTargetCharacter != null)
        {
            CurrentTargetCharacter = null;
            RemoveInteraction(CurrentTargetCharacter);
        }

    }
    public void RemoveInteraction(Interaction interact)
    {
        int index = CurrentProximityInteractions.FindIndex(x => x == interact);
        ResolveCurrentTargetInteraction(index);
        //if (index == -1)
        //{
        //    return;
        //}
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
        GameState.pController.CurrentCharacter.CurrentTargetCharacter = this;
    }
    #endregion

    #region INITIALIZERS
    void InitializeCharacterSheet()
    {
        if (Sheet == null)
            return;

        CurrentStats = new StatPackage();
        CurrentStats.Init();
        MaximumStatValues = new StatPackage();
        MaximumStatValues.Init();

        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
        {
            float stat = CharacterMath.RAW_BASE[i] +
                (CharacterMath.RAW_GROWTH[i] *
                CharacterMath.RAW_MUL_RACE[(int)Sheet.Race, i] *
                Sheet.Level);

            CurrentStats.Stats[i] = stat;
            MaximumStatValues.Stats[i] = stat;
        }

        Resistances = new ElementPackage();// CharacterMath.STATS_ELEMENT_COUNT);
        Resistances.Init();

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            Resistances.Elements[i] = CharacterMath.RES_MUL_RACE[(int)Sheet.Race, i] * (CharacterMath.RES_BASE[i] +
                (CharacterMath.RES_GROWTH[i] * Sheet.Level));
        }

        //Debug.Log($"{Sheet.Name}/{name} : {CurrentStats.Stats.Length} : {MaximumStatValues.Stats.Length}");

        UpdateAbilites();
    }
    void InitializePassiveRegen()
    {
        Effects = new List<Effect>();

        for (int i = 0; i < 3; i++)
        {
            float magnitude = CharacterMath.RAW_MUL_RACE[(int)Sheet.Race, i] * (CharacterMath.BASE_REGEN[i] + 
                (CharacterMath.RAW_GROWTH[i] * Sheet.Level));

            //Debug.Log($"{Sheet.Name} : REGEN {(RawStat)i} : {magnitude}");
            Effects.Add(CreateRegen((RawStat)i, magnitude));
        }
    }
    Effect CreateRegen(RawStat targetStat, float magnitude)
    {
        Effect regen = (Effect)ScriptableObject.CreateInstance("Effect");
        regen.Name = $"{targetStat} REGEN";
        regen.TargetStat = targetStat;
        regen.Value = ValueType.FLAT;
        regen.Action = EffectAction.DMG_HEAL;
        regen.Duration = EffectDuration.SUSTAINED;
        regen.ElementPack = new ElementPackage();
        regen.ElementPack.Init();
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

            for(int j = 0; j < EquipmentSlots[i].Equip.EquipAbilities.Length; j++)
            {
                Abilities.Add(EquipmentSlots[i].Equip.EquipAbilities[j].EquipAbility(this, EquipmentSlots[i].Equip));
            }

        }
    }

    #endregion

    #region EQUIPPING
    public bool EquipSelection(int equipIndex, int ringIndex, int inventoryIndex)
    {
        if (equipIndex != -1)
            return AttemptEquipRemoval(EquipmentSlots, equipIndex);

        if (ringIndex != -1)
            return AttemptEquipRemoval(RingSlots, ringIndex);

        /*if (inventoryIndex == -1 && equipIndex != -1)
        {
            if (isRing)
                return AttemptEquipRemoval(RingSlots, equipIndex);
            return AttemptEquipRemoval(EquipmentSlots, equipIndex);
        }*/
            //return AttemptEquipRemoval(EquipmentSlots[equipIndex], equipIndex);

        if (inventoryIndex != -1)
        {
            if (Inventory.Items[inventoryIndex] == null || !(Inventory.Items[inventoryIndex] is EquipWrapper))
                return false;

            EquipWrapper equip = (EquipWrapper)Inventory.Items[inventoryIndex];

            if (equip is WearableWrapper)
            {
                WearableWrapper wear = (WearableWrapper)equip;
                return EquipWear(wear.Wear.Type, inventoryIndex);
            }

            if (equip is RingWrapper)
                return EquipRing(inventoryIndex);
            
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
    bool EquipWear(EquipSlot equipSlot, int inventoryIndex)
    {
        //int equipIndex = (int)equipSlot;

        if (EquipmentSlots[(int)equipSlot] == null)
        {
            EquipmentSlots[(int)equipSlot] =
                (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return true;
        }

        EquipmentSlots[(int)equipSlot] =
            (EquipWrapper)Inventory.SwapItemSlots(EquipmentSlots[(int)equipSlot], inventoryIndex);
        return true;
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
    bool EquipRing(int inventoryIndex)
    {
        /*if (Inventory.Items[inventoryIndex] == null ||
            !(Inventory.Items[inventoryIndex] is RingWrapper))
            return false;*/

        //RingWrapper ring = (RingWrapper)Inventory.Items[inventoryIndex];
        
        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT;i++)
        {
            if (RingSlots[i] == null)
            {
                RingSlots[i] = (RingWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
                return true;
            }
        }
        RingSlots[0] = (RingWrapper)Inventory.SwapItemSlots(RingSlots[0], inventoryIndex); // Default first index of rings
        return true;
        /*
        if (RingSlots[ringIndex] == null)
        {
            RingSlots[ringIndex] = (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return;
        }*/

        /*if (EquipmentSlots[(int)EquipSlot.RING_R] == null)
        {
            EquipmentSlots[(int)EquipSlot.RING_R] = (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return;
        }*/

        
    }
    bool AttemptEquipRemoval(EquipWrapper[] slotList, int equipIndex)
    {
        if (slotList == null ||
            slotList.Length == 0)
            return false;

        if (equipIndex < 0 ||
            equipIndex >= slotList.Length)
            return false;

        EquipWrapper equip = slotList[equipIndex];
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

    #region COMBAT
    public float GenerateStatModifier(ValueType type, RawStat stat)
    {
        float changeType = 0;
        switch (type)
        {
            case ValueType.NONE:
                changeType = 0;
                break;

            case ValueType.FLAT:
                changeType = 1;
                break;

            case ValueType.PERC_CURR:
                changeType = CurrentStats.Stats[(int)stat] / 100;
                break;

            case ValueType.PERC_MAX:
                changeType = MaximumStatValues.Stats[(int)stat] / 100;
                break;

            case ValueType.PERC_MISS:
                changeType = (MaximumStatValues.Stats[(int)stat] - CurrentStats.Stats[(int)stat]) / 100;
                break;
        }
        return changeType;
    }
    public void ApplySingleEffect(Effect effect)
    {
        //Debug.Log($"{Root.name} : {effect.Name}");
        if (effect.bIsImmune)
            return;
        //Debug.Log("yo0");
        switch(effect.Action)
        {
            case EffectAction.DMG_HEAL:
                //Debug.Log("yo1");
                ApplyDamage(effect);
                break;

            case EffectAction.SPAWN:
                break;
        }
    }
    void ApplyDamage(Effect damage)
    {
        if (CheckDamageImmune(damage.TargetStat))
            return;

        float totalValue = 0;
        float changeType = GenerateStatModifier(damage.Value, damage.TargetStat);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            if (CheckElementalImmune((Element)i))
                continue;

            float change = (changeType * damage.ElementPack.Elements[i]) * (1 - (Resistances.Elements[i] / (Resistances.Elements[i] + CharacterMath.RES_PRIME_DENOM)));
            totalValue += (Element)i == Element.HEALING ? -change : change; // Everything but healing
        }

        if (totalValue == 0)
            return;

        CurrentStats.Stats[(int)damage.TargetStat] -= totalValue;
        CurrentStats.Stats[(int)damage.TargetStat] =
            CurrentStats.Stats[(int)damage.TargetStat] <= MaximumStatValues.Stats[(int)damage.TargetStat] ?
            CurrentStats.Stats[(int)damage.TargetStat] : MaximumStatValues.Stats[(int)damage.TargetStat];
        CurrentStats.Stats[(int)damage.TargetStat] =
            CurrentStats.Stats[(int)damage.TargetStat] >= 0 ?
            CurrentStats.Stats[(int)damage.TargetStat] : 0;

        // Debugging
        bAssetUpdate = true;

        switch (damage.TargetStat)
        {
            case RawStat.HEALTH:
                if (totalValue > 0)
                    DebugState = DebugState.LOSS_H;
                break;

            case RawStat.MANA:
                if (totalValue > 0)
                    DebugState = DebugState.LOSS_M;
                break;

            case RawStat.SPEED:

                break;

            case RawStat.STAMINA:
                if (totalValue > 0)
                    DebugState = DebugState.LOSS_S;
                break;
        }
    }
    public void UpdateResAdjust(Effect adjust, bool apply = true)
    {
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            float change = adjust.ElementPack.Elements[i];
            change = apply ? change : -change;
            Resistances.Elements[i] += change;
        }
    }
    public void UpdateStatAdjust(Effect adjust, bool apply = true)
    {
        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
        {
            float change = adjust.StatAdjustPack.Stats[i];
            change = apply ? change : -change;
            MaximumStatValues.Stats[i] += change;
        }
    }
    public bool CheckCCstatus(CCstatus status)
    {
        return Effects.Find(x => x.Action == EffectAction.CROWD_CONTROL &&
                                 x.TargetCCstatus == status) != null;
    }
    public bool CheckCCimmune(CCstatus status)
    {
        return Effects.Find(x => x.Action == EffectAction.CROWD_CONTROL &&
                                 x.bIsImmune &&
                                 x.TargetCCstatus == status) != null;
    }
    public bool CheckElementalImmune(Element element)
    {
        return Effects.Find(x => x.Action == EffectAction.RES_ADJ &&
                                 x.bIsImmune &&
                                 x.TargetElement == element) != null;
    }
    public bool CheckDamageImmune(RawStat stat)
    {
        return Effects.Find(x => x.Action == EffectAction.DMG_HEAL &&
                                 x.bIsImmune &&
                                 x.TargetStat == stat) != null;
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
            bAssetUpdate = true;
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
    void UpdateRisidualEffects()
    {
        for (int i = Effects.Count - 1; i > -1; i--)
        {
            //Debug.Log($"{Root.name} : {Effects[i].Name}");
            //Effect risidual = character.Effects[i];
            if (Effects[i].Duration == EffectDuration.TIMED)
            {
                Effects[i].Timer -= GlobalConstants.TIME_SCALE;
                if (Effects[i].Timer <= 0)
                {
                    Effects.RemoveAt(i);
                    continue;
                }
            }
            //character.Effects[i] = risidual;
            ApplySingleEffect(Effects[i]);
        }
    }
    void UpdateAssetTimer()
    {
        if (!bAssetTimer)
            return;

        //Debug.Log($"AssetTimer: {Source.name}");

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
        if (!bDebugMode || Assets == null || !bAssetUpdate || bAssetTimer)
            return;

        //Debug.Log($"AssetUpdate: {Source.name}");

        switch (DebugState)
        {
            case DebugState.DEAD:
                if (Assets.Dead != null)
                    Root.gameObject.GetComponent<Renderer>().material = Assets.Dead;
                break;
            case DebugState.DEFAULT:
                if (Assets.Default != null)
                    Root.gameObject.GetComponent<Renderer>().material = Assets.Default;
                break;
            case DebugState.LOSS_H:
                if (Assets.HealthLoss != null)
                    Root.gameObject.GetComponent<Renderer>().material = Assets.HealthLoss;
                break;
            case DebugState.LOSS_M:
                if (Assets.ManaLoss != null)
                    Root.gameObject.GetComponent<Renderer>().material = Assets.ManaLoss;
                break;
            case DebugState.LOSS_S:
                if (Assets.StamLoss != null)
                    Root.gameObject.GetComponent<Renderer>().material = Assets.StamLoss;
                break;
        }

        AssetTimer = GlobalConstants.TIME_BLIP;
        bAssetTimer = !(DebugState == DebugState.DEFAULT 
                     || DebugState == DebugState.DEAD);
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
            Animator.MyAnimator.SetFloat(GlobalConstants.ANIM_HORZ_WALK, 0);
            Animator.MyAnimator.SetFloat(GlobalConstants.ANIM_VERT_WALK, 0);
            return;
        }
        Animator.MyAnimator.SetFloat(GlobalConstants.ANIM_HORZ_WALK, IntentRight);
        Animator.MyAnimator.SetFloat(GlobalConstants.ANIM_VERT_WALK, IntentForward);
        bIntent = false;
    }
    void UpdateCanvas()
    {
        if (CharacterCanvas == null)
            return;


    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (bIsPaused)
            return;

        UpdateRisidualEffects();
        UpdateLife();
        UpdateCooldowns();
        UpdateAnimation();
        UpdateAssetTimer();
        UpdateAssets();
    }
}
