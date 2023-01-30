using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class SC_FPSController : NetworkBehaviour
{


    [Header("Components")]
    public GameObject _fpsRoot;
    public GameObject _tpsRoot;
    public PlayerBody body;

    [Header("Movement Settings")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public bool Fly;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();


        characterController.enabled = false;
        GetComponent<NetworkTransform>().syncDirection = SyncDirection.ClientToServer;
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void SetLifeState(bool alive)
    {
        if (alive)
        {
            Fly = false;
        }
        else
        {
            Fly = true;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        HandleMovement();
        HandleShooting();
        HandleWeaponSwitching();
    }


    private void HandleShooting()
    {
        if (body.beat)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        body.equippedWep.Shoot(transform, transform.forward);
    }


    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
    }

    private float GetCurrentWeaponSpeedMultiplier()
    {
        if (body.equippedWep == null) return 1;

        return body.equippedWep.MovementMultiplier;
    }

    private void HandleMovement()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX * GetCurrentWeaponSpeedMultiplier()) + (right * curSpeedY * GetCurrentWeaponSpeedMultiplier());

        if (Input.GetKey(KeyCode.Space) && canMove && characterController.isGrounded)
        {
            characterController = gameObject.GetComponent<CharacterController>();
            moveDirection.y = jumpSpeed;
        }
        else
        {
            if (!Fly)
            {
                moveDirection.y = movementDirectionY;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftControl) && canMove)
                {
                    characterController = gameObject.GetComponent<CharacterController>();
                    moveDirection.y = -jumpSpeed;
                }
            }

        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            if (!Fly) { moveDirection.y -= gravity * Time.deltaTime; }
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}