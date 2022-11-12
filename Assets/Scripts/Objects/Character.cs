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
    [Header("==== CHARACTER CLASS ====")]
    public CharacterSheet Sheet;

    [Header("Rendering")]
    public CharacterRender Render;
    public AnimatorType AnimType;
    public float IntentForward;
    public float IntentRight;
    public bool bIntent;

    [Header("Stats")]
    public RawStatPackage BaseStats;
    public RawStatPackage CurrentStats;
    public RawStatPackage SustainedStatValues;
    public RawStatPackage MaximumStatValues;
    public ElementPackage BaseResistances;
    public ElementPackage CurrentResistances;

    [Header("Logic")]
    public int ID;
    public bool bIsPaused;
    public bool bIsSpawn;
    public bool bTimedSpawn;
    public int CountBuffer; // Currently used by the strategy system
    public float SpawnTimer;
    public float ChannelTimer;

    [Header("References")]
    public List<Character> AOE_buffer;
    public List<CharacterAbility> Abilities;
    public List<BaseEffect> Risiduals;
    public Party CurrentParty;
    public Character SpawnParent; // WIP
    public Character CurrentTargetCharacter;
    public CharacterAbility CurrentAction;
    public Inventory Inventory;
    public GameObject CharacterCanvas;

    [Header("Slots")]
    public CharacterAbility[] AbilitySlots;
    public Equipment[] EquipmentSlots;
    public Ring[] RingSlots;

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
        UpdateAbilitySlots();
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
    public void InitializeCharacterSheet()
    {
        if (Sheet == null)
            return;

        BaseStats = new RawStatPackage(Sheet);
        MaximumStatValues = new RawStatPackage(BaseStats);
        CurrentStats = new RawStatPackage(BaseStats);

        BaseResistances = new ElementPackage(Sheet);
        CurrentResistances = new ElementPackage(BaseResistances);

        Risiduals = new List<BaseEffect>();
        RebuildAbilityList(ref GameState.ABILITY_INDEX);
        //UpdateAbilites();
    }
    void RebuildAbilityList(ref int abilityId)
    {
        Abilities.Clear();

        foreach (CharacterAbility innate in Sheet.InnateAbilities)
        {
            Abilities.Add(innate.EquipAbility(this, abilityId));
            abilityId++;
        }

        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT; i++)
        {
            if (EquipmentSlots[i] == null)
                continue;

            for (int j = 0; j < EquipmentSlots[i].Abilities.Length; j++)
            {
                Abilities.Add(EquipmentSlots[i].Abilities[j].EquipAbility(this, abilityId, EquipmentSlots[i]));
                abilityId++;
            }
        }

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT; i++)
        {
            if (RingSlots[i] == null)
                continue;

            for (int j = 0; j < RingSlots[i].Abilities.Length; j++)
            {
                Abilities.Add(RingSlots[i].Abilities[j].EquipAbility(this, abilityId, RingSlots[i]));
                abilityId++;
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
            if (inventoryIndex >= Inventory.Items.Count ||
                inventoryIndex < 0)
                return false;

            if (Inventory.Items[inventoryIndex] == null ||
                !(Inventory.Items[inventoryIndex] is Equipment))
                return false;
        }

        Equipment equip = (Equipment)Inventory.Items[inventoryIndex];
        return equip.EquipToCharacter(this, ref GameState.ABILITY_INDEX, inventoryIndex);
    }
    public bool AttemptEquipRemoval(Equipment[] slotList, int equipIndex)
    {
        if (slotList == null ||
            slotList.Length == 0)
            return false;

        if (equipIndex < 0 ||
            equipIndex >= slotList.Length)
            return false;

        Equipment equip = slotList[equipIndex];
        if (equip == null)
            return false;

        return equip.UnEquipFromCharacter(this);
    }
    public Equipment SwapEquipWithInventory(Equipment input, int inventoryIndex)
    {
        // Only worry about hand slots
        if (input is Hand &&
            ((Hand)input).Instantiation != null)
            Destroy(((Hand)input).Instantiation);
        return (Equipment)Inventory.SwapItemSlots(input, inventoryIndex);
    }
    #endregion

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
        CharacterAbility call = AbilitySlots[abilityIndex];
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

        switch (call)
        {
            case ProcAbility:
                return UseTargettedAbility((ProcAbility)call, costModifier);

            case ToggleAbility:
                return UseTargettedAbility((ToggleAbility)call, costModifier);

            case SummonAbility:
                return UseSummonAbility((SummonAbility)call, costModifier);
        }

        Debug.Log("Unrecognized Ability sub-class");
        return false;
    }
    public bool CheckCanCastAbility(CharacterAbility call, float costModifier)
    {
        // Add Global Cooldown

        if (call.CD_Timer > 0) // Check Cooldown
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

        if (call.CastRange > Vector3.Distance(Root.position, CurrentTargetCharacter.Root.position))
            return false; // Check Cast Range

        return true; // Good to do things Sam!
    }
    bool UseTargettedAbility(EffectAbility call, float costModifier)
    {
        if (call.AOE_Range == 0)
        {
            switch (call.AbilityTarget)
            {
                case TargetType.SELF:
                    SpendResource(call, costModifier);
                    call.UseAbility(this);
                    return true;

                case TargetType.ALLY:
                    if (!CheckAllegiance(CurrentTargetCharacter))
                        return false;
                    break;

                case TargetType.ENEMY:
                    if (CheckAllegiance(CurrentTargetCharacter))
                        return false;
                    break;
            }
            SpendResource(call, costModifier);
            call.UseAbility(CurrentTargetCharacter);
            return true;
        }
        if (call.AOE_Range != 0)
        {
            AOEtargetting(ref AOE_buffer, call, GameState.CharacterMan.CharacterPool);
            if (AOE_buffer.Count < 1)
                return false;
            foreach (Character target in AOE_buffer)
                call.UseAbility(target);
        }
        /*if (call.AOE_Range < 0)
        {
            Debug.Log("Negative AOE range!   >:| ");
            return false;
        }*/

        return true;
    }
    bool UseSummonAbility(SummonAbility call, float modifier)
    {
        return false;
    }
    void SpendResource(CharacterAbility call, float costModifier)
    {
        CurrentStats.Stats[(int)call.CostTarget] -= call.CostValue * costModifier;

        if (call is ToggleAbility)
            SustainedStatValues.Stats[(int)call.CostTarget] += call.CostValue * costModifier;

        else
            call.SetCooldown();
    }
    void AOEtargetting(ref List<Character> AOEcandidates, EffectAbility call, List<Character> pool)
    {
        AOEcandidates.Clear();

        if (pool == null)
            return;

        foreach (Character target in pool)
        {
            if (call.AbilityTarget == TargetType.ALLY && !CheckAllegiance(target))
                continue;

            if (call.AbilityTarget == TargetType.ENEMY && CheckAllegiance(target))
                continue;

            if (Vector3.Distance(target.Root.position, Root.position) <= Math.Abs(call.AOE_Range))
                AOEcandidates.Add(target);
        }
    }
    #endregion

    #region UPDATES
    void UpdateLife() // Get a life...
    {
        for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
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
    public void UpdateResAdjust()
    {
        CurrentResistances.Clone(BaseResistances);

        foreach (ResistanceEffect adjust in Risiduals)
        {
            float resValueModifier = GenerateRawStatValueModifier(adjust.Value, RawStat.HEALTH);

            for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            {
                float change = adjust.ResAdjustments.Elements[i] * resValueModifier;
                CurrentResistances.Elements[i] += change;
            }
        }
    }
    public void UpdateStatAdjust()
    {
        MaximumStatValues.Clone(BaseStats);

        foreach (MaxStatEffect adjust in Risiduals)
        {
            for (int i = 0; i < CharacterMath.STATS_RAW_COUNT; i++)
            {
                float statValueModifier = GenerateRawStatValueModifier(adjust.Value, (RawStat)i);
                float change = adjust.StatAdjustPack.Stats[i] * statValueModifier;
                MaximumStatValues.Stats[i] += change;
            }
        }
    }
    void UpdateAbilitySlots()
    {
        for (int i = CharacterMath.ABILITY_SLOTS - 1; i > -1; i--)
            if (AbilitySlots[i] != null && Abilities.FindIndex(x => x.EquipID == AbilitySlots[i].EquipID && x.AbilityID == AbilitySlots[i].AbilityID) < 0)
                AbilitySlots[i] = null;
    }
    void UpdateAbilityCooldowns()
    {
        for (int i = 0; i < AbilitySlots.Length; i++)
            if (AbilitySlots[i] != null)
                AbilitySlots[i].UpdateCooldown();
    }
    void UpdatePassives()
    {
        foreach (PassiveAbility passive in Abilities)
            passive.UpdatePassiveTimer();
    }
    void UpdateRisidualEffects()
    {
        foreach (BaseEffect risidual in Risiduals)
            risidual.ApplySingleEffect(this);
    }
    void UpdateAdjustments()
    {
        MaximumStatValues.Clone(BaseStats);

        foreach (MaxStatEffect statAdjust in Risiduals)
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
        UpdatePassives();
        UpdateAbilityCooldowns();
        UpdateAnimation();
        UpdateAssetTimer();
        UpdateAssets();
    }
}
