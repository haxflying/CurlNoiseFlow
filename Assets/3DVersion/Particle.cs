using UnityEngine;
using System.Collections;

[SerializeField]
public struct Particle 
{
    public Vector3 emitPos;
    public Vector3 pos;
    public Vector4 velocity;
    public Vector3 life;
    public Vector3 size;
    public Vector4 color;


    public Particle(Vector3 emitPos, float life, float size, Color color)
    {
        this.emitPos = emitPos;
        this.pos = Vector3.zero;
        this.velocity = Vector3.zero;
        this.life = new Vector3(0, life, -1);
        this.size = new Vector3(size, size, size);
        Vector3 rnd = Random.insideUnitSphere * 0.5f + 0.5f * Vector3.one;
        this.color = color;// * new Color(rnd.x, rnd.y, rnd.z, 1);
    }
}
