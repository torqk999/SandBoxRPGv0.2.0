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

    [Header("Pawn Settings")]
    public float Mouse = 5;
    public float Roll = .5f;

    [Header("Pawn Logic")]
    public Pawn[] PlayerPawns;
    public Pawn CurrentPawn;

    [Header("Camera Logic")]
    public CameraMode CurrentCameraMode = CameraMode.CHASE;
    public bool bIsZooming = true;
    public bool bCameraLookEnabled;
    public float Zoom;
    public Vector2 FixedXY;

    [Header("Lerp Logic")]
    public float LerpTimer = 0;
    public bool bIsCamLerping = false;
    public Quaternion oldRotation;
    public Vector3 oldPosition;

    #endregion

    #region PUBLIC
    public void ToggleCameraMode()
    {
        if (CurrentCameraMode == CameraMode.FIXED)
            CurrentCameraMode = 0;
        else
            CurrentCameraMode++;

        CameraRealign();
    }
    public bool PossessPawn(Pawn targetPawn)
    {
        if (targetPawn == null || !targetPawn.Source.gameObject.activeSelf)
        {
            Debug.Log("PawnMan failed possession");
            return false;
        }

        oldRotation = GameCamera.transform.parent.rotation;
        oldPosition = GameCamera.transform.parent.position;
        CurrentPawn = targetPawn;
        GameState.Controller.CurrentCharacter = targetPawn as Character;
        GameCamera.transform.parent = targetPawn.Socket;
        LerpTimer = 0;
        bIsCamLerping = true;

        return true;
    }
    public bool ReturnCameraRay(out Ray ray)
    {
        ray = new Ray();
        if (GameCamera == null)
            return false;

        ray = GameCamera.ScreenPointToRay(Input.mousePosition);
        return true;
    }
    #endregion

    void UpdateCurrentInteraction()
    {
        if (CurrentPawn == null || CurrentPawn.bTriggerStateChange == false)
            return;

        CurrentPawn.bTriggerStateChange = false;

        InteractData data = new InteractData();
        data.Type = TriggerType.NONE;
        bool state;

        if (CurrentPawn.CurrentInteractions.Count > 0)
        {
            CurrentPawn.CurrentInteraction = CurrentPawn.CurrentInteractions[0];
            data = CurrentPawn.CurrentInteraction.GetInteractData();
            state = true;
        }
        else
        {
            CurrentPawn.CurrentInteraction = null;
            state = false;
        }

        switch (data.Type)
        {
            case TriggerType.NONE:
                //GameState.bContainerOpen = false;
                break;

            case TriggerType.CONTAINER:
                if (!(CurrentPawn.CurrentInteraction is GenericContainer))
                {
                    state = false;
                    break;
                }
                //GameState.bContainerOpen = true;
                //GenericContainer container = (GenericContainer)CurrentPawn.CurrentInteraction;
                //if (container == null || container == GameState.Controller.targetContainer)
                //    break;
                //GameState.bContainerOpen = false;
                //GameState.UIman.UpdateContainer();
                break;

            case TriggerType.CHARACTER:
                //GameState.bContainerOpen = false;
                if (!(CurrentPawn.CurrentInteraction is Character))
                {
                    state = false;
                    break;
                }
                //state = false;
                break;
        }

        GameState.UIman.UpdateContainer();
        GameState.UIman.UpdateInteractionHUD(state, data);

    }
    void UpdateCurrentTarget()
    {
        if (GameState.Controller.CurrentCharacter != null &&
            GameState.Controller.CurrentCharacter.Target != null)
            GameState.UIman.UpdateInteraction();

        //GameState.UIman.UpdateInteractionHUD(true, GameState.Controller.CurrentCharacter.Target.InteractData);

    }

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

        //currentPawn.GameState = GameState;
        currentPawn.DefPos = newPawnObject.transform.position;
        currentPawn.DefRot = newPawnObject.transform.rotation.eulerAngles;

        if (currentPawn is Character)
        {
            newPawnObject.tag = GlobalConstants.TAG_CHARACTER;
            BuildCharacterBlocker(newPawnObject);
        }

        BuildCameraRig(newPawnObject, currentPawn);
        BuildRigidBody(newPawnObject, currentPawn);
        BuildTriggerVolume(newPawnObject, currentPawn);

        currentPawn.CurrentInteractions = new List<Interaction>();

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
            PlayerPawns[i] = PawnGeneration(CandidateFolder.GetChild(i).gameObject, PureFolder);
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

    #region CAMERA
    void LerpCam()
    {
        if (!bIsCamLerping)
            return;
        if (GameCamera.transform.parent == null)
        { bIsCamLerping = false; return; }

        GameCamera.transform.rotation = Quaternion.Lerp(oldRotation, GameCamera.transform.parent.rotation, LerpTimer);
        GameCamera.transform.position = Vector3.Lerp(oldPosition, GameCamera.transform.parent.position, LerpTimer);
        LerpTimer += GlobalConstants.TIME_SCALE;

        if (LerpTimer >= 1)
            bIsCamLerping = false;
    }
    void UpdateZoom()
    {
        if (!bIsZooming || CurrentPawn == null)
            return;

        Zoom += CurrentPawn.ZoomScale * -Input.mouseScrollDelta.y;
        Zoom = (Zoom < CurrentPawn.ZoomMin) ? CurrentPawn.ZoomMin : Zoom;
        Zoom = (Zoom > CurrentPawn.ZoomMax) ? CurrentPawn.ZoomMax : Zoom;

        /*
        Vector3 newPos = CurrentPawn.Socket.localPosition;
        newPos.z = -Zoom;
        CurrentPawn.Socket.localPosition = newPos;
        */
    }
    void UpdateBoom()
    {
        if (CurrentPawn == null)
            return;

        //Vector3 oldPos = CurrentPawn.Socket.position;
        Vector3 newLocal = CurrentPawn.Socket.localPosition;
        RaycastHit hit;

        //Debug.Log($"Raycast Length: {-(CurrentPawn.Boom.forward * Zoom)}");

        if (Physics.Raycast(CurrentPawn.Boom.position, -CurrentPawn.Boom.forward , out hit, Zoom))
        {
            //Debug.Log($"HitName: {hit.collider.name}");
            newLocal.z = -Vector3.Distance(CurrentPawn.Boom.position, hit.point);
        }
        else
            newLocal.z = -Zoom;

        CurrentPawn.Socket.localPosition = newLocal;
        //Vector3 newPos = CurrentPawn.Socket.position;

        //Debug.DrawLine(oldPos, newPos, Color.blue, 5);
    }
    void CameraRealign()
    {
        switch (CurrentCameraMode)
        {
            case CameraMode.CHASE:
                CurrentPawn.Boom.localRotation = Quaternion.Euler(CurrentPawn.DefRot);
                break;

            case CameraMode.FIXED:
                FixedXY = new Vector2(CurrentPawn.DefRot.y, CurrentPawn.DefRot.x);
                break;
        }
    }
    void UpdateInput()
    {
        if (Input.GetButtonDown("CamReset"))
            CameraRealign();

        // Camera Rotate
        if (!Input.GetButton("CameraLook") || !bCameraLookEnabled)
        {
            GameState.Controller.bIsInControl = true;
            return;
        }

        GameState.Controller.bIsInControl = false;

        float x = Mouse * Input.GetAxisRaw("Mouse X");
        float y = Mouse * Input.GetAxisRaw("Mouse Y");
        float z = Input.GetAxis("Roll") * Roll;

        switch(CurrentCameraMode)
        {
            case CameraMode.CHASE:
                CurrentPawn.Boom.Rotate(-y, x, z, Space.World);
                break;

            case CameraMode.FIXED:
                //CurrentPawn.Source.Rotate(-y, 0, 0, Space.Self);
                //CurrentPawn.Boom.Rotate(0, x, 0, Space.World);
                FixedXY.x += x;
                FixedXY.y += y;
                break;
        }
    }

    void FixedBoom()
    {
        if (CurrentCameraMode != CameraMode.FIXED)
            return;

        CurrentPawn.Boom.rotation = Quaternion.Euler(CurrentPawn.Source.rotation.eulerAngles.x + FixedXY.y, CurrentPawn.Source.rotation.eulerAngles.y + FixedXY.x, 0);
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GeneratePurePawns();
        //PawnInitializer();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateZoom();
        UpdateBoom();
        UpdateInput();
        UpdateCurrentInteraction();
        UpdateCurrentTarget();
        LerpCam();
        FixedBoom();
    }
}