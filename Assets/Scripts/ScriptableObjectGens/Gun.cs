using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun",menuName ="Gun")]
public class Gun : ScriptableObject
{
    public string name;
    public int damage;
    public GameObject prefab;
    public float fireRate;
    public float kickBack;
    public float recoil;
    public float aimSpeed;
 
}
