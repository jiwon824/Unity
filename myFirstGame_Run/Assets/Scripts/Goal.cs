using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    GameObject target;
    private Player player;
    public GameManager manager;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        player = target.GetComponent<Player>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            // 플레이어 오브젝트가 Goal 오브젝트에 닿으면 게임매니저의 GameClear 호출 
            manager.GameClear();
            player.Clear();
        }
    }
}
