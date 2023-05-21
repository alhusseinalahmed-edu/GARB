using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayBob : MonoBehaviour
{

    [SerializeField] MovementHandler movementHandler;
    [SerializeField] InputHandler inputHandler;

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot;

    Vector2 moveInput;
    Vector2 lookInput;

    float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;

    Vector3 bobPosition;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    void BobRotation()
    {
        bobEulerRotation.x = (moveInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));

        bobEulerRotation.y = (moveInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (moveInput != Vector2.zero ? multiplier.z * curveCos * moveInput.x : 0);
    }

    private void Update()
    {
        GetInput();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();


        CompositePositionRotation();
    }

    void GetInput()
    {
        moveInput.x = inputHandler.horz;
        moveInput.y = inputHandler.vert;

        moveInput = moveInput.normalized;

        lookInput.x = inputHandler.MouseX;
        lookInput.y = inputHandler.MouseY;
    }

    void BobOffset()
    {
        bool isGrounded = movementHandler.isGrounded();
        speedCurve += Time.deltaTime * (isGrounded ? movementHandler.controller.velocity.magnitude : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (isGrounded ? 1 : 0) - (moveInput.x * travelLimit.x));

        bobPosition.y = (curveSin * bobLimit.y) - (movementHandler.controller.velocity.y * travelLimit.y);

        bobPosition.z = -(moveInput.y * travelLimit.z);
    }

    void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);

    }
    Vector3 smoothedSwayPos;
    Vector3 smoothedSwayEulerRot;

    void CompositePositionRotation()
    {
        smoothedSwayPos = Vector3.Lerp(smoothedSwayPos, swayPos + bobPosition, Time.deltaTime * smooth);
        smoothedSwayEulerRot = Vector3.Lerp(smoothedSwayEulerRot, swayEulerRot + bobEulerRotation, Time.deltaTime * smoothRot);

        transform.localPosition = smoothedSwayPos;
        transform.localRotation = Quaternion.Euler(smoothedSwayEulerRot);

        //transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

}
