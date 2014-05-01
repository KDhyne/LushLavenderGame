using UnityEngine;

public class SilverBell : Collectable
{
    public override void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Player")
        {
            if (otherObj.GetComponent<Player>().CurrentCollider is BoxCollider && otherObj is CapsuleCollider)
            {
                return;
            }
            
            Debug.Log("+1 Silver Bell");
            var sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
            sceneManager.CurrentSilverBellCount++;

            this.DestroyCollectable();
        }
    }
}
