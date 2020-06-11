using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 敌人死亡掉落的盒子
 */
public class DropBox : MonoBehaviour
{
    private GameObject BagFather;
    private GameObject EnemyBag;
    public GameObject[] Weapons_;//盒子里的武器   0为主武器      1为副武器
    public int[] BulletsNum_;//盒子里的子弹数量   0为主武器数量  1为副武器数量



    private void Start()
    {
        //找到EnemyBag
        BagFather = GameObject.Find("BagFather");
        EnemyBag = BagFather.transform.Find("Bags/EmenyBag").gameObject;
    }

    //进入触发器
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "player") 
        {
            //给EnemyBag赋值
            EnemyBag.SetActive(true);
            EnemyBag.SendMessage("UpdateBag", BulletsNum_, SendMessageOptions.RequireReceiver);
            ItemMove.NowDropBox = this.gameObject;
        }
    }

    //在触发器中
    private void OnTriggerStay(Collider other)
    {
        //如果player进入了触发器中
        if (other.gameObject.name == "player") 
        {
            print("OnTriggerStay");
            //EnemyBag
            //如果在触发器中，EnemyBag没显示
            if (!EnemyBag.activeSelf) 
            {
                EnemyBag.SetActive(true);
                EnemyBag.SendMessage("UpdateBag", BulletsNum_, SendMessageOptions.RequireReceiver);
                ItemMove.NowDropBox = this.gameObject;
            }
        }
    }

    //离开触发器
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "player")
        {
            //EnemyBag.SendMessage("ClearBag", SendMessageOptions.RequireReceiver);
            EnemyBag.SetActive(false);
        }
    }
}
