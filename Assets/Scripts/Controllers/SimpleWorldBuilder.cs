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

        for(int i = 0; i < SampleItems.Count && i < inventory.OccupantRoots.Count; i++)
        {
            if (SampleItems[i] == null)
                continue;

            Debug.Log("Spawning Item...");

            SampleItems[i].InitializeRoot(GameState);
            RootOptions options = new RootOptions(GameState, SampleItems[i], ref GameState.ROOT_SO_INDEX, inventory.OccupantRoots, i);
            inventory.GenerateRootIntoSlot(options);

            Debug.Log("Item spawned!");
        }
    }
    
    public bool BuildTestWorld()
    {
        try
        {
            GameState.NavMesh.GenerateMesh();
            Debug.Log("Nav Mesh Generated!");
            GameState.CharacterMan.SpawnPeeps(PartyStartLocation, SpawnLocations);
            Debug.Log("Peeps Spawned!");
            GameState.pController.InitialPawnControl();
            Debug.Log("Initial Pawn Control Complete!");
            SpawnSampleItems(GameState.UIman.Inventories);
            Debug.Log("Sample Items Spawned!");
            return true;
        }
        catch
        {
            Debug.Log("Test world build incomplete!");
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}