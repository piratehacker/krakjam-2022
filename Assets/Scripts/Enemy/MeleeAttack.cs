using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Player;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public bool attacking = false;
    [HideInInspector] public float knockback = 10f;
    [HideInInspector] public float damage = 10f;

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Player" && attacking) //player
        {
            attacking = false;
            col.gameObject.GetComponent<CharacterController>().Move((col.transform.position - transform.position).normalized * knockback);
            col.gameObject.GetComponent<PlayerHealth>().Health -= damage;
        }
    }
}
