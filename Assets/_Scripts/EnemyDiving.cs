using UnityEngine;
using System.Collections;

public class EnemyDiving : ActorBase 
{
    public override void Start()
    {
        base.Start();
        StartCoroutine(Run());
    }

    public IEnumerator Run()
    {
        //Approach
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(30,15,10), "islocal", true, "time", 1f));
        //Wait
        yield return new WaitForSeconds(1f);
        //Dive toward player
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(-60, -30, 10), "islocal", true, "time", 1f, "easetype", iTween.EaseType.easeInBack));
        //Wait then destroy
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            Debug.Log("Player got hit");
            StartCoroutine(otherCollider.GetComponent<Player>().TakeDamage(1));
        }
    }

}
