using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapTesting : MonoBehaviour
{
    public Tactic[] sloots;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Tactic tac in sloots)
        {
            switch(tac)
            {
                case FloatTactic:
                    Debug.Log("float");
                    break;

                case CCstateTactic:
                    Debug.Log("cc");
                    break;

                case FixedTactic:
                    Debug.Log("fix");
                    break;

                case ProxyTactic:
                    Debug.Log("proxy");
                    break;

                default:
                    Debug.Log("hate chu");
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
