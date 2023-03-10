using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }
    public ParticleSystem fireparticleSystem;
    float lastTimeFired = 0;
    public Transform aimPoint;
    public LayerMask collisionLayers;
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFirePressed)
            {
                Fire(networkInputData.aimForwardVector);
            }
        }
    }
    void Fire(Vector3 aimForwardVector)
    {
        if (Time.time - lastTimeFired < 0.15f)
        {
            return;
        }
        StartCoroutine(FireEffectCO());
        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100f, Object.InputAuthority, out var hitInfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitDistance > 0)
        {
            hitDistance = hitInfo.Distance;
        }

        if (hitInfo.Hitbox != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitInfo.Hitbox.transform.root.name}");
            isHitOtherPlayer = true;
        }
        else if (hitInfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit physx collider {hitInfo.Collider.transform.root.name}");
        }


        if (isHitOtherPlayer)
        {
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1f);
        }
        else
        {
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1f);
        }

        lastTimeFired = Time.time;

    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;
        //fireparticleSystem.Play();
        yield return new WaitForSeconds(0.09f);
        isFiring = false;
    }
    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        Debug.Log($"OnFireChanged: {changed.Behaviour.isFiring}");
        bool isFiringCurrent = changed.Behaviour.isFiring;
        changed.LoadOld();
        bool isFiringOld = changed.Behaviour.isFiring;
        if (isFiringCurrent && !isFiringOld)
        {
            changed.Behaviour.OnFireRemote();
        }
    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
        {
            fireparticleSystem.Play();
        }
    }
}
