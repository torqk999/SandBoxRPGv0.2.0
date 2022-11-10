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
}

public class Character : Pawn, Interaction
{
    #region VARS

    [Header("Character Defs")]
    [Header("==== CHARACTER CLASS ====")]
    public int ID;

    [Header("Rendering")]
    public CharacterRender Render;
    public AnimatorType AnimType;
    public float IntentForward;
    public float IntentRight;
    public bool bIntent;

    [Header("Character Stats")]
    public StatPackage CurrentStats;
    public StatPackage SustainedStatValues;
    public StatPackage MaximumStatValues;
    public ElementPackage Resistances;

    public CharacterSheet Sheet;
    public List<CharacterAbility> Abilities;
    public List<BaseEffect> Risiduals;

    [Header("Character Logic")]
    public bool bIsPaused;
    public bool bIsSpawn;
    public bool bTimedSpawn;
    public int CountBuffer; // Currently used by the strategy system
    public float SpawnTimer;
    public float ChannelTimer;

    public Party CurrentParty;
    public Character SpawnParent; // WIP
    public Character CurrentTargetCharacter;
    public CharacterAbility CurrentAction;
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
    public CharacterAssetPack Assets;
    public DebugState DebugState;
    public bool bDebugMode;
    public bool bAssetUpdate;
    public bool bAssetTimer;
    public float AssetTimer;

    #endregion

    public void UpdateAbilites()
    {
        UpdateAbilityList();
        UpdateAbilitySlots();
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
        Risiduals = new List<BaseEffect>();

        for (int i = 0; i < 3; i++)
        {
            float magnitude = CharacterMath.RAW_MUL_RACE[(int)Sheet.Race, i] * (CharacterMath.BASE_REGEN[i] + 
                (CharacterMath.RAW_GROWTH[i] * Sheet.Level));

            //Debug.Log($"{Sheet.Name} : REGEN {(RawStat)i} : {magnitude}");
            //Risiduals.Add(CreateRegen((RawStat)i, magnitude));
            Risiduals.Add(new CurrentStatEffect((RawStat)i, magnitude));
        }
    }
    #endregion

