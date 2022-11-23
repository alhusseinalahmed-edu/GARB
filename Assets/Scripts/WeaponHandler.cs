using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WeaponHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject[] impacts;
    [SerializeField] TMP_Text ammoText;
    [SerializeField] RecoilHandler recoilHandler;
    [SerializeField] Camera cam;
    public Gun[] guns;
    [SerializeField] AudioSource weaponSource;
    public PhotonView PV;
    [SerializeField] GameObject[] fpsGuns;
    [SerializeField] GameObject[] tpsGuns;
    [SerializeField] AnimatorHandler animatorHandler;
    [SerializeField] PlayerController playerController;


    float nextShootTimer;

    [HideInInspector] public Gun currentGun;
    int previousItemIndex = -1;
    int gunIndex;


    public bool isAiming = false;
    public bool isReloading = false;

    private void Awake()
    {
        foreach (Gun gun in guns)
        {
            gun.currentAmmo = gun.ammoPerMag;
        }
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_Equip", RpcTarget.All, 0);
        }
    }
    public void Equip(int _index)
    {
        PV.RPC("RPC_Equip", RpcTarget.All, _index);
    }
    [PunRPC]
    private void RPC_Equip(int _index)
    {
        if (_index == previousItemIndex) return;
        gunIndex = _index;
        currentGun = guns[gunIndex];
        if(currentGun.weaponType == WeaponType.Primary)
        {
            animatorHandler.UpdateAnimatorFloat("gunIndex", 0f);
        }
        else if(currentGun.weaponType == WeaponType.Secondary)
        {
            animatorHandler.UpdateAnimatorFloat("gunIndex", 1f);
        }
        previousItemIndex = gunIndex;
        isAiming = false;
        ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;
        foreach (GameObject go in tpsGuns)
        {
            if (go.transform.name != currentGun.name)
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
            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name != currentGun.name)
                {
                    go.SetActive(false);
                }
                else
                {
                    go.SetActive(true);
                }
            }            
        }
    }
    public void Shoot()
    {
        if (nextShootTimer > Time.time) return;
        if (currentGun.currentAmmo <= 0) return;
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
                { // Friendly Fire
                    hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(currentGun.damage);
                    PV.RPC("RPC_BulletImpact", RpcTarget.All, hit.point, hitNormal, "Enemy");
                    playerController.PlayHitSound();
                }
                else
                {
                    PV.RPC("RPC_BulletImpact", RpcTarget.All, hit.point, hitNormal, hit.transform.gameObject.tag);
                }
            }
        }
        PV.RPC("RPC_ShootSound", RpcTarget.All);
        animatorHandler.CrossFadeInFixedTime("Shoot", 0.01f);
        recoilHandler.Fire();
        nextShootTimer = Time.time + currentGun.fireRate;
        currentGun.currentAmmo--;
        ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;
    }
    public void Reload()
    {
        PV.RPC("RPC_Reload", RpcTarget.All);
    }
    [PunRPC]
    IEnumerator RPC_Reload()
    {
        GameObject mag = null;
        if (currentGun.name != "Shotgun")
        {
            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name == currentGun.name)
                {
                    mag = go.transform.Find("Mag").gameObject;
                }
            }
        }
        if (PV.IsMine)
        {
            isReloading = true;
            if(mag != null) mag.SetActive(false);

            yield return new WaitForSeconds(currentGun.reloadDuration);
            isReloading = false;
            int reloadAmount = currentGun.ammoPerMag - currentGun.currentAmmo;
            currentGun.currentAmmo = currentGun.ammoPerMag;
            currentGun.ammoLeft -= reloadAmount;
            ammoText.text = currentGun.currentAmmo.ToString() + "/" + currentGun.ammoLeft;
            if (mag != null) mag.SetActive(true);
        }
    }
    [PunRPC]
    void RPC_BulletImpact(Vector3 hitPosition, Vector3 hitNormal, string hitType)
    {
        GameObject impact = impacts[0];
        foreach (GameObject go in impacts)
        {
            if (hitType == go.name)
            {
                impact = go;
            }
        }
        GameObject bulletImpact = Instantiate(impact, hitPosition, Quaternion.LookRotation(hitNormal));
        Destroy(bulletImpact, 10f);
    }
    [PunRPC]
    void RPC_ShootSound()
    {
        weaponSource.PlayOneShot(currentGun.shootSound);
        weaponSource.volume = currentGun.shootVolume;
    }
    public void HandleADS()
    {
        if (isAiming)
        {
            isAiming = false;
            recoilHandler.aiming = false;
            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name == currentGun.name)
                {
                    go.transform.localPosition = currentGun.position;
                    go.transform.localRotation = currentGun.rotation;
                    currentGun.bulletSpread = currentGun.normalBulletSpread;
                }
            }
        }
        else
        {
            isAiming = true;
            recoilHandler.aiming = true;
            foreach (GameObject go in fpsGuns)
            {
                if (go.transform.name == currentGun.name)
                {
                    go.transform.localPosition = currentGun.aimPosition;
                    go.transform.localRotation = currentGun.aimRotation;
                    currentGun.bulletSpread = currentGun.adsBulletSpread;

                }
            }
        }

    }

}
