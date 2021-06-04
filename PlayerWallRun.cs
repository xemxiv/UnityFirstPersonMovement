using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRun : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private Transform orientation = null;
    [SerializeField] private float minWallDistance = 1.0f;
    [SerializeField] private float minJumpHeight= 1.5f;
    RaycastHit leftHit, rightHit;
    private bool leftWall = false;
    private bool rightWall = false;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;
    private Rigidbody rb;
    Vector3 wallRunDirection;
    bool isWallRunning;

    [Header("Wall Run Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunFov;
    [SerializeField] private float wallRunFovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;
    public float currentCamTilt { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckWall();

        if (CanWallRun())
        {
            if (leftWall)
            {
                StartWallRun();     
                WallRunCamera();
                Debug.Log("wall running on the left");
            }
            else if (rightWall)
            {               
                StartWallRun();              
                WallRunCamera();
                Debug.Log("wall running on the right");
            }
            else
            {               
                EndWallRun();
                NormalCamera();
                Debug.Log("not wall running");
            }
        }
        else
        {
            EndWallRun();
            NormalCamera();
            Debug.Log("not wall running");
        }
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void CheckWall()
    {
        leftWall = Physics.Raycast(transform.position, -orientation.right, out leftHit, minWallDistance);
        rightWall = Physics.Raycast(transform.position, orientation.right, out rightHit, minWallDistance);
    }
    
    void StartWallRun()
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (leftWall)
            {
                wallRunDirection = transform.up + leftHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (rightWall)
            {
                wallRunDirection = transform.up + rightHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }

    void EndWallRun()
    {
        rb.useGravity = true;
    }

    void WallRunCamera()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if (leftWall)
        {
            currentCamTilt = Mathf.Lerp(currentCamTilt, -camTilt, camTiltTime * Time.deltaTime);
        }
        else if (rightWall)
        {
            currentCamTilt = Mathf.Lerp(currentCamTilt, camTilt, camTiltTime * Time.deltaTime);
        }

    }

    void NormalCamera()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        currentCamTilt = Mathf.Lerp(currentCamTilt, 0, camTiltTime * Time.deltaTime);
    }
       
}
