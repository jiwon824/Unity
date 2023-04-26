using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    GameObject target;
    private Player player;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        player = target.GetComponent<Player>();
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            // 플레이어가 접촉했다면 플레이어Die(GameOver, Camera Stop)
            player.Die();
        }
    }
}
