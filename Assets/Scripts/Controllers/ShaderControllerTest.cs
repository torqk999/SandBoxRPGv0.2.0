using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderControllerTest : MonoBehaviour
{
    public Material TargetMaterial;
    public string TargetParameter;
    public Vector4 TargetVector;
    public Shader myShader;
    public Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //myRenderer.materials[];
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetMaterial != null)
        {
           // Debug.Log("Running");
            TargetMaterial.SetVector("Offsets", TargetVector);
        }
    }
}
