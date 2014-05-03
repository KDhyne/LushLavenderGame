using UnityEngine;

public class SilverBell : Collectable
{
    public override void ApplyCollectedEffect(Player player)
    {
        Debug.Log("+1 Silver Bell");
        var sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        sceneManager.CurrentSilverBellCount++;
    }
}