    #region ABILITIES
    void UpdateAbilitySlots()
    {
        for (int i = CharacterMath.ABILITY_SLOTS - 1; i > -1; i--)
            if (AbilitySlots[i] != null && Abilities.FindIndex(x => x.EquipID == AbilitySlots[i].EquipID) < 0)
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
                Abilities.Add(EquipmentSlots[i].Equip.EquipAbilities[j].EquipAbility(this, EquipmentSlots[i].Equip, false));
            }
        }

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT; i++)
        {
            if (RingSlots[i] == null)
                continue;

            for (int j = 0; j < RingSlots[i].Equip.EquipAbilities.Length; j++)
            {
                Abilities.Add(RingSlots[i].Equip.EquipAbilities[j].EquipAbility(this, RingSlots[i].Equip, false));
            }
        }
    }

    #endregion

    #region EQUIPPING
    public bool InventoryEquipSelection(int equipIndex, int ringIndex, int inventoryIndex)
    {
        if (equipIndex != -1)
            return AttemptEquipRemoval(EquipmentSlots, equipIndex);

        if (ringIndex != -1)
            return AttemptEquipRemoval(RingSlots, ringIndex);

        if (inventoryIndex != -1)
        {
            if (Inventory.Items[inventoryIndex] == null || !(Inventory.Items[inventoryIndex] is EquipWrapper))
                return false;

            EquipWrapper equip = (EquipWrapper)Inventory.Items[inventoryIndex];
            bool check = false;

            switch(equip)
            {
                case WearableWrapper:
                    check = InventoryEquipWear(((Wearable)equip.Equip).Type, inventoryIndex);
                    break;

                case RingWrapper:
                    check = InventoryEquipRing(inventoryIndex);
                    break;

                case OneHandWrapper:
                    check = InventoryEquipOneHand(inventoryIndex);
                    break;

                case TwoHandWrapper:
                    check = InventoryEquipTwoHand(inventoryIndex);
                    break;

                case OffHandWrapper:
                    check = InventoryEquipOneHand(inventoryIndex, false);
                    break;

                case ShieldWrapper:
                    check = InventoryEquipOneHand(inventoryIndex, false);
                    break;
            }

            if (check)
            {
                GameState.SceneMan.InstantiateHandEquip(equip, Render);
                return true;
            }
        }

        return false;
    }
    public bool InventoryEquipWear(EquipSlot equipSlot, int inventoryIndex)
    {
        if (EquipmentSlots[(int)equipSlot] == null)
        {
            EquipmentSlots[(int)equipSlot] =
                (EquipWrapper)Inventory.RemoveIndexFromInventory(inventoryIndex);
            return true;
        }

        EquipmentSlots[(int)equipSlot] = SwapEquipWithInventory(EquipmentSlots[(int)equipSlot], inventoryIndex);
        return true;
    }
    public bool InventoryEquipOneHand(int inventoryIndex, bool main = true)
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

            EquipmentSlots[IND] =
                SwapEquipWithInventory(EquipmentSlots[IND], inventoryIndex);
            return true;
        }
        
        return false;
    }
    public bool InventoryEquipTwoHand(int inventoryIndex)
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
                SwapEquipWithInventory(EquipmentSlots[(int)EquipSlot.MAIN], inventoryIndex);
            Inventory.PushItemIntoInventory(EquipmentSlots[(int)EquipSlot.OFF]);

            EquipmentSlots[(int)EquipSlot.OFF] = EquipmentSlots[(int)EquipSlot.MAIN];
            return true;
        }
        if (EquipmentSlots[(int)EquipSlot.MAIN] != null && EquipmentSlots[(int)EquipSlot.OFF] == null) // main occupied
        {
            EquipmentSlots[(int)EquipSlot.MAIN] =
                SwapEquipWithInventory(EquipmentSlots[(int)EquipSlot.MAIN], inventoryIndex);

            EquipmentSlots[(int)EquipSlot.OFF] = EquipmentSlots[(int)EquipSlot.MAIN];
            return true;
        }
        if (EquipmentSlots[(int)EquipSlot.MAIN] == null && EquipmentSlots[(int)EquipSlot.OFF] != null) // off occupied
        {
            EquipmentSlots[7] =
                SwapEquipWithInventory(EquipmentSlots[(int)EquipSlot.OFF], inventoryIndex);

            EquipmentSlots[(int)EquipSlot.MAIN] = EquipmentSlots[(int)EquipSlot.OFF];
            return true;
        }
        Debug.Log("How did you get here? >.>");
        return false;
    }
    public bool InventoryEquipRing(int inventoryIndex)
    {
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
    }
    public bool AttemptEquipRemoval(EquipWrapper[] slotList, int equipIndex)
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

            if (equip.Instantiation != null)
                Destroy(equip.Instantiation);

            return true;
        }
        return false;
    }
    public EquipWrapper SwapEquipWithInventory(EquipWrapper input, int inventoryIndex)
    {
        // Only worry about hand slots
        if (input.Instantiation != null)
            Destroy(input.Instantiation);
        return (EquipWrapper)Inventory.SwapItemSlots(input, inventoryIndex);
    }
    #endregion

    #region COMBAT
    public float GenerateStatValueModifier(ValueType type, RawStat stat)
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
    public void ApplySingleEffect(BaseEffect effect)
    {
        switch(effect)
        {
            case CurrentStatEffect:
                ApplyDamage((CurrentStatEffect)effect);
                break;

            case ImmuneEffect:
                ApplyCleanse((ImmuneEffect)effect);
                break;
        }
    }
    public void ApplyCleanse(ImmuneEffect effect)
    {
        /*
        int index = Risiduals.FindIndex(x => x.Action == effect.Action && !x.bIsImmune);

        if (index < 0)
            return;
        */
        foreach(CrowdControlEffect ccEffect in Risiduals)
        {
            if (ccEffect.TargetCCstatus == effect.TargetCCstatus)
                Risiduals.Remove(ccEffect);
        }

        /*switch (effect.TargetCC)
        {
            case EffectAction.DMG_HEAL:
                if (Risiduals[index].TargetElement == effect.TargetElement)
                    Risiduals[index] = null;
                break;

            case EffectAction.RES_ADJ:
                if (Risiduals[index].TargetElement == effect.TargetElement)
                    Risiduals[index] = null;
                break;

            case EffectAction.STAT_ADJ:
                if (Risiduals[index].TargetStat == effect.TargetStat)
                    Risiduals[index] = null;
                break;
        }*/

    }
    public void AddRisidiualEffect(BaseEffect mod, int equipId)
    {
        BaseEffect modInstance = null;//(BaseEffect)ScriptableObject.CreateInstance("Effect");
        //modInstance.CloneEffect(mod);

        switch (mod)
        {
            case CurrentStatEffect:
                modInstance = (CurrentStatEffect)ScriptableObject.CreateInstance("CurrentStatEffect");
                ((CurrentStatEffect)modInstance).CloneEffect(mod, equipId);
                break;

            case MaxStatEffect:
                break;

            case ResistanceEffect:
                break;

            case CrowdControlEffect:
                break;
        }

        Risiduals.Add(modInstance);
    }
    void ApplyDamage(CurrentStatEffect damage)
    {
        if (CheckDamageImmune(damage.TargetStat))
            return;

        float totalValue = 0;
        float changeType = GenerateStatValueModifier(damage.Value, damage.TargetStat);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            //if (CheckElementalImmune((Element)i))
                //continue;

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
    public void UpdateResAdjust(ResistanceEffect adjust, bool apply = true)
    {
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            float change = adjust.ResAdjustments.Elements[i];
            change = apply ? change : -change;
            Resistances.Elements[i] += change;
        }
    }
    public void UpdateStatAdjust(MaxStatEffect adjust, bool apply = true)
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
        return Risiduals.Find(x => x is CrowdControlEffect &&
                                 ((CrowdControlEffect)x).TargetCCstatus == status) != null;
    }
    public bool CheckCCimmune(CCstatus status)
    {
        return Risiduals.Find(x => x is ImmuneEffect &&
                                 ((ImmuneEffect)x).TargetCCstatus == status) != null;
    }
    /*public bool CheckElementalImmune(Element element)
    {
        return Risiduals.Find(x => x.Action == EffectAction.RES_ADJ &&
                                 x.bIsImmune &&
                                 x.TargetElement == element) != null;
    }*/
    public bool CheckDamageImmune(RawStat stat)
    {
        return Risiduals.Find(x => x is InvulnerableEffect &&
                                 ((InvulnerableEffect)x).TargetStat == stat) != null;
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
            Risiduals.Add(new CrowdControlEffect("Death", CCstatus.DEAD));
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
        for (int i = Risiduals.Count - 1; i > -1; i--)
        {
            //if (!(Effects[i] is ProcEffect))
                //continue;
            //Debug.Log($"{Root.name} : {Effects[i].Name}");
            //BaseEffect risidual = character.Effects[i];
            //ProcEffect proc = (ProcEffect)Effects[i];

            ApplySingleEffect(Risiduals[i]);
            if (Risiduals[i].EquipID < 0)
            {
                Risiduals[i].Timer -= GlobalConstants.TIME_SCALE;
                if (Risiduals[i].Timer <= 0)
                {
                    Risiduals.RemoveAt(i);
                    continue;
                }
            }
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
        //Debug.Log("Intentions");
        IntentForward = forward;
        IntentRight = right;
        bIntent = true;
    }
    void UpdateAnimation()
    {
        if (Render == null)
            return;

        //Debug.Log("Animating");

        if (!bIntent)
        {
            Render.MyAnimator.SetFloat(GlobalConstants.ANIM_HORZ_WALK, 0);
            Render.MyAnimator.SetFloat(GlobalConstants.ANIM_VERT_WALK, 0);
            return;
        }         
        
        Render.MyAnimator.SetFloat(GlobalConstants.ANIM_HORZ_WALK, IntentRight);
        Render.MyAnimator.SetFloat(GlobalConstants.ANIM_VERT_WALK, IntentForward);
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
