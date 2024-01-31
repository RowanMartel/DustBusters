using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected MenuManager menuManager;

    // GameObjects related to player's ability to hold props
    protected GameObject go_lookingAtObject;
    protected GameObject go_heldPosition;
    protected GameObject go_heldObject;
    public GameObject Go_heldObject {  get { return go_heldObject; } }

    // Reference to the camera and ability to look around
    protected GameObject go_cameraContainer;

    // Default position for where the player holds objects
    protected Vector3 v3_heldPositionReset;

    protected Rigidbody rb_player;

    // State prevents the player from moving when in menus or doing chores
    public enum State
    {
        inactive,
        active
    }

    protected State en_state = State.inactive;
    public State En_state { get { return en_state; } set { en_state = value; } }

    // This handles player rotation and camera position
    protected float flt_cameraVertical = 0;
    protected float flt_playerRotate;

    // This takes in what the player is looking at
    public Ray ray_playerView;

    // Bools to track jumping
    public bool bl_hasJumped = false;
    public bool bl_isGrounded = true;

    public LayerMask lm;

    public GameObject go_curRegion;

    void Start()
    {

        // Grabbing required references to objects and systems
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();

        go_lookingAtObject = GameObject.Find("Floor");
        go_heldPosition = GameObject.Find("HeldPosition");
        v3_heldPositionReset = go_heldPosition.transform.localPosition;

        rb_player = GetComponent<Rigidbody>();
        go_cameraContainer = GameObject.Find("Player/CameraContainer");
    }

    // Update is called once per frame
    void Update()
    {
        // This calls the method to fire the raycast that tells us what the player is looking at
        DoPlayerView();

        // Player's ability to interact
        if (Input.GetKeyDown(KeyCode.E)) Interact();

        if (en_state == State.active)
        {
            MoveCamera();

            // This allows the player to use the reach mechanic with their mousewheel to put props on hard to reach surfaces.
            DoPlayerReach();

            // Player Jump tracking
            if (Input.GetKeyDown(KeyCode.Space) && bl_isGrounded)
            {
                bl_hasJumped = true;
                bl_isGrounded = false;
            }

            // Handles Crouch
            if (Input.GetKeyDown(KeyCode.LeftShift)) LeanTween.moveLocalY(go_cameraContainer, 0f, 0.25f); // bl_isCrouching = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) LeanTween.moveLocalY(go_cameraContainer, 0.5f, 0.25f); // bl_isCrouching = false;
        }

        // Toggles pause
        if (Input.GetKeyDown(KeyCode.Escape)) menuManager.TogglePause();
    }
    void FixedUpdate()
    {
        // This handles a held objects position in front of player while player is active
        if (go_heldObject != null && en_state == State.active)
        {
            Vector3 direction = go_heldObject.transform.position - go_heldPosition.transform.position;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);
            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            go_heldObject.transform.rotation = transform.rotation;

            go_heldObject.transform.Rotate(go_heldObject.GetComponent<Pickupable>().v3_heldRotationMod);
        }

        // This handles a held objects position in front of player while player is inactive, used during chore activities
        else if (go_heldObject != null && en_state == State.inactive)
        {
            Vector3 heldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.5f));
            Vector3 direction = go_heldObject.transform.position - heldPosition;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);

            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            go_heldObject.transform.rotation = transform.rotation;

            go_heldObject.transform.Rotate(go_heldObject.GetComponent<Pickupable>().v3_heldRotationMod);
        }

        // Player can only move and jump if in Active state
        if (en_state == State.active)
        {
            if (bl_hasJumped)
            {
                rb_player.AddRelativeForce(Vector3.up * Settings.int_playerJumpForce, ForceMode.Impulse);
                bl_hasJumped = false;
            }
            if (!bl_isGrounded)
            {
                rb_player.AddForce(-Vector3.up * Settings.int_playerJumpForce);
            }

            DoPlayerMovement();
        }
    }

    // This handles the player's ability to look up and down, and rotate the player.
    void MoveCamera()
    {
        float camV = flt_cameraVertical + Input.GetAxis("Mouse Y");

        flt_cameraVertical = Mathf.Clamp(camV, -90f, 80f);

        float flipCamV = camV * -1;

        go_cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV, 0, 0);

        flt_playerRotate = Input.GetAxis("Mouse X");

        transform.Rotate(0.0f, flt_playerRotate * Settings.flt_lookSensitivity, 0.0f);
    }

    // This forces the player to look at a particular point in world space, available for locking onto chores
    public void LookAt(Vector3 position)
    {
        LeanTween.rotateLocal(go_cameraContainer, position, 0.25f);
    }

    // This handles the player's basic forward/backward/left/right movement, mapped to the WASD keys.
    void DoPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rb_player.AddForce(transform.forward * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            rb_player.AddForce(transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(transform.right * Settings.int_playerSpeed * 0.75f);
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(transform.right * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(-transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(transform.right * Settings.int_playerSpeed * 0.75f);
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(-transform.forward * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(-transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(-transform.right * Settings.int_playerSpeed * 0.75f);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            rb_player.AddForce(-transform.right * Settings.int_playerSpeed);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            rb_player.AddForce(transform.forward * Settings.int_playerSpeed * 0.75f);
            rb_player.AddForce(-transform.right * Settings.int_playerSpeed * 0.75f);
        }
    }

    // This handles the player's view at the crosshair and if pointed at an Interactable object, will activate the object's outline to indicate it can be interacted with.
    void DoPlayerView()
    {
        ray_playerView = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray_playerView, out hit, 5, lm))
        {
            if (hit.collider.gameObject != go_lookingAtObject)
            {
                if (go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;

                go_lookingAtObject = hit.collider.gameObject;

                if (go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = true;
            }
        }
        if (!Physics.Raycast(ray_playerView, out hit, 3, lm) && go_lookingAtObject != null)
        {
            if (go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;
            go_lookingAtObject = null;
        }
    }

    // Handles the player's ability to extend where the held prop is positioned, like reaching out in front of them
    void DoPlayerReach()
    {
        if (go_heldPosition.transform.localPosition.z >= 1.0f && go_heldPosition.transform.localPosition.z <= 2.0f)
        {
            go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, go_heldPosition.transform.localPosition.z + Input.mouseScrollDelta.y * 0.1f);

            if (go_heldPosition.transform.localPosition.z > 2) go_heldPosition.transform.localPosition = go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 2.0f);
            if (go_heldPosition.transform.localPosition.z < 1) go_heldPosition.transform.localPosition = go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 1.0f);
        }
    }

    // This handles the interactions between the player and the props and environment.
    void Interact()
    {
        go_heldPosition.transform.localPosition = v3_heldPositionReset;

        if (go_heldObject == null && go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            //Interact with an object while not holding anything
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                //Pick up said object
                go_heldObject = go_lookingAtObject;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                go_heldObject.GetComponent<Rigidbody>().useGravity = false;
                go_heldObject.GetComponent<Outline>().enabled = false;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                go_heldObject.layer = layerIgnoreRaycast;
                if(go_heldObject == GameManager.ghost.go_curHeldItem)
                {
                    GameManager.ghost.GetRobbed();
                }
            }
            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
        else if(go_heldObject != null && (go_lookingAtObject == null || go_lookingAtObject.tag != "Interactable"))
        {
            //Drop held item
            go_heldObject.layer = 0;
            go_heldObject.GetComponent<Rigidbody>().useGravity = true;
            Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
            go_heldObject = null;
        }
        else if (go_heldObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            //Interact with object while holding another object
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                //Drop old item
                go_heldObject.layer = 0;
                go_heldObject.GetComponent<Rigidbody>().useGravity = true;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                go_heldObject = null;

                //Pick up new item
                if (go_heldObject != null)
                    Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
                go_heldObject = go_lookingAtObject;
                Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

                go_heldObject.GetComponent<Rigidbody>().useGravity = false;
                go_heldObject.GetComponent<Outline>().enabled = false;

                int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
                go_heldObject.layer = layerIgnoreRaycast;
                if (go_heldObject == GameManager.ghost.go_curHeldItem)
                {
                    GameManager.ghost.GetRobbed();
                }
            }
            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
    }

    // Deactivates the player control in event of death
    public void Die()
    {
        en_state = State.inactive;
    }

    // These reset the player's ability to jump when they hit the floor and prevents double jumping
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") bl_isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Floor") bl_isGrounded = false;
    }

    // This turns player control on and off and handles mouse confinement
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
