using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool canMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey);
    private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouch && characterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canHeadbob = true;
    [SerializeField] private bool slideOnSlopes = true;
    [SerializeField] private bool useStamina = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Moving Variables")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeFallSpeed = 8f;
    [SerializeField] private float acceleration = 1.5f;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 30f;

    [Header("Crouch Variables")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 1.5f;
    [SerializeField] private float timeToCrouch = 0.3f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouch;

    [Header("Headbob Variables")]
    [SerializeField] private float walkHeadbobSpeed = 14f;
    [SerializeField] private float walkHeadbobAmount = 0.5f;
    [SerializeField] private float sprintHeadbobSpeed = 18f;
    [SerializeField] private float sprintHeadbobAmount = 1f;
    [SerializeField] private float crouchHeadbobSpeed = 8f;
    [SerializeField] private float crouchHeadbobAmount = 0.25f;
    private float defaultPosY = 0f;
    private float timer;

    [Header("Stamina Variables")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaUseSpeed = 5f;
    [SerializeField] private float timeUntilRegenStart = 3f;
    [SerializeField] private float staminaRegenTick = 2f;
    [SerializeField] private float staminaRegenSpeed = 0.1f;
    private float currentStamina;
    private Coroutine regeneratingStamina;
    public static Action<float> staminaChange;

    private Vector3 hitPointNormal;
    private bool isSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    public GameObject saveMenu;
    private bool inSavePoint;
    private bool isMenuActive;
    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        currentStamina = maxStamina;
        defaultPosY = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && inSavePoint)
        {
            // Toggle the menu's active state
            isMenuActive = !isMenuActive;

            // Activate/deactivate the save menu accordingly
            saveMenu.SetActive(isMenuActive);

            // Set cursor lock state and visibility based on menu state
            if (isMenuActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; // Lock the cursor when the menu is closed
                Cursor.visible = false;
            }
        }
        if (canMove)
        {       
            HandleMovementInput();

            if (canJump)
            {
                HandleJump();
            }

            if (canCrouch)
            {
                HandleCrouch();
            }

            if (canHeadbob)
            {
                HandleHeadbob();
            }

            if (useStamina)
            {
                HandleStamina();
            }

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        if (characterController.isGrounded)
        {
            float targetSpeed = isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;
            float targetInputX = targetSpeed * Input.GetAxis("Vertical");
            float targetInputY = targetSpeed * Input.GetAxis("Horizontal");

            currentInput.x = Mathf.MoveTowards(currentInput.x, targetInputX, Time.deltaTime * acceleration);
            currentInput.y = Mathf.MoveTowards(currentInput.y, targetInputY, Time.deltaTime * acceleration);

            float moveDirectionY = moveDirection.y;

            float cameraRotation = Camera.main.transform.eulerAngles.y * Mathf.Deg2Rad;

            Vector3 cameraForward = new Vector3(Mathf.Sin(cameraRotation), 0, Mathf.Cos(cameraRotation));

            moveDirection = cameraForward * currentInput.x + Camera.main.transform.right * currentInput.y;
            moveDirection.y = moveDirectionY;
        }
    }



    private void HandleJump()
    {
        if (!isCrouching)
        {
            if (shouldJump)
            {
                moveDirection.y = jumpForce;
            }
        }
    }

    private void HandleCrouch()
    {
        if (shouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void HandleHeadbob()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchHeadbobSpeed : isSprinting ? sprintHeadbobSpeed : walkHeadbobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultPosY + Mathf.Sin(timer) * (isCrouching ? crouchHeadbobAmount : isSprinting ? sprintHeadbobAmount : walkHeadbobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && currentInput != Vector2.zero && !isCrouching)
        {
            if (regeneratingStamina != null)
            {
                StopCoroutine(regeneratingStamina);
                regeneratingStamina = null;
            }
  
            currentStamina -= staminaUseSpeed * Time.deltaTime;

            if (currentStamina < 0)
            {
                currentStamina = 0;
            }

            staminaChange?.Invoke(currentStamina);

            if (currentStamina <= 0)
            {
                canSprint = false;
                canJump = false;
            }
        }
        if (!isSprinting && currentStamina < maxStamina && regeneratingStamina == null)
        {
            regeneratingStamina = StartCoroutine(RegenerateStamina());
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if ( slideOnSlopes && isSliding)
        {
            moveDirection = new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeFallSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {

        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouch = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouch = false;
    }

    private IEnumerator RegenerateStamina()
    {
        yield return new WaitForSeconds(timeUntilRegenStart);
        WaitForSeconds timeToWait = new WaitForSeconds(staminaRegenTick);

        while (currentStamina < maxStamina)
        {
            if (currentStamina > 0)
            {
                canSprint = true;
                canJump = true;
            }

            currentStamina += staminaRegenSpeed;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            staminaChange?.Invoke(currentStamina);

            yield return timeToWait;
        }

        regeneratingStamina = null;

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("SavePoint"))
        {
            inSavePoint = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SavePoint"))
        {
            inSavePoint = false;
        }
    }
}
