using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    private Vector3 initialPosition;

    public InputHandler inputHandler;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (inputHandler.isPaused) return;
        float movementX = -inputHandler.MouseX * amount;
        float movementY = -inputHandler.MouseY * amount;

        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }
}