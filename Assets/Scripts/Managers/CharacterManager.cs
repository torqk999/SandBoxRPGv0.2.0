using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RPGconstants;

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
    
    public GameObject CharacterPrefab;
    public GameObject CharCanvasPrefab;

    [Header("CharacterWardrobe")]
    public List<MaterialProfile> MatProfiles;

    [Header("Default Party Defs")]
    public Transform DefaultPartyPrefabs;
    public Transform MobPrefabs;
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

    #region WORLD GENERATION
    public void SpawnPeeps(Transform partyStartLocation, Transform mobSpawnLocationFolder)
    {
        InitializeCharacterSheets();

        /// CHECK THESE FIRST!!! ///
        GameObject mobPrefab = MobPrefabs.GetChild(0).gameObject;
        CurrentPartyIndex = 0;

        CreateDefaultParty(partyStartLocation);
        CreateCloneParty(mobPrefab, mobSpawnLocationFolder, Faction.BADDIES);

        UpdatePartyFoes();
    }
    #endregion

    #region CHARACTER GENERATION
    public void InitializeCharacterSheets()
    {
        if (DefaultPartyPrefabs == null)
            return;

        Debug.Log($"defaultCount: {DefaultPartyPrefabs.childCount}");

        for (int i = 0; i < DefaultPartyPrefabs.childCount; i++)
        {
            Character nextChar = DefaultPartyPrefabs.GetChild(i).GetComponent<Character>();
            if (nextChar == null || nextChar.Sheet == null)
                continue;
            nextChar.Sheet.Initialize(false);
        }

        if (MobPrefabs == null)
            return;

        Debug.Log($"mobCount: {MobPrefabs.childCount}");

        for (int i = 0; i < MobPrefabs.childCount; i++)
        {
            Character nextChar = MobPrefabs.GetChild(i).GetComponent<Character>();
            if (nextChar == null || nextChar.Sheet == null)
                continue;
            nextChar.Sheet.Initialize(false);
        }
    }
    public void CreateDefaultParty(Transform startLocation = null)
    {
        CreateLiteralParty(DefaultPartyPrefabs, Faction.GOODIES,
            DefaultPartyFormation, startLocation, false);
    }
    bool CreateLiteralParty(Transform partyPrefabFolder, Faction faction, Formation formation, Transform startPosition, bool fresh = true)
    {
        if (partyPrefabFolder == null)
        {
            Debug.Log("partyPrefabFolder null, no party generated");
            return false;
        }

        Party literalParty = GenerateParty(faction, formation);

        for (int i = 0; i < partyPrefabFolder.childCount; i++)
        {
            Character newCharacter = GenerateCharacter(partyPrefabFolder.GetChild(i).gameObject, literalParty, startPosition, -1, fresh);
            if (newCharacter == null)
            {
                Debug.Log("Literal Character generation failed");
                continue;
            }

            newCharacter.bControllable = true;
            literalParty.Members.Add(newCharacter);
        }

        Parties.Add(literalParty);
        return true;
    }
    public bool CreateCloneParty(GameObject mobPrefab, Transform spawnPointFolder, Faction faction)
    {
        if (mobPrefab == null ||
            spawnPointFolder == null)
            return false;

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
        return true;
    }
    Party GenerateParty(Faction faction, Formation formation = null, int startIndex = 0)
    {
        GameObject partyObject = new GameObject();

        partyObject.name = faction.ToString();
        partyObject.transform.parent = CharacterPartyFolder;

        Party newParty = partyObject.AddComponent<Party>();

        newParty.PartyLoot = new Inventory();
        newParty.Faction = faction;
        newParty.CurrentMemberIndex = startIndex;

        newParty.Formation = formation;
        if (formation != null)
            formation.Parent = newParty;

        return newParty;
    }
    Character GenerateCharacter(GameObject prefab, Party party, Transform spawnTransform = null/*, Wardrobe wardrobe = null*/, int index = -1, bool fresh = true)
    {
        Character newCharacter = (Character)GameState.PawnMan.PawnGeneration(prefab, party.transform, spawnTransform);
        if (newCharacter == null)
            return null;

        Character sourceCharacter = prefab.GetComponent<Character>();

        if (index != -1)
            newCharacter.Root.name = $"{prefab.name} : {index}";

        SetupSheet(newCharacter, sourceCharacter, index, fresh);
        SetupAI(newCharacter);
        SetupCharacterCanvas(newCharacter);
        SetupCharacter(newCharacter, party);
        SetupRender(newCharacter);

        //if (wardrobe != null)
        //    GameState.EQUIPMENT_INDEX = wardrobe.CloneAndEquipWardrobe(newCharacter, GameState.EQUIPMENT_INDEX);

        CharacterPool.Add(newCharacter);
        
        return newCharacter;
    }

    private void SetupRender(Character newCharacter)
    {
        GameObject newRenderObject = Instantiate(CharacterPrefab, newCharacter.transform);
        newRenderObject.SetActive(true);
        CharacterRender newRender = newRenderObject.GetComponent<CharacterRender>();
        newCharacter.Render = newRender;
        newRender.MyCharacter = newCharacter;

        // Need hand slot logic
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
        character.AbilitySlots = new ProcAbility[CharacterMath.ABILITY_SLOTS];
        character.EquipmentSlots = new Equipment[CharacterMath.EQUIP_SLOTS_COUNT];
        character.RingSlots = new Ring[CharacterMath.RING_SLOT_COUNT];

        // Initialize
        character.InitializeCharacterSheet();
    }
    void SetupSheet(Character character, Character source, int index, bool fresh)
    {
        if (source == null ||
            source.Sheet == null)
            return;

        // Sheet
        character.Sheet = (CharacterSheet)ScriptableObject.CreateInstance("CharacterSheet");
        RootOptions options = new RootOptions(ref GameState.ROOT_SO_INDEX);
        character.Sheet.Clone(source.Sheet, options);
        if (fresh)
            character.Sheet.Initialize();
        
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
