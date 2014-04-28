using UnityEngine;
using System.Collections;

public class TriggerDiving : MonoBehaviour
{

    public GameObject EnemyDiving;

    private Transform spawnpoint;

    void Start()
    {
        spawnpoint = Camera.main.transform.FindChild("DiveSpawnPoint").transform;
    }

    //When the player collides with this trigger...
    //Spawn a new EnemyDiving prefab...
    //      Place it just off camera
    //      Attach it to the camera object

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            var bird = Instantiate(EnemyDiving, spawnpoint.position, Quaternion.identity) as GameObject;
            bird.transform.parent = Camera.main.transform;
        }
    }
}
