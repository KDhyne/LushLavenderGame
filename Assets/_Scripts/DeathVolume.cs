using UnityEngine;

namespace Assets._Scripts
{
    class DeathVolume : MonoBehaviour
    {
        void OnTriggerStay(Collider otherCollider)
        {
            if (otherCollider.tag == "Player")
            {
                var player = otherCollider.GetComponent<Player>();

                if (player.CurrentCollider is BoxCollider && otherCollider is CapsuleCollider)
                    return;

                player.DestroyActor();
            }
        }
    }
}
