using UnityEngine;

public interface IDamaneable{
    void TakeHit(float damage, RaycastHit hit);// 구현부는 여기서 만들 필요가 없다.
    void TakeDamage(float damage);// 구현부는 여기서 만들 필요가 없다.
}