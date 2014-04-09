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
        iTween.MoveUpdate(cameraManTransform.gameObject, cameraTarget.transform.position - (Vector3.forward * 15), .2f);
    }
}
