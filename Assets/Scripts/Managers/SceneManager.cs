using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameState GameState;
    public GameObject LootBagPrefab;
    public GameObject LootTriggerVolumePrefab;
    public Transform LootBagFolder;

    public List<GameObject> OneHandPrefabs;
    public List<GameObject> TwoHandPrefabs;
    public List<GameObject> OffHandPrefabs;

    public int CurrentLootBagIndex;

    public bool TakeFromContainer(Character character, GenericContainer lootBag, int index)
    {
        // Meeeeeeeeeeeeeeeer -_________-
        return false;
    }
    public bool PushIntoContainer(Character character, int inventoryIndex)
    {
        // 1. Check for container
        // 2. Check for lootBag
        // 3. Create lootbag
        if (GameState.UIman.CurrentPage == CharPage.Looting && GameState.pController.targetContainer != null &&
            GameState.pController.CurrentCharacter.Inventory.TransferItem(GameState.pController.targetContainer.Inventory, inventoryIndex))
            return true;

        int index = CheckForLootBagInteractionIndex(character);
        if (index > -1)
        {
            Debug.Log("old bag");
            Inventory oldLootBag = ((GenericContainer)character.CurrentProximityInteractions[index]).Inventory;
            if (GameState.pController.CurrentCharacter.Inventory.TransferItem(oldLootBag, inventoryIndex))
                return true;
        }

        Debug.Log("new bag");
        Inventory newLootBag = CreateLootBag(character);

        return false;
    }
    int CheckForLootBagInteractionIndex(Character character)
    {
        return character.CurrentProximityInteractions.FindIndex(x => x.GetInteractData().Type == TriggerType.LOOTBAG);
    }
    Inventory CreateLootBag(Character character)
    {
        if (LootBagPrefab == null || LootBagPrefab.GetComponent<GenericContainer>() == null)
            return null;

        GameObject newLootBag = Instantiate(LootBagPrefab, character.Root.position, character.Root.rotation, LootBagFolder);
        newLootBag.name = newLootBag.name.Replace("(Clone)", ":" + CurrentLootBagIndex);

        GameObject newTriggerVolume = Instantiate(LootTriggerVolumePrefab,
            newLootBag.transform.position,
            newLootBag.transform.rotation,
            newLootBag.transform);
        newTriggerVolume.SetActive(true);
        newTriggerVolume.name = "TRIGGER VOLUME:" + newLootBag.name;

        newLootBag.SetActive(true);
        LootTriggerVolume trigger = newTriggerVolume.GetComponent<LootTriggerVolume>();
        GenericContainer container = newLootBag.GetComponent<GenericContainer>();
        container.GameState = GameState;
        container.TriggerTag = GlobalConstants.TAG_CHARACTER;
        trigger.parent = container;
        return container.Inventory;
    }
    public void RemoveFromLootBag(int index)
    {

    }
    public bool InstantiateHandEquip(EquipWrapper equip, CharacterRender render)
    {
        GameObject newEquipObject = null;

        if (equip == null ||
            render == null)
            return false;

        try
        {
            switch (equip)
            {
                case OneHandWrapper:
                    newEquipObject = Instantiate(OneHandPrefabs[(int)((OneHand)equip.Equip).Type], render.MainHandSlot);
                    break;

                case TwoHandWrapper:
                    newEquipObject = Instantiate(TwoHandPrefabs[(int)((TwoHand)equip.Equip).Type], render.MainHandSlot);
                    break;

                case OffHandWrapper:
                    newEquipObject = Instantiate(OffHandPrefabs[(int)((OffHand)equip.Equip).Type], render.OffHandSlot);
                    break;

                default:
                    Debug.Log("Wasn't a hand equip...");
                    return false;
            }
        }
        catch
        {
            Debug.Log("Prefab missing! Could not generate!");
            return false;
        }

        if (newEquipObject != null)
        {
            equip.Instantiation = newEquipObject;
            newEquipObject.transform.localPosition = new Vector3(0, 0, 0);
            newEquipObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            newEquipObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
        
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
