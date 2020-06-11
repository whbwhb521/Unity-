using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 队友管理器
 */
public class PlayerEnemyManager : MonoBehaviour
{
    public GameObject mPlayerEnemy;//其他角色的模型
    private Hashtable _Penemy = new Hashtable();
    public static PlayerEnemyManager Instance;
    public GameObject PlayerEnemys;

    private void Awake()
    {
        Instance = this;
    }


    //增加一个玩家
    public bool AddPlayerEnemy(string acco) 
    {
        //玩家已经登录或者玩家为自身时，无法创建
        if (!_Penemy.Contains(acco) && !UIManager.account.Equals(acco))
        {
            GameObject p = GameObject.Instantiate(mPlayerEnemy);
            p.name = acco;
            p.transform.parent = PlayerEnemys.transform;
            PlayerEnemy pe = p.GetComponent<PlayerEnemy>();
            pe.SetAcco(acco);
            _Penemy.Add(acco, pe);
            return true;
        }
        else
        {
            return false;
        }

    }

    //删除一个玩家
    public void RemovePlayerEnemy(string acco) 
    {
        PlayerEnemy pe = (PlayerEnemy)_Penemy[acco];
        pe.Destory();
        _Penemy.Remove(acco);
    }

    //

    //更新玩家状态
    //(帐号，位置，朝向，是否射击，是否射击2，是否换弹，是否使用魔法剑，魔法剑是否CD
    //，当前武器,走路状态,是否跳跃，是否着路)

    public void UpDatePlayerEnemy(string acco, Vector3 pos, Vector3 rot, bool shooting,
        bool shooting2, bool IsChangeBullet, bool isMagic, bool isMagicCD, int currentWeapon,int RunFlag, bool JumpFlag, bool isOnground, bool isDeath)
    {
        PlayerEnemy pe = (PlayerEnemy)_Penemy[acco];
        //设置玩家位置,当前武器，动画播放
        pe.Move(pos, rot);
        pe.SetAnimatorInfo(shooting,shooting2,IsChangeBullet,currentWeapon, RunFlag, JumpFlag, isOnground, isDeath);
        pe.AnimatorControl();
    }
}
