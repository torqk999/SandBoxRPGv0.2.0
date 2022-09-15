using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialProfile
{
    public MaterialType Type;
    public Material Material;
}

public class CharacterManager : MonoBehaviour
{
    [Header("References")]
    public GameState GameState;
    public Transform CharacterPartyFolder;

    [Header("CharacterWardrobe")]
    public List<MaterialProfile> MatProfiles;

    [Header("Default Party Defs")]
    public Transform DefaultPartyPrefabs;
    public Transform DefaultPartyStart;
    public Formation DefaultPartyFormation;

    [Header("Logic")]
    public List<Character> CharacterPool;
    public List<Party> Parties;
    public int CurrentPartyIndex;

    #region POSSESSION
    public void ToggleParty()
    {
        CurrentPartyIndex++;
        CurrentPartyIndex = (CurrentPartyIndex >= Parties.Count) ? 0 : CurrentPartyIndex;

        if (Parties.Count > 0)
            ResetCharacterIndex();

        GameState.bPartyChanged = true;
    }
    void ResetCharacterIndex()
    {
        if (Parties[CurrentPartyIndex] == null)
            return;

        Parties[CurrentPartyIndex].CurrentMemberIndex = 0;
    }
    public void ToggleCharacter()
    {
        if (Parties[CurrentPartyIndex] == null)
            return;

        Parties[CurrentPartyIndex].CurrentMemberIndex++;
        Parties[CurrentPartyIndex].CurrentMemberIndex = (Parties[CurrentPartyIndex].CurrentMemberIndex >= Parties[CurrentPartyIndex].Members.Count) ?
            0 : Parties[CurrentPartyIndex].CurrentMemberIndex;

        GameState.bPartyChanged = true;
    }
    public Character PullCurrentCharacter()
    {
        //Debug.Log($"PartyCount: {Parties.Count}");
        //Debug.Log($"MemberCount: {Parties[CurrentPartyIndex].Members.Count}");

        if (Parties.Count < 1 ||
            Parties[CurrentPartyIndex] == null ||
            Parties[CurrentPartyIndex].Members.Count < 1)
            return null;

        return Parties[CurrentPartyIndex].Members[Parties[CurrentPartyIndex].CurrentMemberIndex];
    }
    #endregion

