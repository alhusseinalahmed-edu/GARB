using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] PlayerController playerController;
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] InputHandler inputHandler;

    public bool isCamRecoilActive = false;
    [Header("Camera Shit")]
    public float rotationSpeed = 6f;
    public float returnSpeed = 25f;
    public bool aiming = false;
    private Vector3 currentRotation;
    private Vector3 Rot;

    [Header("Weapon Shit")]
    [SerializeField] Transform recoilPosition;
    [SerializeField] Transform rotationPoint;
    [SerializeField] Transform cameraHolder;
    [Header("Speed Settings:")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 weaponRot;

    private void FixedUpdate()
    {

        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalRecoilSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        weaponRot = Vector3.Slerp(weaponRot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(weaponRot);

    }
    public void Fire()
    {
        Vector3 RecoilRotationAiming = weaponHandler.currentGun.RecoilRotationAim;
        Vector3 RecoilRotationAim = weaponHandler.currentGun.RecoilRotationAim;
        Vector3 RecoilKickBackAim = weaponHandler.currentGun.RecoilKickBackAim;
        Vector3 RecoilKickBack = weaponHandler.currentGun.RecoilKickBack;
        if (aiming)
        {
            currentRotation += new Vector3(-RecoilRotationAiming.x, Random.Range(-RecoilRotationAiming.y,
                RecoilRotationAiming.y), Random.Range(-RecoilRotationAiming.z, RecoilRotationAiming.z));
            rotationalRecoil += new Vector3(-RecoilRotationAim.x, Random.Range(-RecoilRotationAim.y, RecoilRotationAim.y), Random.Range(-RecoilRotationAim.z, RecoilRotationAim.z));
            positionalRecoil += new Vector3(Random.Range(-RecoilKickBackAim.x, RecoilKickBackAim.x), Random.Range(-RecoilKickBackAim.y, RecoilKickBackAim.y), RecoilKickBackAim.z);

        }
        else
        {
            positionalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
        }
    }
}
