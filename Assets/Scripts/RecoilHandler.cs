using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] PlayerController playerController;
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] InputHandler inputHandler;

    [Header("Weapon Shit")]
    [SerializeField] Transform recoilPosition;
    [SerializeField] Transform rotationPoint;
    [Header("Speed Settings:")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;
    public float rotationalReturnSpeed = 38f;

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 weaponRot;

    public Transform cam;


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
        Vector3 RecoilKickBack = weaponHandler.currentGun.RecoilKickBack;
        positionalRecoil += new Vector3(Random.Range(-RecoilKickBack.x, RecoilKickBack.x), Random.Range(-RecoilKickBack.y, RecoilKickBack.y), RecoilKickBack.z);
        GetComponentInChildren<CameraRecoil>().Fire();
    }
}
