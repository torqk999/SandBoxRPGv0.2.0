using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWorldBuilder : MonoBehaviour
{
    public GameState GameState;
    public Transform SpawnLocations;
    public Transform PartyStartLocation;
    //public GameObject MobPrefab;
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

            item.InitializeSource();

            inventory.Items.Add(item.GenerateItem(GameState.EQUIPMENT_INDEX, true));

            if (item is Equipment)
                GameState.EQUIPMENT_INDEX++;
        }
    }
    
    void BuildTestWorld()
    {
        GameState.NavMesh.GenerateMesh();

        GameState.CharacterMan.SpawnPeeps(PartyStartLocation, SpawnLocations);

        GameState.pController.InitialPawnControl();

        SpawnSampleItems(GameState.CharacterMan.Parties[GameState.CharacterMan.CurrentPartyIndex].PartyLoot);
    }

    // Start is called before the first frame update
    void Start()
    {
        BuildTestWorld();
    }
}