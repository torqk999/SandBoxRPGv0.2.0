using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    FLESH,
    HAIR,
    WOOD,
    IRON,
    STEEL,
    GOLD,
    TRIM_AURA,
    LEATHER
}
public enum PsystemType
{
    BALL_FIRE,
    CONE_FIRE,
    CONE_FROST,
    BEAM_SOLO
}
public class SceneManager : MonoBehaviour
{
    public GameState GameState;
    public GameObject LootBagPrefab;
    public GameObject LootTriggerVolumePrefab;
    public Transform LootBagFolder;

    public List<GameObject> OneHandPrefabs;
    public List<GameObject> TwoHandPrefabs;
    public List<GameObject> OffHandPrefabs;
    public List<GameObject> ShieldPrefabs;

    public List<GameObject> PsystemPrefabs;

    public List<Material> Materials;
    public Material[] BUFFER;

    #region LOOT
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
    #endregion

    #region INSTANTIATIONS & DESTUCTIONS
    public bool InstantiateHandEquip(Hand hand, CharacterRender render, bool putOn = true)
    {
        if (hand == null ||
            render == null)
            return false;

        GameObject newHand = null;

        try
        {
            if (putOn)
            {
                switch (hand)
                {
                    case OneHand:
                        newHand = Instantiate(OneHandPrefabs[(int)((OneHand)hand).Type], render.MainHandSlot);
                        break;

                    case TwoHand:
                        newHand = Instantiate(TwoHandPrefabs[(int)((TwoHand)hand).Type], render.MainHandSlot);
                        break;

                    case OffHand:
                        newHand = Instantiate(OffHandPrefabs[(int)((OffHand)hand).Type], render.OffHandSlot);
                        break;

                    case Shield:
                        newHand = Instantiate(ShieldPrefabs[(int)((Shield)hand).Type], render.ShieldSlot);
                        break;

                    default:
                        Debug.Log("Wasn't a hand equip...");
                        return false;
                }
                hand.Instantiation = newHand;
            }
            else
                Destroy(hand.Instantiation);
        }
        catch
        {
            Debug.Log("Prefab missing! Could not generate!");
            return false;
        }

        try
        {
            if (newHand == null)
            {
                Debug.Log("Instantiated hand equip successfully removed!");
                return true;
            }
                

            MeshRenderer newHandMesh = newHand.GetComponent<MeshRenderer>();
            if (newHandMesh == null)
            {
                Debug.Log("Mesh Renderer componenet not found!");
            }


            newHand.name = "oh rally?";

            newHand.transform.localPosition = new Vector3(0, 0, 0);
            newHand.transform.localRotation = Quaternion.Euler(0, 0, 0);
            newHand.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            BUFFER = newHandMesh.materials;
            for (int i = 0; i < BUFFER.Length; i++)
            {
                Debug.Log($"materialName: {BUFFER[i].name}");
                if (BUFFER[i].name.Contains("Handle"))
                    BUFFER[i] = Materials[(int)hand.HandleMaterial];
                if (BUFFER[i].name.Contains("Base") ||
                    BUFFER[i].name.Contains("Metal"))
                    BUFFER[i] = Materials[(int)hand.BaseMaterial];
            }

            //BUFFER[0] = Materials[(int)hand.BaseMaterial];
            //BUFFER[1] = Materials[(int)hand.HandleMaterial];
            newHandMesh.materials = BUFFER;
        }
        catch
        {
            Debug.Log("Material source and/or target missing! Could not modify!");
            return false;
        }

        return true;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
