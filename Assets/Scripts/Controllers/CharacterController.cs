using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("===CHAR CONTROL===")]
    public GameState GameState;
    public Transform Focus;
    public Pawn CurrentPawn;
    public Character CurrentCharacter;
    public Character TargetCharacter;
    public float IntentTimer;
    public float IntentDuration;
    //public bool IntentLerp;

    public Vector3 IntentVector;
    public Vector3 IntentRotations;

    //public Vector3 OldIntentVector;
    //public Vector3 OldRotationVector;

    public Vector3 CurrentIntent;
    public Vector3 CurrentRotation;

    public void UpdateCharacterAnimationState()
    {
        //Debug.Log($"IntentVector: {IntentVector}");

        if (CurrentCharacter == null)
            return;

        CurrentIntent = ((5 * CurrentIntent) + IntentVector) / 6;
        CurrentRotation = ((5 * CurrentRotation) + IntentRotations) / 6;

        float trueForward = Vector3.Dot(IntentVector, CurrentCharacter.RigidBody.velocity) / (IntentVector.magnitude * CurrentCharacter.RigidBody.velocity.magnitude);

        //CurrentCharacter.UpdateAnimationIntents(trueForward, IntentVector.x);
        CurrentCharacter.UpdateAnimationIntents(CurrentIntent.z, CurrentIntent.x);
    }

    void LerpIntent()
    {

    }



    // Start is called before the first frame update
    void Start()
    {
        IntentDuration = GlobalConstants.TIME_BLIP;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCharacterAnimationState();
    }
}
