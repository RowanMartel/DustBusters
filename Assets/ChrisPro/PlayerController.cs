using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    GameObject lookingAtObject;
    GameObject heldObject;

    GameObject rightHold;
    public enum State
    {
        inactive,
        active
    }

    private State state = State.active;

    public float walkSpeed = 2;
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

    public Ray playerView;


    void Start()
    {
        lookingAtObject = GameObject.Find("Floor");
        rightHold = GameObject.Find("RightHold");

        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        cameraContainer = GameObject.Find("Player/CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        if (heldObject != null) heldObject.transform.position = rightHold.transform.position;

        playerView = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(playerView, out hit, 100))
        {
            if(hit.collider.gameObject != lookingAtObject)
            {
                if (lookingAtObject.tag == "Interactable") lookingAtObject.GetComponent<Outline>().enabled = false;

                lookingAtObject = hit.collider.gameObject;

                if(lookingAtObject.tag == "Interactable") lookingAtObject.GetComponent<Outline>().enabled = true;
            }
        }

        if (state == State.active)
        {
            MoveCamera();

            if (Input.GetKeyDown(KeyCode.LeftShift)) isRunning = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) isRunning = false;

            if (Input.GetKeyDown(KeyCode.E)) Interact();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                jump = true;
                isGrounded = false;
            }

            playerForward = Input.GetAxis("Vertical");
            playerSideStep = Input.GetAxis("Horizontal");
            playerRotate = Input.GetAxis("Mouse X");

            transform.Rotate(0.0f, playerRotate, 0.0f);
        }
    }
    void FixedUpdate()
    {
        if (state == State.active)
        {
            if (jump)
            {
                rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jump = false;
            }

            if (!isRunning) speed = walkSpeed;
            if (isRunning) speed = runSpeed;

            rb.AddForce(transform.forward * playerForward * speed);
            rb.AddForce(transform.right * playerSideStep * speed);
        }
    }

    void MoveCamera()
    {
        float camV = cameraVertical + Input.GetAxis("Mouse Y");

        cameraVertical = Mathf.Clamp(camV, -60f, 80f);

        float flipCamV = camV * -1;

        cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, 0, 0);
    }

    void Interact()
    {
        // Once I know more about the interaction system and what is needed from my side I can build this
        if (heldObject == null)
        {
            if (lookingAtObject.gameObject.tag == "Interactable") heldObject = lookingAtObject;
        }
        else heldObject = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") isGrounded = true;
    }

    void TogglePlayerControl()
    {
        switch(state)
        {
            case State.active:
                state = State.inactive;
                Cursor.lockState = CursorLockMode.None;
                break;

            case State.inactive:
                state = State.active;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}
