using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 敌人管理器
 */
public class EmenyManager : MonoBehaviour
{
    public static EmenyManager Instance;
    public GameObject mEnemy;
    private Hashtable _AIenemy = new Hashtable();
    public GameObject Enemys;
    public static string Emenymsg;
    public static string acco;

    private void Awake()
    {
        Instance = this;
    }
    //增加敌人AI
    public bool AddEnemy(string name, string PosInfo) 
    {
        if (!_AIenemy.Contains(name))
        {
            string[] args1 = PosInfo.Split(new char[] { ' ' });
            string posx = args1[0];
            string posy = args1[1];
            GameObject p = GameObject.Instantiate(mEnemy,new Vector3(Convert.ToInt32(posx), 0.1f, Convert.ToInt32(posy)), Quaternion.identity);
            p.name = name;
            p.transform.parent = Enemys.transform;
            EmenyInfo ei = p.GetComponent<EmenyInfo>();
            ei.setName(name);
            p.tag = "enemy";
            _AIenemy.Add(name, ei);
            return true;
        }
        else 
        {
            return false;        
        }
        
    }


    //更新玩家状态
    //(帐号，位置，朝向，是否射击，血量)

    public void UpDatePlayerEnemyPos(string acco, string pos)
    {
        //解析Pos
        string[] args1 = pos.Split(new char[] { ' ' });
        EmenyInfo ei = (EmenyInfo)_AIenemy[acco];
        string posx = args1[0];
        string posy = args1[1];
        string playerposx = args1[2];
        string playerposy = args1[3];
        ei.targetPos = new Vector3(Convert.ToInt32(posx), 0.0f, Convert.ToInt32(posy));
        ei.PlayerPos = new Vector3(Convert.ToSingle(playerposy), 0.0f, -Convert.ToSingle(playerposx));
    }
    //更新玩家血量 血量不足则死亡并删除
    public void UpDatePlayerEnemyBlood(string acco, int blood, string Killer) 
    {
        EmenyInfo ei = (EmenyInfo)_AIenemy[acco];
        ei.SetBlood(blood, Killer);
        if (blood <= 0) 
        {
            _AIenemy.Remove(acco);
        }
    }
}
