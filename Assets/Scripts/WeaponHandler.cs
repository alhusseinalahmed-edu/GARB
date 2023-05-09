using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WeaponHandler : MonoBehaviour
{
    [Header("Refs")]
    public AnimatorHandler animatorHandler;
    public PlayerController playerController;
    public RecoilHandler recoilHandler;
    public TMP_Text ammoText;
    public Image crosshair;
    public Camera cam;
    public InputHandler inputHandler;
    [SerializeField] AudioSource weaponSource;
    public GameObject[] impacts;
    public PhotonView PV;
    public ObjectPool objectPool;
    public ObjectPool muzzleFlashesPool;

    public GameObject[] iconList;

    [HideInInspector] public Animator weaponAnimator;

    [Header("Guns")]
    public GameObject[] fpsGuns;
    public GameObject[] tpsGuns;
    public Gun[] guns;

    float nextShootTimer;

    [HideInInspector] public Gun currentGun;
    int previousItemIndex = -1;
    int gunIndex;
    Transform currentGunTranform;


    public bool isReloading = false;

    public float zoomFOV = 40.0f;
    public float smoothTime = 0.1f;

    private float originalFOV;
    private float targetFOV = 0;
    private float currentVelocity = 0.0f;

    private void Start()
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_Equip", RpcTarget.All, 0);
            // Set the starting field of view
            originalFOV = cam.fieldOfView;
            foreach (Gun gun in guns)
            {
                gun.currentAmmo = gun.ammoPerMag;
                gun.ammoLeft = gun.startingAmmo;
            }
            ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;

        }
    }
    private void Update()
    {
        if (targetFOV != 0)
        {
            cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref currentVelocity, smoothTime);
        }
    }
    public void AimDownSights()
    {
        if (currentGunTranform == null) return;
        if (currentGun.weaponType == WeaponType.Knife) return;
        if (inputHandler.isAiming)
        {
            inputHandler.isAiming = false;
            currentGunTranform.localPosition = currentGun.Position;
            currentGunTranform.localRotation = currentGun.Rotation;

        }
        else if (!inputHandler.isAiming)
        {
            inputHandler.isAiming = true;
            currentGunTranform.localPosition = currentGun.ADS_Position;
            currentGunTranform.localRotation = currentGun.ADS_Rotation;
        }
        zoomFOV = currentGun.zoomFOV;
        targetFOV = inputHandler.isAiming ? zoomFOV : originalFOV;
    }
    public void Equip(int _index)
    {
        if (inputHandler.isAiming)
        {
            AimDownSights();
        }
        PV.RPC("RPC_Equip", RpcTarget.All, _index);
    }
    [PunRPC]
    private void RPC_Equip(int _index)
    {
        if (_index == previousItemIndex) return;
        gunIndex = _index;
        currentGun = guns[gunIndex];
        if (currentGun.weaponType == WeaponType.Primary)
        {
            animatorHandler.UpdateAnimatorFloat("gunIndex", 0f);
        }
        else if (currentGun.weaponType == WeaponType.Secondary)
        {
            animatorHandler.UpdateAnimatorFloat("gunIndex", 1f);
        }
        else if (currentGun.weaponType == WeaponType.Knife)
        {
            animatorHandler.UpdateAnimatorFloat("gunIndex", 2f);

        }
        previousItemIndex = gunIndex;
        inputHandler.isAiming = false;
        ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;
        foreach (GameObject go in tpsGuns)
        {
            if (go.transform.name != currentGun.name + "_TP")
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);
            }
        }
        if (PV.IsMine)
        {
            playerController.maxVelocity = playerController.defaultVelocity * currentGun.movementSpeedMultiplier;

            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name != currentGun.name)
                {
                    go.SetActive(false);
                }
                else
                {
                    go.SetActive(true);
                    weaponAnimator = go.transform.GetComponent<Animator>();
                    currentGunTranform = go.transform;
                    currentGunTranform.localPosition = currentGun.Position;
                    currentGunTranform.localRotation = currentGun.Rotation;
                }
            }
            // Change icon
            foreach (GameObject icon in iconList)
            {
                if (icon.name == currentGun.name + " Icon")
                {
                    icon.SetActive(true);

                }
                else
                {
                    icon.SetActive(false);
                }
            }

        }
    }
    public void Shoot()
    {
        if (nextShootTimer > Time.time) return;
        if (currentGun.currentAmmo <= 0)
        {
            PV.RPC("RPC_PlayWeaponSound", RpcTarget.All, "Out Of Ammo");
            nextShootTimer = Time.time + currentGun.fireRate * 2;
            return;
        }
        if (PV.IsMine)
        {
            if (weaponAnimator != null)
            {
                weaponAnimator.CrossFadeInFixedTime("Shoot", 0.01f);

            }
            else
            {
                foreach (GameObject go in fpsGuns)
                {
                    if (go.transform.name == currentGun.name)
                    {
                        weaponAnimator = go.transform.GetComponent<Animator>();
                        weaponAnimator.CrossFadeInFixedTime("Shoot", 0.01f);
                    }
                }
            }
        }
        if (currentGun.weaponType != WeaponType.Knife)
        {

            HandleBulletSpread();
            RaycastHit hit;
            for (int i = 0; i < Mathf.Max(1, currentGun.pellets); i++)
            {
                Vector3 t_spread = cam.transform.position + cam.transform.forward * 1000f;
                t_spread += Random.Range(-currentGun.bulletSpread, currentGun.bulletSpread) * cam.transform.up;
                t_spread += Random.Range(-currentGun.bulletSpread, currentGun.bulletSpread) * cam.transform.right;
                t_spread -= cam.transform.position;
                t_spread.Normalize();

                if (Physics.Raycast(cam.transform.position, t_spread, out hit, currentGun.weaponRange))
                {
                    Vector3 hitNormal = hit.normal;
                    if (hit.collider.gameObject.GetComponent<PlayerController>())
                    { 
                        hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(currentGun.damage);
                        playerController.PlayHitSound();
                        PV.RPC("RPC_SpawnDecal", RpcTarget.All, hit.point, hitNormal, 2);
                    }
                    else if(hit.collider.gameObject.GetComponent<Rigidbody>())
                    {
                        hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(hit.transform.forward * 20f, ForceMode.Impulse);
                    }
                    else
                    {
                        PV.RPC("RPC_SpawnDecal", RpcTarget.All, hit.point, hitNormal, 0);
                    }
                }
            }
            // Weapon Kickback

            recoilHandler.Fire(inputHandler.isAiming);
            // Camera Recoil (Actual Recoil)

            currentGun.currentAmmo--;
            ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;
            PV.RPC("RPC_MuzzleFlash", RpcTarget.All);
            SpawnLocalEffects();
        }
        else if (currentGun.weaponType == WeaponType.Knife)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, transform.forward, out hit, currentGun.weaponRange))
            {
                Vector3 hitNormal = hit.normal;
                if (hit.collider.gameObject.GetComponent<PlayerController>())
                { // Friendly Fire
                    hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(currentGun.damage);
                    playerController.PlayHitSound();
                }
                else
                {
                    PV.RPC("RPC_SpawnDecal", RpcTarget.All, hit.point, hitNormal, 0);
                }
            }
        }
        PV.RPC("RPC_PlayWeaponSound", RpcTarget.All, currentGun.name + " Shoot");
        animatorHandler.CrossFadeInFixedTime("Shoot", 0.01f);
        nextShootTimer = Time.time + currentGun.fireRate;
    }
    void SpawnLocalEffects()
    {
        Transform Muzzle = currentGunTranform.Find("Muzzle");

        if (Muzzle)
        {
            GameObject muzzleFlash = objectPool.GetPooledObject(1);
            if (muzzleFlash != null)
            {
                muzzleFlash.transform.parent = Muzzle.transform;
                muzzleFlash.transform.position = Muzzle.position;
                muzzleFlash.transform.rotation = Muzzle.rotation;
                muzzleFlash.SetActive(true);
            }
            StartCoroutine(DisableDecals(muzzleFlash));
        }
    }

    [PunRPC]
    void RPC_MuzzleFlash()
    {
        GameObject currentGun_TP = null;
        foreach (GameObject go in tpsGuns)
        {
            if (go.transform.name == currentGun.name + "_TP")
            {
                currentGun_TP = go;
            }
        }
        GameObject muzzleFlash = objectPool.GetPooledObject(1);
        if (muzzleFlash != null)
        {
            muzzleFlash.transform.parent = currentGun_TP.transform;
            muzzleFlash.transform.position = currentGun_TP.transform.Find("MuzzleFlash").position;
            muzzleFlash.transform.rotation = currentGun_TP.transform.Find("MuzzleFlash").rotation;
            muzzleFlash.SetActive(true);
        }
        StartCoroutine(DisableDecals(muzzleFlash));

    }
    public void Reload()
    {
        PV.RPC("RPC_Reload", RpcTarget.All);
    }
    [PunRPC]
    IEnumerator RPC_Reload()
    {
        if (PV.IsMine)
        {
            if (currentGun.ammoLeft > 0)
            {
                isReloading = true;
                weaponAnimator.CrossFadeInFixedTime("Reload", 0.01f);
                weaponAnimator.SetBool("Reload", true);
                yield return new WaitForSeconds(currentGun.reloadDuration);
                isReloading = false;
                weaponAnimator.SetBool("Reload", false);

                int reloadAmount = currentGun.ammoPerMag - currentGun.currentAmmo;
                if (currentGun.ammoLeft - reloadAmount > 0)
                {
                    currentGun.currentAmmo = currentGun.ammoPerMag;
                    currentGun.ammoLeft -= reloadAmount;

                }
                else if (currentGun.currentAmmo + currentGun.ammoLeft < currentGun.ammoPerMag)
                {
                    currentGun.currentAmmo += currentGun.ammoLeft;
                    currentGun.ammoLeft = 0;
                }
                ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;
            }
        }
    }
    [PunRPC]
    void RPC_SpawnDecal(Vector3 hitPosition, Vector3 hitNormal, int hitType)
    {
        GameObject decal = objectPool.GetPooledObject(hitType);
        if (decal != null)
        {
            decal.transform.position = hitPosition;
            decal.transform.rotation = Quaternion.LookRotation(hitNormal);
            decal.SetActive(true);
        }
        StartCoroutine(DisableDecals(decal));
    }
    IEnumerator DisableDecals(GameObject go)
    {
        yield return new WaitForSeconds(1);
        go.SetActive(false);
    }
    [PunRPC]
    void RPC_PlayWeaponSound(string clipName)
    {
        Debug.Log(clipName);
        foreach(AudioClip clip in currentGun.weaponSounds)
        {
            if(clip.name == clipName)
            {
                weaponSource.PlayOneShot(clip);
                weaponSource.volume = currentGun.weaponVolume;

            }
        }
    }
    public void HandleBulletSpread()
    {
        if (playerController.isMoving)
        {
            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name == currentGun.name)
                {
                    currentGun.bulletSpread = currentGun.runningBulletSpread;
                }
            }
        }
        else
        {
            inputHandler.isAiming = true;
            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name == currentGun.name)
                {
                    currentGun.bulletSpread = currentGun.normalBulletSpread;
                }
            }
        }

    }

}
