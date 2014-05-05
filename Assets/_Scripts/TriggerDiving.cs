using UnityEngine;

public class TriggerDiving : MonoBehaviour
{
    public GameObject EnemyDiving;
    public bool Active;

    private Transform spawnpoint;
    private Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        spawnpoint = Camera.main.transform.FindChild("DiveSpawnPoint").transform;
        Active = true;
    }

    void Update()
    {
        if (player.CurrentActorState == ActorBase.ActorState.Dead && !Active)
        {
            Active = true;
        }
    }

    //When the player collides with this trigger...
    //Spawn a new EnemyDiving prefab...
    //      Place it just off camera
    //      Attach it to the camera object
    void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Player" && Active)
        {
            Active = false;

            var bird = (GameObject)Instantiate(EnemyDiving, spawnpoint.position, Quaternion.identity);
            bird.transform.parent = Camera.main.transform;
        }
    }
}
