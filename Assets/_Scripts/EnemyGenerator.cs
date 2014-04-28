using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour 
{
    //
    public GameObject[] go_enemyTypeArray;

    public GameObject go_enemy;

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("SpawnEnemy", 0, 1.5f);
        InvokeRepeating("SpawnEnemy", 10, 2f);
	}

	// Update is called once per frame
	void Update ()
    {

	}


    void SpawnEnemy()
    {
        Instantiate(go_enemy, new Vector3(Random.Range(-15, 15), 55, 0), Quaternion.identity);
    }

}
