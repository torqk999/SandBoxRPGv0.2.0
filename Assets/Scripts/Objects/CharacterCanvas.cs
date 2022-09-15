using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvas : MonoBehaviour
{
    public Camera Cam;
    public Character Character;

    public Vector3 Offset;
    public Text LevelText;
    public Slider HealthBar;
    public Transform EffectContainer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Cam != null)
            transform.rotation = Cam.transform.rotation;
    }
}
