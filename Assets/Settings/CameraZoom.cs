using Unity.Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private CinemachineCamera vcam;

   
    public Rigidbody2D targetRb;

   // zoom is zoom out

    public float minZoom = 5f;       
    public float maxZoom = 9f;       
    public float speedThreshold = 25f; 
    public float zoomSpeed = 5f;
    public float heightThreshold = 25f;
    public float zoomSpeedHeight = 15f;
    public float maxZoomHeight = 20f;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (targetRb == null || vcam == null) return;


        //for speed

      
        float currentSpeed = targetRb.linearVelocity.magnitude;

       
        float speedPercentage = Mathf.Clamp01(currentSpeed / speedThreshold);
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, speedPercentage);

       
        vcam.Lens.OrthographicSize = Mathf.Lerp(vcam.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeed);



        //for height


        float currenthHeight = targetRb.transform.position.y;
        float heightPercentage = Mathf.Clamp01(currenthHeight / heightThreshold);


        targetZoom = Mathf.Lerp(minZoom, maxZoomHeight, heightPercentage);


        vcam.Lens.OrthographicSize = Mathf.Lerp(vcam.Lens.OrthographicSize, targetZoom, Time.deltaTime * zoomSpeedHeight);


    }
}