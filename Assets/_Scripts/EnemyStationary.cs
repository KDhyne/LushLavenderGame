using UnityEngine;
using System.Collections;

public class EnemyStationary : S_ActorBase 
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Debug.Log("Butts");
            //Player takes damamge
            StartCoroutine(collider.gameObject.GetComponent<S_Player>().TakeDamage(this.AttackValue));
        }
        if (collider.tag == "Projectile")
        {
            StartCoroutine(this.TakeDamage(1));
        }
    }
}
