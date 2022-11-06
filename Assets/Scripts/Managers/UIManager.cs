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
    public bool GameStateLinked = false;
    public bool UIinitialized = false;

    [Header("Indexing")]
    //public Interaction CurrentInteraction;
    public GameMenu CurrentMenu;
    public CharPage CurrentPage;
    public CharPage OldPage;

    [Header("Prefabs")]
    public GameObject InventoryButtonPrefab;
    public GameObject SkillListButtonPrefab;
    public GameObject SkillSlotButtonPrefab;
    public GameObject EffectPanelPrefab;
    public GameObject PartyMemberPanelPrefab;
    

    [Header("Folders")]
    public Transform InventoryButtonContent;
    public Transform ContainerButtonContent;
    public Transform SkillButtonContent;
    public Transform EffectStatsContent;
    public Transform HotBarButtonContent;
    public Transform PartyMembers;

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
    public int InventoryListSelection;
    public int ContainerListSelection;
    public int SkillListSelection;

    public int SelectedEquipSlot;
    public int SelectedRingSlot;
    public int SelectedAbilitySlot;

    public Button[] EquipButtons;
    public Button[] RingButtons;


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
    void CreateCallBackIdentity(Button button, int index, ButtonType type)
    {
        switch (type)
        {
            case ButtonType.INVENTORY:
                button.onClick.AddListener(() => UpdateSelections(ButtonType.INVENTORY, index));
                break;

            case ButtonType.CONTAINER:
                button.onClick.AddListener(() => UpdateSelections(ButtonType.CONTAINER, index));
                break;

            case ButtonType.LIST_SKILL:
                button.onClick.AddListener(() => UpdateSelections(ButtonType.LIST_SKILL, index));
                break;

            case ButtonType.SLOT_SKILL:
                button.onClick.AddListener(() => UpdateSelections(ButtonType.SLOT_SKILL, index));
                break;

            case ButtonType.SLOT_EQUIP:
                button.onClick.AddListener(() => UpdateSelections(ButtonType.SLOT_EQUIP, index));
                break;

            case ButtonType.SLOT_RING:
                button.onClick.AddListener(() => UpdateSelections(ButtonType.SLOT_RING, index));
                break;

            case ButtonType.KEY_MAP:
                button.onClick.AddListener(() => Remap(index, button));
                break;
        }
    }
    void CreateHoverIdentity(EventTrigger trigger, int index, ButtonType type)
    {
        EventTrigger.Entry enter = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();

        enter.eventID = EventTriggerType.PointerEnter;
        exit.eventID = EventTriggerType.PointerExit;

        switch (type)
        {
            case ButtonType.INVENTORY:
                enter.callback.AddListener((enter) => InventoryItemHover(index));
                exit.callback.AddListener((exit) => InventoryItemHover(index, false));
                break;

            case ButtonType.CONTAINER:
                break;

            case ButtonType.LIST_SKILL:
                break;

            case ButtonType.KEY_MAP:
                break;
        }

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
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

            CreateCallBackIdentity(action0, 2 * i, ButtonType.KEY_MAP);
            CreateCallBackIdentity(action1, 2 * i + 1, ButtonType.KEY_MAP);
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
        UpdateActionBar();
    }
    #endregion

    #region ACTIONS
    public void PauseMenuCanvasNavigation(int index)
    {
        if (index < -1 || index >= PauseMenuCanvas.gameObject.transform.childCount)
        {
            CurrentMenu = GameMenu.NONE;
            PauseMenuRefresh();
            return;
        }
        CurrentMenu = (GameMenu)index;
        PauseMenuRefresh();
    }
    public void EquipSelection()
    {
        if (!GameState.pController.CurrentCharacter.InventoryEquipSelection(SelectedEquipSlot, SelectedRingSlot, InventoryListSelection))
            return;

        
        GameState.pController.CurrentCharacter.UpdateAbilites();
        RefreshGameMenuCanvas();
        UpdateActionBar(); // may have lost equipped abillities
    }
    public void AbilitySelection(bool bEquip)
    {
        if (SelectedAbilitySlot < 0)
            return;

        GameState.pController.CurrentCharacter.AbilitySlots[SelectedAbilitySlot] = (bEquip) ?
            GameState.pController.CurrentCharacter.Abilities[SkillListSelection] : null;
        UpdateActionBar();
    }
    public void DropSelectedInventoryItem()
    {
        GameState.SceneMan.PushIntoContainer(GameState.pController.CurrentCharacter, InventoryListSelection);
        Debug.Log("Completed LootBag Action");
    }
    public void LootSelectedContainerItem()
    {
        GameState.pController.CurrentCharacter.Inventory.LootContainer(GameState.pController.targetContainer, ContainerListSelection, InventoryListSelection);
        UpdateGameMenuCanvasState(CharPage.Looting);
    }
    void EquipmentSlotClick(int index)
    {
        //if (index >= CharacterMath.EQUIP_SLOTS_COUNT)
        //    return;

        SelectedEquipSlot = index;

        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT && i < EquipButtons.Length; i++)
            EquipButtons[i].image.color = (i == index) ? Selected : Unselected;
    }
    void RingSlotClick(int index)
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
    public void InventoryItemHover(int index, bool hovering = true)
    {
        InventoryListSelection = index;
        string @event = hovering ? "Entered" : "Exited";
        //Debug.Log($"Mouse has {@event} Inv Item {index}");
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
    }
    #endregion

    #region MENUS
    public void PauseMenuRefresh()
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
                SelectedAbilitySlot = -1;

                CharSheet.SetActive(true);
                Equipment.SetActive(true);
                Inventory.SetActive(true);
                break;

            case CharPage.Looting:
                SelectedAbilitySlot = -1;

                Inventory.SetActive(true);
                Container.SetActive(true);
                break;

            case CharPage.Skills:
                SelectedEquipSlot = -1;

                Strategy.SetActive(true);
                Skills.SetActive(true);
                break;
        }

        RefreshGameMenuCanvas();
        UpdateActionBar();
        UpdatePartyPanel();
    }
    void RefreshGameMenuCanvas()
    {
        switch (CurrentPage)
        {
            case CharPage.None:
                SkillListClick(-1);
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
                PopulateEffectPanels();
                break;
        }
    }
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
                GameState.pController.CurrentCharacter.EquipmentSlots[i].Equip.Sprite != null)
            {
                buttonImage.sprite = GameState.pController.CurrentCharacter.EquipmentSlots[i].Equip.Sprite;
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
                GameState.pController.CurrentCharacter.RingSlots[i].Equip.Sprite != null)
            {
                buttonImage.sprite = GameState.pController.CurrentCharacter.RingSlots[i].Equip.Sprite;
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
    void PopulateInventoryButtons(Inventory inventory, ButtonType type)
    {
        // Clear old buttons
        Transform targetContent = (type == ButtonType.CONTAINER) ? ContainerButtonContent : InventoryButtonContent;
        for (int i = targetContent.childCount - 1; i > -1; i--)
            Destroy(targetContent.GetChild(i).gameObject);

        // Populate new buttons
        int index = 0;
        foreach (ItemWrapper item in inventory.Items)
        {
            if (item == null)
                continue;

            GameObject newButtonObject = Instantiate(InventoryButtonPrefab, targetContent);
            newButtonObject.SetActive(true);
            CreateCallBackIdentity(newButtonObject.GetComponent<Button>(), index, type);
            index++;

            newButtonObject.transform.GetChild(0).GetComponent<Text>().text = (item is StackableWrapper) ? ((StackableWrapper)item).CurrentQuantity.ToString() : string.Empty;

            Image newImage = newButtonObject.GetComponent<Image>();
            try { newImage.sprite = item.Sprite; }
            catch { Debug.Log("missing button sprite"); }
        }
    }
    void PopulateSkillListButtons()
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

            GameObject newButtonObject = Instantiate(SkillListButtonPrefab, SkillButtonContent);
            newButtonObject.SetActive(true);
            CreateCallBackIdentity(newButtonObject.GetComponent<Button>(), index, ButtonType.LIST_SKILL);
            index++;

            newButtonObject.transform.GetChild(0).GetComponent<Text>().text = ability.Name;
        }
    }
    void PopulateSkillSlotButtons()
    {
        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            GameObject newButtonObject = Instantiate(SkillSlotButtonPrefab, HotBarButtonContent);
            newButtonObject.SetActive(true);
            CreateCallBackIdentity(newButtonObject.GetComponent<Button>(), i, ButtonType.SLOT_SKILL);
        }
    }
    void PopulateEffectPanels()
    {
        // Clear old effectPanels
        for (int i = EffectStatsContent.childCount - 1; i > -1; i--)
            Destroy(EffectStatsContent.GetChild(i).gameObject);

        if (SkillListSelection < 0 || SkillListSelection >= GameState.pController.CurrentCharacter.Abilities.Count)
            return;

        if (GameState.pController.CurrentCharacter.Abilities[SkillListSelection] == null)
            return;

        foreach (Effect effect in GameState.pController.CurrentCharacter.Abilities[SkillListSelection].Effects)
        {
            GameObject newEffectPanel = Instantiate(EffectPanelPrefab);
            StringBuilder outputBuild = new StringBuilder(GlobalConstants.STR_BUILD_CAP);
            outputBuild.Append($"Effect: {effect.Name}\n");
            outputBuild.Append($"Action: {effect.Action}\n");
            outputBuild.Append($"Duration: {(effect.Duration == EffectDuration.TIMED ? effect.DurationLength.ToString() : effect.Duration.ToString())}\n");

            switch(effect.Action)
            {
                case EffectAction.DMG_HEAL:
                    outputBuild.Append($"Target: {effect.TargetStat}\n");
                    outputBuild.Append($"Amount: {effect.Value}\n");

                    //Debug.Log($"{effect.Name} : {effect.ElementPack.Elements.Length}");

                    for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
                    {
                        //if (effect.ElementPack.Elements[i] == 0)
                        //    continue;

                        outputBuild.Append($"\n{(Element)i} : {effect.ElementPack.Elements[i]}");
                    }
                    break;

                case EffectAction.SPAWN:
                    break;

                case EffectAction.CROWD_CONTROL:
                    outputBuild.Append($"Type: {effect.TargetCCstatus}");
                    break;
            }

            if (effect.Sprite != null)
                newEffectPanel.transform.GetChild(1).GetComponent<Image>().sprite = effect.Sprite;

            newEffectPanel.transform.GetChild(0).GetComponent<Text>().text = outputBuild.ToString();

            newEffectPanel.transform.SetParent(EffectStatsContent);
            newEffectPanel.transform.localScale = new Vector3(1, 1, 1);
            //newEffectPanel.transform.parent = EffectStatsContent;
        }
    }
    void PopulateEquipAndRingSlotButtons()
    {
        for (int i = 0; i < CharacterMath.EQUIP_SLOTS_COUNT && i < EquipButtons.Length; i++)
        {
            if (EquipButtons[i] != null)
            {
                //Debug.Log($"GearCallback: {i}");
                CreateCallBackIdentity(EquipButtons[i], i, ButtonType.SLOT_EQUIP);
            }
        }
            
        for (int i = 0; i < CharacterMath.RING_SLOT_COUNT && i < RingButtons.Length; i++)
        {
            if (RingButtons[i] != null)
            {
                //Debug.Log($"RingCallbacks: {i}");
                CreateCallBackIdentity(RingButtons[i], i, ButtonType.SLOT_RING);
            }
        }
    }
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

        if (character.Sheet.Portrait != null)
            newMemberPanel.transform.GetChild(0).GetComponent<Image>().sprite = character.Sheet.Portrait;
    }
    void UpdateCooldownBars()
    {
        if (HotBarButtonContent == null ||
            HotBarButtonContent.transform.childCount == 0 ||
            GameState.pController.CurrentCharacter == null)
            return;

        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            Slider slider = HotBarButtonContent.GetChild(i).GetChild(0).GetComponent<Slider>();
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

            slider.value = 1 - (GameState.pController.CurrentCharacter.AbilitySlots[i].CD_Timer /
                GameState.pController.CurrentCharacter.AbilitySlots[i].CD_Duration);
        }
    }
    public void UpdateActionBar()
    {
        if (HotBarButtonContent == null)
            return;

        Actionbar.SetActive(GameState.bHUDactive);
        if (!Actionbar.activeSelf)
            return;

        for (int i = 0; i < CharacterMath.ABILITY_SLOTS && i < HotBarButtonContent.transform.childCount; i++)
        {
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

    void UIselectionRefresh()
    {
        SelectedAbilitySlot = -1;
        SelectedEquipSlot = -1;
        InventoryListSelection = -1;
        ContainerListSelection = -1;
        SkillListSelection = -1;
    }
    void UIinitializer()
    {
        PopulateEquipAndRingSlotButtons();
        PopulateSkillSlotButtons();
        UpdateActionBar();

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
            UIselectionRefresh();
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
        UpdateCooldownBars();
        //CheckMap();
    }
}
