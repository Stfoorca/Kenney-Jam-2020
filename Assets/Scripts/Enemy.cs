using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")] public float normalMoveSpeed;
    public float cutsceneMoveSpeed;
    private float moveSpeed;

    private bool facingRight;
    private Vector2 move;
    public bool drawGizmos;
    public float gizmoLength = 5;

    [Header("Combat")]
    private AnimationController animCon;
    private SpriteRenderer sprite;
    public GameObject target;
    private GameManager gameManager;
    public float AggroDistance;
    public float ShootDistance;
    public bool shooter;
    public GameObject bulletPrefab;
    public Transform shootPosition;
    public float shootCooldown;
    private float shootTime;
    private bool targetInRange;
    private bool shootInRange;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animCon = GetComponent<AnimationController>();
    }

    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        animCon.Init();
    }

    public void Init()
    {
        gameManager = GameManager.instance;
        target = gameManager.player;
        shootTime = shootCooldown;
        moveSpeed = normalMoveSpeed;
    }

    void Update()
    {
        if (gameManager.gameState == GameManager.GameState.PLAYING)
        {
            if (target == null) target = gameManager.player;
            moveSpeed = normalMoveSpeed;
            targetInRange = Vector2.Distance(target.transform.position, transform.position) < AggroDistance;
            shootInRange = Vector2.Distance(target.transform.position, transform.position) < ShootDistance;
            //direction = Quaternion.AngleAxis(directionAngle, Vector3.forward) * Vector3.right;
            if (target != null)
                move = (target.transform.position - transform.position).normalized;

            if (move.x >= 0)
                facingRight = true;
            else
                facingRight = false;

            FlipSprite();

            if (shooter && shootInRange)
            {
                if (shootTime < shootCooldown)
                    shootTime += Time.deltaTime;
                else
                {
                    Shoot();
                    shootTime = 0;
                }
            }
        }
        else if (gameManager.gameState == GameManager.GameState.END)
        {
            moveSpeed = cutsceneMoveSpeed;
            if (!GetComponent<SpriteRenderer>().isVisible)
                gameManager.RemoveEnemy(gameObject);
            move = -((Vector3)gameManager.lastPlayerPosition - transform.position).normalized;
            if (move.x >= 0)
                facingRight = true;
            else
                facingRight = false;

            FlipSprite();
        }
    }
    
    void FixedUpdate()
    {
        if(target != null && targetInRange && gameManager.gameState == GameManager.GameState.PLAYING)
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        else if (gameManager.gameState == GameManager.GameState.END)
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }


    void Shoot()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/EnemyShoot");
        //Transform bullet = ObjectPooling.Spawn(bulletPrefab).transform;
        Transform bullet = Lean.Pool.LeanPool.Spawn(bulletPrefab).transform;
        bullet.position = transform.position;
        bullet.GetComponent<Rigidbody2D>().velocity = move * bullet.GetComponent<Bullet>().initialSpeed;
    }

    void FlipSprite()
    {
        if (facingRight && sprite.flipX)
            sprite.flipX = false;
        else if(!facingRight && !sprite.flipX)
            sprite.flipX = true;
    }
    
    private void OnDrawGizmos()
    {
        if(drawGizmos)
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + move * gizmoLength);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player") && !gameManager.playerInvicible)
        {
            other.gameObject.GetComponent<Player>().Die();
        }
    }

    public void Die()
    {
        SFXSpawner.instance.SpawnSheetExplosion(transform.position, sprite.sprite, Color.white, Vector2.right, 360);
        SFXSpawner.instance.SpawnBloodExplosion(transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/EnemyDeath");
        gameManager.KillEnemy(gameObject);
    }


}
