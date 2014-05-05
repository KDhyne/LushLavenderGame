using UnityEngine;
using System.Collections;

public class EnemyDiving : ActorBase
{
    private Vector3 flyInPoint;

    public override void Start()
    {
        base.Start();
        flyInPoint = Camera.main.transform.FindChild("DiveFlyInPoint").transform.localPosition;

        StartCoroutine(Run());
    }

    public IEnumerator Run()
    {
        //Approach
        iTween.MoveTo(this.gameObject, iTween.Hash("position", flyInPoint, "islocal", true, "time", 1f));
        //Wait
        yield return new WaitForSeconds(0.75f);
        //Dive toward player
        this.SpriteAnimator.SetBool("Diving", true);
        iTween.MoveTo(this.gameObject, iTween.Hash("position", new Vector3(-75, -20, 10), "islocal", true, "time", 1f, "easetype", iTween.EaseType.easeInBack));
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
