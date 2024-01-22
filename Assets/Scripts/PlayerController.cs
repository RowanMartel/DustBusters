using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameObject go_lookingAtObject;
    public GameObject go_heldObject;

    GameObject go_heldPosition;
    Vector3 v3_heldPositionReset;
    public enum State
    {
        inactive,
        active
    }

    public State en_state = State.inactive;

    public float flt_speed;
    bool bl_hasJumped = false;
    public float flt_jumpForce;

    Rigidbody rb_player;
    GameObject go_cameraContainer;

    float flt_cameraVertical = 0;

    float flt_playerRotate;


    public Ray ray_playerView;
    public float flt_mouseSensitivity;

    bool bl_isGrounded = true;
    bool bl_isCrouching = false;
    void Start()
    {
        go_lookingAtObject = GameObject.Find("Floor");
        go_heldPosition = GameObject.Find("HeldPosition");
        v3_heldPositionReset = go_heldPosition.transform.localPosition;

        // Cursor.lockState = CursorLockMode.Locked;

        rb_player = GetComponent<Rigidbody>();
        go_cameraContainer = GameObject.Find("Player/CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        ray_playerView = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray_playerView, out hit, 5))
        {
            if(hit.collider.gameObject != go_lookingAtObject)
            {
                if (go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;

                go_lookingAtObject = hit.collider.gameObject;

                if(go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = true;
            }
        }
        if (!Physics.Raycast(ray_playerView, out hit, 3) && go_lookingAtObject != null)
        {
            if(go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;
            go_lookingAtObject = null;
        }

        if (Input.GetKeyDown(KeyCode.E)) Interact();

        if (en_state == State.active)
        {
            if (go_heldPosition.transform.localPosition.z >= 1.0f && go_heldPosition.transform.localPosition.z <= 2.0f)
            {
                go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, go_heldPosition.transform.localPosition.z + Input.mouseScrollDelta.y * 0.1f);

                if (go_heldPosition.transform.localPosition.z > 2) go_heldPosition.transform.localPosition = go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 2.0f);
                if (go_heldPosition.transform.localPosition.z < 1) go_heldPosition.transform.localPosition = go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 1.0f);
            }

            MoveCamera();

            if (Input.GetKeyDown(KeyCode.Space) && bl_isGrounded)
            {
                bl_hasJumped = true;
                bl_isGrounded = false;
            }

            flt_playerRotate = Input.GetAxis("Mouse X");

            transform.Rotate(0.0f, flt_playerRotate * flt_mouseSensitivity, 0.0f);

            if (Input.GetKey(KeyCode.LeftShift)) bl_isCrouching = true;
        }

        if (bl_isCrouching) Debug.Log("crouching");
    }
    void FixedUpdate()
    {
        if (go_heldObject != null && en_state == State.active)
        {
            Vector3 direction = go_heldObject.transform.position - go_heldPosition.transform.position;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);
            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            go_heldObject.transform.rotation = transform.rotation;

            go_heldObject.transform.Rotate(go_heldObject.GetComponent<Pickupable>().heldRotationMod);
        }
        else if(go_heldObject != null && en_state == State.inactive)
        {
            Vector3 heldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.5f));
            Vector3 direction = go_heldObject.transform.position - heldPosition;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);

            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            go_heldObject.transform.rotation = transform.rotation;

            go_heldObject.transform.Rotate(go_heldObject.GetComponent<Pickupable>().heldRotationMod);
        }

        if (en_state == State.active)
        {
            if (bl_hasJumped)
            {
                rb_player.AddRelativeForce(Vector3.up * flt_jumpForce, ForceMode.Impulse);
                bl_hasJumped = false;
            }
            if (!bl_isGrounded)
            {
                rb_player.AddForce(-Vector3.up * flt_jumpForce);
            }

            DoPlayerMovement();
        }
    }

    void MoveCamera()
    {
        float camV = flt_cameraVertical + Input.GetAxis("Mouse Y");

        flt_cameraVertical = Mathf.Clamp(camV, -90f, 80f);

        float flipCamV = camV * -1;

        go_cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, 0, 0);
    }

    void DoPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rb_player.AddForce(transform.forward * flt_speed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            rb_player.AddForce(transform.forward * flt_speed * 0.75f);
            rb_player.AddForce(transform.right * flt_speed * 0.75f);
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(transform.right * flt_speed);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(-transform.forward * flt_speed * 0.75f);
            rb_player.AddForce(transform.right * flt_speed * 0.75f);
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(-transform.forward * flt_speed);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(-transform.forward * flt_speed * 0.75f);
            rb_player.AddForce(-transform.right * flt_speed * 0.75f);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(-transform.right * flt_speed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(transform.forward * flt_speed * 0.75f);
            rb_player.AddForce(-transform.right * flt_speed * 0.75f);
        }
    }

    void Interact()
    {
        go_heldPosition.transform.localPosition = v3_heldPositionReset;

        if (go_heldObject == null && go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                go_heldObject = go_lookingAtObject;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                go_heldObject.GetComponent<Rigidbody>().useGravity = false;
                go_heldObject.GetComponent<Outline>().enabled = false;

                //heldObject.transform.position = midHold.transform.position;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                go_heldObject.layer = layerIgnoreRaycast;
            }
            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
        else if(go_heldObject != null && (go_lookingAtObject == null || go_lookingAtObject.tag != "Interactable"))
        {
            go_heldObject.layer = 0;
            go_heldObject.GetComponent<Rigidbody>().useGravity = true;
            Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
            go_heldObject = null;
        }
        else if (go_heldObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                go_heldObject.layer = 0;
                go_heldObject.GetComponent<Rigidbody>().useGravity = true;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                go_heldObject = null;

                if (go_heldObject != null)
                    Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                go_heldObject = go_lookingAtObject;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                go_heldObject.GetComponent<Rigidbody>().useGravity = false;
                go_heldObject.GetComponent<Outline>().enabled = false;

                //heldObject.transform.position = midHold.transform.position;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                go_heldObject.layer = layerIgnoreRaycast;
            }
            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
    }

    public void Die()
    {
        en_state = State.inactive;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") bl_isGrounded = true;
    }

    public void TogglePlayerControl()
    {
        switch(en_state)
        {
            case State.active:
                en_state = State.inactive;
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case State.inactive:
                en_state = State.active;
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }
}
