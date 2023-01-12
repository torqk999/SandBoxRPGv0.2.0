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
    LEGS,
    BOOTS,
    MAIN,
    OFF,
    RING_0,
    RING_1
}

public class Character : Pawn, Interaction
{
    #region VARS
    [Header("==== CHARACTER CLASS ====")]
    public CharacterSheet Sheet;

    [Header("Rendering")]
    public CharacterRender Render;
    public AnimatorType AnimType;
    public float IntentForward;
    public float IntentRight;
    public bool bIntent;

    [Header("Stats")]
    public CharacterSlots Slots;
    public RawStatPackage BaseStats;
    public RawStatPackage CurrentStats;
    public RawStatPackage SustainedStatValues;
    public RawStatPackage MaximumStatValues;
    public ElementPackage BaseResistances;
    public ElementPackage CurrentResistances;

    [Header("Logic")]
    public bool bIsPaused;
    public bool bIsSpawn;
    public bool bTimedSpawn;
    public int CountBuffer; // Currently used by the strategy system
    public float SpawnTimer;
    public float ChannelTimer;
    public float GlobalCDtimer;

    [Header("References")]
    public List<Character> AOE_buffer;
    // public List<BaseEffect> Risiduals;
    public Party CurrentParty;
    public Character SpawnParent; // WIP
    public Character CurrentTargetCharacter;
    public CharacterAbility CurrentAction;

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
    public void InitializeCharacterSheet()
    {
        if (Sheet == null)
            return;

        BaseStats = new RawStatPackage(Sheet);
        MaximumStatValues.Clone(BaseStats);
        CurrentStats.Clone(BaseStats);

        BaseResistances = new ElementPackage(Sheet);
        CurrentResistances.Clone(BaseResistances);

        //Risiduals = new List<BaseEffect>();
        //RebuildAbilityList();
        //UpdateAbilites();
    }
    /*void RebuildAbilityList()
    {
        Slots.Abilities.Occupants.Places.Clear();

        if (Sheet.InnateAbilities != null)
        foreach (CharacterAbility innate in Sheet.InnateAbilities)
            innate.EquipAbility(this);


        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT; i++)
        {
            if (Slots.Equips.Occupants.Places[i] == null)
                continue;

            for (int j = 0; j < ((Equipment)Slots.Equips.Roots[i]).Abilities.Length; j++)
                ((Equipment)Slots.Equips.Roots[i]).Abilities[j].EquipAbility(this);
        }

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT; i++)
        {
            if (Slots.Rings.Roots[i] == null)
                continue;

            for (int j = 0; j < ((Equipment)Slots.Rings.Roots[i]).Abilities.Length; j++)
                ((Equipment)Slots.Rings.Roots[i]).Abilities[j].EquipAbility(this);
        }
    }*/
    #endregion

    /*#region EQUIPPING
    public bool InventoryEquipSelection(int equipIndex, int inventoryIndex)
    {
        if (equipIndex != -1)
        {
            //if (ringIndex)
                //return AttemptEquipRemoval((EquipmentButton[])Slots.Rings.Occupants.Places, equipIndex);
            return AttemptEquipRemoval((EquipmentButton[])Slots.Equips.Occupants.Places, equipIndex);
        }
            
        if (inventoryIndex != -1)
        {
            if (inventoryIndex >= Slots.Inventory.Occupants.Places.Length ||
                inventoryIndex < 0)
                return false;

            if (Slots.Inventory.Occupants.Places[inventoryIndex] == null ||
                !(Slots.Inventory.Occupants.Places[inventoryIndex] is EquipmentButton))
                return false;
        }

        Equipment equip = (Equipment)((EquipmentButton)Slots.Inventory.Occupants.Places[inventoryIndex]).Root;
        return equip.EquipToCharacter(this);
    }
    public bool AttemptEquipRemoval(EquipmentButton[] slotList, int equipIndex)
    {
        if (slotList == null ||
            slotList.Length == 0)
            return false;

        if (equipIndex < 0 ||
            equipIndex >= slotList.Length)
            return false;

        Equipment equip = (Equipment)slotList[equipIndex].Root;
        if (equip == null)
            return false;

        return equip.UnEquipFromCharacter();
    }
    public Equipment SwapEquipWithInventory(Equipment input, int inventoryIndex)
    {
        // Only worry about hand slots
        if (input is Hand &&
            ((Hand)input).Instantiation != null)
            Destroy(((Hand)input).Instantiation);
        return (Equipment)Slots.Inventory.SwapItems(input, inventoryIndex);
    }
    #endregion*/

