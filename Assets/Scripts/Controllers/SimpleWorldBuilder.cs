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
    public Wardrobe CloneWardrobe;

    public List<SimpleAIcontroller> myAIpool;
    public List<ItemObject> SampleItems;

    public void SpawnSampleItems(Inventory inventory)
    {
        if (inventory == null)
        {
            Debug.Log("null inventory!");
            return;
        }

        foreach (ItemObject item in SampleItems)
        {
            if (item == null)
                continue;

            inventory.Items.Add(item.GenerateItem(GameState.EQUIPMENT_INDEX, true));

            if (item is Equipment)
                GameState.EQUIPMENT_INDEX++;
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

        GameState.CharacterMan.CreateCloneParty(MobPrefab, SpawnLocations, Faction.BADDIES, CloneWardrobe);
    }
    void BuildTestWorld()
    {
        GameState.NavMesh.GenerateMesh();

        //GameState.testPath.GenerateNewPath(GameState.testPath.START.position, GameState.testPath.END.position, out GameState.testPath.TP);

        GameState.CharacterMan.CreateLiteralParty(GameState.CharacterMan.DefaultPartyPrefabs, Faction.GOODIES,
            GameState.CharacterMan.DefaultPartyFormation, PartyStartLocation); // Migrations -______-
        SpawnMobs();
        GameState.CharacterMan.UpdatePartyFoes();
        GameState.pController.InitialPawnControl();
        SpawnSampleItems(GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].PartyLoot);
    }

    // Start is called before the first frame update
    void Start()
    {
        BuildTestWorld();
    }
}