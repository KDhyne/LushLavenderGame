using UnityEngine;

public class EnemyStationary : ActorBase 
{
    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            //Player takes damamge
            StartCoroutine(otherCollider.gameObject.GetComponent<Player>().TakeDamage(this.AttackValue));
        }
        if (otherCollider.tag == "Projectile")
        {
            StartCoroutine(this.TakeDamage(1));
        }
    }
}
