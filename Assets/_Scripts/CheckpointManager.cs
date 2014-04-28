using System.Collections.Generic;

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private readonly List<GameObject> checkpoints = new List<GameObject>();

    private GameObject activeCheckpoint;

    void Start()
    {
        //Get all child gameobjects and add them to the checkpoint list
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i).gameObject;
            this.checkpoints.Add(c);
        }
        //Set active checkpoint to first in list
        this.SetActiveCheckpoint(this.checkpoints[0]);
    }

    public void SetActiveCheckpoint(GameObject gameObject)
    {
        activeCheckpoint = gameObject;
    }

    public Vector2 GetPlayerSpawnLocation()
    {
        return activeCheckpoint.transform.position;
    }

    void OnGUI()
    {
        if (UnityEngine.GUI.Button(new Rect(50f, 100f, 100f, 50f), "Respawn"))
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = this.GetPlayerSpawnLocation();
        }
    }
}