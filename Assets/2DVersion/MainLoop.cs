using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class MainLoop : MonoBehaviour {

    public ComputeShader noiseCompute;
    public int instanceCount;
    public Vector2 gridCount;


    private int emitKernel, updateKernel;
    private ComputeBuffer particleBuffer;

    private void Initialize()
    {
        emitKernel = noiseCompute.FindKernel("Emit");
        updateKernel = noiseCompute.FindKernel("Update");

        if(emitKernel < 0 || updateKernel < 0)
        {
            Debug.LogError("kernel not find!");
            return;
        }


    }

    private void Start()
    {
            
    }

}
