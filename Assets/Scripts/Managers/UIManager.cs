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

/*
public enum ToolTipType
{
    ITEM_EQUIP,
    ITEM_CONSUME,

}

[System.Serializable]
public struct ToolTipData
{
    public TriggerType Type;
    public string Splash;
    public float HealthCurrent;
    public float HealthMax;
}
*/

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
    public GameObject HotBarButtonPrefab;
    public GameObject DraggableButtonPrefab;
    public GameObject EffectPanelPrefab;
    public GameObject PartyMemberPanelPrefab;

    [Header("Folders")]
    public Transform PartyMembers;
    public Transform InventoriesContent;
    //public Transform ContainerButtonContent;
    public Transform SkillButtonContent;
    public Transform EffectStatsContent;
    public Transform HotBarButtonContent;
    //public Transform HotBarPlaceHolderContent;

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
    public GameObject Equipment;

    [Header("==RIGHT==")]
    public GameObject Inventory;
    public GameObject Skills;

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
    public Inventory CurrentlyViewedInventory;
    public Inventory CurrentlyViewedContainer;

    public List<Inventory> Inventories;
    public List<SelectableButton> SkillListButtons;

    public SelectableButton[] EquipButtons;
    public PlaceHolderButton[] EquipPlaceHolders;

    public SelectableButton[] RingButtons;
    public PlaceHolderButton[] RingPlaceHolders;

    public SelectableButton[] HotBarButtons;

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

    /*public SelectableButton ReturnSelectableTarget(SelectableButton drag, List<SelectableButton> candidates)
    {

    }*/
    public GameObject GenerateInventoryPanel(string name = "New Inventory")
    {
        GameObject newInventory = Instantiate(InventoryPrefab, InventoriesContent);
        newInventory.name = name;
        return newInventory;
    }
    public PlaceHolderButton GeneratePlaceHolder(ButtonOptions options)
    {
        GameObject placeObject = GenerateButtonObject(options);
        PlaceHolderButton placeButton = placeObject.AddComponent<PlaceHolderButton>();
        SetupSelectableButtonOptions(placeButton, options);
        return placeButton;
    }
    public EquipmentButton GenerateEquipButton(ButtonOptions options)
    {
        GameObject equipObject = GenerateButtonObject(options);
        EquipmentButton equipButton = equipObject.AddComponent<EquipmentButton>();
        SetupSelectableButtonOptions(equipButton, options);
        return equipButton;
    }
    public InventoryButton GenerateInventoryButton(ButtonOptions options)
    {
        GameObject inventoryObject = GenerateButtonObject(options);
        InventoryButton inventoryButton = inventoryObject.AddComponent<InventoryButton>();
        SetupSelectableButtonOptions(inventoryButton, options);
        inventoryButton.Place = options.PlaceHolder;
        return inventoryButton;
    }

    void SetupSelectableButtonOptions(SelectableButton button, ButtonOptions options)
    {
        button.Init();
        button.Assign(options.Root);

        button.SlotFamily = (SelectableButton[])options.Folder;
        button.SlotIndex = options.Index;
        button.SlotFamily[button.SlotIndex] = button;

        button.transform.SetParent(options.Home);
        button.transform.localScale = Vector3.one;
    }
    public GameObject GenerateButtonObject(ButtonOptions options)
    {
        GameObject prefab = null;
        switch(options.Type)
        {
            case ButtonType.DRAG:
                prefab = DraggableButtonPrefab;
                break;

            case ButtonType.HOT_BAR:
                prefab = HotBarButtonPrefab;
                break;
        }
        GameObject newButton = null;
        if (options.Home == null)
            newButton = Instantiate(prefab);
        else
            newButton = Instantiate(prefab, options.Home);

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

        UnselectPool(HotBarButtons);
        UnselectPool(SkillListButtons);
    }
    /*public bool ButtonRelocation(ref Vector2 location, List<SelectableButton> buttons)
    {
        return false;
    }
    public bool ItemRelease(ref Vector2 location, SelectableButton button)
    {
        if (button == null)
        {
            location = Vector2.zero;
            return false;
        }


        location = Vector2.one;
        return true;
    }*/
    /*public bool EquipSelection(int equipIndex, int inventoryIndex, bool ringIndex = false)
    {
        if (!GameState.pController.CurrentCharacter.InventoryEquipSelection(equipIndex, inventoryIndex, ringIndex))
            return false;

        //GameState.pController.CurrentCharacter.UpdateAbilites();
        RefreshGameMenuCanvas();
        return true;
    }*/
    public void HotBarSelection(int slotIndex)
    {
        if (slotIndex < 0 ||
            slotIndex >= CharacterMath.ABILITY_SLOTS)
            return;

        GameState.pController.CurrentCharacter.CurrentAction = 
            GameState.pController.CurrentCharacter.Abilities[slotIndex];
    }
    /*public void DropSelectedInventoryItem(int inventoryIndex)
    {
        GameState.SceneMan.PushIntoContainer(GameState.pController.CurrentCharacter, inventoryIndex);
        Debug.Log("Completed LootBag Action");
    }*/
    public void LootSelectedContainerItem(int containerIndex, int inventoryIndex)
    {
        GameState.pController.CurrentCharacter.Inventory.LootContainer(GameState.pController.targetContainer, containerIndex, inventoryIndex);
        UpdateGameMenuCanvasState(CharPage.Looting);
    }
    /*public void EquipmentSlotClick(int index)
    {
        //if (index >= CharacterMath.EQUIP_SLOTS_COUNT)
        //    return;

        SelectedEquipSlot = index;

        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT && i < EquipButtons.Length; i++)
            EquipButtons[i].image.color = (i == index) ? Selected : Unselected;
    }
    public void RingSlotClick(int index)
    {
        //if (index >= CharacterMath.RING_SLOT_COUNT)
        //    return;

        SelectedRingSlot = index;

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT && i < RingButtons.Length; i++)
            RingButtons[i].image.color = (i == index) ? Selected : Unselected;
    }
    public void ContainerItemClick(int index)
    {
        ContainerListSelection = index;
        ContainerText.text = (index > -1) ? GameState.pController.targetContainer.Inventory.Items[index].Name : "NothingSelected";

        for (int i = 0; i < ContainerButtonContent.childCount; i++)
            ContainerButtonContent.GetChild(i).GetComponent<Button>().image.color = (i == index) ? Selected : Unselected;
    }
    public void InventoryItemClick(int index)
    {
        InventoryListSelection = index;
        InventoryText.text = (index > -1) ? GameState.pController.CurrentCharacter.Inventory.Items[index].Name : "NothingSelected";

        for (int i = 0; i < InventoryButtonContent.childCount; i++)
            InventoryButtonContent.GetChild(i).GetComponent<Button>().image.color = (i == index) ? Selected : Unselected;
    }
    public void SkillListClick(int index)
    {
        SkillListSelection = index;

        for (int i = 0; i < SkillButtonContent.childCount; i++)
            SkillButtonContent.GetChild(i).GetComponent<Button>().image.color = (i == index) ? Selected : Unselected;

        PopulateEffectPanels();
    }
    public void AbilitySlotClick(int index)
    {
        if (CurrentPage != CharPage.Skills)
            return;
        SelectedAbilitySlot = index;
        UpdateActionBar();
    }*/
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
                break;

            case CharPage.Character:
                //SelectedAbilitySlot = -1;
                UnselectPool(HotBarButtons);
                CharSheet.SetActive(true);
                Equipment.SetActive(true);
                Inventory.SetActive(true);
                break;

            case CharPage.Looting:
                //SelectedAbilitySlot = -1;
                UnselectPool(HotBarButtons);
                Inventory.SetActive(true);
                Container.SetActive(true);
                break;

            case CharPage.Skills:
                //SelectedEquipSlot = -1;
                UnselectPool(HotBarButtons);
                Strategy.SetActive(true);
                Skills.SetActive(true);
                break;
        }

        //RefreshGameMenuCanvas();
        //UpdateActionBar();
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
    /*void RefreshGameMenuCanvas()
    {
        switch (CurrentPage)
        {
            case CharPage.None:
                //SkillListClick(-1);
                break;

            case CharPage.Character:
                PopulateInventoryButtons(GameState.pController.CurrentCharacter.Inventory, ButtonType.INVENTORY);
                UpdateEquipAndRingSlots();
                break;

            case CharPage.Looting:
                PopulateInventoryButtons(GameState.pController.CurrentCharacter.Inventory, ButtonType.INVENTORY);
                if (GameState.pController.CurrentCharacter.CurrentTargetInteraction != null && GameState.pController.CurrentCharacter.CurrentTargetInteraction is GenericContainer)
                    PopulateInventoryButtons(((GenericContainer)GameState.pController.CurrentCharacter.CurrentTargetInteraction).Inventory, ButtonType.CONTAINER);
                break;

            case CharPage.Skills:
                PopulateSkillListButtons();
                //PopulateEffectPanels();
                break;
        }
    }
    void UpdateSelections(ButtonType type, int index)
    {
        //Debug.Log($"SelectionType: {type}");

        switch (type)
        {
            case ButtonType.INVENTORY:
                InventoryItemClick(index);
                ContainerItemClick(-1);
                EquipmentSlotClick(-1);
                RingSlotClick(-1);
                break;

            case ButtonType.CONTAINER:
                InventoryItemClick(-1);
                ContainerItemClick(index);
                EquipmentSlotClick(-1);
                RingSlotClick(-1);
                break;

            case ButtonType.SLOT_EQUIP:
                InventoryItemClick(-1);
                ContainerItemClick(-1);
                EquipmentSlotClick(index);
                RingSlotClick(-1);
                break;

            case ButtonType.SLOT_RING:
                InventoryItemClick(-1);
                ContainerItemClick(-1);
                EquipmentSlotClick(-1);
                RingSlotClick(index);
                break;

            case ButtonType.SLOT_SKILL:
                AbilitySlotClick(index);
                break;

            case ButtonType.LIST_SKILL:
                SkillListClick(index);
                break;
        }
    }*/
    void UpdateEquipAndRingSlots()
    {
        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT && i < EquipButtons.Length; i++)
        {
            if (EquipButtons[i] == null)
                continue;
            Image buttonImage = EquipButtons[i].GetComponent<Image>();
            if (buttonImage == null)
                continue;
            if (GameState.pController.CurrentCharacter.EquipmentSlots[i] != null &&
                GameState.pController.CurrentCharacter.EquipmentSlots[i].sprite != null)
            {
                buttonImage.sprite = GameState.pController.CurrentCharacter.EquipmentSlots[i].sprite;
                continue;
            }
            if (i < EmptyGearSprites.Length && i > -1 && EmptyGearSprites[i] != null)
            {
                buttonImage.sprite = EmptyGearSprites[i];
                continue;
            }
            if (EmptyButtonSprite != null)
                buttonImage.sprite = EmptyButtonSprite;

            //try { EquipButtons[i].GetComponent<Image>().sprite = GameState.pController.CurrentCharacter.EquipmentSlots[i].Equip.Sprite; }
            //catch { try {  } catch { EquipButtons[i].GetComponent<Image>().sprite = EmptyButtonSprite; } }
        }

        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT && i < RingButtons.Length; i++)
        {
            if (RingButtons[i] == null)
                continue;
            Image buttonImage = RingButtons[i].GetComponent<Image>();
            if (buttonImage == null)
                continue;
            if (GameState.pController.CurrentCharacter.RingSlots[i] != null &&
                GameState.pController.CurrentCharacter.RingSlots[i].sprite != null)
            {
                buttonImage.sprite = GameState.pController.CurrentCharacter.RingSlots[i].sprite;
                continue;
            }
            if (EmptyRingSprite != null)
            {
                buttonImage.sprite = EmptyRingSprite;
                continue;
            }
            if (EmptyButtonSprite != null)
                buttonImage.sprite = EmptyButtonSprite;
            //try { RingButtons[i].GetComponent<Image>().sprite = GameState.pController.CurrentCharacter.RingSlots[i].Equip.Sprite; }
            //catch { RingButtons[i].GetComponent<Image>().sprite = EmptyButtonSprite; }
        }
    }
    void UpdateInventoryButtons(Inventory inventory)
    {

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
    void BuildHotBarButtons()
    {
        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            GameObject newButtonObject = Instantiate(HotBarButtonPrefab, HotBarButtonContent);
            newButtonObject.SetActive(true);
            PlaceHolderButton newPlace = newButtonObject.AddComponent<PlaceHolderButton>();
            //CreateRemapCallBack(newButtonObject.GetComponent<Button>(), i, ButtonType.SLOT_SKILL);
        }
    }
    public void PopulateEffectPanels(CharacterAbility selection)
    {
        // Clear old effectPanels
        for (int i = EffectStatsContent.childCount - 1; i > -1; i--)
            Destroy(EffectStatsContent.GetChild(i).gameObject);

        //if (SkillListSelection < 0 || SkillListSelection >= GameState.pController.CurrentCharacter.Abilities.Count)
        //    return;

        //if (GameState.pController.CurrentCharacter.Abilities[SkillListSelection] == null)
        //    return;

        //CharacterAbility selection = GameState.pController.CurrentCharacter.Abilities[SkillListSelection];

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
    /*void PopulateEquipAndRingPlaceHolderButtons()
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
    }*/
    void UpdateToolTipState()
    {

    }
    void UpdateToolTipPosition()
    {

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
    /*void UpdateCooldownBars()
    {
        if (HotBarButtonContent == null ||
            HotBarButtonContent.transform.childCount == 0 ||
            GameState.pController.CurrentCharacter == null)
            return;

        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            Slider slider = HotBarButtonContent.GetChild(i).GetChild(1).GetComponent<Slider>();
            //Text myText = ActionButtons.transform.GetChild(i).GetChild(1).GetComponent<Text>();

            if (slider == null)
            {
                Debug.Log("Slider component missing");
                return;
            }

            if (GameState.pController.CurrentCharacter.AbilitySlots[i] == null)
            {
                slider.value = 1;
                continue;
            }

            //Debug.Log($"Updating slot:{i}");

            slider.value = 1 - (GameState.pController.CurrentCharacter.AbilitySlots[i].Logic.CD_Timer /
                GameState.pController.CurrentCharacter.AbilitySlots[i].CD_Duration);
        }
    }*/
    /*public void UpdateActionBar()
    {
        if (HotBarButtonContent == null)
            return;

        Actionbar.SetActive(GameState.bHUDactive);
        if (!Actionbar.activeSelf)
            return;

        for (int i = 0; i < CharacterMath.ABILITY_SLOTS && i < HotBarButtonContent.transform.childCount; i++)
        {
            //HotBarButtons[i].Select();
            //Slider mySlider = ActionButtons.transform.GetChild(i).GetChild(0).GetComponent<Slider>();
            //Debug.Log($"ContainerName: {HotBarButtonContent.transform.GetChild(i).GetChild(1).name}");

            Text myText = HotBarButtonContent.transform.GetChild(i).GetChild(1).GetComponent<Text>();
            Image image = HotBarButtonContent.transform.GetChild(i).GetComponent<Image>();

            //Debug.Log($"TxtFound: {myText != null} | ImgFound: {image != null}");

            image.sprite = (GameState.pController.CurrentCharacter != null
                && GameState.pController.CurrentCharacter.AbilitySlots[i] != null
                && GameState.pController.CurrentCharacter.AbilitySlots[i].Sprite != null) ?
            GameState.pController.CurrentCharacter.AbilitySlots[i].Sprite : EmptyButtonSprite;

            //Debug.Log($"TargetIndex: {GameState.KeyMap.Default.Length + i}");
            int index = GameState.KeyMap.Default.Length + i;
            //Debug.Log($"TargetAction: {GameState.KeyMap.Map[index].Action} : {GameState.KeyMap.Map[index].Index}");

            myText.text = $"{GameState.KeyMap.Map[GameState.KeyMap.Default.Length + i].Keys[0]}"; // : " +
                                                                                                  //$"{GameState.KeyMap.Map[GameState.KeyMap.Default.Length + i].Keys[1]}";
            image.color = (SelectedAbilitySlot == i) ? Selected : Unselected;
        }
    }*/
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
        //InventoryButtons = new List<SelectableButton>();

        PopulateEquipAndRingSlots();
        BuildHotBarButtons();
        //UpdateActionBar();

        CurrentMenu = GameMenu.NONE;

        AllSheetElements.Add(Inventory);
        AllSheetElements.Add(Equipment);
        AllSheetElements.Add(Container);
        AllSheetElements.Add(Skills);
        AllSheetElements.Add(CharSheet);
        AllSheetElements.Add(Strategy);

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
        try
        {
            GameState = (GameState)GameObject.FindGameObjectWithTag("GAME").GetComponent("GameState");
            UIinitializer();
            PauseMenuRefresh();
            //UIselectionRefresh();
            UIinitialized = true;
        }

        catch
        {
            Debug.Log("Failed to initialize UI");
        }
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
