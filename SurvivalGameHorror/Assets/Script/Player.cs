using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed; // Speed of the player movement
    public float walkSpeed = 3;
    public float sprintSpeed = 5;
    public float staminaMax = 100f;
    public float staminaRegenRate = 10f;
    public float sprintStaminaCost = 20f;
    public float bobbingAmount = 0.05f;// Amount of head bobbing
    public float walkBobbingSpeed = 0.2f;
    public float sprintBobbingSpeed = 15;
    public int Health;
    public float mouseSensitivity = 100f; // Sensitivity of mouse movement
    public AudioClip[] footstepSounds; // Array of footstep sounds
    public float footstepInterval = 0.5f; // Interval between each footstep sound
    public Slider staminaBar;
    public Slider healthBar;
    public GameObject Menu;
    public GameObject saveMenu;
    public ShortcutManager ShortcutManager;

    private Vector3 originalCameraPosition;
    private bool canSprint;
    private bool isSprinting;
    private List<int> playedFootstepIndices = new List<int>();
    private Rigidbody rb;
    private Animator staminabarAnimator;
    private float verticalLookRotation;
    private AudioSource audioSource;
    private float lastFootstepTime;
    private bool isLeftFootstep;
    private Vector3 movement;
    private bool inSavePoint;
    public bool Paused;
    public float currentStamina;

    void Start()
    {
        originalCameraPosition = Camera.main.transform.localPosition;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        lastFootstepTime = Time.time; // Initialize last footstep time
        isLeftFootstep = true; // Start with left footstep
        currentStamina = staminaMax; // Set initial stamina to max
        staminabarAnimator = staminaBar.GetComponent<Animator>();
        Paused = false;
    }

    void Update()
    {
        if (Paused)
        {
            // If paused, reset the camera position to its original position
            Camera.main.transform.localPosition = originalCameraPosition;
            return; // Exit the method early
        }

        if (IsMoving())
        {
            // Calculate the current bobbing speed based on whether the player is sprinting or walking
            float currentBobbingSpeed = isSprinting ? sprintBobbingSpeed : walkBobbingSpeed;

            // Calculate the vertical position of the camera based on head bobbing
            float waveslice = Mathf.Sin(Time.time * currentBobbingSpeed);
            Vector3 newPosition = originalCameraPosition;
            newPosition.y += waveslice * bobbingAmount; // Add the bobbing effect to the y-position

            // Calculate the lateral sway
            float lateralWaveslice = Mathf.Cos(Time.time * currentBobbingSpeed);
            newPosition.x += lateralWaveslice * bobbingAmount / 2f; // Adjust sway intensity

            // Apply the new position to the camera
            Camera.main.transform.localPosition = newPosition;
        }
        else
        {
            // If not moving, reset the camera position to its original position
            Camera.main.transform.localPosition = originalCameraPosition;
        }

        if (Input.GetKeyDown(KeyCode.E) && inSavePoint)
        {
            Paused = !Paused;
            saveMenu.SetActive(Paused);
            Cursor.lockState = Paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Paused;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused; // Toggle pause state
            Cursor.lockState = Paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Paused;
            Menu.SetActive(Paused);
        }

        if (!Paused)
        {
            // Player Movement
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Sprinting
            if (canSprint)
            {
                isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina >= sprintStaminaCost && verticalInput > 0;
            }
            else
            {
                isSprinting = false;
            }

            // Set move speed based on whether sprinting or not
            float currentMoveSpeed = isSprinting ? sprintSpeed : walkSpeed;

            // Apply movement
            movement = new Vector3(horizontalInput, 0f, verticalInput) * currentMoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position + transform.TransformDirection(movement));

            // Adjust stamina
            if (isSprinting)
            {
                if (currentStamina > 21)
                {
                    footstepInterval = .45f;
                }
                currentStamina -= sprintStaminaCost * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, staminaMax);
            }
            else if (currentStamina < staminaMax)
            {
                footstepInterval = .65f;
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, staminaMax);
            }

            // Mouse Look
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;

            // Rotate the player horizontally (around Y-axis)
            transform.Rotate(Vector3.up * mouseX);

            // Rotate the camera vertically (around X-axis)
            verticalLookRotation -= mouseY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); // Limit vertical rotation to prevent flipping
            Camera.main.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

            // Play footstep sounds with interval and alternating between two footstep sounds
            if (movement != Vector3.zero && IsGrounded() && Time.time - lastFootstepTime > footstepInterval)
            {
                PlayFootstepSound();
                lastFootstepTime = Time.time; // Update last footstep time
            }
        }

        UpdateStaminaBar();
        UpdateHealthBar();
    }



    bool IsMoving()
    {
        // Check if the player is currently moving
        return Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
    }
    // Check if the player is grounded
    bool IsGrounded()
    {
        // You can implement your own grounded check here.
        // For simplicity, you can use a raycast or other methods to check if the player is grounded.
        return true;
    }
    void UpdateStaminaBar()
    {
        // Update the value of the stamina UI slider
        if (staminaBar != null)
        {
            staminaBar.value = currentStamina;
        }
        if (currentStamina <= 21)
        {
            staminaRegenRate = 5;
            staminabarAnimator.Play("StaminaEmty");
            canSprint = false;
        }
        if (currentStamina >= 99)
        {
            canSprint = true;
            staminaRegenRate = 10;
            staminabarAnimator.Play("StaminaFull");
        }
    }
    void UpdateHealthBar()
    {
        // Update the value of the stamina UI slider
        if (healthBar != null)
        {
            healthBar.value = Health;
        }
    }
    void PlayFootstepSound()
    {
        if (movement.magnitude > 0.05f)
        {
            if (playedFootstepIndices.Count == footstepSounds.Length)
            {
                playedFootstepIndices.Clear();
            }
            List<int> availableIndices = Enumerable.Range(0, footstepSounds.Length).ToList();
            availableIndices.RemoveAll(index => playedFootstepIndices.Contains(index));
            availableIndices = availableIndices.OrderBy(i => Random.value).ToList();
            int indexToPlay = availableIndices[0];
            playedFootstepIndices.Add(indexToPlay);
            AudioClip footstepSound = footstepSounds[indexToPlay];
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(footstepSound);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            ShortcutManager.AddItem(other.name);
            Destroy(other.gameObject);
        }
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
