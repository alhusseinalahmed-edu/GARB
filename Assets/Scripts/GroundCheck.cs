using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            playerController.Grounded = true;
            playerController.doubleJump = 1;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            playerController.Grounded = true;
            playerController.doubleJump = 1;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            playerController.Grounded = false;
        }
    }
}
