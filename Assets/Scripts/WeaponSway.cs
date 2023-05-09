using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;
    public float moveMultiplier = 0.5f;
    public float lookMultiplier = 1f;
    private Vector3 initialPosition;


    public InputHandler inputHandler;

    

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (inputHandler.isPaused) return;

        bool idleMouse = inputHandler.MouseY == 0 || inputHandler.MouseX == 0;
        float movementX;
        float movementY;

        if(idleMouse)
        {
            // If mouse isnt moving
            movementX = -inputHandler.horz * amount;
            movementY = -inputHandler.vert * amount;
            if(inputHandler.isAiming)
            {
                movementX = Mathf.Clamp(movementX, -maxAmount * moveMultiplier, maxAmount * moveMultiplier);
                movementY = Mathf.Clamp(movementY, -maxAmount * moveMultiplier, maxAmount * moveMultiplier);
                Vector3 finalPosition = new Vector3(movementX, 0, 0);
                transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
            }
            else
            {
                movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
                movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);
                Vector3 finalPosition = new Vector3(movementX, movementY, 0);
                transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
            }
        }
        else
        {
            if (inputHandler.mouseXSensitivity == 0 || inputHandler.mouseYSensitivity == 0) return;
            movementX = -inputHandler.MouseX * amount;
            movementY = -inputHandler.MouseY * amount;
            movementX = Mathf.Clamp(movementX, -maxAmount * lookMultiplier, maxAmount * lookMultiplier);
            movementY = Mathf.Clamp(movementY, -maxAmount * lookMultiplier, maxAmount * lookMultiplier);
            Vector3 finalPosition = new Vector3(movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
        }

    }
}