using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class MainLoop : MonoBehaviour {

    public ComputeShader noiseCompute;
    public int instanceCount;
    public Vector3 startForce;
    [Range(0.001f, 1)]
    public float size;
    [Range(0, 2f)]
    public float speedScale;
    [Range(0, 5f)]
    public float forceScale;
    [Range(0, 10f)]
    public float emitRadius;
    public Color baseColor;
    [Range(0,10f)]
    public float minlife = 3f;
    [Range(1,15f)]
    public float maxlife = 5f;
    public Material instanceMat;
    public Transform boundSphere;

    private int emitKernel, updateKernel;
    private ComputeBuffer particleBuffer;
    private float timer = 0;
    private uint tx, ty, tz;

    private void Initialize()
    {
        emitKernel = noiseCompute.FindKernel("Emit");
        updateKernel = noiseCompute.FindKernel("Update");

        if(emitKernel < 0 || updateKernel < 0)
        {
            Debug.LogError("kernel not find!");
            return;
        }

        noiseCompute.GetKernelThreadGroupSizes(emitKernel, out tx, out ty, out tz);

        if (particleBuffer != null)
            particleBuffer.Release();

        particleBuffer = new ComputeBuffer(instanceCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Particle)));
        Particle[] particles = new Particle[particleBuffer.count];
        for (int i = 0; i < particles.Length; i++)
        {
            Color color = Color.green;
            
            particles[i] = new Particle(Random.insideUnitSphere * emitRadius, Random.Range(minlife, maxlife), size, baseColor);
        }
        particleBuffer.SetData(particles);
    }

    private void Start()
    {
        Initialize();   
    }

    private void Update()
    {
        noiseCompute.SetVector("_times", new Vector2(Time.deltaTime, timer));
        noiseCompute.SetVector("startForce", startForce);
        noiseCompute.SetFloat("speedScale", speedScale);
        noiseCompute.SetFloat("forceScale", forceScale);

        Vector3 center = boundSphere.position;
        float radius = boundSphere.lossyScale.x / 2f;
        noiseCompute.SetVector("sphereData", new Vector4(center.x, center.y, center.z, radius));
        noiseCompute.SetBuffer(emitKernel, "buf", particleBuffer);
        noiseCompute.Dispatch(emitKernel, Mathf.CeilToInt((float)instanceCount / (float)tx), (int)ty, (int)tz);

        noiseCompute.SetBuffer(updateKernel, "buf", particleBuffer);
        noiseCompute.Dispatch(updateKernel, Mathf.CeilToInt((float)instanceCount / (float)tx), (int)ty, (int)tz);

        timer += Time.deltaTime;
    }

    private void OnRenderObject()
    {
        Matrix4x4 modelMatrix = transform.localToWorldMatrix;
        instanceMat.SetPass(0);
        instanceMat.SetBuffer("buf", particleBuffer);
        instanceMat.SetMatrix("modelMatrix", modelMatrix);
        Graphics.DrawProcedural(MeshTopology.Points, instanceCount);
    }

    private void OnDisable()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
    }
}
