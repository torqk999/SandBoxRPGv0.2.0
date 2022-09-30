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
    public GameObject CharCanvasPrefab;

    [Header("CharacterWardrobe")]
    public List<MaterialProfile> MatProfiles;

    [Header("Default Party Defs")]
    public Transform DefaultPartyPrefabs;
    //public Transform DefaultPartyStart;
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
        if (call == null) // Am I a joke to you?
            return false;

        float modifier = caller.GenerateStatModifier(call.CostType, call.CostTarget);

        if (!CheckAbility(call, caller, modifier))
            return false;

        return TargetAbility(call, caller, modifier);
    }
    public bool CheckAbility(CharacterAbility call, Character caller, float modifier)
    {
        if (call.CD_Timer > 0) // Check Cooldown
            return false;

        switch(call.AbilityType) // Check CC
        {
            case AbilityType.ATTACK:
                if (caller.CheckCCstatus(CCstatus.UN_ARMED))
                    return false;
                break;

            case AbilityType.SPELL:
                if (caller.CheckCCstatus(CCstatus.SILENCED))
                    return false;
                break;

            case AbilityType.POWER:
                break;
        }

        if (call.CostValue * modifier > caller.CurrentStats.Stats[(int)call.CostTarget])
            return false;

        return true; // Good to do things Sam!
    }
    bool TargetAbility(CharacterAbility call, Character caller, float modifier)
    {
        switch (call.RangeType)
        {
            case RangeType.SELF:
                ApplyAbilitySingle(caller, call);
                break;

            case RangeType.TARGET:
                if (caller.CurrentTargetCharacter == null)
                    return false;
                if (Vector3.Distance(caller.CurrentTargetCharacter.Root.position, caller.Root.position) <= call.RangeValue)
                    ApplyAbilitySingle(caller.CurrentTargetCharacter, call);
                break;

            case RangeType.AOE:
                List<Character> targets = AOEtargetting(call, caller);
                if (targets.Count < 1)
                    return false;
                foreach (Character target in targets)
                    ApplyAbilitySingle(target, call);
                break;
        }
        UseAbility(call, caller, modifier);
        return true;
    }
    void UseAbility(CharacterAbility call, Character caller, float modifier)
    {
        caller.CurrentStats.Stats[(int)call.CostTarget] -= call.CostValue * modifier;
        call.SetCooldown();
    }
    List<Character> AOEtargetting(CharacterAbility call, Character caller)
    {
        List<Character> AOEcandidates = new List<Character>();


        foreach (Character target in CharacterPool)
        {
            if (call.Target == TargetType.ALLY && target.Sheet.Faction != caller.Sheet.Faction)
                continue;

            if (call.Target == TargetType.ENEMY && target.Sheet.Faction == caller.Sheet.Faction)
                continue;

            if (Vector3.Distance(target.Root.position, caller.Root.position) <= call.RangeValue)
                AOEcandidates.Add(target);
        }
            
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
                    target.ApplyRisidualEffect(call.Effects[i]);
                    break;

                case EffectDuration.SUSTAINED:
                    target.ApplyRisidualEffect(call.Effects[i]);
                    break;
            }
        }
    }
    #endregion

    #region CHECK-UPS
    public void ToggleCharactersPauseState(bool bPause = false)
    {
        foreach (Character character in CharacterPool)
            character.bIsPaused = bPause;
    }
    public void UpdatePartyFoes()
    {
        foreach (Party source in Parties)
            foreach (Party target in Parties)
                if (source != target)
                    source.Foes.AddRange(target.Members);   
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
    public void CreateCloneParty(GameObject mobPrefab, Transform spawnPointFolder, Faction faction, Wardrobe wardrobe)
    {
        Party cloneParty = GenerateParty(faction);

        for (int i = 0; i < spawnPointFolder.childCount; i++)
        {
            if (!spawnPointFolder.GetChild(i).gameObject.activeSelf)
                continue;

            Character newCharacter = GenerateCharacter(mobPrefab, cloneParty, spawnPointFolder.GetChild(i), wardrobe, i);
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
    Character GenerateCharacter(GameObject prefab, Party party, Transform spawnTransform = null, Wardrobe wardrobe = null, int index = -1, bool fresh = true)
    {
        Character newCharacter = (Character)GameState.PawnMan.PawnGeneration(prefab, party.transform, spawnTransform);
        if (newCharacter == null)
            return null;

        Character source = prefab.GetComponent<Character>();

        if (index != -1)
            newCharacter.Root.name = $"{prefab.name} : {index}";

        SetupSheet(newCharacter, source, index, fresh);
        SetupAI(newCharacter);
        SetupCharacterCanvas(newCharacter);
        SetupCharacter(newCharacter, party);

        if (wardrobe != null)
            GameState.EQUIPMENT_INDEX = wardrobe.CloneAndEquipWardrobe(newCharacter, GameState.EQUIPMENT_INDEX);

        CharacterPool.Add(newCharacter);

        return newCharacter;
    }
    void EquipWardobe(Character character, Wardrobe wardrobe)
    {

    }

    void SetupCharacterCanvas(Character character)
    {
        if (CharCanvasPrefab == null)
            return;

        GameObject newCanvasObject = Instantiate(CharCanvasPrefab, character.Root);
        CharacterCanvas newCanvas = newCanvasObject.GetComponent<CharacterCanvas>();
        newCanvasObject.transform.localPosition = newCanvas.Offset;
        newCanvas.Cam = GameState.GameCamera;
        newCanvas.Character = character;
    }
    void SetupCharacter(Character character, Party party)
    {
        // References
        character.GameState = GameState;
        character.CurrentParty = party;
        character.bDebugMode = GameState.bDebugEffects;
        character.Sheet.Faction = party.Faction;
        character.Inventory = party.PartyLoot;
        character.CurrentProximityInteractions = new List<Interaction>();

        // Slots
        character.AbilitySlots = new CharacterAbility[CharacterMath.ABILITY_SLOTS];
        character.EquipmentSlots = new EquipWrapper[CharacterMath.EQUIP_SLOTS_COUNT];
        character.RingSlots = new RingWrapper[CharacterMath.RING_SLOT_COUNT];

        // Initialize
        character.InitializeCharacter();
    }
    void SetupSheet(Character character, Character source, int index, bool fresh)
    {
        if (source == null)
            return;

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
    }
    void SetupAI(Character newCharacter, bool bAwake = true)
    {
        if (newCharacter == null)
            return;

        SimpleAIcontroller newAI = newCharacter.gameObject.GetComponent<SimpleAIcontroller>();
        SimpleAIpathingController newPathing = newCharacter.gameObject.GetComponent<SimpleAIpathingController>();
        Strategy newStrat = newCharacter.gameObject.GetComponent<Strategy>();

        if (newAI != null)
        {
            newAI.GameState = GameState;
            
            newAI.CurrentCharacter = newCharacter;
            newAI.IsAIawake = bAwake;

            if (newPathing != null)
            {
                newPathing.GameState = GameState;
                newAI.Pathing = newPathing;
                newAI.Pathing.myAI = newAI;
            }

            if (newStrat != null)
            {
                newAI.Strategy = newStrat;
                newStrat.GameState = GameState;
                newStrat.Character = newCharacter;
            }
        }
    }
    #endregion

    #region CHARACTER REMOVAL

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
