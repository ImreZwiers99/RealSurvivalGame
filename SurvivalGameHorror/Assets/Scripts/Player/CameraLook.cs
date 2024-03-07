using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Look Variables")]
    //[SerializeField, Range(1, 10)] private float lookSpeedX = 2f;
    //[SerializeField, Range(1, 10)] private float lookSpeedY = 2f;
    //[SerializeField, Range(1, 180)] private float upperLookLimit = 80f;
    //[SerializeField, Range(1, 180)] private float lowerLookLimit = 80f;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform _camera;
    [SerializeField] private float camSens = 200f;
    [SerializeField] private float camAcc = 5f;
    private float rotationX;
    private float rotationY;

    void Update()
    {
        //rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        //rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        //transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
        rotationX += Input.GetAxis("Mouse Y") * camSens * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse X") * camSens * Time.deltaTime;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        hand.localRotation = Quaternion.Euler(-rotationX, rotationY, 0);

        transform.localRotation = Quaternion.Lerp(transform.localRotation,
            Quaternion.Euler(0, rotationY, 0), camAcc * Time.deltaTime);
        _camera.localRotation = Quaternion.Lerp(_camera.localRotation,
            Quaternion.Euler(-rotationX, 0, 0), camAcc * Time.deltaTime);
    }
}
