using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum GameMenu
{
    NONE = -1,
    MAIN,
    KEY_MAP,
    OPTIONS,
    HELP,
    ABOUT,
}

public enum CharPage
{
    None,
    Character,
    Looting,
    Skills,
}

public class UIManager : MonoBehaviour
{
    #region VARS
    [Header("===GRAPHICS===")]

    public List<UIProfile> UIGraphicBin;

    public Sprite[] EmptyGearSprites;
    public Sprite EmptyRingSprite;
    public Sprite EmptyButtonSprite;

    [Header("===LOGIC===")]
    public GameState GameState;
    public UIToolTip ToolTip;
    public DraggableButton Dragging;

    public bool GameStateLinked = false;
    public bool UIinitialized = false;

    [Header("Indexing")]
    //public Interaction CurrentInteraction;
    public GameMenu CurrentMenu;
    public CharPage CurrentPage;
    public CharPage OldPage;

    [Header("Prefabs")]
    //public GameObject InventoryButtonPrefab;
    //public GameObject SkillListButtonPrefab;
    //public GameObject SkillSlotButtonPrefab;
    public Sprite PlaceHolderSprite;

    public GameObject InventoryPrefab;
    public GameObject HotBarPrefab;
    public GameObject EquipPagePrefab;
    public GameObject SkillPagePrefab;

    public GameObject SkillButtonPrefab;
    public GameObject DraggableButtonPrefab;

    public GameObject EffectPanelPrefab;
    public GameObject PartyMemberPanelPrefab;

    [Header("Folders")]
    public Transform PartyMembers;

    public RectTransform InventoriesContent;
    public RectTransform ContainersContent;
    public RectTransform EffectStatsContent;

    [Header("HUD")]
    public GameObject Interaction;
    public GameObject Actionbar;
    public GameObject Party;

    [Header("CharSheets")]
    public List<GameObject> AllSheetElements = new List<GameObject>();

    [Header("==LEFT==")]
    public GameObject CharSheet;
    public GameObject Container;
    public GameObject Strategy;

    [Header("==MIDDLE==")]
    public GameObject EquipmentPanel;

    [Header("==RIGHT==")]
    public GameObject InventoryPanel;
    public GameObject SkillsPanel;

    /////////////////////////////////////////////////////////

    [Header("Canvases")]
    //public Canvas ToolTipCanvas;
    public Canvas PauseMenuCanvas;
    public Canvas GameMenuCanvas;
    public Canvas HUDcanvas;

    [Header("KeyMap")]
    public Transform KeyMapContent;
    public GameObject KeyMapSample;
    public Button CurrentRemap;

    ////////////////////////////////////////////////////////

    [Header("Menu Logic")]
    // Cheat!!
    //public Inventory CurrentlyViewedInventory;
    //public Inventory CurrentlyViewedContainer;

    public SlotPanel InventoryPlaceHolders;
    public SlotPanel EquipPlaceHolders;
    public SlotPanel RingPlaceHolders;
    public SlotPanel HotBarPlaceHolder;
    public SlotPanel SkillPlaceHolder;

    [Header("Interaction Logic")]
    public Text InteractionHUDnameText;
    public Text InteractionHUDvalueText;
    public Slider InteractionHUDslider;

    [Header("Debugging")]
    public Text InventoryText;
    public Text ContainerText;

    public Color Unselected = new Color(1, 1, 1);
    public Color Selected = new Color(0, 1, 0);

    #endregion

