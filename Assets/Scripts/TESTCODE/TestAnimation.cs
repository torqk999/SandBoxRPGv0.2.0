using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;


public class TestAnimation : Animator
{
    public Animator MyAnimator;
    public GameObject myObject;
    public Component myManager;
    public int MyAnimationState;
    public int AnimationLayer;

    public float AniCombatTimer;
    

    public enum CharAnimationState
    {

        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Attacking,
        Casting









    }
    public enum CharAnimation

    {
        OneHandChop,
        OneHandSlice,
        OneHandPoke,
        TwoHandSwordChop,
        TwoHandSwordSlice,
        TwoHandSwordPoke,
        HAxeChop,
        HAxeSlice,
        HAxePoke,
        BowShot,
        XBowShot,
        ShieldBash,
        ShieldBlock



    }
    public enum AnimationTarget
    {
        ElementalBall,
        ElementalArrow

    }    



    // Start is called before the first frame update
    void Start()
    {
        
        
        
    
    }

    // Update is called once per frame
    void Update()
    {
        
       
        

        if (Input.GetKey(KeyCode.W))
            MyAnimator.SetBool("Walking", true);
        else
            MyAnimator.SetBool("Walking", false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyAnimator.SetTrigger("Swing");
            //myAnimator.ResetTrigger("Swing");
        }
        walkingAnimation();

    }
    public void CombatTimer()

    {

        AniCombatTimer -= GlobalConstants.TIME_SCALE;

    }

    public void walkingAnimation()

    {
        int WalkingLayer = MyAnimator.GetLayerIndex("MovementLayer");
        

        if (MyAnimator.GetFloat("horizontalMove") + MyAnimator.GetFloat("verticalMove") != 0f)
        {

            MyAnimator.SetLayerWeight(WalkingLayer, 1);
        }
        if (MyAnimator.GetFloat("horizontalMove") + MyAnimator.GetFloat("verticalMove") == 0)
        {

            MyAnimator.SetLayerWeight(WalkingLayer, 0);
        }




        

    }    
    public void AnimationAttempt(CharacterAbility AttemptedAbility = null)

    {
        if (AttemptedAbility == null)

        {

            return;
        }
        int CombatLayer = MyAnimator.GetLayerIndex("CombatLayer");

        if (MyAnimator.GetLayerWeight(CombatLayer) < 0)

        {
            MyAnimator.SetTrigger("inCombat");

           if (MyAnimator.GetBool("Attacking") == false)
            {

                Quaternion CurrentQuat = MyAnimator.rootRotation;
                MyAnimator.SetBoneLocalRotation(HumanBodyBones.UpperChest, CurrentQuat);
            }
           

            if (MyAnimator.GetBool("Attacking") == true)

            {
                


               // AttemptedAbility.
                
                 //MyAnimator.SetBoneLocalRotation(HumanBodyBones.UpperChest,tempQuat);
                
                
            }

            
            
             if(MyAnimator.GetLayerWeight(CombatLayer) == 0)

            {
                MyAnimator.ResetTrigger("inCombat"); 

            }


            return;
        }

    }
}
