using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 movementInputVector = Vector2.zero;
    private Vector2 viewInputVector = Vector2.zero;
    private bool isJumpButtonPressed = false;
    private bool isFireButtonPressed = false;
    LocalCameraHandler localCameraHandler;
    private CharacterMovementHandler characterMovementHandler;
    void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterMovementHandler.Object.HasInputAuthority)
        {
            return;
        }
        movementInputVector.x = Input.GetAxis("Horizontal");
        movementInputVector.y = Input.GetAxis("Vertical");

        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Jump"))
        {
            isJumpButtonPressed = true;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            isFireButtonPressed = true;
        }
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;
        networkInputData.movementInput = movementInputVector;
        networkInputData.isJumpPressed = isJumpButtonPressed;
        networkInputData.isFirePressed = isFireButtonPressed;
        isJumpButtonPressed = false;
        isFireButtonPressed = false;
        return networkInputData;
    }
}
