using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class Pawn : MonoBehaviour
{
    [Header("User Settings")]
    [Header("==== PAWN CLASS ====")]

    public ControlMode ControlMode;

    [Header ("Zoom Settings")]
    public float ZoomMax;
    public float ZoomMin;
    public float ZoomScale;
    public float ZoomCurrent;

    [Header("Pawn Components")]
    public GameState GameState;
    public CharacterController CurrentController;
    public Transform Socket;
    public Transform Boom;
    public Transform Source;
    public Rigidbody RigidBody;

    [Header("Pawn Logic")]
    public Vector3 DefPos; // Used for homeTeleport
    public Vector3 DefRot;
    public Vector3 CurrentVelocity;

    public Interaction CurrentInteraction;
    public List<Interaction> CurrentInteractions;
    public int InteractionCount;

    public bool bHasTriggerVolume;
    public bool bTriggerStateChange = false;
    public bool bUsesGravity;
    public bool bIsGrounded;
    public bool bControllable;

    void UpdateCurrentInteraction()
    {
        if (bTriggerStateChange == false)
            return;
        Debug.Log("UpdatingInteraction");

        bTriggerStateChange = false;

        InteractData data = new InteractData();
        data.Type = TriggerType.NONE;
        bool state;

        InteractionCount = CurrentInteractions.Count;

        if (CurrentInteractions.Count > 0)
        {
            CurrentInteraction = CurrentInteractions[0];
            data = CurrentInteraction.GetInteractData();
            state = true;
        }
        else
        {
            CurrentInteraction = null;
            state = false;
        }

        switch (data.Type)
        {
            case TriggerType.NONE:
                break;

            case TriggerType.CONTAINER:
                if (!(CurrentInteraction is GenericContainer))
                {
                    state = false;
                    break;
                }
                break;

            case TriggerType.CHARACTER:
                if (!(CurrentInteraction is Character))
                {
                    state = false;
                    break;
                }
                break;
        }

        if (this == GameState.Controller.CurrentPawn)
        {
            GameState.UIman.UpdateContainer();
            GameState.UIman.UpdateInteractionHUD(state, data);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag != string.Empty && collision.transform.CompareTag(GlobalConstants.TAG_GROUND))
            bIsGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        bIsGrounded = false;
    }
    void Start()
    {
    }
    private void Update()
    {
        UpdateCurrentInteraction();
    }
}