    #region COMBAT
    public float GenerateRawStatValueModifier(ValueType type, RawStat stat)
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
    /*
    public float GenerateResValueModifier(ValueType type, Element res)
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
                changeType = CurrentResistances.Elements[(int)res] / 100;
                break;

                // lets try sumthin saucy... perc_max & perc_min give bonus res based on characters relative health proportions

            case ValueType.PERC_MAX:
                changeType = MaximumStatValues.Stats[(int)RawStat.HEALTH] / 100;
                //changeType = (CurrentResistances.Elements[(int)res] - BaseResistances.Elements[(int)res]) / 100;
                break;

            case ValueType.PERC_MISS:
                changeType = (MaximumStatValues.Stats[(int)RawStat.HEALTH] - CurrentStats.Stats[(int)RawStat.HEALTH]) / 100;
                //changeType = (MaximumStatValues.Stats[(int)stat] - CurrentStats.Stats[(int)stat]) / 100;
                break;
        }
        return changeType;
    }
    */
    public bool CheckCCstatus(CCstatus status)
    {
        return Slots.Risiduals.Find(x => x is CrowdControlEffect &&
                                 ((CrowdControlEffect)x).TargetCCstatus == status) != null;
    }
    public bool CheckCCimmune(CCstatus status)
    {
        return Slots.Risiduals.Find(x => x is ImmuneEffect &&
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
        return Slots.Risiduals.Find(x => x is InvulnerableEffect &&
                                 ((InvulnerableEffect)x).TargetStat == stat) != null;
    }
    public bool CheckAllegiance(Character target)
    {
        if (Sheet == null)
            return false;

        if (target == null ||
            target.Sheet == null)
            return false;

        return Sheet.Faction == target.Sheet.Faction;
    }
    public bool AttemptAbility(int abilityIndex)
    {
        CharacterAbility call = (CharacterAbility)Slots.HotBar[abilityIndex];
        if (call == null) // Am I a joke to you?
            return false;

        AttemptAbility(call);
        return true;
    }
    public bool AttemptAbility(CharacterAbility call)
    {
        float costModifier = GenerateRawStatValueModifier(call.CostType, call.CostTarget);

        if (!CheckCanCastAbility(call, costModifier))
            return false;

        if (!call.HasEligableTarget())
            return false;

        call.UseAbility(CurrentTargetCharacter);

        SpendResource(call, costModifier);

        GlobalCDtimer = CharacterMath.GLOBAL_COOLDOWN;
        return true;
    }
    public bool CheckCanCastAbility(CharacterAbility call, float costModifier)
    {
        if (GlobalCDtimer != 0)
            return false;

        if (CurrentTargetCharacter == null)
            return false;

        if (call.Logic.CD_Timer > 0)
            return false;

        switch (call.School) // Check CC
        {
            case School.BERZERKER:
            case School.WARRIOR:
            case School.RANGER:
            case School.ROGUE:
                if (CheckCCstatus(CCstatus.UN_ARMED))
                    return false;
                break;

            case School.PYROMANCER:
            case School.GEOMANCER:
            case School.NECROMANCER:
            case School.AEROTHURGE:
            case School.HYDROSOPHIST:
                if (CheckCCstatus(CCstatus.SILENCED))
                    return false;
                break;

            case School.MONK:
                if (CheckCCstatus(CCstatus.DRUNK))
                    return false;
                break;
        }

        if (call.CostValue * costModifier > CurrentStats.Stats[(int)call.CostTarget])
            return false; // Check Cost

        //if (call.CastRange > Vector3.Distance(Root.position, CurrentTargetCharacter.Root.position))
        //    return false; // Check Cast Range

        return true; // Good to do things Sam!
    }
    void SpendResource(CharacterAbility call, float costModifier)
    {
        CurrentStats.Stats[(int)call.CostTarget] -= call.CostValue * costModifier;

        if (call is ToggleAbility)
            SustainedStatValues.Stats[(int)call.CostTarget] += call.CostValue * costModifier;

        else
            call.SetCooldown();
    }
    #endregion
    public CrowdControlEffect RawCrowdControlGeneration(string name, CCstatus status, Sprite sprite = null) // Hard indefinite CC creation (ez death)
    {
        CrowdControlEffect newEffect = (CrowdControlEffect)ScriptableObject.CreateInstance("CrowdControlEffect");
        newEffect.Name = name;
        newEffect.sprite = sprite;
        newEffect.TargetCCstatus = status;
        return newEffect;
    }
    #region UPDATES
    public void UpdateAbilites()
    {
        UpdateAbilitySlots();
    }
    void UpdateLife() // Get a life...
    {
        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
        {
            CurrentStats.Stats[i] = (CurrentStats.Stats[i] < 0) ? 0 : CurrentStats.Stats[i];
            CurrentStats.Stats[i] = (CurrentStats.Stats[i] > MaximumStatValues.Stats[i]) ? MaximumStatValues.Stats[i] : CurrentStats.Stats[i];
        }

        if (CurrentStats.Stats[(int)RawStat.HEALTH] == 0)
        {
            if(Slots.Risiduals.Find(x => x is CrowdControlEffect && ((CrowdControlEffect)x).TargetCCstatus == CCstatus.DEAD) == null)
                Slots.Risiduals.Add(RawCrowdControlGeneration("Death", CCstatus.DEAD));

            bAssetUpdate = true;
            DebugState = DebugState.DEAD;

            IntentForward = 0;
            IntentRight = 0;

            if (RigidBody != null)
            {
                RigidBody.useGravity = false;
                RigidBody.velocity = Vector3.zero;
            }
        }
    }
    public void UpdateResAdjust()
    {
        CurrentResistances.Clone(BaseResistances);

        foreach (ResistanceEffect adjust in Slots.Risiduals)
        {
            float resValueModifier = GenerateRawStatValueModifier(adjust.Value, RawStat.HEALTH);

            for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            {
                float change = adjust.ResAdjustments.Amplification[i] * resValueModifier;
                CurrentResistances.Elements[i] += change;
            }
        }
    }
    public void UpdateStatAdjust()
    {
        MaximumStatValues.Clone(BaseStats);

        foreach (MaxStatEffect adjust in Slots.Risiduals)
        {
            for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
            {
                float statValueModifier = GenerateRawStatValueModifier(adjust.Value, (RawStat)i);
                float change = adjust.StatAdjustPack.Amplification[i] * statValueModifier;
                MaximumStatValues.Stats[i] += change;
            }
        }
    }
    void UpdateAbilitySlots()
    {
        for (int i = CharacterMath.HOT_BAR_SLOTS - 1; i > -1; i--)
            if (Slots.HotBar[i] != null && Slots.Skills.Find(x => x.RootLogic.Options.ID == Slots.HotBar[i].RootLogic.Options.ID) == null)
                Slots.HotBar[i] = null;
    }
    void UpdateAbilityCooldowns()
    {
        
        for (int i = 0; i < Slots.HotBar.Count; i++)
            if (Slots.HotBar[i] != null)
                ((CharacterAbility)Slots.HotBar[i]).UpdateCooldowns();
    }
    void UpdatePassiveAbilities()
    {
        
        foreach (CharacterAbility ability in Slots.Skills)
        {
            ability.UpdatePassiveTimer();
        }
    }
    void UpdateRisidualEffects()
    {
        foreach (BaseEffect risidual in Slots.Risiduals)
            risidual.ApplySingleEffect(this);
    }
    void UpdateAdjustments()
    {
        MaximumStatValues.Clone(BaseStats);

        foreach (MaxStatEffect statAdjust in Slots.Risiduals)
        {
            for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
            {
                //MaximumStatValues.Stats[i] += (GenerateStatValueModifier(statAdjust.Value,  ) * statAdjust.StatAdjustPack.Stats[i]);
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
    void UpdateAnimation(bool pause = false)
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
    void UpdateGlobalCooldown()
    {
        if (GlobalCDtimer != 0)
        {
            GlobalCDtimer -= GlobalConstants.TIME_SCALE;
            GlobalCDtimer = GlobalCDtimer < 0 ? 0 : GlobalCDtimer;
        }
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

        // UpdateLife();
        // UpdatePassiveAbilities();
        // UpdateRisidualEffects();
        // UpdateAbilityCooldowns();
        UpdateGlobalCooldown();
        UpdateAnimation();
        UpdateAssetTimer();
        UpdateAssets();
    }
}
