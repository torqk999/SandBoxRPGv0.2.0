using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    public Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            myAnimator.SetBool("Walking", true);
        else
            myAnimator.SetBool("Walking", false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnimator.SetTrigger("Swing");
            //myAnimator.ResetTrigger("Swing");
        }
            

    }
}
