using UnityEngine;
using System.Collections;

public class S_Enemy : S_Actor 
{
    public Texture[] mat_icons;
    float f_moveSpeed;
    public S_GUI s_GUI;

	// Use this for initialization
	public override void Start () 
    {
        //Inherit Base Start
        base.Start();
        //Find the S_GUI 
        s_GUI = (S_GUI)GameObject.Find("GUI Manager").GetComponent("S_GUI");

        //Set the speed based on the f_enemySpeed in s_GUI
        f_moveSpeed = s_GUI.f_enemySpeed;

        //Set a random texture from an array of textures
        go_actorSprite.renderer.material.mainTexture = mat_icons[Random.Range(0, mat_icons.Length)];
	}
	
	// Update is called once per frame
    public override void Update() 
    {
        base.Update();

        //Move down over time
        transform.Translate(Vector3.down * 3 * Time.deltaTime * f_moveSpeed);

        if (transform.position.y <= -3)
        {
            s_GUI.i_loseScore++;
            Destroy(this.gameObject);
        }

	}
}
