using System.Collections;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    public float maxRecoilAngle = 5.0f;
    public float recoilSpeed = 5.0f;
    public float returnSpeed = 10.0f;

    private float currentRecoilAngle = 0.0f;
    private bool isReturning = false;
    private Quaternion originalRotation;

    [SerializeField] private WeaponHandler weaponHandler;

    private void Start()
    {
        // Save the camera's original rotation
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        // Apply recoil motion
        if (currentRecoilAngle > 0.0f)
        {
            float angleAmount = Mathf.Min(currentRecoilAngle, recoilSpeed * Time.deltaTime);
            transform.Rotate(Vector3.left, angleAmount);
            currentRecoilAngle -= angleAmount;
        }
        else
        {
            float returnAmount = returnSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, returnAmount);

        }
    }
    public void Fire(bool aim)
    {
        if(aim)
        {
            currentRecoilAngle += weaponHandler.currentGun.recoilPower * 0.6f;
        }
        else
        {
            currentRecoilAngle += weaponHandler.currentGun.recoilPower;
        }
    }
}
