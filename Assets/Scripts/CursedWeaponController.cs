using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedWeaponController : MonoBehaviour
{
    public Rigidbody2D owner;
    public Transform graphicsParent;
    public LineRenderer tether;

    private Rigidbody2D rb;

    [Header("Movement")]
    public float speedScale = 0.1f;
    public float distanceLimit = 5;

    private float minimumDistance = 0.0f;
    private Vector2 mouseInput;
    private Vector2 localPos;
    private Vector2 localPosPrev;
    private float currentSpeed;

    [Header("Pull")]
    public float pullForce = 500;
    public float pullCooldownDuration = 0.3f;
    public float rangeSpeedRequiredToPull = 500;

    private float pullCooldownStartTime = -Mathf.Infinity;

    [Header("Hit")]
    public string hittableTag = "Hittable";
    //public float speedRequiredToHit = 300;

    [Header("Swing")]
    private float swingCooldownDuration = 1;
    private float swingCooldownStartTime = -Mathf.Infinity;

    private GameManager gameManager;

    private TalkingSword talk;
    private bool isTalking;

    private void Awake()
    {
        if (tether == null) tether = GetComponentInChildren<LineRenderer>();
        if (graphicsParent == null) graphicsParent = transform.Find("GraphicsParent");
        rb = GetComponent<Rigidbody2D>();
        talk = GetComponent<TalkingSword>();
        Init();
        UpdateTether();
    }

    private void Start()
    {
        gameManager = GameManager.instance;
        isTalking = true;
        speedScale = gameManager.mediumSense;
    }

    private void Update()
    {
        if (gameManager.gameState == GameManager.GameState.END && !isTalking)
        {
            isTalking = true;
            talk.ShowText();
        }
        else if (gameManager.gameState == GameManager.GameState.PLAYING)
        {
            isTalking = false;
            float angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg - 90;
            graphicsParent.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Tether
            UpdateTether();

            //Input
            mouseInput.x = Input.GetAxis("Mouse X");
            mouseInput.y = Input.GetAxis("Mouse Y");

            //Position
            localPosPrev = localPos;
            localPos += mouseInput * speedScale;

            currentSpeed = (localPosPrev - localPos).magnitude / Time.deltaTime;

            if (localPos.magnitude > distanceLimit)
            {
                float rangeSpeed = Mathf.Abs((localPosPrev.magnitude - localPos.magnitude)) / Time.deltaTime;
                //Debug.Log(distanceDelta);

                if (rangeSpeed > rangeSpeedRequiredToPull * speedScale &&
                    Time.time - pullCooldownStartTime > pullCooldownDuration)
                {
                    PullOwner();
                }

                localPos = localPos.normalized * distanceLimit;
            }

            //if(currentSpeed > )
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.gameState == GameManager.GameState.PLAYING)
        {
            Vector2 targetVector = owner.position + localPos - (Vector2) transform.position;

            rb.velocity = targetVector / Time.deltaTime;
        }
        else if (gameManager.gameState == GameManager.GameState.END)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            return;
        }
        //float targetAngle = (Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg - 90) - transform.rotation.eulerAngles.z;

        //rb.angularVelocity = targetAngle / Time.deltaTime;

    }

    public void Init()
    {
        //localPos = owner.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UpdateTether()
    {
        if (owner == null)
            return;
        Vector3[] tetherPos = { tether.transform.position, owner.transform.position };
        tether.SetPositions(tetherPos);
    }

    private void PullOwner()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SwordDash");
        owner.AddForce(localPos.normalized * pullForce);
        pullCooldownStartTime = Time.time;
        StartCoroutine(FindObjectOfType<Player>().Dziabniety());
        //Debug.Log($"Pulled {owner.name} in direction {localPos.normalized}.");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (currentSpeed > speedRequiredToHit * speedScale)
        {
            if (collision.collider.CompareTag(hittableTag))
            {
                //Debug.Log($"{collision.collider.name} has been hit. Sword speed: {currentSpeed}.");
                collision.collider.GetComponent<Enemy>().Die();

                //SpriteRenderer colSR = collision.collider.GetComponent<SpriteRenderer>();
                //if (colSR != null) SFXSpawner.instance.SpawnSheetExplosion(colSR.transform.position, colSR.sprite, Color.white, Vector2.right, 360);
            }
        }
    }
}
