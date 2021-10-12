using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    private string playerTag = "Player";

    public float initialSpeed = 10f;

    public float lifetime = 20f;
    private GameManager gameManager;
    private Vector2 moveDir;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
    }
    
    private void Init()
    {
        gameManager = GameManager.instance;
        Invoke("Despawn", lifetime);
    }
    
    private void Despawn()
    {
        Lean.Pool.LeanPool.Despawn(gameObject);
        //ObjectPooling.Despawn(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) && !gameManager.playerInvicible)
        {
            collision.GetComponent<Player>().Die();
            Despawn();
        }
    }
    
}
