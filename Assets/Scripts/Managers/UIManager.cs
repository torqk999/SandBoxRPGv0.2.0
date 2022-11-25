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
    public Sprite PlaceHolderSprite;

    [Header("===LOGIC===")]
    public GameState GameState;
    public UIToolTip ToolTip;
    public RootScriptObject Dragging;

    //public bool GameStateLinked = false;
    public bool UIinitialized = false;

    [Header("Indexing")]
    //public Interaction CurrentInteraction;
    public GameMenu CurrentMenu;
    public CharPage CurrentPage;
    public CharPage OldPage;

    [Header("Prefabs")]
    public GameObject CharacterInfoPrefab;
    public GameObject ButtonPanelPrefab;
    //public GameObject ButtonListPrefab;

    public GameObject SkillButtonPrefab;
    public GameObject DraggableButtonPrefab;

    public GameObject EffectPanelPrefab;
    public GameObject PartyMemberPanelPrefab;

    [Header("Random Folders")]
    public RectTransform CharacterInfoContent;
    public RectTransform EffectStatsContent;

    [Header("Menu Logic")]
    //public Character CurrentlyViewedCharacter;

    public Page Containers;
    public Page Inventories;
    public Page Equipments;
    public Page HotBars;
    public Page SkillLists;

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
    public Canvas ToolTipCanvas;
    public Canvas PauseMenuCanvas;
    public Canvas GameMenuCanvas;
    public Canvas HUDcanvas;

    [Header("KeyMap")]
    public Transform KeyMapContent;
    public GameObject KeyMapSample;
    public Button CurrentRemap;

    ////////////////////////////////////////////////////////

    [Header("HUD")]
    public GameObject Interaction;
    public GameObject Actionbar;
    public GameObject Party;
    public Transform PartyMembers; // ??

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
    public RootButton GeneratePlaceHolder(ButtonOptions options)
    {
        options.ButtonType = ButtonType.PLACE;
        GameObject placeObject = GenerateButtonObject(options);
        RootButton placeButton = placeObject.AddComponent<RootButton>();
        placeButton.Init(options);
        placeObject.name = $"{placeButton.PlaceType} Place : {placeButton.SlotIndex}";
        return placeButton;
    }
    public RootButton GenerateSkillButton(ButtonOptions options)
    {
        return null;
    }
    public GameObject GenerateButtonObject(ButtonOptions options)
    {
        GameObject prefab = null;
        switch (options.ButtonType)
        {
            case ButtonType.PLACE:
                prefab = DraggableButtonPrefab;
                break;
                //return Instantiate(prefab, options.Page.PlaceHolders.PhysicalParent);

            case ButtonType.ITEM:
                prefab = DraggableButtonPrefab;
                break;
                //return Instantiate(prefab, options.Page.Occupants.PhysicalParent);

            case ButtonType.SKILL:
                prefab = SkillButtonPrefab;
                break;
                //return Instantiate(prefab, options.Page.Occupants.PhysicalParent);
        }

        if (prefab != null)
            return Instantiate(prefab, options.Panel.PhysicalParent);
        return null;
    }

    #endregion

    #region REMAP
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
    public void SelectCharacter(Character character)
    {
        if (character == null)
            return;

        SwapInPanel(Inventories, character.Slots.Inventory);
        SwapInPanel(Equipments, character.Slots.Equips);
        SwapInPanel(HotBars, character.Slots.HotBar);
        SwapInPanel(SkillLists, character.Slots.Skills);

        Debug.Log("Character Selected!");
    }
    void SwapInPanel(Page page, RootPanel panel)
    {
        if (page == null || panel == null)
            return;

        page.Occupants = panel;
        for (int i = 0; i < page.PlaceHolders.List.Count; i++)
            page.PlaceHolders.List[i].Assign(page.Occupants.List[i]);
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
        if (buttons != null)
            foreach (SelectableButton button in buttons)
                if (button != null)
                    button.UnSelect();
    }
    public void UnselectPool(SelectableButton[] buttons)
    {
        if (buttons != null)
            foreach (SelectableButton button in buttons)
                if (button != null)
                    button.UnSelect();
    }
    public void CharacterPageSelection()
    {
        Debug.Log("refresh toggled!");
        Inventories.Refresh = true;

        UnselectPool(Inventories.PlaceHolders.List);
        UnselectPool(Equipments.PlaceHolders.List);
    }
    public void StrategyPageSelection()
    {
        UnselectPool(HotBars.PlaceHolders.List);
        UnselectPool(SkillLists.PlaceHolders.List);
    }
    public void HotBarSelection(int slotIndex)
    {
        if (slotIndex < 0 ||
            slotIndex >= CharacterMath.HOT_BAR_SLOTS)
            return;

        GameState.pController.CurrentCharacter.CurrentAction =
            (CharacterAbility)GameState.pController.CurrentCharacter.Slots.HotBar.List[slotIndex];
    }
    public void LootSelectedContainerItem(int containerIndex, int inventoryIndex)
    {
        //Containers.LootContainer(GameState.pController.targetContainer, containerIndex, inventoryIndex);
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
                //UnselectPool(CurrentlyViewedCharacter.Slots.HotBar.List);
                CharSheet.SetActive(true);
                EquipmentPanel.SetActive(true);
                InventoryPanel.SetActive(true);
                CharacterPageSelection();
                break;

            case CharPage.Looting:
                //UnselectPool(CurrentlyViewedCharacter.Slots.HotBar.List);
                InventoryPanel.SetActive(true);
                Container.SetActive(true);
                break;

            case CharPage.Skills:
                //UnselectPool(CurrentlyViewedCharacter.Slots.HotBar.List);
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
    public Panel GenerateButtonPage(Page source, bool grid = false)
    {
        GameObject newPanelObject = Instantiate(ButtonPanelPrefab, source.ParentContent);

        if (!grid)
        {
            GridLayout gridLayout = newPanelObject.GetComponent<GridLayout>();
            Destroy(gridLayout);
        }

        Panel newPanel = newPanelObject.GetComponent<Panel>();
        newPanel.Setup(source);
        //Debug.Log("New button panel made!");
        return newPanel;
    }
    void BuildSlotPlaceHolders()
    {
        Debug.Log("Initial slot build");

        ButtonOptions buttonOptions = new ButtonOptions(Inventories.PlaceHolders, PlaceHolderType.INVENTORY, true, CharacterMath.PARTY_INVENTORY_MAX);
        Inventories.Setup(buttonOptions);

        Debug.Log("Inventory placeHolders built!");

        buttonOptions = new ButtonOptions(HotBars.PlaceHolders, PlaceHolderType.SKILL, true, CharacterMath.HOT_BAR_SLOTS);
        HotBars.Setup(buttonOptions);

        Debug.Log("HotBar placeHolders built!");
    }

    private void InitEquipPlaceHolders()
    {
        ButtonOptions options = new ButtonOptions(null);
        foreach (RootButton button in Equipments.PlaceHolders.List)
        {
            //Debug.Log("Initializing Equip PlaceHolder");
            button.Init(options);
        }
            
    }

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
    /*void PopulateEquipAndRingSlots()
    {
        for (int i = 0; i < CharacterMath.WEAR_SLOTS_COUNT && i < EquipButtons.Length; i++)
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
    }*/
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

    void MenuInitializer()
    {
        //PopulateEquipAndRingSlots();
        //BuildHotBarPlaceHolders();


        CurrentMenu = GameMenu.NONE;

        AllSheetElements.Add(InventoryPanel);
        AllSheetElements.Add(EquipmentPanel);
        AllSheetElements.Add(Container);
        AllSheetElements.Add(SkillsPanel);
        AllSheetElements.Add(CharSheet);
        AllSheetElements.Add(Strategy);
        AllSheetElements.Add(Party);

        foreach (GameObject obj in AllSheetElements)
            if (obj != null)
                obj.SetActive(false);

        //Debug.Log("yo");

        GameMenuCanvas.gameObject.SetActive(false);
        PauseMenuCanvas.gameObject.SetActive(false);

        //Debug.Log("Menus Initialized");
    }
    private void OnGUI()
    {
        CheckMap();
    }

    public void Init()
    {
        Debug.Log($"UI_init pre: {UIinitialized}");

        if (UIinitialized == true)
            return;

        GameState = (GameState)GameObject.FindGameObjectWithTag("GAME").GetComponent("GameState");
        if (GameState == null)
            return;
        UIinitialized = true;

        MenuInitializer();
        Debug.Log($"Menu success");
        BuildSlotPlaceHolders();
        Debug.Log($"PlaceHolder success");
        InitEquipPlaceHolders();
        Debug.Log("EquipHolder success");
        PauseMenuRefresh();
        Debug.Log($"PauseMenu success");

        Debug.Log($"UI_init post: {UIinitialized}");
    }

    

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!UIinitialized ||
            !GameState.Populated)
            return;

        RepopulateMemberPanels();
        UpdatePartyPanel();
        UpdateInteractionHUD();
        //UpdateCooldownBars();
        //CheckMap();
    }
}
