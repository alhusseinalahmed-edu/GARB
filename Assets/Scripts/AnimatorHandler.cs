using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimatorHandler : MonoBehaviour
{
    [Header("Refs")]
    public InputHandler inputHandler;
    public PlayerController playerController;
    public WeaponHandler weaponHandler;
    public PhotonView PV;
    public Animator animator;

    [Header("Settings")]
    public float movementBlendTreeSmooth;

    
    private void Update()
    {
        HandleAnimator();
    }
    private void HandleAnimator()
    {
        animator.SetFloat("vert", inputHandler.vert, 1f, Time.deltaTime * movementBlendTreeSmooth);
        animator.SetFloat("horz", inputHandler.horz, 1f, Time.deltaTime * movementBlendTreeSmooth);
    }
    public void SetBool(string name, bool isTrue)
    {
        animator.SetBool(name, isTrue);
    }
    public void CrossFadeInFixedTime(string name, float transition)
    {
        PV.RPC("RPC_CrossFadeInFixedTime", RpcTarget.All, name, transition);
    }
    [PunRPC]
    private void RPC_CrossFadeInFixedTime(string name, float transition)
    {
        animator.CrossFadeInFixedTime(name, transition);
    }
    public void UpdateAnimatorFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    // Local
    public string GetCurrentGunAnimation()
    {
        Animator gunAnimator = weaponHandler.weaponAnimator;
        AnimatorClipInfo[] currentClipInfo = gunAnimator.GetCurrentAnimatorClipInfo(0);
        string currentClipName = currentClipInfo[0].clip.name;
        return currentClipName;
    }

}
