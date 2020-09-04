using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Photon.Pun;


public class Weapon : MonoBehaviourPun
{
    #region variables  
    public Gun[] loadout;
    public Transform weaponParent;
    int currentIndex;
    private GameObject currentEquipment;
        private float currentCoolDown;
        public GameObject bulletHolePrefab;
        public LayerMask canBeShot;
    #endregion
    #region Monobehaviour callbacks
    void Update()
    {
            if (!photonView.IsMine)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                photonView.RPC("Equip",RpcTarget.All,0);
        }
            if (currentEquipment != null)
                Aim(Input.GetMouseButton(1));
            if (currentEquipment != null) {
                if (Input.GetMouseButton(0) && currentCoolDown<=0) {
                    photonView.RPC("Shoot", RpcTarget.All);
                }
                currentEquipment.transform.localPosition = Vector3.Lerp(currentEquipment.transform.localPosition, Vector3.zero, Time.deltaTime * 8f);
            }
            if (currentCoolDown > 0)
                currentCoolDown -= Time.deltaTime;
    }

        #endregion
        #region private methods

        void Aim(bool isAiming) {
            Transform TAnchor = currentEquipment.transform.Find("Anchor");
            Transform THip = currentEquipment.transform.Find("States/Hip");
            Transform TADS = currentEquipment.transform.Find("States/ADS");

            if (isAiming)
            {
                TAnchor.position = Vector3.Lerp(TAnchor.position, TADS.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
            else {
                TAnchor.position = Vector3.Lerp(TAnchor.position, THip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
        }

        
        [PunRPC]
        void Equip(int index) {
        if (currentEquipment != null) {
            Destroy(currentEquipment);
        }
            currentIndex = index;
        GameObject TempNewEquipment = Instantiate<GameObject>(loadout[index].prefab, weaponParent.position,weaponParent.rotation,weaponParent);
        TempNewEquipment.transform.localPosition = Vector3.zero;
        TempNewEquipment.transform.localEulerAngles = Vector3.zero;
        TempNewEquipment.GetComponent<Sway>().isMine = photonView.IsMine;
        currentEquipment = TempNewEquipment;
    }

        [PunRPC]
        void Shoot() {
            // Transform TSpawn = transform.Find("Cameras/Normal Camera");
               Transform TSpawn = transform.Find("Weapon/Pistol(Clone)/Anchor/Design/Barrel");
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(TSpawn.position, TSpawn.forward, out hit, 1000f, canBeShot)) {
                GameObject newHole = Instantiate<GameObject>(bulletHolePrefab,hit.point+hit.normal*0.0000001f,Quaternion.identity);
                newHole.transform.LookAt(hit.point+hit.normal *1f);
                Destroy(newHole, 5f);
                if (!photonView.IsMine) {
                    if (hit.collider.gameObject.layer == 12) {

                        photonView.RPC("TakeDamage",RpcTarget.All,loadout[currentIndex].damage);

                    }
                }
                    }
            //Gun recoil and kickback
            currentEquipment.transform.Rotate(-loadout[currentIndex].recoil,0,0);
            currentEquipment.transform.position += currentEquipment.transform.forward * loadout[currentIndex].kickBack;

            currentCoolDown = loadout[currentIndex].fireRate;
        }

        [PunRPC]
       private void  TakeDamage(int pDamage) {
        GetComponent<Motion>().TakeDamage(pDamage);
        }
    #endregion
}
