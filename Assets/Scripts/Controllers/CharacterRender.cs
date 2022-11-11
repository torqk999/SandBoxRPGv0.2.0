using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public enum GearSlot
{
    NECK,
    CHEST,
    BELT,
    LEGS,
    BOOTS,
    ARMS,
    PAULDRONS,
    GLOVES
}

public enum GearType
{
    NAKED,
    CLOTH,
    LEATHER,
    RING_MAIL,
    PLATE
}

/*public enum HandSlot
{
    MAIN,
    OFF,
    SHIELD
}*/

public enum HandPosition
{
    AXE,
    SWORD,
    WAND,
    BOW,
    SPEAR,
    STAFF,
    DAGGER,
    SHIELD
}



[Serializable]
public class WardrobeProfile
{
    public string Name;
    public int ID;
}
[Serializable]
public class GearProfile : WardrobeProfile
{
    public GearSlot Slot;
    public GearType GearType;
    public MaterialType MatType;
    public SkinnedMeshRenderer thisMesh;
}
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
    Chop,
    Slice,
    Poke,
    Self
}
/*public enum AnimationTarget
{
    ElementalBall,
    ElementalArrow
}*/

public class CharacterRender : MonoBehaviour
{
    //public CharacterManager CharManager;
    public Character MyCharacter;
    public Animator MyAnimator;

    public Transform Base;
    public Transform MainHandSlot;
    public Transform OffHandSlot;
    public Transform ShieldSlot;

    public bool bRunning;
    public int AnimationLayer;
    public float AniCombatTimer;

    public HandPosition CurrentWepPose;

    StringBuilder MeshTarget = new StringBuilder();

    public void CombatTimer()
    {
        AniCombatTimer -= GlobalConstants.TIME_SCALE;
    }
    public void walkingAnimation()
    {
        int WalkingLayer = MyAnimator.GetLayerIndex("MovementLayer");

        if (MyAnimator.GetFloat("horizontalMove") + MyAnimator.GetFloat("verticalMove") != 0f)
        {
            //Debug.Log("LayerON");
            MyAnimator.SetLayerWeight(WalkingLayer, 1);
        }

        /*if (MyAnimator.GetFloat("horizontalMove") + MyAnimator.GetFloat("verticalMove") == 0)
        {
            //Debug.Log("LayerOFF");
            MyAnimator.SetLayerWeight(WalkingLayer, 0);
        }*/
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

            if (MyAnimator.GetLayerWeight(CombatLayer) == 0)
            {
                MyAnimator.ResetTrigger("inCombat");
            }

            return;
        }
    }
    public void UpdateGearSlot(Equipment equip, bool putOn = true)
    {
        if (!(equip is Wearable))
            return;

        MeshTarget.Clear();
        //MeshTarget.Append($"{wear.EquipSkill}.{wear.Type}");
    }
    /*public void UpdateHandSlot(Hand hand, bool putOn)
    {
        if (hand == null)
            return;

        switch(hand)
        {
            case OneHand:
                //if (!putOn)

                break;

            case OffHand:
                break;

            case Shield:
                break;

            case TwoHand:
                break;
        }
        //if (hand == null)
        //{
        //    MyAnimator.SetInteger("")
        //}
        //MyAnimator
    }*/

    // Start is called before the first frame update
    void Start()
    {
        //int WalkingLayer = MyAnimator.GetLayerIndex("MovementLayer");
       // MyAnimator.SetLayerWeight(WalkingLayer, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!bRunning)
            return;
       
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
}
