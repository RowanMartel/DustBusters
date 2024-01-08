using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        inactive,
        active
    }

    private State state = State.active;

    public float walkSpeed;
    public float runSpeed;
    bool isRunning = false;
    bool jump = false;
    public float jumpForce = 8f;

    Rigidbody rb;
    GameObject cameraContainer;

    float camH = 0;
    float camV = 0;

    float camVertical;
    float camHorizontal;

    float rotatePlayer;
    float playerForward;
    float sideStep;

    float speed;

    public bool isGrounded = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        cameraContainer = GameObject.Find("Player/CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        camVertical = camV + Input.GetAxis("Mouse Y");

        CameraControls(camHorizontal, camVertical);

        rotatePlayer = Input.GetAxis("Mouse X");

        sideStep = Input.GetAxis("Horizontal");

        playerForward = Input.GetAxis("Vertical");

        if (!isRunning) speed = walkSpeed;
        if (isRunning) speed = runSpeed;

        if (Input.GetKeyDown(KeyCode.LeftShift)) isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

        transform.Rotate(0.0f, rotatePlayer, 0.0f);
        rb.AddForce(transform.forward * playerForward);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jump = true;
            isGrounded = false;
        }
    }
    void FixedUpdate()
    {
        if (jump)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jump = false;
        }
    }

    void CameraControls(float horizontal, float vertical)
    {
        //camH = Mathf.Clamp(horizontal, -25f, 25f);
        camV = Mathf.Clamp(vertical, -10f, 90f);

        float flipCamV = camV * -1;

        cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, camH - 90, 0);
    }
}
