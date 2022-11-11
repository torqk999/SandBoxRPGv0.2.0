using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 
    public void AddRisidiualEffect(BaseEffect mod, int equipId)
    {
        //BaseEffect 
        Risiduals.Add(mod.GenerateEffect(equipId));
    }
    void ApplyDamage(CurrentStatEffect damage)
    {
        if (CheckDamageImmune(damage.TargetStat))
            return;

        float totalDamage = 0;
        float damageAmount = GenerateStatValueModifier(damage.Value, damage.TargetStat);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            //if (CheckElementalImmune((Element)i))
                //continue;

            float change = (damageAmount * damage.ElementPack.Elements[i]) * (1 - (CurrentResistances.Elements[i] / (CurrentResistances.Elements[i] + CharacterMath.RES_PRIME_DENOM)));
            totalDamage += (Element)i == Element.HEALING ? -change : change; // Everything but healing
        }

        if (totalDamage == 0)
            return;

        CurrentStats.Stats[(int)damage.TargetStat] -= totalDamage;
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
                if (totalDamage > 0)
                    DebugState = DebugState.LOSS_H;
                break;

            case RawStat.MANA:
                if (totalDamage > 0)
                    DebugState = DebugState.LOSS_M;
                break;

            case RawStat.SPEED:

                break;

            case RawStat.STAMINA:
                if (totalDamage > 0)
                    DebugState = DebugState.LOSS_S;
                break;
        }
    } 

 */

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
    public StatPackage BaseStats;
    public StatPackage CurrentStats;
    public StatPackage SustainedStatValues;
    public StatPackage MaximumStatValues;
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
    public float GeneratePotency(Equipment equip = null)
    {
        int equipSkill = equip == null ? (int)SkillType.UN_ARMED : (int)equip.EquipSkill;
        float weaponLevelFactor = equip == null ? 0 : equip.EquipLevel;

        return 1 +                                                                              // Base

        (((Sheet.SkillsLevels.Levels[equipSkill] * CharacterMath.CHAR_LEVEL_FACTOR) +           // Level

        (weaponLevelFactor * CharacterMath.WEP_LEVEL_FACTOR) +                                  // Weapon

        (Sheet.SkillsLevels.Levels[equipSkill] * CharacterMath.SKILL_MUL_LEVEL[equipSkill])) *  // Skill

        CharacterMath.SKILL_MUL_RACE[(int)Sheet.Race, equipSkill]);                             // Race
    }
    public void UpdateAbilites()
    {
        RebuildAbilityList();
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

        Debug.Log("Initializing Char Sheet");

        BaseStats = new StatPackage(Sheet);
        MaximumStatValues = new StatPackage(BaseStats);
        CurrentStats = new StatPackage(BaseStats);

        BaseResistances = new ElementPackage(Sheet);
        CurrentResistances = new ElementPackage(BaseResistances);

        Debug.Log("Char Packages Initialized");

        Risiduals = new List<BaseEffect>();

        //if ()
        foreach(BaseEffect innate in Sheet.InnatePassives)
        {
            Risiduals.Add(innate.GenerateEffect(GeneratePotency(), true, 0));
        }

        UpdateAbilites();
    }
    void InitializePassiveRegen()
    {
        Risiduals = new List<BaseEffect>();

        for (int i = 0; i < 3; i++)
        {
            float magnitude = CharacterMath.RAW_MUL_RACE[(int)Sheet.Race, i] * (CharacterMath.BASE_REGEN[i] + 
                (CharacterMath.RAW_GROWTH[i] * Sheet.Level));

            CurrentStatEffect newRegen = (CurrentStatEffect)ScriptableObject.CreateInstance("CurrentStatEffect");
            newRegen.FormRegen((RawStat)i, magnitude);
            Risiduals.Add(newRegen);
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
    void RebuildAbilityList()
    {
        Abilities.Clear();

        foreach (CharacterAbility innate in Sheet.InnateAbilities)
        {
            Abilities.Add(innate.GenerateAbility(GeneratePotency()));
        }

        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT; i++)
        {
            if (EquipmentSlots[i] == null)
                continue;

            for(int j = 0; j < EquipmentSlots[i].Abilities.Length; j++)
            {
                Abilities.Add(EquipmentSlots[i].Abilities[j].EquipAbility(this, EquipmentSlots[i]));
            }
        }

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT; i++)
        {
            if (RingSlots[i] == null)
                continue;

            for (int j = 0; j < RingSlots[i].Abilities.Length; j++)
            {
                Abilities.Add(RingSlots[i].Abilities[j].EquipAbility(this, RingSlots[i]));
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

            Equipment equip = (Equipment)Inventory.Items[inventoryIndex];

            if (equip.EquipCharacter(this, inventoryIndex) != -1)
            {
                //GameState.SceneMan.InstantiateHandEquip(equip, Render);
                return true;
            }
        }
        return false;
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

        return equip.EquipCharacter(this) != -1;
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
    public float GenerateStatValueModifier(ValueType type, RawStat stat, bool root = false)
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
    public void ApplyCleanse(ImmuneEffect effect)
    {
        foreach(CrowdControlEffect ccEffect in Risiduals)
        {
            if (ccEffect.TargetCCstatus == effect.TargetCCstatus)
                Risiduals.Remove(ccEffect);
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

        float costModifier = GenerateStatValueModifier(call.CostType, call.CostTarget);

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
            case AbilitySchool.ATTACK:
                if (CheckCCstatus(CCstatus.UN_ARMED))
                    return false;
                break;

            case AbilitySchool.SPELL:
                if (CheckCCstatus(CCstatus.SILENCED))
                    return false;
                break;

            case AbilitySchool.POWER:
                break;
        }

        if (call.CostValue * costModifier > CurrentStats.Stats[(int)call.CostTarget])
            return false; // Check Cost

        if (call.CastRange > Vector3.Distance(Root.position, CurrentTargetCharacter.Root.position))
            return false; // Check Cast Range

        return true; // Good to do things Sam!
    }
    bool UseTargettedAbility(TargettedAbility call, float costModifier)
    {
        if (call.AOE_Range == 0)
        {
            switch(call.AbilityTarget)
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
        }
        if (call.AOE_Range != 0)
        {
            List<Character> targets = AOEtargetting(call, GameState.CharacterMan.CharacterPool);
            if (targets.Count < 1)
                return false;
            foreach (Character target in targets)
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
    List<Character> AOEtargetting(TargettedAbility call, List<Character> pool)
    {
        List<Character> AOEcandidates = new List<Character>();

        if (pool == null)
            return AOEcandidates;

        foreach (Character target in pool)
        {
            if (call.AbilityTarget == TargetType.ALLY && !CheckAllegiance(target))
                continue;

            if (call.AbilityTarget == TargetType.ENEMY && CheckAllegiance(target))
                continue;

            if (Vector3.Distance(target.Root.position, Root.position) <= Math.Abs(call.AOE_Range))
                AOEcandidates.Add(target);
        }

        return AOEcandidates;
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
    public void UpdateResAdjust(ResistanceEffect adjust, bool apply = true)
    {
        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            float change = adjust.ResAdjustments.Elements[i];
            change = apply ? change : -change;
            CurrentResistances.Elements[i] += change;
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
            Risiduals[i].ApplySingleEffect(this);
            if (Risiduals[i].EquipID < 0) // Proc abiility, will run out
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
    void UpdateAdjustments()
    {
        MaximumStatValues.Clone(BaseStats);

        foreach(MaxStatEffect statAdjust in Risiduals)
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