    #region EXPERIMENTAL
    void foo()
    {
        EventTrigger trigger = this.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { foo2((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    void foo2(PointerEventData data)
    {

    }
    #endregion

    #region BUTTONS

    public CharacterSlots GenerateCharacterSlotsPanel()
    {
        CharacterSlots newSlots = null;
        return newSlots;
    }

    public GameObject GenerateInventoryPanel(Transform contentFolder, string name = "New Inventory")
    {
        GameObject newInventory = Instantiate(InventoryPrefab, contentFolder);
        newInventory.name = name;
        return newInventory;
    }
    public PlaceHolderButton GeneratePlaceHolder(ButtonOptions options)
    {
        options.ButtonType = ButtonType.PLACE;
        GameObject placeObject = GenerateButtonObject(options);
        PlaceHolderButton placeButton = placeObject.AddComponent<PlaceHolderButton>();
        placeButton.Init(options);
        placeObject.name = $"{placeButton.PlaceType} Place : {placeButton.SlotIndex}";
        return placeButton;
    }
    public SkillButton GenerateSkillButton(ButtonOptions options)
    {
        return null;
    }
    public GameObject GenerateButtonObject(ButtonOptions options)
    {
        GameObject prefab = null;// DraggableButtonPrefab;
        GameObject newButton = null;
        switch(options.ButtonType)
        {
            case ButtonType.PLACE:
                prefab = DraggableButtonPrefab;
                newButton = Instantiate(prefab, options.Panel.PlaceHolders.PlaceContent);
                break;

            case ButtonType.DRAG:
                prefab = DraggableButtonPrefab;
                newButton = Instantiate(prefab, options.Panel.Occupants.PlaceContent);
                break;

            case ButtonType.SKILL:
                prefab = SkillButtonPrefab;
                newButton = Instantiate(prefab, options.Panel.Occupants.PlaceContent);
                break;
        }

        return newButton;
    }

    void CreateRemapCallBack(Button button, int index)
    {
        button.onClick.AddListener(() => Remap(index, button));
    }
    void PopulateKeyMaps()
    {
        if (GameState.KeyMap == null ||
            KeyMapContent == null)
            return;

        for (int i = KeyMapContent.childCount - 1; i > -1; i--)
            Destroy(KeyMapContent.GetChild(i).gameObject);

        for (int i = 0; i < GameState.KeyMap.Map.Length; i++)
        {
            GameObject newKeyMap = Instantiate(KeyMapSample, KeyMapContent);

            Text actionLabel = newKeyMap.transform.GetChild(0).GetComponent<Text>();
            Button action0 = newKeyMap.transform.GetChild(1).GetComponent<Button>();
            Button action1 = newKeyMap.transform.GetChild(2).GetComponent<Button>();

            string index = GameState.KeyMap.Map[i].Index > -1 ? $" {GameState.KeyMap.Map[i].Index}" : string.Empty;
            actionLabel.text = GameState.KeyMap.Map[i].Action + index;

            action0.transform.GetChild(0).GetComponent<Text>().text = GameState.KeyMap.Map[i].Keys[0].ToString();
            action1.transform.GetChild(0).GetComponent<Text>().text = GameState.KeyMap.Map[i].Keys[1].ToString();

            CreateRemapCallBack(action0, 2 * i);
            CreateRemapCallBack(action1, 2 * i + 1);
        }
    }
    void Remap(int index, Button self)
    {
        if (GameState.KeyMap.bMapOpen)
            return;

        CurrentRemap = self;
        GameState.KeyMap.OpenKeyMap(index);
        self.transform.GetChild(0).GetComponent<Text>().text = "_";

        //Debug.Log(index.ToString());
    }
    void CheckMap()
    {
        if (CurrentRemap == null)
            return;

        if (!GameState.KeyMap.bMapOpen)
            return;

        //if (!Input.anyKeyDown)
        //    return;

        if (Event.current == null)
            return;

        //Debug.Log(Event.current.type);

        //if (!Event.current.isMouse &&
        //    Event.current.type == EventType.MouseDown)
        //{
        /*for (int i = 0; i < GlobalConstants.TOTAL_MOUSE_BUTTONS; ++i)
        {
            KeyCode target = (KeyCode)((int)KeyCode.Mouse0 + i);

            if (Input.GetKeyDown(target))
            {
                ResolveMap(target);
            return;
            }
        }*/


        //}

        if (Event.current.type != EventType.MouseDown &&
            Event.current.keyCode != KeyCode.None)
        {
            ResolveMap(Event.current.keyCode);
        }
    }
    void ResolveMap(KeyCode code)
    {
        GameState.KeyMap.CloseMap(code);
        CurrentRemap.transform.GetChild(0).GetComponent<Text>().text = Event.current.keyCode.ToString();
        //UpdateActionBar();
    }
    #endregion

    #region ACTIONS
    public void QuitGame()
    {
        GameState.QuitGame();
    }
    public void PauseGame(bool pause = true)
    {
        GameState.GamePause(pause);
    }
    public void ToggleCharacterPage(CharPage page)
    {
        UpdateGameMenuCanvasState(page);
        GameState.pController.TogglePlayStatus();
    }
    public void PauseMenuCanvasNavigation(int index)
    {
        if (index < -1 || index >= PauseMenuCanvas.gameObject.transform.childCount)
            CurrentMenu = GameMenu.NONE;

        else
            CurrentMenu = (GameMenu)index;

        PauseMenuRefresh();
    }
    public void UnselectPool(List<SelectableButton> buttons)
    {
        UnselectPool(buttons.ToArray());
    }
    public void UnselectPool(SelectableButton[] buttons)
    {
        if (buttons != null)
            foreach (SelectableButton button in buttons)
                if (button != null)
                    button.UnSelect();
    }
    public void CharacterPageSelection(SelectableButton selection)
    {
        if (selection == null)
            return;

        UnselectPool(GameState.pController.CurrentCharacter.Inventory.Panel.Occupants);
        UnselectPool(EquipButtons);
        UnselectPool(RingButtons);
        //UnselectPool(HotBarButtons);
    }
    public void StrategyPageSelection(SelectableButton selection)
    {
        if (selection == null)
            return;

        UnselectPool(HotBarPlaceHolder.Occupants);
        UnselectPool(SkillListButtons);
    }
    public void HotBarSelection(int slotIndex)
    {
        if (slotIndex < 0 ||
            slotIndex >= CharacterMath.HOT_BAR_SLOTS)
            return;

        GameState.pController.CurrentCharacter.CurrentAction = 
            GameState.pController.CurrentCharacter.Abilities[slotIndex];
    }
    public void LootSelectedContainerItem(int containerIndex, int inventoryIndex)
    {
        GameState.pController.CurrentCharacter.Inventory.LootContainer(GameState.pController.targetContainer, containerIndex, inventoryIndex);
        UpdateGameMenuCanvasState(CharPage.Looting);
    }
    #endregion

    #region MENUS
    public void UpdateGameMenuCanvasState(CharPage page)
    {
        if (GameState.bPause)
        {
            GameMenuCanvas.gameObject.SetActive(false);
            return;
        }

        GameMenuCanvas.gameObject.SetActive(true);

        CurrentPage = CurrentPage == page ? CharPage.None : page;

        foreach (GameObject obj in AllSheetElements)
            obj.SetActive(false);

        switch (CurrentPage)
        {
            case CharPage.None:
                GameMenuCanvas.gameObject.SetActive(false);
                Party.SetActive(true);
                break;

            case CharPage.Character:
                UnselectPool(HotBarPlaceHolder.Occupants);
                CharSheet.SetActive(true);
                EquipmentPanel.SetActive(true);
                InventoryPanel.SetActive(true);
                break;

            case CharPage.Looting:
                UnselectPool(HotBarPlaceHolder.Occupants);
                InventoryPanel.SetActive(true);
                Container.SetActive(true);
                break;

            case CharPage.Skills:
                UnselectPool(HotBarPlaceHolder.Occupants);
                Strategy.SetActive(true);
                SkillsPanel.SetActive(true);
                break;
        }

        UpdatePartyPanel();
    }
    void PauseMenuRefresh()
    {
        // Set menu
        for (int i = 0; i < PauseMenuCanvas.transform.childCount; i++)
            PauseMenuCanvas.transform.GetChild(i).gameObject.SetActive((i == (int)CurrentMenu) ? true : false);

        PauseMenuCanvas.gameObject.SetActive((int)CurrentMenu != -1);
        //return; // <<<<<<<<<<<<<<<<<<<<<

        // Clean-ups
        GameState.KeyMap.bMapOpen = false;

        switch (CurrentMenu)
        {
            case GameMenu.MAIN:
                break;

            case GameMenu.KEY_MAP:
                PopulateKeyMaps();
                break;

            case GameMenu.OPTIONS:
                break;

            case GameMenu.HELP:
                break;
        }
    }
    void UpdateSkillListButtons()
    {
        // Clear old buttons
        for (int i = SkillButtonContent.childCount - 1; i > -1; i--)
            Destroy(SkillButtonContent.GetChild(i).gameObject);

        // Populate new buttons
        int index = 0;
        foreach (CharacterAbility ability in GameState.pController.CurrentCharacter.Abilities)
        {
            if (ability == null)
                continue;

            GameObject newButtonObject = Instantiate(DraggableButtonPrefab, SkillButtonContent);

            newButtonObject.transform.GetChild(0).GetComponent<Text>().text = ability.Name;
            if (ability.sprite != null)
                newButtonObject.transform.GetChild(1).GetComponent<Image>().sprite = ability.sprite;


            newButtonObject.SetActive(true);
            //CreateRemapCallBack(newButtonObject.GetComponent<Button>(), index, ButtonType.LIST_SKILL);
            index++;
        }
    }
    void BuildCharacterPlaceHolders()
    {
        InventoryPlaceHolders.Resize(CharacterMath.PARTY_INVENTORY_MAX);
        EquipPlaceHolders.Resize(CharacterMath.EQUIP_SLOTS_COUNT);
        RingPlaceHolders.Resize(CharacterMath.RING_SLOT_COUNT);
        HotBarPlaceHolder.Resize(CharacterMath.HOT_BAR_SLOTS);
    //SkillPlaceHolder.Resize(CharacterMath.PARTY_INVENTORY_MAX);
    }
    /*void BuildHotBarPlaceHolders()
    {
        HotBarPlaceHolder.Resize(CharacterMath.ABILITY_SLOTS);
        ButtonOptions options = new ButtonOptions(HotBarPlaceHolder);
        options.PlaceType = PlaceHolderType.SKILL;
        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            options.Index = i;
            GeneratePlaceHolder(options);
        }
    }*/
    public void PopulateEffectPanels(CharacterAbility selection)
    {
        // Clear old effectPanels
        for (int i = EffectStatsContent.childCount - 1; i > -1; i--)
            Destroy(EffectStatsContent.GetChild(i).gameObject);

        switch (selection)
        {
            case EffectAbility:
                foreach (BaseEffect effect in ((EffectAbility)selection).Effects)
                {
                    GameObject newEffectPanel = Instantiate(EffectPanelPrefab, EffectStatsContent);
                    StringBuilder outputBuild = new StringBuilder(GlobalConstants.STR_BUILD_CAP);
                    outputBuild.Append($"Effect: {effect.Name}\n");
                    outputBuild.Append($"Duration: {effect.RisidualDuration}\n");

                    switch (effect)
                    {
                        case CurrentStatEffect:

                            outputBuild.Append($"Target: {((CurrentStatEffect)effect).TargetStat}\n");
                            outputBuild.Append($"Amount: {((CurrentStatEffect)effect).Value}\n");

                            for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
                            {
                                if (((CurrentStatEffect)effect).ElementPack.Elements[i] == 0)
                                    continue;

                                outputBuild.Append($"\n{(Element)i}\n" +
                                    $"Base: {((CurrentStatEffect)effect).ElementPack.Elements[i]}\n" +
                                    $"Current: {((CurrentStatEffect)effect).ElementPack.Amplification[i]}\n");
                            }
                            break;

                        case CrowdControlEffect:
                            outputBuild.Append($"Type: {((CrowdControlEffect)effect).TargetCCstatus}");
                            break;
                    }

                    Text effectText = newEffectPanel.transform.GetChild(0).GetComponent<Text>();
                    //CanvasRenderer render = newEffectPanel.GetComponent<CanvasRenderer>();
                    effectText.text = outputBuild.ToString();

                    if (effect.sprite != null)
                        newEffectPanel.transform.GetChild(1).GetComponent<Image>().sprite = effect.sprite;



                    newEffectPanel.transform.SetParent(EffectStatsContent);
                    newEffectPanel.transform.localScale = new Vector3(1, 1, 1);

                    newEffectPanel.SetActive(true);
                }
                break;
        }
    }
    void PopulateEquipAndRingSlots()
    {
        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT && i < EquipButtons.Length; i++)
        {
            if (EquipButtons[i] != null)
            {
                //Debug.Log($"GearCallback: {i}");
                //CreateRemapCallBack(EquipButtons[i], i, ButtonType.SLOT_EQUIP);
            }
        }

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT && i < RingButtons.Length; i++)
        {
            if (RingButtons[i] != null)
            {
                //Debug.Log($"RingCallbacks: {i}");
                //CreateRemapCallBack(RingButtons[i], i, ButtonType.SLOT_RING);
            }
        }
    }
    #endregion

    #region HUD
    /*public void UpdateInteraction()
    {
        if (GameState.Controller.CurrentCharacter == null)
            return;

        if (GameState.Controller.CurrentCharacter.CurrentInteraction == null)
            UpdateInteractionHUD();
        else
            UpdateInteractionHUD(GameState.Controller.CurrentCharacter.CurrentInteraction.GetInteractData());

    }*/
    void UpdatePartyPanel()
    {
        if (Party == null ||
            PartyMembers == null ||
            PartyMemberPanelPrefab == null)
            return;

        if (GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].Members.Count !=
            PartyMembers.childCount)
        {
            Party.SetActive(false);
            return; // Mis-match somewhere, extrenal panel generation
        }

        Party.SetActive(GameState.bHUDactive &&
            !GameState.bGameMenuOpen &&
            GameState.CharacterMan.Parties.Count > 0 &&
            GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].Members.Count > 0);

        if (!Party.activeSelf)
            return; // Party screen is off, doesn't need updating

        for (int i = 0; i < PartyMembers.childCount; i++)
            UpdateMemberPanel(PartyMembers.GetChild(i), GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].Members[i]);
    }
    void UpdateMemberPanel(Transform memberPanel, Character character)
    {
        memberPanel.gameObject.GetComponent<Image>().color =
            (character == GameState.pController.CurrentCharacter) ?
            Color.green : Color.white;

        for (int i = 0; i < 3; i++) // <---   Hard coded value
        {
            Slider slider = memberPanel.GetChild(1).GetChild(i).GetComponent<Slider>();
            Text text = memberPanel.GetChild(2).GetChild(i).GetComponent<Text>();
            slider.value = character.CurrentStats.Stats[i] / character.MaximumStatValues.Stats[i];
            text.text = $" {Math.Round(character.CurrentStats.Stats[i], GlobalConstants.DECIMAL_PLACES)} / {Math.Round(character.MaximumStatValues.Stats[i], GlobalConstants.DECIMAL_PLACES)}";
        }
    }
    void RepopulateMemberPanels()
    {
        if (!GameState.bPartyChanged)
            return;

        for (int i = PartyMembers.childCount - 1; i > -1; i--)
            Destroy(PartyMembers.GetChild(i).gameObject);

        foreach (Character member in GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].Members)
            GenerateMemberPanel(member);

        GameState.bPartyChanged = false;
    }
    void GenerateMemberPanel(Character character)
    {
        GameObject newMemberPanel = Instantiate(PartyMemberPanelPrefab, PartyMembers);

        if (character.Sheet.sprite != null)
            newMemberPanel.transform.GetChild(0).GetComponent<Image>().sprite = character.Sheet.sprite;
    }
    void UpdateInteractionHUD()
    {
        if (GameState.pController == null ||
            GameState.pController.CurrentCharacter == null)
            return;

        Interaction.SetActive(GameState.pController.CurrentCharacter.CurrentTargetInteraction != null && !GameMenuCanvas.gameObject.activeSelf);
        //Interaction.SetActive(state);
        //GameState.bLootingOpen = Container.activeSelf;

        if (!Interaction.gameObject.activeSelf ||
            GameState.pController.CurrentCharacter.CurrentTargetInteraction == null)
            return;

        InteractData data = GameState.pController.CurrentCharacter.CurrentTargetInteraction.GetInteractData();

        if (data is CharacterData)
        {
            CharacterData charData = (CharacterData)data;
            if (InteractionHUDnameText != null)
            {
                char append = (charData.myCharacter != null && charData.myCharacter == GameState.pController.CurrentCharacter.CurrentTargetCharacter) ? '+' : ' ';
                InteractionHUDnameText.text = append + data.Name + append;
            }

            if (InteractionHUDvalueText != null)
            {
                InteractionHUDvalueText.gameObject.SetActive(true);
                InteractionHUDvalueText.text = (charData.HealthCurrent > 0) ?
                    Math.Round(charData.HealthCurrent, GlobalConstants.DECIMAL_PLACES) + "/" + Math.Round(charData.HealthMax, GlobalConstants.DECIMAL_PLACES)
                    : "DEAD X_X";
            }

            if (InteractionHUDslider != null)
            {
                InteractionHUDslider.gameObject.SetActive(true);
                InteractionHUDslider.value = charData.HealthCurrent / charData.HealthMax;
            }
        }
        else
        {
            if (InteractionHUDvalueText != null)
            {
                InteractionHUDvalueText.gameObject.SetActive(false);
            }
            if (InteractionHUDslider != null)
            {
                InteractionHUDslider.gameObject.SetActive(false);
            }
        }
        if (data is LootData)
        {
            LootData lootData = (LootData)data;
            Debug.Log($"{lootData.Quality}");
        }


    }
    #endregion

    #region GRAPHICS


    #endregion

    void UIinitializer()
    {
        PopulateEquipAndRingSlots();
        BuildHotBarPlaceHolders();

        CurrentMenu = GameMenu.NONE;

        AllSheetElements.Add(InventoryPanel);
        AllSheetElements.Add(EquipmentPanel);
        AllSheetElements.Add(Container);
        AllSheetElements.Add(SkillsPanel);
        AllSheetElements.Add(CharSheet);
        AllSheetElements.Add(Strategy);
        AllSheetElements.Add(Party);

        foreach (GameObject obj in AllSheetElements)
            obj.SetActive(false);

        GameMenuCanvas.gameObject.SetActive(false);
        PauseMenuCanvas.gameObject.SetActive(false);
    }
    private void OnGUI()
    {
        CheckMap();
    }

    void Start()
    {
        //try
        //{
            GameState = (GameState)GameObject.FindGameObjectWithTag("GAME").GetComponent("GameState");
            UIinitializer();
            PauseMenuRefresh();
            UIinitialized = true;
        //}

        //catch
        //{
            //Debug.Log("Failed to initialize UI");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIinitialized &&
            GameStateLinked)
            Start();

        RepopulateMemberPanels();
        UpdatePartyPanel();
        UpdateInteractionHUD();
        //UpdateCooldownBars();
        //CheckMap();
    }
}
