using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="FPS/New gun")]
public  class Gun : ScriptableObject
{

    [Header("References")]
    public GameObject muzzleFlash;
    public AudioClip[] weaponSounds;
    [Range(0f, 1f)]
    public float weaponVolume;

    [Header("Weapon Info")]
    public WeaponType weaponType;
    public string name;

    [Header("Weapon Stats")]
    public int damage;
    public int pellets;
    public float fireRate = 0.2f;
    public float weaponRange = 100f;
    public float movementSpeedMultiplier;

    [Header("Weapon Ammo Settings")]
    public int ammoPerMag = 30;
    public float reloadDuration = 2;
    public int currentAmmo;
    public int ammoLeft = 120;
    public int startingAmmo;


    [Header("Weapon Spread")]
    public float bulletSpread;
    public float runningBulletSpread = 50;
    public float normalBulletSpread = 25;


    [Header("Recoil Settings:")]
    public Vector3 RecoilRotationWeapon = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
    public float recoilPower = 200;

    [Header("Viemodel Settings:")]
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 ADS_Position;
    public Quaternion ADS_Rotation;
    public float zoomFOV = 40f;

}

public enum WeaponType
{
    Primary,
    Secondary,
    Knife,
    Sniper,
}
