using UnityEngine;

public class S_CameraMan : MonoBehaviour
{
    enum CameraState
    {
        ZoomIn,
        ZoomOut
    }

    Camera C_mainCamera;
    Transform T_cameraManTransform;
    Transform T_cameraTarget;

    CameraState st_currentCameraState;

    // Use this for initialization
    void Start()
    {
        C_mainCamera = Camera.main;
        T_cameraManTransform = transform;
        T_cameraTarget = GameObject.Find("Player").transform;
        st_currentCameraState = CameraState.ZoomIn;
    }

    // Update is called once per frame
    void Update()
    {
        iTween.MoveUpdate(T_cameraManTransform.gameObject, T_cameraTarget.transform.position - (Vector3.forward * 15), .2f);

        //Change the size of the camera
        if (Input.GetKey(KeyCode.W))
        {
            //st_currentCameraState = CameraState.zoomOut;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //st_currentCameraState = CameraState.zoomIn;
        }

        //States
        switch (st_currentCameraState)
        {
            case CameraState.ZoomIn:
                {
                    C_mainCamera.orthographicSize = iTween.FloatUpdate(C_mainCamera.orthographicSize, 15f, 7f);
                    //float cameraHeight = iTween.FloatUpdate(T_cameraManTransform.position.y, 14f, 7f);
                    //T_cameraManTransform.position = new Vector3(T_cameraManTransform.position.x, cameraHeight, T_cameraManTransform.position.z);
                }
                break;
            case CameraState.ZoomOut:
                {
                    C_mainCamera.orthographicSize = iTween.FloatUpdate(C_mainCamera.orthographicSize, 25f, 7f);
                    float cameraHeight = iTween.FloatUpdate(T_cameraManTransform.position.y, 25f, 7f);
                    T_cameraManTransform.position = new Vector3(T_cameraManTransform.position.x, cameraHeight, T_cameraManTransform.position.z);
                }
                break;
            default:
                break;
        }
    }
}