    #region COMBAT
    public bool AttemptAbility(int abilityIndex, Character caller)
    {
        CharacterAbility call = caller.AbilitySlots[abilityIndex];
        float modifier = caller.GenerateValueModifier(call.CostType, call.CostTarget);

        if (!CheckAbility(call, caller, modifier))
            return false;

        //float[] stats = caller.CurrentStats.PullData();
        caller.CurrentStats.Stats[(int)call.CostTarget] -= call.CostValue * modifier;
        //caller.CurrentStats.EnterData(stats);

        call.SetCooldown();
        TargetAbility(call, caller);
        return true;
    }
    public bool CheckAbility(CharacterAbility call, Character caller, float modifier)
    {
        if (call == null) // Am I a joke to you?
            return false;

        if (call.CD_Timer > 0) // Check Cooldown
            return false;

        if (caller.CurrentCCstates[(int)call.AbilityType]) // Check CC
            return false;

        //modifier = caller.GenerateValueModifier(call.CostType, call.CostTarget);

        if (call.CostValue * modifier > caller.CurrentStats.Stats[(int)call.CostTarget])
            return false;

        return true; // Good to do things Sam!
    }
    void TargetAbility(CharacterAbility call, Character caller)
    {
        switch (call.RangeType)
        {
            case RangeType.SELF:
                ApplyAbilitySingle(caller, call);
                break;

            case RangeType.TARGET:
                if (caller.CurrentTargetCharacter == null)
                    return;
                if (Vector3.Distance(caller.CurrentTargetCharacter.Source.position, caller.Source.position) <= call.RangeValue)
                    ApplyAbilitySingle(caller.CurrentTargetCharacter, call);
                break;

            case RangeType.AOE:
                List<Character> targets = AOEtargetting(call, caller);
                if (targets.Count < 1)
                    return;
                foreach (Character target in targets)
                    ApplyAbilitySingle(target, call);
                break;
        }
    }
    List<Character> AOEtargetting(CharacterAbility call, Character caller)
    {
        List<Character> AOEcandidates = new List<Character>();

        foreach (Character target in CharacterPool)
            if (Vector3.Distance(target.Source.position, caller.Source.position) <= call.RangeValue)// &&
                //target.Sheet.Faction != caller.Sheet.Faction)
                AOEcandidates.Add(target);

        return AOEcandidates;
    }
    void ApplyAbilitySingle(Character target, CharacterAbility call)
    {
        for (int i = 0; i < call.Effects.Length; i++)
        {
            Effect mod = call.Effects[i];

            switch (mod.Duration)
            {
                case EffectDuration.ONCE:
                    target.ApplySingleEffect(call.Effects[i]);
                    break;

                case EffectDuration.TIMED:
                    ApplyRisidualEffect(target, call.Effects[i]);
                    break;

                case EffectDuration.SUSTAINED:
                    ApplyRisidualEffect(target, call.Effects[i]);
                    break;
            }
        }
    }
    /*void ApplySingleEffect(Character target, Effect mod)
    {
        float totalValue = 0;

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++) // Everything but healing
        {
            float change = mod.ElementPack.Elements[i] * (1 - target.Resistances.Elements[i]);
            totalValue += (Element)i == Element.HEALING ? -change : change;
        }

        if (totalValue == 0)
            return;

        target.CurrentStats.Stats[(int)mod.TargetStat] -= totalValue;
        target.CurrentStats.Stats[(int)mod.TargetStat] =
            target.CurrentStats.Stats[(int)mod.TargetStat] <= target.MaximumStatValues.Stats[(int)mod.TargetStat] ?
            target.CurrentStats.Stats[(int)mod.TargetStat] : target.MaximumStatValues.Stats[(int)mod.TargetStat];
        target.CurrentStats.Stats[(int)mod.TargetStat] =
            target.CurrentStats.Stats[(int)mod.TargetStat] >= 0 ?
            target.CurrentStats.Stats[(int)mod.TargetStat] : 0;

        target.bAssetUpdate = true;

        switch (mod.TargetStat)
        {
            case RawStat.HEALTH:
                if (totalValue > 0)
                    target.DebugState = DebugState.LOSS_H;
                break;

            case RawStat.MANA:
                if (totalValue > 0)
                    target.DebugState = DebugState.LOSS_M;
                break;

            case RawStat.SPEED:

                break;

            case RawStat.STAMINA:
                if (totalValue > 0)
                    target.DebugState = DebugState.LOSS_S;
                break;
        }
    }*/
    void ApplyRisidualEffect(Character target, Effect mod)
    {
        Effect modInstance = (Effect)ScriptableObject.CreateInstance("Effect");
        modInstance.Clone(mod);
        target.Effects.Add(modInstance);
    }
    #endregion

    #region CHECK-UPS
    public void ToggleCharactersPauseState(bool bPause = false)
    {
        foreach (Character character in CharacterPool)
            character.bIsPaused = bPause;
    }

    #endregion

