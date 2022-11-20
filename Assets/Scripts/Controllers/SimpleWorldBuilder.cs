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

    public void SpawnSampleItems(SlotPage inventory)
    {
        if (inventory == null)
        {
            Debug.Log("null inventory!");
            return;
        }

        for(int i = 0; i < SampleItems.Count && i < inventory.Occupants.Places.Length; i++)
        {
            if (SampleItems[i] == null)
                continue;

            SampleItems[i].InitializeRoot(GameState);

            inventory.GenerateItem(SampleItems[i], i);
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