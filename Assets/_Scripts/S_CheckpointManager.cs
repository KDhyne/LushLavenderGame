using System.Collections.Generic;

using UnityEngine;

public class S_CheckpointManager : MonoBehaviour
{
    private readonly List<GameObject> Checkpoints = new List<GameObject>();

    private GameObject activeCheckpoint;

    void Start()
    {
        //Get all child gameobjects and add them to the checkpoint list
        for (int i = 0; i < transform.childCount; i++)
        {
            var c = transform.GetChild(i).gameObject;
            Checkpoints.Add(c);
        }
        //Set active checkpoint to first in list
        this.SetActiveCheckpoint(Checkpoints[0]);
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
        if (GUI.Button(new Rect(50f, 100f, 100f, 50f), "Respawn"))
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = this.GetPlayerSpawnLocation();
        }
    }
}