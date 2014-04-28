using UnityEngine;

public class S_CameraMan : MonoBehaviour
{
    private Transform cameraManTransform;
    private Transform cameraTarget;

    // Use this for initialization
    void Start()
    {
        cameraManTransform = Camera.main.transform;
        cameraTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        iTween.MoveUpdate(cameraManTransform.gameObject, 
            new Vector3(cameraTarget.transform.position.x - (Vector3.forward.x * 15),
                2.5f,
                -10f), 
            .2f);
    }
}
