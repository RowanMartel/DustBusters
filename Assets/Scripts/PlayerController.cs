using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    protected MenuManager menuManager;

    // Audio related variables
    AudioSource as_source;
    public AudioClip ac_jump;
    public AudioClip ac_land;

    // GameObjects related to player's ability to hold props
    protected GameObject go_lookingAtObject;
    protected GameObject go_heldPosition;
    protected GameObject go_heldObject;
    public float flt_heldObjDistFromWall;
    protected GameObject go_heldPosContainer;

    public GameObject Go_heldObject {  get { return go_heldObject; } }

    // Reference to the camera and ability to look around
    protected GameObject go_cameraContainer;
    public CinemachineVirtualCamera vc_playerCamera;

    // Default position for where the player holds objects
    protected Vector3 v3_heldPositionReset;

    protected Rigidbody rb_player;

    public Collider[] a_col_playerColliders;

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
    public LayerMask lookMask;

    public GameObject go_curRegion;

    // Stair-related variables
    public bool bl_onStairs;
    public Transform tr_footOrigin;
    public LayerMask lm_stairsRay;

    public Transform tr_doorPusherTrans;

    private void Awake()
    {
        GameManager.playerController = this;
    }

    void Start()
    {
        // Grabbing required references to objects and systems
        menuManager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        menuManager.GamePaused += OnPause;
        menuManager.GameUnpaused += OnUnpause;

        go_lookingAtObject = GameObject.Find("Floor");
        go_heldPosition = GameObject.Find("HeldPosition");
        v3_heldPositionReset = go_heldPosition.transform.localPosition;

        rb_player = GetComponent<Rigidbody>();
        go_cameraContainer = GameObject.Find("Player/CameraContainer");
        go_heldPosContainer = GameObject.Find("Player/HeldPosContainer");

        as_source = GetComponent<AudioSource>();

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "ChrisTestScene") TogglePlayerControl();

        
    }

    // Update is called once per frame
    void Update()
    {
        // This calls the method to fire the raycast that tells us what the player is looking at
        GetLookedAtObject();

        // This sends required info to the tooltip
        menuManager.UpdateTooltip(go_lookingAtObject, go_heldObject);

        // Player's ability to interact
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && En_state == State.active) Interact();

        if(!GameManager.menuManager.Bl_paused && GameManager.menuManager.Bl_allowPause) if (Input.GetKeyDown(KeyCode.C)) GameManager.menuManager.ToggleChoreSheet();

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
                GameManager.soundManager.PlayClip(ac_jump, as_source, true);
            }

            // Handles Crouch
            if (Input.GetKeyDown(KeyCode.LeftShift)) LeanTween.moveLocalY(go_cameraContainer, 0f, 0.25f);
            if (Input.GetKeyUp(KeyCode.LeftShift)) LeanTween.moveLocalY(go_cameraContainer, 1f, 0.25f);
            if (Input.GetKeyDown(KeyCode.LeftShift)) LeanTween.moveLocalY(go_heldPosContainer, 0f, 0.25f);
            if (Input.GetKeyUp(KeyCode.LeftShift)) LeanTween.moveLocalY(go_heldPosContainer, 1f, 0.25f);
        }

        // Toggles pause
        if (Input.GetKeyDown(KeyCode.Escape)) menuManager.TogglePause();

        // Toggles GUI
        if (Input.GetKeyDown(KeyCode.F1)) menuManager.ToggleGUI();

        // This forces the player to drop a prop if it gets too far away from them
        if (go_heldObject != null)
        {
            if (En_state == State.inactive) return;

            Vector3 v3_propDirection = go_heldObject.transform.position - transform.position;
            float flt_propDistance = v3_propDirection.magnitude;

            if (flt_propDistance > 3f)
            {
                DropItem();
            }
        }
    }
    void FixedUpdate()
    {
        // This handles a held objects position in front of player while player is active
        if (go_heldObject != null) // && en_state == State.active)
        {
            Pickupable pu_pickup = go_heldObject.GetComponent<Pickupable>();            

            Vector3 v3_modifiedHeldPosition = go_heldPosition.transform.TransformPoint(go_heldPosition.transform.localPosition.x + pu_pickup.v3_heldPositionMod.x, go_heldPosition.transform.localPosition.y /*- 0.5f*/ + pu_pickup.v3_heldPositionMod.y, go_heldPosition.transform.localPosition.z - 1 + pu_pickup.v3_heldPositionMod.z);

            //Casts a ray to ensure that the object isn't being shoved into a wall
            RaycastHit hit;
            Debug.DrawRay(go_heldPosContainer.transform.position, v3_modifiedHeldPosition - go_heldPosContainer.transform.position, Color.red);
            if(Physics.Raycast(go_heldPosContainer.transform.position, v3_modifiedHeldPosition - go_heldPosContainer.transform.position, out hit, Vector3.Distance(go_heldPosContainer.transform.position, v3_modifiedHeldPosition), lookMask))
            {
                if (hit.collider != null)
                {
                    // Debug.Log(hit.collider.gameObject.name);
                    v3_modifiedHeldPosition = hit.point - ((hit.point - go_heldPosContainer.transform.position) * flt_heldObjDistFromWall);
                }
            }

            Vector3 direction = go_heldObject.transform.position - v3_modifiedHeldPosition;
            float distance = direction.magnitude;
            Vector3 force = direction.normalized;

            //Positions object appropriately
            if (distance > 0) go_heldObject.GetComponent<Rigidbody>().AddForce(-force * distance * 10, ForceMode.VelocityChange);
            go_heldObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if(!pu_pickup.bl_doorKnob) go_heldObject.transform.rotation = go_heldPosition.transform.rotation;
            go_heldObject.transform.Rotate(pu_pickup.v3_heldRotationMod);

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

    // This drops the held item
    void DropItem()
    {
        go_heldObject.layer = go_heldObject.GetComponent<Pickupable>().int_startingLayer;
        go_heldObject.GetComponent<Rigidbody>().useGravity = true;
        Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>(), false);
        go_heldObject.GetComponent<Pickupable>().Drop();
        go_heldObject = null;
    }

    // This picksup an item
    void PickUpItem(GameObject go_item)
    {
        go_heldObject = go_item;
        Physics.IgnoreCollision(go_heldObject.GetComponent<Collider>(), GetComponent<Collider>());

        go_heldObject.GetComponent<Rigidbody>().useGravity = false;
        go_heldObject.GetComponent<Outline>().enabled = false;

        int layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        go_heldObject.layer = layerIgnoreRaycast;
        if (go_heldObject == GameManager.ghost.go_curHeldItem)
        {
            GameManager.ghost.GetRobbed();
        }
        go_heldObject.GetComponent<Pickupable>().Interact();
    }


    // This handles the player's ability to look up and down, and rotate the player.
    void MoveCamera()
    {
        float camV = flt_cameraVertical + Input.GetAxis("Mouse Y");

        flt_cameraVertical = Mathf.Clamp(camV, -90f / Settings.flt_lookSensitivity, 80f / Settings.flt_lookSensitivity);

        float flipCamV = camV * -1;

        go_cameraContainer.transform.localRotation = Quaternion.Euler(flipCamV * Settings.flt_lookSensitivity, 0, 0);
        go_heldPosContainer.transform.localRotation = Quaternion.Euler(flipCamV * Settings.flt_lookSensitivity, 0, 0);

        flt_playerRotate = Input.GetAxis("Mouse X");

        transform.Rotate(0.0f, flt_playerRotate * Settings.flt_lookSensitivity, 0.0f);
    }

    // This forces the player to look at a particular point in world space, available for locking onto chores
    public void LookAt(Vector3 position)
    {
        LeanTween.rotateLocal(go_cameraContainer, position, 0.25f);
        LeanTween.rotateLocal(go_heldPosContainer, position, 0.25f);
    }

    // This handles the player's basic forward/backward/left/right movement, mapped to the WASD keys.
    void DoPlayerMovement()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // if pressing forwards but not left or right,
        {
            if (bl_onStairs) // check if on stairs
            {
                // if so, raycast forwards from the bottom of the player, hitting only the stairs layer
                Physics.Raycast(tr_footOrigin.position, transform.forward, out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null) // if that raycast hits then the player must be ascending the stairs
                    rb_player.AddForce(RotateVector(transform.forward * Settings.int_playerSpeed, transform.right, -30)); // propel the player at a forwards/upwards angle
                else // if the raycast didn't hit,
                {
                    // raycast *backwards* from the bottom of the player
                    Physics.Raycast(tr_footOrigin.position, -transform.forward, out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    //Debug.DrawRay(tr_footOrigin.position, -transform.forward * 5, Color.red, 5); // debug code
                    if (hit2.collider != null) // if this one hits then the player must be descending the stairs
                        rb_player.AddForce(RotateVector(transform.forward * Settings.int_playerSpeed / 2, transform.right, 30)); // propel the player at a forwards/downwards angle
                    else // if neither raycast hit then the player must be moving sideways on the stairs
                        rb_player.AddForce(transform.forward * Settings.int_playerSpeed); // push the player with a forwards force
                }
            }
            else // if the player isn't on the stairs, push them with a forwards force
                rb_player.AddForce(transform.forward * Settings.int_playerSpeed);

            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) // if pressing forwards and right,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(transform.forward, transform.right, .5f), out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                {
                    rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * transform.forward, transform.right, -30));
                    rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * transform.right, transform.forward, 30));

                }
                else
                {
                    Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(-transform.forward, transform.right, .5f), out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                    {
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed / 2 * transform.forward, transform.right, 30));
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed / 2 * transform.right, transform.forward, -30));

                    }
                    else
                    {
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.forward);
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.right);
                    }
                }
            }
            else
            {
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.forward);
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.right);
            }
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 45, 0);
        }

        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) // if pressing right but not forwards or backwards,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, transform.right, out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                    rb_player.AddForce(RotateVector(transform.right * Settings.int_playerSpeed, transform.forward, 30));
                else
                {
                    Physics.Raycast(tr_footOrigin.position, -transform.right, out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                        rb_player.AddForce(RotateVector(transform.right * Settings.int_playerSpeed / 2, transform.forward, -30));
                    else
                    {
                        rb_player.AddForce(transform.right * Settings.int_playerSpeed);
                    }
                }
            }
            else
                rb_player.AddForce(transform.right * Settings.int_playerSpeed);
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 90, 0);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) // if pressing backwards and right,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(-transform.forward, transform.right, .5f), out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                {
                    rb_player.AddForce(RotateVector(0.5f * Settings.int_playerSpeed * -transform.forward, transform.right, 30));
                    rb_player.AddForce(RotateVector(0.5f * Settings.int_playerSpeed * transform.right, transform.forward, 30));
                }
                else
                {
                    Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(transform.forward, -transform.right, .5f), out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                    {
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * -transform.forward, transform.right, -30));
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * transform.right, transform.forward, -30));
                    }
                    else
                    {
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.forward);
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.right);
                    }
                }
            }
            else
            {
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.forward);
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.right);
            }
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 135, 0);
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) // if pressing backwards but not left or right,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, -transform.forward, out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                    rb_player.AddForce(RotateVector(-transform.forward * Settings.int_playerSpeed, transform.right, 30));
                else
                {
                    Physics.Raycast(tr_footOrigin.position, transform.forward, out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                        rb_player.AddForce(RotateVector(-transform.forward * Settings.int_playerSpeed / 2, transform.right, -30));
                    else
                    {
                        rb_player.AddForce(-transform.forward * Settings.int_playerSpeed);
                    }
                }
            }
            else
                rb_player.AddForce(-transform.forward * Settings.int_playerSpeed);
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 180, 0);
        }

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) // if pressing forwards and left,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(-transform.forward, -transform.right, .5f), out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                {
                    rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * -transform.forward, transform.right, 30));
                    rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * -transform.right, transform.forward, -30));

                }
                else
                {
                    Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(transform.forward, transform.right, .5f), out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                    {
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed / 2 * -transform.forward, transform.right, -30));
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed / 2 * -transform.right, transform.forward, 30));

                    }
                    else
                    {
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.forward);
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.right);
                    }
                }
            }
            else
            {
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.forward);
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.right);
            }
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 225, 0);
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) // if pressing left but not forwards or backwards,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, -transform.right, out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                    rb_player.AddForce(RotateVector(-transform.right * Settings.int_playerSpeed, transform.forward, -30));
                else
                {
                    Physics.Raycast(tr_footOrigin.position, transform.right, out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                        rb_player.AddForce(RotateVector(-transform.right * Settings.int_playerSpeed / 2, transform.forward, 30));
                    else
                    {
                        rb_player.AddForce(-transform.right * Settings.int_playerSpeed);
                    }
                }
            }
            else
                rb_player.AddForce(-transform.right * Settings.int_playerSpeed);
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 270, 0);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) // if pressing forwards and left,
        {
            if (bl_onStairs)
            {
                Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(transform.forward, -transform.right, .5f), out RaycastHit hit, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                if (hit.collider != null)
                {
                    rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * transform.forward, transform.right, -30));
                    rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed * -transform.right, transform.forward, -30));

                }
                else
                {
                    Physics.Raycast(tr_footOrigin.position, Vector3.Lerp(-transform.forward, transform.right, .5f), out RaycastHit hit2, 2, lm_stairsRay, QueryTriggerInteraction.Collide);
                    if (hit2.collider != null)
                    {
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed / 2 * transform.forward, transform.right, 30));
                        rb_player.AddForce(RotateVector(0.75f * Settings.int_playerSpeed / 2 * -transform.right, transform.forward, 30));

                    }
                    else
                    {
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.forward);
                        rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.right);
                    }
                }
            }
            else
            {
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * transform.forward);
                rb_player.AddForce(0.75f * Settings.int_playerSpeed * -transform.right);
            }
            tr_doorPusherTrans.localEulerAngles = new Vector3(0, 315, 0);
        }
    }

    //rotates force vectors by the given angle around the specified axis
    Vector3 RotateVector(Vector3 vector, Vector3 axis, float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
        return rotation * vector;
    }

    // This handles the player's view at the crosshair and if pointed at an Interactable object, will activate the object's outline to indicate it can be interacted with.
    void GetLookedAtObject()
    {
        if (En_state == State.inactive) return;

        ray_playerView = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));

        if (Physics.Raycast(ray_playerView, out RaycastHit hit, 5, lookMask))
        {
            if (hit.collider.gameObject != go_lookingAtObject)
            {
                if (go_lookingAtObject != null && go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;

                go_lookingAtObject = hit.collider.gameObject;

                if (go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = true;
            }
        }
        if (!Physics.Raycast(ray_playerView, out _, 3, lm) && go_lookingAtObject != null)
        {
            if (go_lookingAtObject.CompareTag("Interactable")) go_lookingAtObject.GetComponent<Outline>().enabled = false;
            go_lookingAtObject = null;
        }
    }

    // Handles the player's ability to extend where the held prop is positioned, like reaching out in front of them
    void DoPlayerReach()
    {
        if (go_heldPosition.transform.localPosition.z >= 0.8f && go_heldPosition.transform.localPosition.z <= 1.6f)
        {
            go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, go_heldPosition.transform.localPosition.z + Input.mouseScrollDelta.y * 0.1f);

            if (go_heldPosition.transform.localPosition.z > 1.6f) go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 1.6f);
            if (go_heldPosition.transform.localPosition.z < 0.8f) go_heldPosition.transform.localPosition = new Vector3(go_heldPosition.transform.localPosition.x, go_heldPosition.transform.localPosition.y, 0.8f);
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
                PickUpItem(go_lookingAtObject);
                return;
            }

            go_lookingAtObject.GetComponent<Interactable>().Interact();
        }
        else if(go_heldObject != null && (go_lookingAtObject == null || go_lookingAtObject.tag != "Interactable"))
        {
            //Drop held item
            DropItem();
        }
        else if (go_heldObject != null && go_lookingAtObject.CompareTag("Interactable"))
        {
            //Interact with object while holding another object
            Pickupable pickupable = go_lookingAtObject.GetComponent<Pickupable>();

            if (pickupable != null)
            {
                // if the looked at object is an unlit candle, and the player is holding the lighter:
                if(go_lookingAtObject.GetComponent<Candle>() && !go_lookingAtObject.GetComponent<Candle>().bl_lit && go_heldObject.GetComponent<Pickupable>().bl_lighter)
                {
                    go_lookingAtObject.GetComponent<Interactable>().Interact();
                }
                else
                {
                    //Drop old item
                    DropItem();

                    //Pick up new item
                    PickUpItem(go_lookingAtObject);
                    return;
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
        if (collision.gameObject.CompareTag("Floor"))
        {
            bl_isGrounded = true;
        }
        if (collision.gameObject.layer == 15)
            bl_onStairs = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (!bl_isGrounded && collision.relativeVelocity.y > 2)
            {
                GameManager.soundManager.PlayClip(ac_land, as_source, true);
                Debug.Log("y vel: " + collision.relativeVelocity.y);
            }
        }
    }

    //Player loses ground when they leave the ground
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor")) bl_isGrounded = false;

        if (collision.gameObject.layer == 15)
            bl_onStairs = false;
    }

    //Disable player colliders and rb
    public void DisablePhysics()
    {
        foreach (Collider col in a_col_playerColliders)
        {
            col.enabled = false;
        }
        rb_player.useGravity = false;
    }

    //Enable player colliders and rb
    public void EnablePhysics()
    {
        foreach (Collider col in a_col_playerColliders)
        {
            col.enabled = true;
        }
        rb_player.useGravity = true;
    }

    // This turns player control on and off and handles mouse confinement
    public void TogglePlayerControl()
    {
        switch(en_state)
        {
            // turning inactive from active
            case State.active:
                en_state = State.inactive;
                Cursor.lockState = CursorLockMode.Confined;
                if (Go_heldObject != null && !GameManager.menuManager.Bl_paused && !GameManager.menuManager.bl_choreListUp)
                {
                    Go_heldObject.GetComponent<Renderer>().enabled = false;
                    Go_heldObject.GetComponent<Rigidbody>().Sleep();
                }
                break;
            // turning active from inactive
            case State.inactive:
                if (!GameManager.Bl_inCleaningGame)
                {
                    en_state = State.active;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                if (Go_heldObject != null && !GameManager.menuManager.Bl_paused && !GameManager.menuManager.bl_choreListUp && !GameManager.Bl_inCleaningGame)
                {
                    Go_heldObject.GetComponent<Renderer>().enabled = true;
                    Go_heldObject.GetComponent<Rigidbody>().Sleep();
                }
                break;
        }
    }

    //These are tied to the MenuManager's pause and unpause events
    void OnPause(object source, EventArgs e)
    {
        if(en_state == State.active) TogglePlayerControl();
    }
    void OnUnpause(object source, EventArgs e)
    {
        if(!GameManager.menuManager.bl_choreListUp)TogglePlayerControl();
    }
}