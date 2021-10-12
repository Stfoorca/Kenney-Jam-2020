using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{

    public Transform attackPoint;
    public LayerMask enemyLayers;
    [SerializeField] private float attackRange = .5f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    void Attack()
    {
        //animController.Play("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit " + enemy.name);
        }
    }

    void Shoot(GameObject objectToSpawn)
    {
        GameObject shootObject = Instantiate(objectToSpawn, attackPoint.position, attackPoint.rotation);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
