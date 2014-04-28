using System;

using UnityEngine;
using System.Collections;

public class EnemyDiving : S_ActorBase 
{
    enum DiveState
    {
        Approach,
        Wait,
        Diving
    }

    

    private DiveState currentState;

    public override void Start()
    {
        base.Start();
        currentState = DiveState.Approach;

        StartCoroutine(Run());
    }

    public override void Update()
    {
        base.Update();
    }

    public IEnumerator Run()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(30,15,10), "islocal", true, "time", 1f));
        yield return new WaitForSeconds(1f);
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(-60, -30, 10), "islocal", true, "time", 1f, "easetype", iTween.EaseType.easeInBack));
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            Debug.Log("Player got hit");
            StartCoroutine(collider.GetComponent<S_Player>().TakeDamage(1));
        }
    }

}
