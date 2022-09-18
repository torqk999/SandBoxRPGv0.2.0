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
    public Transform Socket;
    public Transform Boom;
    public Transform Root;
    public Rigidbody RigidBody;

    [Header("Pawn Logic")]
    public Vector3 DefPos; // Used for homeTeleport
    public Vector3 DefRot;
    public Vector3 CurrentVelocity;

    public bool bHasTriggerVolume;
    public bool bUsesGravity;
    public bool bIsGrounded;
    public bool bControllable;

    /*
    public Interaction CurrentTargetInteraction;
    public List<Interaction> CurrentProximityInteractions;
    public int InteractionCount;

    public void RemoveInteraction(Interaction interact)
    {
        int index = CurrentProximityInteractions.FindIndex(x => x == interact);
        if (index == -1)
            return;

        ResolveCurrentTargetInteraction(index);
        CurrentProximityInteractions.Remove(interact);
        
    }
    void ResolveCurrentTargetInteraction(int index)
    {
        if(index < 0 || index >= CurrentProximityInteractions.Count || CurrentProximityInteractions.Count <= 1)
        {
            CurrentTargetInteraction = null;
            return;
        }

        index++;
        index = index >= CurrentProximityInteractions.Count ? 0 : index;

        CurrentTargetInteraction = CurrentProximityInteractions[index];
    }
    */

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
        //UpdateCurrentInteraction();
    }
}
