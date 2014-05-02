using UnityEngine;

public class SilverBell : Collectable
{
    public override void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.tag == "Player")
        {
            Debug.Log("+1 Silver Bell");
            var sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
            sceneManager.CurrentSilverBellCount++;

            this.DestroyCollectable();
        }
    }
}
