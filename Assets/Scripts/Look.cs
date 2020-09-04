using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


    public class Look : MonoBehaviourPunCallbacks
    {

        #region variables
        public float XSensitivity;
        public float YSensitivity;
        public float MaxAngle;
        public Transform player;
        public Transform cams;
        public Transform weapon;
        private Quaternion camCenter;
        private static  bool cursorLock = true;
        #endregion

        #region Monobehavior callbacks
        void Start()
        {
         

            camCenter = cams.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine)
                return;

            SetYRotation();
            SetXRotation();
            UpdateCursorLock();
        }

        #endregion

        #region private methods
        void SetYRotation()
        {
            float TempInput = Input.GetAxis("Mouse Y") * YSensitivity * Time.deltaTime;
            Quaternion TempAdj = Quaternion.AngleAxis(TempInput, Vector3.left);
            Quaternion TempDelta = cams.localRotation * TempAdj;
            if (Quaternion.Angle(camCenter, TempDelta) < MaxAngle)
            {
                cams.localRotation = TempDelta;
            }
            weapon.rotation = cams.rotation;
        }

        void SetXRotation()
        {
            float TempInput = Input.GetAxis("Mouse X") * XSensitivity * Time.deltaTime;
            Quaternion TempAdj = Quaternion.AngleAxis(TempInput, Vector3.up);
            Quaternion TempDelta = player.localRotation * TempAdj;

            player.localRotation = TempDelta;


        }

        void UpdateCursorLock()
        {
            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLock = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLock = true;
                }
            }

        }

        #endregion



    }
