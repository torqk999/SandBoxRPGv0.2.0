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
    public bool Generated;

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
        Parties[CurrentPartyIndex].CurrentMemberIndex = (Parties[CurrentPartyIndex].CurrentMemberIndex >= Parties[CurrentPartyIndex].MemberSheets.Count) ?
            0 : Parties[CurrentPartyIndex].CurrentMemberIndex;

        //GameState.bPartyChanged = true;
    }
    public Character PullCurrentCharacter()
    {
        //Debug.Log($"PartyCount: {Parties.Count}");
        //Debug.Log($"MemberCount: {Parties[CurrentPartyIndex].Members.Count}");

        if (Parties.Count < 1 ||
            Parties[CurrentPartyIndex] == null ||
            Parties[CurrentPartyIndex].MemberSheets.Count < 1)
            return null;

        return ((CharacterSheet)Parties[CurrentPartyIndex].MemberSheets[Parties[CurrentPartyIndex].CurrentMemberIndex]).Posession;
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
                    foreach (CharacterSheet foe in target.MemberSheets)
                        source.Foes.Add(foe.Posession); 
    }

    #endregion

    #region WORLD GENERATION
    public void SpawnPeeps(Transform partyStartLocation, Transform mobSpawnLocationFolder)
    {
        if (Generated)
            return;

        /// CHECK THESE FIRST!!! ///
        InitializeCharacterSheets();
        Debug.Log("Sheets initialized!");
        GameObject mobPrefab = MobPrefabs.GetChild(0).gameObject;
        CurrentPartyIndex = 0;

        CreateDefaultParty(partyStartLocation);
        CreateCloneParty(mobPrefab, mobSpawnLocationFolder, Faction.BADDIES);

        UpdatePartyFoes();

        Generated = true;
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
            //literalParty.Members.List.Add(newCharacter.Sheet); 
            literalParty.MemberSheets.Add(newCharacter.Sheet); Debug.Log("member added to literal party");
        }
        Debug.Log($"member count: {literalParty.MemberSheets.Count}");

        //GameState.UIman.CurrentlyViewedInventory = literalParty.PartyLoot; // <<-- Temporary hack
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
            cloneParty.MemberSheets.Add(newCharacter.Sheet); Debug.Log("member added to clone party");
            spawnPointFolder.GetChild(i).gameObject.SetActive(false);
        }

        //cloneParty.PartyLoot.PhysicalParent.gameObject.SetActive(false); // <<-- Temporary hack
        Parties.Add(cloneParty);
        return true;
    }
    Party GenerateParty(Faction faction, Formation formation = null, string name = "", int startIndex = 0) // add custom party names...
    {
        GameObject partyObject = new GameObject();

        partyObject.name = faction.ToString();
        partyObject.transform.parent = CharacterPartyFolder;

        Party newParty = partyObject.AddComponent<Party>();

        newParty.SetupParty(GameState, faction, name);
        newParty.CurrentMemberIndex = startIndex;

        newParty.Formation = formation;
        if (formation != null)
            formation.Parent = newParty;

        return newParty;
    }
    Character GenerateCharacter(GameObject prefab, Party party, Transform spawnTransform = null/*, Wardrobe wardrobe = null*/, int index = -1, bool fresh = true)
    {
        Debug.Log("Generating Character...");

        Character newCharacter = (Character)GameState.PawnMan.PawnGeneration(prefab, party.transform, spawnTransform);
        if (newCharacter == null)
            return null;

        Character sourceCharacter = prefab.GetComponent<Character>();

        if (index != -1)
            newCharacter.Root.name = $"{prefab.name} : {index}";

        newCharacter.GameState = GameState;
        newCharacter.bDebugMode = GameState.bDebugEffects;
        newCharacter.CurrentProximityInteractions = new List<Interaction>();

        RootOptions options = new RootOptions(GameState, sourceCharacter.Sheet, ref GameState.ROOT_SO_INDEX, party.MemberSheets, index);
        SetupCharacterSheet(newCharacter, options, fresh);
        SetupCharacterSlots(newCharacter);
        SetupCharacterAI(newCharacter);
        SetupCharacterCanvas(newCharacter);
        SetupCharacterParty(newCharacter, party);
        SetupCharacterRender(newCharacter);

        //if (wardrobe != null)
        //    GameState.EQUIPMENT_INDEX = wardrobe.CloneAndEquipWardrobe(newCharacter, GameState.EQUIPMENT_INDEX);

        CharacterPool.Add(newCharacter);


        Debug.Log("Character generated!");
        return newCharacter;
    }

    private void SetupCharacterSlots(Character newCharacter)
    {
        //newCharacter.Slots.Inventory = new List<RootScriptObject>(CharacterMath.PARTY_INVENTORY_MAX);

        newCharacter.Slots.Equips = new List<RootScriptObject>(CharacterMath.EQUIP_SLOTS_COUNT);
        for (int i = 0; i < newCharacter.Slots.Equips.Capacity; i++)
            newCharacter.Slots.Equips.Add(null);

        newCharacter.Slots.HotBar = new List<RootScriptObject>(CharacterMath.HOT_BAR_SLOTS);
        for (int i = 0; i < newCharacter.Slots.HotBar.Capacity; i++)
            newCharacter.Slots.HotBar.Add(null);
    }

    private void SetupCharacterRender(Character newCharacter)
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
    void SetupCharacterParty(Character character, Party party)
    {
        character.CurrentParty = party;
        character.Sheet.Faction = party.Faction;
        character.Slots.Inventory = party.PartyLoot;

        Debug.Log($"Char Loot Setup: {party.PartyLoot != null} ");
        
        // Initialize
        
    }
    void SetupCharacterSheet(Character character, RootOptions options, bool fresh)
    {
        if (options.Source == null)
            return;

        // Sheet
        character.Sheet = (CharacterSheet)ScriptableObject.CreateInstance("CharacterSheet");
        //RootOptions options = new RootOptions(GameState, source.Sheet, ref GameState.ROOT_SO_INDEX, GameState.UIman.Parties, index);
        character.Sheet.Clone(options);
        character.Sheet.Posession = character;
        if (fresh)
            character.Sheet.Initialize();
        
        if (options.Index > -1)
            character.Sheet.Name += options.Index.ToString();

        character.InitializeCharacterSheet();
    }
    void SetupCharacterAI(Character newCharacter, bool bAwake = true)
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
