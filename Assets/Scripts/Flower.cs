using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    private GameManager gameManager;
    private string playerTag = "Player";
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            gameManager.RemoveFlower(gameObject);
        }
    }
}
