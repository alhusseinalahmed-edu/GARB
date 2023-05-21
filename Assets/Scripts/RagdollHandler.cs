using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RagdollHandler : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    public float ragdollDestroyTime = 30f;

    [SerializeField] Transform model;
    [SerializeField] PhotonView PV;
    public void Awake()
    {
        ragdollRigidbodies = model.GetComponentsInChildren<Rigidbody>();
        ragdollColliders = model.GetComponentsInChildren<Collider>();
    }

    public void Setup(bool dead)
    {
        if (dead)
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.isKinematic = false;
            }
            foreach (Collider collider in ragdollColliders)
            {
                collider.enabled = true;
            }
        }
        else
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.isKinematic = true;
            }
            foreach (Collider collider in ragdollColliders)
            {
                collider.enabled = false;
            }
        }
    }
    public void Die()
    {
        PV.RPC("RPC_deadRagdoll", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_deadRagdoll()
    {
        if (PV.IsMine) model.gameObject.SetActive(true);
        Setup(true);
        model.SetParent(null);
        Destroy(model.gameObject, ragdollDestroyTime);
    }

}
