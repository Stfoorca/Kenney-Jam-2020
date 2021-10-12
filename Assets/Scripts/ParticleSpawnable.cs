using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawnable : MonoBehaviour
{
    ParticleSystem ps;
    public float lifetime = 2;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if (ps != null)
            ps.Play();
        Invoke("Despawn", lifetime);
        
    }

    private void Despawn()
    {
        Lean.Pool.LeanPool.Despawn(gameObject);
    }

}
