using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupRagdoll : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

    [SerializeField] Transform model;
    [SerializeField] GameObject[] models;
    public void Awake()
    {
        ragdollRigidbodies = model.GetComponentsInChildren<Rigidbody>();
        ragdollColliders = model.GetComponentsInChildren<Collider>();
    }

    public void Setup(bool isTrue)
    {
        if (isTrue)
        {
            foreach(Rigidbody rb in ragdollRigidbodies)
            {
                rb.isKinematic = false;
            }
            foreach(Collider collider in ragdollColliders)
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

}
