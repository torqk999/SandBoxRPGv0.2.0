using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWorldBuilder : MonoBehaviour
{
    public GameState GameState;
    public Transform SpawnLocations;
    public Transform PartyStartLocation;
    public GameObject MobPrefab;
    public Transform MobFolder;
    public GenericContainer LootBox;

    public List<SimpleAIcontroller> myAIpool = new List<SimpleAIcontroller>();
    public List<Stackable> SampleObjects;
    public List<Wearable> SampleGear;
    public List<Ring> SampleRings;
    public List<OneHand> SampleOneHands;
    public List<OffHand> SampleOffHands;
    public List<TwoHand> SampleTwoHands;

    public void SpawnSampleItems(Inventory inventory)
    {
        if (inventory == null)
        {
            Debug.Log("null inventory!");
            return;
        }

        foreach (Stackable item in SampleObjects)
        {
            if (item == null)
                continue;
            inventory.Items.Add(new StackableWrapper(item));
            LootBox.Inventory.Items.Add(new StackableWrapper(item));
        }

        foreach (Wearable wear in SampleGear)
        {
            if (wear == null)
                continue;

            WearableWrapper newWrapper = new WearableWrapper(wear, true);
            newWrapper.Equip.EquipID = GameState.EQUIPMENT_INDEX;
            GameState.EQUIPMENT_INDEX++;

            inventory.Items.Add(newWrapper);
            LootBox.Inventory.Items.Add(newWrapper);
        }
        foreach (Ring ring in SampleRings)
        {
            if (ring == null)
                continue;

            RingWrapper newWrapper = new RingWrapper(ring, true);
            newWrapper.Equip.EquipID = GameState.EQUIPMENT_INDEX;
            GameState.EQUIPMENT_INDEX++;

            inventory.Items.Add(newWrapper);
            LootBox.Inventory.Items.Add(newWrapper);
        }

        foreach (OneHand oneHand in SampleOneHands)
        {
            if (oneHand == null)
                continue;

            OneHandWrapper newWrapper = new OneHandWrapper(oneHand, true);
            newWrapper.Equip.EquipID = GameState.EQUIPMENT_INDEX;
            GameState.EQUIPMENT_INDEX++;

            inventory.Items.Add(newWrapper);
            LootBox.Inventory.Items.Add(newWrapper);
        }

        foreach (OffHand offHand in SampleOffHands)
        {
            if (offHand == null)
                continue;

            OffHandWrapper newWrapper = new OffHandWrapper(offHand, true);
            newWrapper.Equip.EquipID = GameState.EQUIPMENT_INDEX;
            GameState.EQUIPMENT_INDEX++;

            inventory.Items.Add(newWrapper);
            LootBox.Inventory.Items.Add(newWrapper);
        }

        foreach (TwoHand twoHand in SampleTwoHands)
        {
            if (twoHand == null)
                continue;
            TwoHandWrapper newWrapper = new TwoHandWrapper(twoHand, true);
            newWrapper.Equip.EquipID = GameState.EQUIPMENT_INDEX;
            GameState.EQUIPMENT_INDEX++;

            inventory.Items.Add(newWrapper);
            LootBox.Inventory.Items.Add(newWrapper);
        }
    }
    public void SpawnMobs()
    {
        
        if (MobPrefab == null)
        {
            Debug.Log("Mob Prefab Missing!");
            return;
        }
        if (MobPrefab.GetComponent<Character>() == null)
        {
            Debug.Log("Character Script Missing!");
            return;
        }
        if (MobPrefab.GetComponent<Character>().Sheet == null)
        {
            Debug.Log("CharacterSheet Missing!");
            return;
        }

        GameState.CharacterMan.CreateCloneParty(MobPrefab, SpawnLocations, Faction.BADDIES);
    }
    void BuildTestWorld()
    {
        GameState.NavMesh.GenerateMesh();

        //GameState.testPath.GenerateNewPath(GameState.testPath.START.position, GameState.testPath.END.position, out GameState.testPath.TP);

        GameState.CharacterMan.CreateLiteralParty(GameState.CharacterMan.DefaultPartyPrefabs, Faction.GOODIES,
            GameState.CharacterMan.DefaultPartyFormation, PartyStartLocation); // Migrations -______-
        SpawnMobs();
        
        GameState.pController.InitialPawnControl();

        //GameState.testBuilder.SpawnSampleItems(GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].PartyLoot);//GameState.Controller.CurrentCharacter);
    }

    // Start is called before the first frame update
    void Start()
    {
        BuildTestWorld();
    }
}