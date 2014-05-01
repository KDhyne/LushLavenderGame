using UnityEngine;

public class SpeedUp : Collectable
{
    public override void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Player")
        {
            var player = otherObj.GetComponent<Player>();
            player.SpeedUpLevel++;

            this.DestroyCollectable();
        }
    }
}
