using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold; // 총을 잡는 위치(=손의 위치)
    public Gun startingGun; // 처음 시작 무기
    Gun equippedGun; // 현재 장착 중인 총을 저장할 변수

    void Start()
    {
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun; //as Gun으로 Gun타입으로 형변환 시켜줌
        equippedGun.transform.parent = weaponHold;
    }
    public void Shoot()
    {
        // 장착 중인 무기가 있는지 확인해야 함.
        // 무기가 없는데 총알 쏘면 안됨.
        if (equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }
}
