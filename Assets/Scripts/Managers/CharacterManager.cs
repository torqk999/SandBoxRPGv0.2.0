using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header ("References")]
    public GameState GameState;
    public Transform CharacterPartyFolder;

    [Header ("Default Party Defs")]
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

        if (!CheckAbility(call, caller))
            return false;

        float[] stats = caller.CurrentStats.PullData();
        stats[(int)call.CostType] -= call.CostValue;
        caller.CurrentStats.EnterData(stats);

        call.SetCooldown();
        TargetAbility(call, caller);
        return true;
    }
    public bool CheckAbility(CharacterAbility call, Character caller)
    {
        if (call == null) // Am I a joke to you?
            return false;

        if (call.CD_Timer > 0) // Check Cooldown
            return false;

        if (caller.CCstatus[(int)call.AbilityType]) // Check CC
            return false;

        /*
        switch (call.AbilityType) // Check CC 
        {
            case AbilityType.POWER:
                if ()
                break;

            case AbilityType.ATTACK:
                break;

            case AbilityType.SPELL:
                break;
        }
        */
        
        if (call.CostType != CostType.NONE) // Check Cost
        {
            float[] stats = caller.CurrentStats.PullData();
            if (call.CostValue > stats[(int)call.CostType])
                return false;
        }

        /*
        switch (call.CostType) // Check Cost / ApplyCost
        {
            case CostType.HEALTH:
                if (call.CostValue > caller.CurrentStats.HEALTH)
                    return false;
                caller.CurrentStats.HEALTH -= call.CostValue;
                break;

            case CostType.STAMINA:
                if (call.CostValue > caller.CurrentStats.STAMINA)
                    return false;
                caller.CurrentStats.STAMINA -= call.CostValue;
                break;

            case CostType.MANA:
                if (call.CostValue > caller.CurrentStats.MANA)
                    return false;
                caller.CurrentStats.MANA -= call.CostValue;
                break;

            case CostType.NONE:
                break;
        }
        */

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
                if (caller.Target == null)
                    return;
                if (Vector3.Distance(caller.Target.Source.position, caller.Source.position) <= call.RangeValue)
                    ApplyAbilitySingle(caller.Target, call);
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

        foreach(Character target in CharacterPool)
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

            switch (mod.Type)
            {
                case EffectType.ONCE:
                    ApplySingleEffect(target, call.Effects[i]);
                    break;

                case EffectType.TIMED:
                    ApplyRisidualEffect(target, call.Effects[i]);
                    break;

                case EffectType.PASSIVE:
                    ApplyRisidualEffect(target, call.Effects[i]);
                    break;
            }
        }
    }
    void ApplySingleEffect(Character target, Effect mod)
    {
        float[] baseData = mod.ElementPack.PullData();
        float[] resData = target.Resistances.PullData();
        float totalValue = 0;

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT - 1; i++) // Everything but healing
            totalValue += baseData[i] * (1 - resData[i]);

        // Healing, it is expected that this element is the last index of all the elements
        totalValue -= baseData[CharacterMath.STATS_ELEMENT_COUNT - 1] * (1 - resData[CharacterMath.STATS_ELEMENT_COUNT - 1]);

        switch (mod.Target)
        {
            case EffectTarget.HEALTH:
                target.CurrentStats.HEALTH -= totalValue;
                if (totalValue > 0)
                    target.DebugState = DebugState.LOSS_H;
                break;

            case EffectTarget.MANA:
                target.CurrentStats.MANA -= totalValue;
                if (totalValue > 0)
                    target.DebugState = DebugState.LOSS_M;
                break;

            case EffectTarget.SPEED:
                target.CurrentStats.SPEED -= totalValue;
                break;

            case EffectTarget.STAMINA:
                target.CurrentStats.STAMINA -= totalValue;
                if (totalValue > 0)
                    target.DebugState = DebugState.LOSS_S;
                break;
        }
        if (totalValue > 0)
            target.bAssetUpdate = true;
    }
    void ApplyRisidualEffect(Character target, Effect mod)
    {
        Effect modInstance = new Effect(mod);
        target.Effects.Add(modInstance);
    }
    #endregion

    #region CHECK-UPS
    void UpdateCharacters()
    {
        foreach(Character character in CharacterPool)
        {
            if (character.bIsAlive)
                UpdateRisidualEffects(character);
        }
    }
    void UpdateRisidualEffects(Character character)
    {
        for (int i = character.Effects.Count - 1; i > -1; i--)
        {
            Effect risidual = character.Effects[i];
            if (risidual.Type == EffectType.TIMED)
            {
                risidual.Timer -= GlobalConstants.TIME_SCALE;
                if (risidual.Timer <= 0)
                {
                    character.Effects.RemoveAt(i);
                    continue;
                }    
            }
            character.Effects[i] = risidual;

            ApplySingleEffect(character, character.Effects[i]);
        }
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

        for(int i = 0; i < partyPrefabFolder.childCount; i++)
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

        for(int i = 0; i < spawnPointFolder.childCount; i++)
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
    Character GenerateCharacter(GameObject prefab, Party party, Transform spawnTransform = null, int index = -1)
    {
        Character newCharacter = (Character)GameState.PawnMan.PawnGeneration(prefab, party.transform, spawnTransform);
        if (newCharacter == null)
            return null;

        SetupCharacter(newCharacter, index);

        if (index != -1)
            newCharacter.Source.name = prefab.name + ":" + index;

        newCharacter.GameState = GameState;
        newCharacter.bDebugMode = GameState.bDebugEffects;
        newCharacter.Sheet.Faction = party.Faction;
        newCharacter.Inventory = party.PartyLoot;
        
        SetupAI(newCharacter);

        CharacterPool.Add(newCharacter);

        return newCharacter;
    }
    void SetupCharacter(Character character, int index)
    {
        // Slots
        character.CCstatus = new bool[3];
        character.AbilitySlots = new CharacterAbility[CharacterMath.ABILITY_SLOTS];
        character.EquipmentSlots = new EquipWrapper[CharacterMath.EQUIP_SLOTS];

        // Sheet
        CharacterSheet sheetInstance = (CharacterSheet)ScriptableObject.CreateInstance("CharacterSheet");
        sheetInstance.Clone(character.Sheet);
        character.Sheet = sheetInstance;

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

    #region OLD
    /*
    public void EquipAbilities(EquipWrapper equipWrapper, Character character)
    {
        foreach(CharacterAbility ability in equipWrapper.Equip.EquipAbilites)
        {
            character.Abilities.Add(ability.EquipAbility(character, equipWrapper.Equip));
            Debug.Log("Ability Added!");
        }
    }
    public void UnEquipAbilities(EquipWrapper equip)
    {

    }
    */
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //CreateLiteralParty(DefaultPartyPrefabs, Faction.GOODIES);
        //GameState.testBuilder.SpawnMobs();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.bPause)
            return;

        UpdateCharacters();
    }
}
