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

    [Header("Input Settings")]
    public float KeyAxisScale;
    public float MouseAxisScale;
    public float RollScale;
    public float JumpForce;
    public bool bHasTriggerVolume;

    [Header ("Pawn Components")]
    public Transform Socket;
    public Transform Boom;
    public Transform Source;
    //public Transform Container; // maybe needed in the future?
    public Rigidbody RigidBody;

    [Header("Pawn Logic")]
    public Vector3 DefPos; // Used for homeTeleport
    public Vector3 DefRot;
    public Vector3 CurrentVelocity;
    public Interaction CurrentInteraction;
    public List<Interaction> CurrentInteractions;
    

    public bool bTriggerStateChange = false;
    public bool bUsesGravity;
    public bool bIsGrounded;
    public bool bControllable;

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
    }
}
