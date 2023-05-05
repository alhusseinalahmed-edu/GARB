using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="FPS/New gun")]
public  class Gun : ScriptableObject
{

    [Header("Prefabs")]
    public GameObject muzzleFlash;

    [Header("Weapon Stats")]
    public WeaponType weaponType;
    public int damage;
    public string name;
    public float fireRate = 0.2f;
    public int pellets;
    public float weaponRange = 100f;
    public int ammoPerMag = 30;
    public float reloadDuration = 2;
    public int currentAmmo;
    public int ammoLeft = 120;
    public int startingAmmo;
    public float zoomFOV = 40f;
    [Header("Other")]
    public AudioClip[] weaponSounds;
    [Range(0f, 1f)]
    public float weaponVolume;

    [Header("Recoil Settings:")]
    public Vector3 RecoilRotationWeapon = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);
    public float recoilPower = 200;
    public float bulletSpread;
    public float runningBulletSpread = 50;
    public float normalBulletSpread = 25;

    [Header("Viemodel Settings")]
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 ADS_Position;
    public Quaternion ADS_Rotation;

}

public enum WeaponType
{
    Primary,
    Secondary,
    Knife,
    Sniper,
}
