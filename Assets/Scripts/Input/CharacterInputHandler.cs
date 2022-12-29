using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 movementInputVector = Vector2.zero;
    private Vector2 viewInputVector = Vector2.zero;
    private bool isJumpButtonPressed = false;
    LocalCameraHandler localCameraHandler;
    void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        movementInputVector.x = Input.GetAxis("Horizontal");
        movementInputVector.y = Input.GetAxis("Vertical");

        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y");

        if(Input.GetButtonDown("Jump")){
            isJumpButtonPressed = true;
        }
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;
        networkInputData.movementInput = movementInputVector;
        networkInputData.isJumpPressed = isJumpButtonPressed;
        isJumpButtonPressed = false;
        return networkInputData;
    }
}
