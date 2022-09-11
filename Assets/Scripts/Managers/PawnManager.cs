using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    CHASE,
    FIXED
}

public class PawnManager : MonoBehaviour
{
    #region VARS
    public GameState GameState;

    [Header("Object References")]
    public Camera GameCamera;
    public Transform CandidateFolder;
    public Transform PureFolder;

    [Header("Prefabs")]
    public GameObject TriggerVolumePrefab;
    public GameObject CharacterBlockerPrefab;
    public GameObject CharacterCanvasPrefab;

    [Header("Pawn Logic")]
    public Pawn[] PlayerPawns;
    #endregion

    #region PAWNGENERATION
    public Pawn PawnGeneration(GameObject prefab, Transform targetFolder, Transform spawnTransform = null)
    {
        Pawn samplePawn = prefab.GetComponent<Pawn>();
        if (samplePawn == null)
            return null;

        GameObject newPawnObject;
        if (spawnTransform == null)
            newPawnObject = PawnObjectInstantiation(prefab, prefab.transform);
        else
            newPawnObject = PawnObjectInstantiation(prefab, spawnTransform);

        Pawn currentPawn = newPawnObject.GetComponent<Pawn>();
        newPawnObject.transform.parent = targetFolder;

        currentPawn.GameState = GameState;
        currentPawn.DefPos = newPawnObject.transform.position;
        currentPawn.DefRot = newPawnObject.transform.rotation.eulerAngles;

        if (currentPawn is Character)
        {
            newPawnObject.tag = GlobalConstants.TAG_CHARACTER;
            //BuildCharacterBlocker(newPawnObject);
        }

        BuildCameraRig(newPawnObject, currentPawn);
        BuildRigidBody(newPawnObject, currentPawn);
        BuildTriggerVolume(newPawnObject, currentPawn);

        //currentPawn.CurrentProximityInteractions = new List<Interaction>();

        prefab.SetActive(false); // disable template

        return currentPawn;
    }
    GameObject PawnObjectInstantiation(GameObject template, Transform spawnTransform)
    {
        GameObject clone = Instantiate(template, spawnTransform.position, spawnTransform.rotation);
        clone.name = clone.name.Replace("(Clone)", "");
        clone.SetActive(true);
        return clone;
    }
    void BuildCameraRig(GameObject pawnObject, Pawn currentPawn)
    {
        GameObject empty = new GameObject();
        currentPawn.Source = pawnObject.transform;

        // Boom
        GameObject boom = Instantiate(empty, pawnObject.transform.position, Quaternion.Euler(0, 0, 0), pawnObject.transform);
        boom.transform.localRotation = Quaternion.Euler(currentPawn.DefRot);
        boom.name = "CAM:BOOM:" + pawnObject.name;
        currentPawn.Boom = boom.transform;

        // Socket
        GameObject socket = Instantiate(empty, boom.transform.position, Quaternion.Euler(0, 0, 0), boom.transform);
        socket.transform.localRotation = Quaternion.Euler(Vector3.zero);
        socket.name = "CAM:SOCKET:" + pawnObject.name;
        currentPawn.Socket = socket.transform;

        Destroy(empty);
    }
    void BuildRigidBody(GameObject pawnObject, Pawn currentPawn)
    {
        Rigidbody rigidBody = pawnObject.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            currentPawn.RigidBody = rigidBody;
            currentPawn.bUsesGravity = rigidBody.useGravity;
            if (GameState != null)
                GameState.RigidBodyPawns.Add(currentPawn);
            //State.grav.Affected.Add(pawnObject);
        }
    }
    void BuildTriggerVolume(GameObject pawnObject, Pawn currentPawn)
    {
        if (currentPawn.bHasTriggerVolume && currentPawn is Character)
        {
            GameObject newTriggerVolume = Instantiate(TriggerVolumePrefab,
                pawnObject.transform.position,
                pawnObject.transform.rotation,
                pawnObject.transform);
            newTriggerVolume.SetActive(true);
            newTriggerVolume.name = "TRIGGER VOLUME:" + pawnObject.name;
            PawnTriggerVolume newTriggerScript = newTriggerVolume.GetComponent<PawnTriggerVolume>();
            newTriggerScript.Parent = (Character)currentPawn;
        }
    }
    void BuildCharacterBlocker(GameObject pawnObject)
    {
        if (CharacterBlockerPrefab == null)
            return;

        GameObject newBlocker = Instantiate(CharacterBlockerPrefab, pawnObject.transform.position, pawnObject.transform.rotation, pawnObject.transform);
        newBlocker.name = $"BLOCKER:{pawnObject.name}";
        newBlocker.SetActive(true);
        Physics.IgnoreCollision(newBlocker.GetComponent<Collider>(), pawnObject.GetComponent<Collider>(), true);
    }
    void GeneratePurePawns()
    {
        if (CandidateFolder == null || CandidateFolder.transform.childCount < 1)
        {
            PlayerPawns = new Pawn[0];
            return;
        }

        PlayerPawns = new Pawn[CandidateFolder.transform.childCount];

        for (int i = CandidateFolder.childCount - 1; i > -1; i--)
        {
            PlayerPawns[i] = PawnGeneration(CandidateFolder.GetChild(i).gameObject, PureFolder);
        }
    }
    #endregion

    #region EXPERIMENTAL

    void AutoEquipAbilities(Character character)
    {
        int index = 0;
        foreach(CharacterAbility ability in character.Abilities)
        {
            character.AbilitySlots[index] = ability;
            index++;
            if (index >= CharacterMath.ABILITY_SLOTS)
                return;
        }
    }
    GameObject AttatchCharacterBillboard(Transform containerTransform)
    {
        GameObject output = null;

        if (CharacterCanvasPrefab == null || containerTransform == null)
            return output;

        output = Instantiate(CharacterCanvasPrefab, containerTransform);

        return output;
    }

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        GeneratePurePawns();
    }

    // Update is called once per frame
    void Update()
    {

    }
}