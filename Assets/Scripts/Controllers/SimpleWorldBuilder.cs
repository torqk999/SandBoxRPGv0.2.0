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

    public void SpawnSampleItems(Page inventory)
    {
        if (inventory == null)
        {
            Debug.Log("null inventory!");
            return;
        }

        for(int i = 0; i < SampleItems.Count && i < inventory.OccupantRoots.List.Count; i++)
        {
            if (SampleItems[i] == null)
                continue;

            SampleItems[i].InitializeRoot(GameState);
            RootOptions options = new RootOptions(SampleItems[i], inventory, i);
            inventory.GenerateRootIntoSlot(options);
        }
    }
    
    public bool BuildTestWorld()
    {
        try
        {
            GameState.NavMesh.GenerateMesh();

            GameState.CharacterMan.SpawnPeeps(PartyStartLocation, SpawnLocations);

            GameState.pController.InitialPawnControl();

            SpawnSampleItems(GameState.UIman.Inventories);

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}