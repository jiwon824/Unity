using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold; // ���� ��� ��ġ(=���� ��ġ)
    public Gun startingGun; // ó�� ���� ����
    Gun equippedGun; // ���� ���� ���� ���� ������ ����

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
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun; //as Gun���� GunŸ������ ����ȯ ������
        equippedGun.transform.parent = weaponHold;
    }
    public void Shoot()
    {
        // ���� ���� ���Ⱑ �ִ��� Ȯ���ؾ� ��.
        // ���Ⱑ ���µ� �Ѿ� ��� �ȵ�.
        if (equippedGun != null)
        {
            equippedGun.Shoot();
        }
    }
}
