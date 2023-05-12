using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] PlayerController playerController;
    [SerializeField] AnimatorHandler animatorHandler;
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] PhotonView PV;
    [SerializeField] GameObject scoreboardUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] Scoreboard scoreBoard;
    public float mouseXSensitivity;
    public float mouseYSensitivity;
    public bool isAiming = false;

    [HideInInspector] public float vert;
    [HideInInspector] public float horz;
    [HideInInspector] public float MouseX;
    [HideInInspector] public float MouseY;
    public bool isPaused = false;
    float verticalLookRotation;
    private float zoomSensitivityMultiplier;

    public bool isTesting;

    private void Start()
    {
        mouseXSensitivity = PlayerPrefs.GetFloat("MouseXSensitivity");
        mouseYSensitivity = PlayerPrefs.GetFloat("MouseYSensitivity");
        zoomSensitivityMultiplier = PlayerPrefs.GetFloat("ZoomSensitivityMultiplier");

        if(mouseXSensitivity == 0)
        {
            mouseXSensitivity = 1;
        }
        if(mouseYSensitivity == 0)
        {
            mouseYSensitivity = 1;
        }
        if(zoomSensitivityMultiplier == 0)
        {
            zoomSensitivityMultiplier = 1;
        }
    }
    public void UpdateInputOptions()
    {
        mouseXSensitivity = PlayerPrefs.GetFloat("MouseXSensitivity");
        mouseYSensitivity = PlayerPrefs.GetFloat("MouseYSensitivity");
        zoomSensitivityMultiplier = PlayerPrefs.GetFloat("ZoomSensitivityMultiplier");
    }
    private void Look()
    {
        if (isPaused) return;
        verticalLookRotation += MouseY * mouseYSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);


        transform.Rotate(Vector3.up * MouseX * mouseXSensitivity);
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inGameUI.activeSelf)
            {
                isPaused = false;
                inGameUI.SetActive(false);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                isPaused = true;
                inGameUI.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

            }
        }

        if (isPaused) return;
        vert = Input.GetAxisRaw("Vertical");
        horz = Input.GetAxisRaw("Horizontal");

        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");


        if (isTesting)
        { 
            for (int i = 0; i < weaponHandler.guns.Length; i++)
            {
            
                if (Input.GetKeyDown((i + 1).ToString()) && !weaponHandler.isReloading)
                {
                    weaponHandler.Equip(i);
                    break;
                }
            }
        }
        if (Input.GetMouseButton(0) && !weaponHandler.isReloading)
        {
            weaponHandler.Shoot();
        }
        if(Input.GetMouseButtonDown(1) && !weaponHandler.isReloading)
        {
            weaponHandler.AimDownSights();
            if (!isAiming)
            {
                mouseXSensitivity = PlayerPrefs.GetFloat("MouseXSensitivity");
                mouseYSensitivity = PlayerPrefs.GetFloat("MouseYSensitivity");
            }
            else
            {
                mouseXSensitivity *= zoomSensitivityMultiplier;
                mouseYSensitivity *= zoomSensitivityMultiplier;
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && !weaponHandler.isReloading && weaponHandler.currentGun.currentAmmo != weaponHandler.currentGun.ammoPerMag)
        {
            string state = animatorHandler.GetCurrentGunAnimation();
            if (!state.Contains("Idle")) return;
            weaponHandler.Reload();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(scoreboardUI.activeSelf)
            {
                scoreboardUI.SetActive(false);              
            }
            else
            {
                scoreboardUI.SetActive(true);
                scoreBoard.UpdateScoreboard();
            }
        }
    }

    private void Update()
    {
        if (!PV.IsMine) return;
        HandleInput();
        Look();
    }

}
