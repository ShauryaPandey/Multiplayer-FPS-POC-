using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Sway : MonoBehaviour
{
    #region variables
    Quaternion originRotation;
    public float intensity = 100;
    public float smooth = 10;
    public bool isMine;
    #endregion
    #region Monobehaviour callbacks
    private void Start()
    {
       

        originRotation = transform.localRotation;
    }
    private void Update()
    {
        
        UpdateSway();   
    }

    #endregion
    #region private methods
    private void UpdateSway() {
        float TXMouse = Input.GetAxis("Mouse X");
        float TYMouse = Input.GetAxis("Mouse Y");
        if (!isMine) {
            TXMouse = 0;
            TYMouse = 0;
        }
        Quaternion TempXRotation = Quaternion.AngleAxis(intensity*TXMouse,Vector3.down);
        Quaternion TempYRotation = Quaternion.AngleAxis(intensity *TYMouse,Vector3.right);
        Quaternion TargetRotation = originRotation * TempXRotation * TempYRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation,TargetRotation,Time.deltaTime*smooth);
    }
    #endregion
}
