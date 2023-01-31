using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle; // 총구의 위치(총알이 나올 위치)
    public Projectile projectile;  // 어떤 발사체를 쏠지
    public float msBetweenShots = 100; // 무기의 연사력
    public float muzzleVelocity = 35; // 총알이 발사되는 순간 총알의 속력

    float nextShotTime;

    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {

            nextShotTime = Time.time + msBetweenShots/1000;

            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);
        }
        

    }
}
