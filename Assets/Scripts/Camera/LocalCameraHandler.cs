using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;
    Camera localCamera;
    float cameraRotationX = 0f;
    float cameraRotationY = 0f;
    Vector2 viewInput;
    CustomNetworkCharacterControllerPrototype customNetworkCharacterControllerPrototype;
    void Awake()
    {
        localCamera = GetComponent<Camera>();
        customNetworkCharacterControllerPrototype = GetComponentInParent<CustomNetworkCharacterControllerPrototype>();
    }

    void Start()
    {
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }
    }

    void LateUpdate()
    {
        if (cameraAnchorPoint == null)
        {
            return;
        }
        if (!localCamera.enabled)
        {
            return;
        }

        localCamera.transform.position = cameraAnchorPoint.position;
        cameraRotationX += viewInput.y * Time.deltaTime * customNetworkCharacterControllerPrototype.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        cameraRotationY += viewInput.x * Time.deltaTime * customNetworkCharacterControllerPrototype.rotationSpeed;
        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0f);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
