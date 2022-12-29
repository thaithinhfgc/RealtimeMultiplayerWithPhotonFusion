using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterMovementHandler : NetworkBehaviour
{
    private CustomNetworkCharacterControllerPrototype characterController;
    private Camera localCamera;
    void Awake()
    {
        characterController = GetComponent<CustomNetworkCharacterControllerPrototype>();
        localCamera = GetComponentInChildren<Camera>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            transform.forward = networkInputData.aimForwardVector;
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;
            
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            characterController.Move(moveDirection);
            if (networkInputData.isJumpPressed)
            {
                characterController.Jump();
            }
            CheckFallRespawned();
        }
    }

    void CheckFallRespawned()
    {
        if (transform.position.y < -22)
        {
            transform.position = Utils.GetRandomSpanwPosition();
        }
    }
}
