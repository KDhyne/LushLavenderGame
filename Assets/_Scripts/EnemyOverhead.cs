using UnityEngine;

public class EnemyOverhead : ActorBase 
{
    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Projectile")
        {
            StartCoroutine(this.TakeDamage(1));
        }
    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            var player = otherCollider.GetComponent<Player>();

            if (player.CurrentCollider is BoxCollider && otherCollider is CapsuleCollider)
                return;
            
            //Player takes damamge
            StartCoroutine(otherCollider.gameObject.GetComponent<Player>().TakeDamage(this.AttackValue));
        }
    }
}