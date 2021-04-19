using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float xRotation = 0f;
    [SerializeField] float yRotation = 0f;
    [SerializeField] Transform heroBody;

    [SerializeField] Image haircrossCanvas;

    bool cameraLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    void LockCamera()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        cameraLocked = true;
    }

    void ReleaseCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraLocked = false;
        Cursor.visible = true;

        haircrossCanvas.rectTransform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraLocked)
        {
            haircrossCanvas.rectTransform.position = Input.mousePosition;
            return;
        }
            

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20f, 15f);

        //yRotation -= mouseX;
        //yRotation = Mathf.Clamp(yRotation, -45f, 45f);

        //transform.localRotation = Quaternion.Euler(xRotation, -yRotation, 0f);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0f);


        heroBody.Rotate(Vector3.up * mouseX);
    }
}
