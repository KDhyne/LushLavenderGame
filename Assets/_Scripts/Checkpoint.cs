using System.Collections.Generic;

using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public void Activate()
    {
        renderer.material.color = Color.magenta;
        transform.parent.GetComponent<CheckpointManager>().SetActiveCheckpoint(gameObject);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            this.Activate();
        }
    }
}