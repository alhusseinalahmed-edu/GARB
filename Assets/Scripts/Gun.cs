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
    public float bulletSpread;
    public float runningBulletSpread = 50;
    public float normalBulletSpread = 25;
    public int currentAmmo;
    public int ammoLeft = 120;

    [Header("Other")]
    public AudioClip shootSound;
    [Range(0f, 1f)]
    public float shootVolume;

    [Header("Recoil Settings:")]
    public Vector3 RecoilRotationWeapon = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(0.015f, 0f, -0.2f);

}

public enum WeaponType
{
    Primary,
    Secondary,
}
