using UnityEngine;
using System.Collections;

public class temp_platformMove : MonoBehaviour
{
    private Vector3 v_standardPosition;

    public float fl_x = 0;
    public float fl_y = 0;
    public float fl_z = 0;

    //Time to move between positions
    public float f_timeToMove;


    // Use this for initialization
    void Start()
    {
        v_standardPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        //Debug.Log(v_standardPosition.x);
        //Debug.Log(v_standardPosition.y);
        //Debug.Log(v_standardPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        iTween.MoveTo(this.gameObject, iTween.Hash("x", v_standardPosition.x + fl_x, "y", v_standardPosition.y + fl_y, "z", v_standardPosition.z + fl_z, "islocal", true, "time", f_timeToMove, "looptype", "pingpong", "easetype", "easeInOutQuad"));

    }






    ////Initial platform position
    //public Vector3 v3_initPos;
    ////position to move to
    //public Vector3 v3_movePos;
    ////Time to move between positions
    //public float f_timeToMove;

    //// Use this for initialization
    //void Start()
    //{
    //    v3_initPos = this.gameObject.transform.position;
    //    //Vector3 temp = v3_initPos + v3_movePos;
    //    Debug.Log("Starting");
    //    //iTween.MoveTo(this.gameObject, iTween.Hash("postion", temp, "time", 5, "loopType", "pingpong", "easeType", "easeInOutQuad"));
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    iTween.MoveTo(this.gameObject, iTween.Hash("postion", v3_initPos + v3_movePos, "time", f_timeToMove, "loopType", "pingpong", "easeType", "easeInOutQuad", "isLocal", true));
    //}
}
