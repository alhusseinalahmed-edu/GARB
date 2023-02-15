using UnityEngine;

public class Headbob : MonoBehaviour
{

    // The amount of head bob when the player is running
    public float runningBobAmount = 0.1f;

    // The amount of head bob when the player is standing still
    public float standingBobAmount = 0.01f;

    // The speed at which the head bob animates
    public float bobSpeed = 5.0f;

    // The original position of the camera
    private Vector3 originalPos;

    void Start()
    {
        // Store the original position of the camera
        originalPos = transform.localPosition;
    }

    void Update()
    {
        float bobAmount;
        float movementSpeed = Input.GetAxis("Vertical");

        // Check if the player is running
        if (movementSpeed > 0)
        {
            bobAmount = runningBobAmount;
        }
        else
        {
            bobAmount = standingBobAmount;
        }

        // Bob the camera
        transform.localPosition = originalPos + new Vector3(0, Mathf.Sin(Time.time * bobSpeed), 0) * bobAmount;
    }
}