    #region CHARACTER GENERATION
    public void CreateLiteralParty(Transform partyPrefabFolder, Faction faction, Formation formation, Transform startPosition)
    {
        if (partyPrefabFolder == null)
        {
            Debug.Log("partyPrefabFolder null, no party generated");
            return;
        }

        Party literalParty = GenerateParty(faction, formation);

        for (int i = 0; i < partyPrefabFolder.childCount; i++)
        {
            Character newCharacter = GenerateCharacter(partyPrefabFolder.GetChild(i).gameObject, literalParty, startPosition);
            if (newCharacter == null)
            {
                Debug.Log("Literal Character generation failed");
                continue;
            }

            newCharacter.bControllable = true;
            literalParty.Members.Add(newCharacter);
        }

        Parties.Add(literalParty);
    }
    public void CreateCloneParty(GameObject mobPrefab, Transform spawnPointFolder, Faction faction)
    {
        Party cloneParty = GenerateParty(faction);

        for (int i = 0; i < spawnPointFolder.childCount; i++)
        {
            if (!spawnPointFolder.GetChild(i).gameObject.activeSelf)
                continue;

            Character newCharacter = GenerateCharacter(mobPrefab, cloneParty, spawnPointFolder.GetChild(i), i);
            if (newCharacter == null)
            {
                Debug.Log("Clone Character generation failed");
                continue;
            }

            newCharacter.bControllable = false;
            cloneParty.Members.Add(newCharacter);
            spawnPointFolder.GetChild(i).gameObject.SetActive(false);
        }

        Parties.Add(cloneParty);
    }
    Party GenerateParty(Faction faction, Formation formation = null, int startIndex = 0)
    {
        GameObject partyObject = new GameObject();

        partyObject.name = faction.ToString();
        partyObject.transform.parent = CharacterPartyFolder;

        Party newParty = partyObject.AddComponent<Party>();

        newParty.PartyLoot.MaxCount = CharacterMath.PARTY_INVENTORY_MAX;
        newParty.Faction = faction;
        newParty.CurrentMemberIndex = startIndex;

        newParty.Formation = formation;
        if (formation != null)
            formation.Parent = newParty;

        return newParty;
    }
    Character GenerateCharacter(GameObject prefab, Party party, Transform spawnTransform = null, int index = -1, bool fresh = true)
    {
        Character newCharacter = (Character)GameState.PawnMan.PawnGeneration(prefab, party.transform, spawnTransform);
        if (newCharacter == null)
            return null;

        Character source = prefab.GetComponent<Character>();

        if (source != null)
            SetupCharacter(newCharacter, source, index, fresh);

        if (index != -1)
            newCharacter.Source.name = $"{prefab.name} : {index}";

        newCharacter.GameState = GameState;
        newCharacter.bDebugMode = GameState.bDebugEffects;
        newCharacter.Sheet.Faction = party.Faction;
        newCharacter.Inventory = party.PartyLoot;
        newCharacter.CurrentProximityInteractions = new List<Interaction>();

        SetupAI(newCharacter);

        CharacterPool.Add(newCharacter);

        return newCharacter;
    }
    void SetupCharacter(Character character, Character source,  int index, bool fresh)
    {
        // Slots
        character.CurrentCCstates = new bool[CharacterMath.STATS_CC_COUNT];
        character.AbilitySlots = new CharacterAbility[CharacterMath.ABILITY_SLOTS];
        character.EquipmentSlots = new EquipWrapper[CharacterMath.EQUIP_SLOTS_COUNT];
        character.RingSlots = new RingWrapper[CharacterMath.RING_SLOT_COUNT];

        Debug.Log($"sourceSheetName: {source.Sheet.Name}");

        // Sheet
        character.Sheet = (CharacterSheet)ScriptableObject.CreateInstance("CharacterSheet");
        if (source != null && source.Sheet != null)
        {
            character.Sheet.Clone(source.Sheet);
            if (fresh)
                character.Sheet.Fresh();
        }

        if (index > -1)
            character.Sheet.Name += index.ToString();

        character.bIsAlive = true;
        character.InitializeCharacter();
    }
    void SetupAI(Character newCharacter, bool bAwake = true)
    {
        if (newCharacter == null)
            return;

        SimpleAIcontroller newAI = newCharacter.gameObject.GetComponent<SimpleAIcontroller>();
        SimpleAIpathingController newPathing = newCharacter.gameObject.GetComponent<SimpleAIpathingController>();
        //Strategy newStrategy = newCharacter.gameObject.GetComponent<Strategy>();

        if (newAI != null)
        {
            newAI.GameState = GameState;
            newAI.Strategy = new Strategy(newCharacter); // Simple solution for now, will need presets
            newAI.CurrentCharacter = newCharacter;
            newAI.bIsAwake = bAwake;

            if (newPathing != null)
            {
                newPathing.GameState = GameState;
                newAI.Pathing = newPathing;
                newAI.Pathing.myAI = newAI;
            }
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
        //if (GameState.bPause)
        //    return;

        //UpdateCharacters();
    }
}
