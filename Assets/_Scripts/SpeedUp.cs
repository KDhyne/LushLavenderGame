using UnityEngine;

public class SpeedUp : Collectable
{
    public float SpeedUpAmount;

    public override void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Player")
        {
            var player = otherObj.GetComponent<Player>();
            player.MaxHorizontalSpeed += SpeedUpAmount;

            this.DestroyCollectable();
        }
    }
}
