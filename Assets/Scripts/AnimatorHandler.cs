using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;
    [SerializeField] PlayerController playerController;
    [SerializeField] WeaponHandler weaponHandler;
    public PhotonView PV;
    [SerializeField] Animator animator;


    private void HandleAnimator()
    {
        animator.SetFloat("vert", inputHandler.vert, 1f, Time.deltaTime * 15f);
        animator.SetFloat("horz", inputHandler.horz, 1f, Time.deltaTime * 15f);
        animator.SetBool("isGrounded", playerController.Grounded);
    }

    private void Update()
    {
        HandleAnimator();
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

}
