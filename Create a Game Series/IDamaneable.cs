using UnityEngine;

public interface IDamaneable{
    void TakeHit(float damage, RaycastHit hit);// �����δ� ���⼭ ���� �ʿ䰡 ����.
    void TakeDamage(float damage);// �����δ� ���⼭ ���� �ʿ䰡 ����.
}