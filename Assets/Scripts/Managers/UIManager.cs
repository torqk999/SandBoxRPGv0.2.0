using System;
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
    //Equipment,
    Looting,
    Skills,
    //Strategy
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
    public List<UIGraphic> UIGraphicBin; 

    [Header("===LOGIC===")]
    public GameState GameState;

    [Header("Indexing")]
    public GameMenu CurrentMenu;
    public CharPage CurrentPage;
    public CharPage OldPage;

    [Header("Prefabs")]
    public GameObject InventoryButtonPrefab;
    public GameObject SkillButtonPrefab;
    public GameObject EffectPanelPrefab;
    public GameObject PartyMemberPanelPrefab;

    [Header("Folders")]
    public Transform InventoryButtonContent;
    public Transform ContainerButtonContent;
    public Transform SkillButtonContent;
    public Transform EffectStatsContent;
    public Transform ActionButtons;
    public Transform ActionCooldownBars;
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
    public Canvas HUDcanvas;
    public Canvas GameMenuCanvas;

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
    public int SelectedAbilitySlot;

    public Button[] EquipButtons;
    public Sprite EmptyButtonSprite;

    [Header("Interaction Logic")]
    public Text InteractionHUDsplashText;
    public Text InteractionHUDvalueText;
    public Slider InteractionHUDslider;

    [Header("Debugging")]
    public Text InventoryText;
    public Text ContainerText;
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
                button.onClick.AddListener(() => InventoryItemClick(index));
                break;

            case ButtonType.CONTAINER:
                button.onClick.AddListener(() => ContainerItemClick(index));
                break;

            case ButtonType.SKILL:
                button.onClick.AddListener(() => SkillListClick(index));
                break;

            case ButtonType.KEY_MAP:
                button.onClick.AddListener(() => Remap(index, button));
                break;
        }
    }
    void CreateHoverIdentity(EventTrigger trigger, int index, ButtonType type)
    {
        //Debug.Log("Hoverin");

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

            case ButtonType.SKILL:
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

        Debug.Log("herro prease");

        for (int i = KeyMapContent.childCount - 1; i > -1; i--)
            Destroy(KeyMapContent.GetChild(i).gameObject);

        Debug.Log($"MapSize: {GameState.KeyMap.Map.Length}");

        for (int i = 0; i < GameState.KeyMap.Map.Length; i++)
        {
            GameObject newKeyMap = Instantiate(KeyMapSample, KeyMapContent);

            Text actionLabel = newKeyMap.transform.GetChild(0).GetComponent<Text>();
            Button action0 = newKeyMap.transform.GetChild(1).GetComponent<Button>();
            Button action1 = newKeyMap.transform.GetChild(2).GetComponent<Button>();

            actionLabel.text = GameState.KeyMap.Map[i].Action.ToString();
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
    void ResolveMap()
    {
        if (CurrentRemap == null)
            return;

        if (!GameState.KeyMap.bMapOpen)
            return;

        if (Event.current == null)
            return;

        if (Event.current.isKey &&
            Event.current.keyCode != KeyCode.None)
        {
            GameState.KeyMap.CloseMap(Event.current.keyCode);
            CurrentRemap.transform.GetChild(0).GetComponent<Text>().text = Event.current.keyCode.ToString();
        }
    }

    #endregion

    #region ACTIONS
    public void PauseMenuCanvasNavigation(int index)
    {
        if (index < -1 || index >= PauseMenuCanvas.gameObject.transform.childCount)
        {
            CurrentMenu = GameMenu.NONE;
            GameMenuRefresh();
            return;
        }
        CurrentMenu = (GameMenu)index;
        GameMenuRefresh();
    }
    public void EquipSelection()//bool bLeftHand = true
    {
        if (!GameState.Controller.CurrentCharacter.EquipSelection(SelectedEquipSlot, InventoryListSelection))
            return;
        GameState.Controller.CurrentCharacter.UpdateAbilites();
        UpdateGameMenuCanvasState(CharPage.Character);
        //UpdateSkills();
        UpdateActionBar();
        //UpdateInventory();
        //UpdateEquipment();

    }
    public void AbilitySelection(bool bEquip)
    {
        if (SelectedAbilitySlot < 0)
            return;

        GameState.Controller.CurrentCharacter.AbilitySlots[SelectedAbilitySlot] = (bEquip) ?
            GameState.Controller.CurrentCharacter.Abilities[SkillListSelection] : null;
        UpdateActionBar();
    }
    public void DropSelectedInventoryItem()
    {
        GameState.SceneMan.PushIntoContainer(GameState.Controller.CurrentCharacter, InventoryListSelection);
        Debug.Log("Completed LootBag Action");
    }
    public void LootSelectedContainerItem()
    {
        GameState.Controller.CurrentCharacter.LootContainer(GameState.Controller.targetContainer, ContainerListSelection, InventoryListSelection);

        UpdateGameMenuCanvasState(CharPage.Looting);
    }
    public void EquipmentSlotClick(int index)
    {
        if (index >= CharacterMath.EQUIP_SLOTS)
            return;

        SelectedEquipSlot = index;

        for (int i = 0; i < CharacterMath.EQUIP_SLOTS; i++)
        {
            EquipButtons[i].image.color = (i == index) ? new Color(0, 1, 0) : new Color(1, 1, 1);
        }

        if (SelectedEquipSlot != -1)
        {
            InventoryItemClick(-1);
            ContainerItemClick(-1);
        }
    }
    public void InventoryItemClick(int index)
    {
        InventoryListSelection = index;
        InventoryText.text = (index > -1) ? GameState.Controller.CurrentCharacter.Inventory.Items[index].Name : "NothingSelected";

        //if (index > -1 && GameState.Controller.CurrentCharacter.Inventory.Items[index] is EquipWrapper)
            //InventoryText.text += $"\nAbilityID: {((EquipWrapper)GameState.Controller.CurrentCharacter.Inventory.Items[index]).Equip.AbilityID}";

        for (int i = 0; i < InventoryButtonContent.childCount; i++)
            InventoryButtonContent.GetChild(i).GetComponent<Button>().image.color = (i == index) ? new Color(0, 1, 0) : new Color(1, 1, 1);

        if (InventoryListSelection != -1)
            EquipmentSlotClick(-1);
    }
    public void InventoryItemHover(int index, bool hovering = true)
    {
        InventoryListSelection = index;
        string @event = hovering ? "Entered" : "Exited";
        //Debug.Log($"Mouse has {@event} Inv Item {index}");
    }
    public void ContainerItemClick(int index)
    {
        ContainerListSelection = index;
        ContainerText.text = (index > -1) ? GameState.Controller.targetContainer.Inventory.Items[index].Name : "NothingSelected";

        for (int i = 0; i < ContainerButtonContent.childCount; i++)
            ContainerButtonContent.GetChild(i).GetComponent<Button>().image.color = (i == index) ? new Color(0, 1, 0) : new Color(1, 1, 1);

        if (ContainerListSelection != -1)
            EquipmentSlotClick(-1);
    }
    public void SkillListClick(int index)
    {
        SkillListSelection = index;
        //SkillText.text = (index > -1) ? GameState.Controller.CurrentCharacter.Abilities[index].Name : "NothingSelected";

        for (int i = 0; i < SkillButtonContent.childCount; i++)
            SkillButtonContent.GetChild(i).GetComponent<Button>().image.color = (i == index) ? new Color(0, 1, 0) : new Color(1, 1, 1);


        PopulateEffectPanels();
    }
    public void AbilitySlotClick(int index)
    {
        SelectedAbilitySlot = index;

        UpdateActionBar();
    }

    #endregion

    #region MENUS
    public void GameMenuRefresh()
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
    /*
    public void UpdateContainer()
    {
        if (GameState == null || GameState.Controller.CurrentCharacter == null)
            return;

        GameState.bLootingOpen = GameState.Controller.CurrentCharacter.CurrentInteraction is GenericContainer;

        Container.gameObject.SetActive(GameState.bLootingOpen && GameState.bInventoryOpen);


        if (GameState.bLootingOpen && GameState.bInventoryOpen)
        {
            if (GameState.Controller.CurrentPawn.CurrentInteraction != null && GameState.Controller.CurrentPawn.CurrentInteraction is GenericContainer)
                PopulateInventoryButtons(((GenericContainer)GameState.Controller.CurrentPawn.CurrentInteraction).Inventory, ButtonType.CONTAINER);
        }
        UpdateGameMenuCanvasState();
    }
    public void UpdateInventory()
    {
        if (Inventory == null || GameState == null || GameState.Controller.CurrentCharacter == null)
            return;

        Inventory.gameObject.SetActive(GameState.bInventoryOpen);

        if (GameState.bInventoryOpen)
            PopulateInventoryButtons(GameState.Controller.CurrentCharacter.Inventory, ButtonType.INVENTORY);

        UpdateContainer();
        UpdateGameMenuCanvasState();
    }
    public void UpdateEquipment()
    {
        if (Equipment == null || GameState == null || GameState.Controller.CurrentCharacter == null)
            return;

        Equipment.SetActive(GameState.bEquipmentOpen);

        if (GameState.bEquipmentOpen && EmptyButtonSprite != null)
        {
            for (int i = 0; i < CharacterMath.EQUIP_SLOTS; i++)
            {
                try { EquipButtons[i].GetComponent<Image>().sprite = GameState.Controller.CurrentCharacter.EquipmentSlots[i].Equip.Sprite; }
                catch { EquipButtons[i].GetComponent<Image>().sprite = EmptyButtonSprite; }

            }
        }
        UpdateGameMenuCanvasState();
    }
    public void UpdateSkills()
    {
        if (GameState == null || GameState.Controller.CurrentCharacter == null)
            return;

        if (GameState.bSkillsOpen)
        {
            PopulateSkillButtons();
            PopulateEffectPanels();
        }
        else
        {
            SelectedAbilitySlot = -1;
            UpdateActionBar();
        }
        UpdateGameMenuCanvasState();
    }
    public void UpdateStrategy()
    {
        if (Strategy == null || GameState == null || GameState.Controller.CurrentCharacter == null)
            return;

        Strategy.SetActive(GameState.bStrategyOpen);

        UpdateGameMenuCanvasState();
    }
    */
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

        switch(CurrentPage)
        {
            case CharPage.None:
                GameMenuCanvas.gameObject.SetActive(false);
                break;

            case CharPage.Character:
                // not Skills
                SelectedAbilitySlot = -1;
                UpdateActionBar();
                //
                PopulateInventoryButtons(GameState.Controller.CurrentCharacter.Inventory, ButtonType.INVENTORY);
                PopulateEquipSlots();

                CharSheet.SetActive(true);
                Equipment.SetActive(true);
                Inventory.SetActive(true);
                break;

            case CharPage.Looting:
                // not Skills
                SelectedAbilitySlot = -1;
                UpdateActionBar();
                ///
                PopulateInventoryButtons(GameState.Controller.CurrentCharacter.Inventory, ButtonType.INVENTORY);
                if (GameState.Controller.CurrentPawn.CurrentInteraction != null && GameState.Controller.CurrentPawn.CurrentInteraction is GenericContainer)
                    PopulateInventoryButtons(((GenericContainer)GameState.Controller.CurrentPawn.CurrentInteraction).Inventory, ButtonType.CONTAINER);

                Inventory.SetActive(true);
                Container.SetActive(true);
                break;

            case CharPage.Skills:
                PopulateSkillButtons();
                PopulateEffectPanels();
                Strategy.SetActive(true);
                Skills.SetActive(true);
                break;
        }

        UpdatePartyPanel();
    }
    void PopulateEquipSlots()
    {
        for (int i = 0; i < CharacterMath.EQUIP_SLOTS; i++)
        {
            try { EquipButtons[i].GetComponent<Image>().sprite = GameState.Controller.CurrentCharacter.EquipmentSlots[i].Equip.Sprite; }
            catch { EquipButtons[i].GetComponent<Image>().sprite = EmptyButtonSprite; }

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
            CreateHoverIdentity(newButtonObject.GetComponent<EventTrigger>(), index, type);
            index++;

            newButtonObject.transform.GetChild(0).GetComponent<Text>().text = (item is StackableWrapper) ? ((StackableWrapper)item).CurrentQuantity.ToString() : string.Empty;

            Image newImage = newButtonObject.GetComponent<Image>();
            try { newImage.sprite = item.Sprite; }
            catch { Debug.Log("missing button sprite"); }
        }
    }
    void PopulateSkillButtons()
    {
        // Clear old buttons
        for (int i = SkillButtonContent.childCount - 1; i > -1; i--)
            Destroy(SkillButtonContent.GetChild(i).gameObject);

        // Populate new buttons
        int index = 0;
        foreach (CharacterAbility ability in GameState.Controller.CurrentCharacter.Abilities)
        {
            if (ability == null)
                continue;

            GameObject newButtonObject = Instantiate(SkillButtonPrefab, SkillButtonContent);
            newButtonObject.SetActive(true);
            CreateCallBackIdentity(newButtonObject.GetComponent<Button>(), index, ButtonType.SKILL);
            index++;

            newButtonObject.transform.GetChild(0).GetComponent<Text>().text = ability.Name;
        }
    }
    void PopulateEffectPanels()
    {
        // Clear old effectPanels
        for (int i = EffectStatsContent.childCount - 1; i > -1; i--)
            Destroy(EffectStatsContent.GetChild(i).gameObject);

        if (SkillListSelection < 0 || SkillListSelection >= GameState.Controller.CurrentCharacter.Abilities.Count)
            return;

        if (GameState.Controller.CurrentCharacter.Abilities[SkillListSelection] == null)
            return;

        foreach (Effect effect in GameState.Controller.CurrentCharacter.Abilities[SkillListSelection].Effects)
        {
            GameObject newEffectPanel = Instantiate(EffectPanelPrefab, EffectStatsContent);
            string output = string.Empty;

            output += $"Effect: {effect.Name}\n";
            output += "Duration: ";
            string duration = "Instant\n";
            duration = (effect.Duration < 0) ? "Forever\n" : duration;
            duration = (effect.Duration > 0) ? $"{effect.Duration}\n" : duration;
            output += duration;
            output += "Values:";

            float[] stats = effect.ElementPack.PullData();
            for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
            {
                if (stats[i] == 0)
                    continue;

                output += $"\n{(Element)i}: {stats[i]}";
            }

            newEffectPanel.transform.GetChild(0).GetComponent<Text>().text = output;
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
    public void UpdateInteraction()
    {
        if (GameState.Controller.CurrentCharacter == null)
            return;

        if (GameState.Controller.CurrentCharacter.CurrentInteraction == null)
            UpdateInteractionHUD(false);
        else
            UpdateInteractionHUD(true, GameState.Controller.CurrentCharacter.CurrentInteraction.GetInteractData());
            
    }
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
        float[] current = character.CurrentStats.PullData();
        float[] maximum = character.MaximumStatValues.PullData();

        memberPanel.gameObject.GetComponent<Image>().color =
            (character == GameState.Controller.CurrentCharacter) ?
            Color.green : Color.white;

        for (int i = 0; i < 3; i++)
        {
            Slider slider = memberPanel.GetChild(1).GetChild(i).GetComponent<Slider>();
            Text text = memberPanel.GetChild(2).GetChild(i).GetComponent<Text>();
            slider.value = current[i] / maximum[i];
            text.text = $" {Math.Round(current[i], GlobalConstants.DECIMAL_PLACES)} / {Math.Round(maximum[i],GlobalConstants.DECIMAL_PLACES)}";
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
        if (ActionCooldownBars == null || GameState.Controller.CurrentCharacter == null)
            return;

        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            Slider slider = ActionCooldownBars.GetChild(i).GetComponent<Slider>();

            if (slider == null)
            {
                Debug.Log("Slider component missing");
                return;
            }

            if (GameState.Controller.CurrentCharacter.AbilitySlots[i] == null)
            {
                slider.value = 1;
                continue;
            }

            Debug.Log($"Updating slot:{i}");

            slider.value = 1 - (GameState.Controller.CurrentCharacter.AbilitySlots[i].CD_Timer /
                GameState.Controller.CurrentCharacter.AbilitySlots[i].CD_Duration);
        }
    }
    public void UpdateActionBar()
    {
        if (ActionButtons == null)
            return;

        Actionbar.SetActive(GameState.bHUDactive);
        if (!Actionbar.activeSelf)
            return;

        for (int i = 0; i < CharacterMath.ABILITY_SLOTS; i++)
        {
            int I = (i < 10 && i != 0) ? i - 1 : i;
            I = (i == 0) ? 9 : I;

            Image image = ActionButtons.GetChild(I).gameObject.GetComponent<Button>().GetComponent<Image>();

            image.sprite = (GameState.Controller.CurrentCharacter != null
                && GameState.Controller.CurrentCharacter.AbilitySlots[i] != null
                && GameState.Controller.CurrentCharacter.AbilitySlots[i].Sprite != null) ?
            GameState.Controller.CurrentCharacter.AbilitySlots[i].Sprite : EmptyButtonSprite;

            image.color = (SelectedAbilitySlot == i) ? new Color(0, 1, 0) : new Color(1, 1, 1);
        }
    }
    public void UpdateInteractionHUD(bool state, InteractData data = new InteractData())
    {
        Interaction.SetActive(state);
        //GameState.bLootingOpen = Container.activeSelf;

        if (!Interaction.gameObject.activeSelf)
            return;

        if (InteractionHUDsplashText != null)
            InteractionHUDsplashText.text = data.Splash;

        if (InteractionHUDvalueText != null)
        {
            InteractionHUDvalueText.text = (data.HealthCurrent > 0) ?
                Math.Round(data.HealthCurrent, GlobalConstants.DECIMAL_PLACES) + "/" + Math.Round(data.HealthMax, GlobalConstants.DECIMAL_PLACES)
                : "DEAD X_X";
        }

        if (InteractionHUDslider != null)
        {
            InteractionHUDslider.gameObject.SetActive(data.Type == TriggerType.CHARACTER);
            InteractionHUDslider.value = (data.Type == TriggerType.CHARACTER) ? data.HealthCurrent / data.HealthMax : 0;
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
        CurrentMenu = GameMenu.NONE;
        GameMenuRefresh();
        UIselectionRefresh();

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
        ResolveMap();
    }
    void Start()
    {
        UIinitializer();
        UpdateActionBar();
    }

    // Update is called once per frame
    void Update()
    {
        RepopulateMemberPanels();
        UpdatePartyPanel();
        UpdateInteraction();
        UpdateCooldownBars();
    }
}
