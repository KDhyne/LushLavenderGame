using System.Collections.Generic;

using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public void Activate()
    {
        renderer.material.color = Color.magenta;
        transform.parent.GetComponent<SceneManager>().SetActiveCheckpoint(this.gameObject);
    }

    public void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            this.Activate();
        }
    }
}