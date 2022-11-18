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

        for(int i = 0; i < SampleItems.Count && i < inventory.Items.Length; i++)
        {
            if (SampleItems[i] == null)
                continue;

            SampleItems[i].InitializeRoot(GameState);

            RootOptions options = new RootOptions(ref GameState.ROOT_SO_INDEX);
            inventory.Items[i] = (ItemObject)SampleItems[i].GenerateRootObject(options);
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