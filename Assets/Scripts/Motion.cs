using System.Collections;
using System.Collections.Generic;
using System.Text;
//using System.Numerics;
using UnityEngine;
using Photon.Pun;


    public class Motion : MonoBehaviourPunCallbacks
    {
        #region variables
        public float speed;
        public float speedModifier;
        bool sprint;
        bool jump;
        public Camera normCam;
        private float baseFov;
        private float updateFov =1.5f;
        public float jumpForce;
        public int maxHealth;
        public Transform groundDetector;
        public LayerMask ground;
        private Vector3 weaponParentOrigin;
        public Transform weaponParent;
        private Vector3 targetWeaponBobPosition;
        private float IdleCounter = 0;
        private float MovementCounter = 0;
        public GameObject cameraParent;
        private int currentHealth;

        [SerializeField]
         Rigidbody rig;
        #endregion
        #region Monobehaviour callbacks
        void Start()
        {

            currentHealth = maxHealth;
            cameraParent.SetActive(photonView.IsMine);

            if (!photonView.IsMine) gameObject.layer = 12;


            if(Camera.main)Camera.main.enabled = false;
            baseFov = normCam.fieldOfView;
            weaponParentOrigin = weaponParent.localPosition;
        }

        private void Update()
        {
            if (!photonView.IsMine)
                return;
            //Axles
            float TempHMove = Input.GetAxisRaw("Horizontal");
            float TempVMove = Input.GetAxisRaw("Vertical");

            //Controls
            sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            jump = Input.GetKeyDown(KeyCode.Space);

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && !isJumping && TempVMove > 0 && isGrounded;

            if (isJumping)
            {

                rig.AddForce(Vector3.up * jumpForce);
            }

            //Head bob
            //Head bob when the player is idle
            if (TempHMove == 0 && TempVMove == 0)
            {
                HeadBob(IdleCounter, 0.025f, 0.025f);
                IdleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);

            }//else when the player is moving
            else if (!isSprinting)
            {
                HeadBob(MovementCounter, 0.035f, 0.035f);
                MovementCounter += Time.deltaTime *3f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else {
                HeadBob(MovementCounter, 0.15f, 0.075f);
                MovementCounter += Time.deltaTime * 10f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }
        }
        void FixedUpdate()
        {
            if (!photonView.IsMine)
                return;

            //Axles
            float TempHMove = Input.GetAxisRaw("Horizontal");
            float TempVMove = Input.GetAxisRaw("Vertical");
            
            //Controls
            sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            jump = Input.GetKeyDown(KeyCode.Space);

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position,Vector3.down,0.1f,ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && !isJumping && TempVMove>0 && isGrounded;
            
            Vector3 TempDirection = new Vector3(TempHMove, 0, TempVMove);
            TempDirection.Normalize();
            float tempAdjustSpeed = speed;

            
            if (isSprinting)
            {
                tempAdjustSpeed *= speedModifier;

            }
            rig.velocity = new Vector3(0,rig.velocity.y,0) + transform.TransformDirection(TempDirection) * tempAdjustSpeed * Time.deltaTime;

            UpdateFieldOfView(isSprinting);
        }

      

        #endregion
        #region private methods
        private void UpdateFieldOfView(bool isSprinting)
        {
            if (isSprinting)
            {
                  normCam.fieldOfView = Mathf.Lerp(normCam.fieldOfView, baseFov * updateFov, Time.deltaTime * 8f);
                //normCam.fieldOfView = baseFov * updateFov;
            }
            else
            {
                normCam.fieldOfView = Mathf.Lerp(normCam.fieldOfView, baseFov, Time.deltaTime * 8f);
            }
        }

        private void HeadBob(float pZ, float pXIntensity, float pYIntensity) {

            targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(pZ)*pXIntensity, Mathf.Sin(pZ)*pYIntensity , 0);
        }
        #endregion

        #region public methods
        
        public void TakeDamage(int pHealth ) {
        if (photonView.IsMine)
        {
            currentHealth -= pHealth;
            Debug.Log(currentHealth);
        }
        }
        #endregion
    }

