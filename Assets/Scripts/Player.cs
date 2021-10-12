using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Movement")] 
    public float normalMoveSpeed;
    public float cutsceneMoveSpeed;
    private float moveSpeed = 1f;
    public bool facingRight;
    [HideInInspector]
    public Vector2 move;
    private SpriteRenderer sprite;

    private GameManager gameManager;
    [Header("Flower")]
    public GameObject targetFlower;
    private float lookForFlowerTime;
    public float lookForFlowerInterval;
    public Color randomColor;
    public bool dziabnieted;
    private GameObject sword;
    void Awake(){
        
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        
    }

    void Start()
    {
        gameManager = GameManager.instance;
        lookForFlowerTime = 0f;
        moveSpeed = cutsceneMoveSpeed;
        randomColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 1);
        sword = FindObjectOfType<CursedWeaponController>().gameObject;
    }

    private void Update()
    {
        sprite.color = randomColor;
        if (gameManager.gameState == GameManager.GameState.PLAYING)
        {
            moveSpeed = normalMoveSpeed;
            if (lookForFlowerTime < lookForFlowerInterval)
                lookForFlowerTime += Time.deltaTime;
            else
                SetTargetFlower();
            if (targetFlower != null)
            {
                move = (targetFlower.transform.position - transform.position).normalized;
            }
        }
        else if (gameManager.gameState == GameManager.GameState.END && gameManager.enemies.Count < 1)
        {
            moveSpeed = cutsceneMoveSpeed;
            if (Vector2.Distance(sword.transform.position, transform.position) < 0.5)
            {
                sword.GetComponent<CursedWeaponController>().owner = GetComponent<Rigidbody2D>();
                //gameManager.player = gameObject;
                Camera.main.GetComponent<CameraFollow>().target = gameObject.transform;

                gameManager.NewPlayerFoundSword();
            }
            else
            {
                move = (sword.transform.position - transform.position).normalized;
            }
        }
        else
        {
            return;
        }
        if (move.x >= 0)
            facingRight = true;
        else
            facingRight = false;

        FlipSprite();
    }
    
    private void FixedUpdate()
    {
        if (gameManager.gameState == GameManager.GameState.PAUSED)
            return;
        if (dziabnieted)
            return;
        

        rb.AddForce(move * moveSpeed * 100 * Time.fixedDeltaTime);
    }
    
    public IEnumerator Dziabniety()
    {
        dziabnieted = true;
        yield return new WaitForSeconds(sword.GetComponent<CursedWeaponController>().pullCooldownDuration);
        dziabnieted = false;
    }

    void FlipSprite()
    {
        if (facingRight && sprite.flipX)
            sprite.flipX = false;
        else if(!facingRight && !sprite.flipX)
            sprite.flipX = true;
    }
    
    private void SetTargetFlower()
    {
        float lowestDistance = Mathf.Infinity;
        int lowestDistanceFlowerIndex = 0;
        for (int i = 0; i < gameManager.flowers.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, gameManager.flowers[i].transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                lowestDistanceFlowerIndex = i;
            }
        }
        if(targetFlower == null || targetFlower != gameManager.flowers[lowestDistanceFlowerIndex])
            targetFlower = gameManager.flowers[lowestDistanceFlowerIndex];
    }

    public void Die()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/PlayerDeath");
        SFXSpawner.instance.SpawnSheetExplosion(transform.position, sprite.sprite, randomColor, Vector2.right, 360);

        gameManager.RemovePlayer();

        Destroy(gameObject);
        //gameManager.player = null;
    }


}
