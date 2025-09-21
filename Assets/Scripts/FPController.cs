using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Input (assign from .inputactions)")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;
    public InputActionReference crouchAction;
    public InputActionReference zoomAction;

    [Header("Refs")]
    public Camera playerCamera;
    public Footsteps footsteps;

    [Header("Move")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.5f;
    public float jumpPower = 5f;
    public float gravity = -14f;

    [Header("Look")]
    public float lookSensitivity = 120f;
    public float pitchClamp = 80f;

    [Header("Crouch (hold Ctrl)")]
    public float crouchHeight = 1.2f;
    public float standHeight = 1.8f;
    public float crouchLerp = 12f;
    public float crouchSpeedMultiplier = 0.6f;

    [Header("Zoom (optional)")]
    public float normalFOV = 60f;
    public float zoomFOV = 40f;
    public float zoomLerp = 10f;

    [Header("Internal Constants")]
    public float groundedYVel = -1f;
    public float minStandHeight = 1.2f;
    public float minCrouchHeight = 0.9f;
    public float crouchClearance = 0.2f;
    public float centerYOffsetFactor = 0.5f;
    public float pitchZero = 0f;
    public float rotationX = 0f;
    public float rotationZ = 0f;
    public float crouchLerpFactorBase = 1f;
    public float zoomLerpFactorBase = 1f;

    CharacterController cc;
    float yVel;
    float pitch;
    float targetHeight;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!playerCamera) playerCamera = GetComponentInChildren<Camera>();
        if (!footsteps) footsteps = GetComponent<Footsteps>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera) playerCamera.fieldOfView = normalFOV;

        standHeight = Mathf.Max(standHeight, minStandHeight);
        crouchHeight = Mathf.Clamp(crouchHeight, minCrouchHeight, standHeight - crouchClearance);

        cc.height = standHeight;
        cc.center = new Vector3(cc.center.x, standHeight * centerYOffsetFactor, cc.center.z);
        targetHeight = standHeight;
    }

    void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();
        jumpAction?.action.Enable();
        sprintAction?.action.Enable();
        crouchAction?.action.Enable();
        zoomAction?.action.Enable();
    }

    void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();
        jumpAction?.action.Disable();
        sprintAction?.action.Disable();
        crouchAction?.action.Disable();
        zoomAction?.action.Disable();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        Vector2 look = lookAction ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
        transform.Rotate(rotationX, look.x * lookSensitivity * dt, rotationZ);
        pitch = Mathf.Clamp(pitch - look.y * lookSensitivity * dt, -pitchClamp, pitchClamp);
        if (playerCamera) playerCamera.transform.localRotation = Quaternion.Euler(pitch, pitchZero, pitchZero);

        bool isCrouching =
            (crouchAction && crouchAction.action.IsPressed()) ||
            (Keyboard.current != null && (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed));

        Vector2 move = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector3 wish = (transform.right * move.x + transform.forward * move.y).normalized;

        float speed = (sprintAction && sprintAction.action.IsPressed()) ? sprintSpeed : walkSpeed;
        if (isCrouching) speed *= crouchSpeedMultiplier;

        if (cc.isGrounded)
        {
            yVel = groundedYVel;
            if (jumpAction && jumpAction.action.WasPressedThisFrame() && !isCrouching)
            {
                yVel = jumpPower;
                footsteps?.PlayJump();
            }
        }
        else
        {
            yVel += gravity * dt;
        }

        cc.Move((wish * speed + Vector3.up * yVel) * dt);

        targetHeight = isCrouching ? crouchHeight : standHeight;
        float crouchT = crouchLerpFactorBase - Mathf.Exp(-crouchLerp * dt);
        cc.height = Mathf.Lerp(cc.height, targetHeight, crouchT);
        Vector3 c = cc.center;
        c.y = Mathf.Lerp(c.y, cc.height * centerYOffsetFactor, crouchT);
        cc.center = c;

        if (playerCamera && zoomAction)
        {
            bool zooming = zoomAction.action.IsPressed();
            float targetFov = zooming ? zoomFOV : normalFOV;
            float zoomT = zoomLerpFactorBase - Mathf.Exp(-zoomLerp * dt);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, zoomT);
        }
    }
}
