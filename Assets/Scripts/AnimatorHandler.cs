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
    [SerializeField] float movementBlendTreeSmooth;


    private void HandleAnimator()
    {
        animator.SetFloat("vert", inputHandler.vert, 1f, Time.deltaTime * movementBlendTreeSmooth);
        animator.SetFloat("horz", inputHandler.horz, 1f, Time.deltaTime * movementBlendTreeSmooth);
    }
    public void SetBool(string name, bool isTrue)
    {
        animator.SetBool(name, isTrue);
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
