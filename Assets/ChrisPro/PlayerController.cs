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

    float cameraVertical = 0;

    float playerForward;
    float playerSideStep;
    float playerRotate;

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
        MovePlayer();
        MoveCamera();

        if (Input.GetKeyDown(KeyCode.LeftShift)) isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

        if (Input.GetKeyDown(KeyCode.E)) Interact();

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

    void MovePlayer()
    {
        playerForward = Input.GetAxis("Vertical");
        playerSideStep = Input.GetAxis("Horizontal");
        playerRotate = Input.GetAxis("Mouse X");

        if (!isRunning) speed = walkSpeed;
        if (isRunning) speed = runSpeed;

        transform.Rotate(0.0f, playerRotate, 0.0f);
        rb.AddForce(transform.forward * playerForward * speed);
        rb.AddForce(transform.right * playerSideStep);
    }
    void MoveCamera()
    {
        float camV = cameraVertical + Input.GetAxis("Mouse Y");

        cameraVertical = Mathf.Clamp(camV, -40f, 80f);

        float flipCamV = camV * -1;

        cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, 0, 0);
    }

    void Interact()
    {
        // Once I know more about the interaction system and what is needed from my side I can build this
        Debug.Log("Pressed Activate");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") isGrounded = true;
    }
}
