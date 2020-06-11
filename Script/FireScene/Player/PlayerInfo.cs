using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 人物信息
     */
public class PlayerInfo : MonoBehaviour
{
    public int currentHP = 100;
    private Image LifeBar;
    private Text LifeNum;
    private Image ExpBar;
    private Text tx_grade;
    public int[] Bullets_ ;//后备子弹数量
    public int[] frontBullets_ ;
    public static PlayerInfo Instance;
    public static string playerinfo = "test 49 0.38 -42 0 0 0 60 30 100 30 30 1 0";
    public int grade;   //人物等级
    public float exp;     //人物经验
    private float maxexp;
    public float timer = 0.6f;
    private Animator PlayerAnimator;     //Player的动画


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        LifeBar = GameObject.Find("LifeBar").GetComponent<Image>();
        LifeNum = GameObject.Find("LifeNum").GetComponent<Text>();
        ExpBar = GameObject.Find("exp").GetComponent<Image>();
        tx_grade = GameObject.Find("tx_grade").GetComponent<Text>();
        string[] args = playerinfo.Split(new char[] { ' ' });
        transform.position = new Vector3(Convert.ToSingle(args[1]), Convert.ToSingle(args[2]), Convert.ToSingle(args[3]));
        transform.eulerAngles = new Vector3(Convert.ToSingle(args[4]), Convert.ToSingle(args[5]), Convert.ToSingle(args[6]));
        Bullets_ = new int[] { Convert.ToInt32(args[7]), Convert.ToInt32(args[8]) };
        currentHP = Convert.ToInt32(args[9]);
        frontBullets_ = new int[] { Convert.ToInt32(args[10]), Convert.ToInt32(args[11]), 0 };
        grade = Convert.ToInt32(args[12]);
        exp = Convert.ToSingle(args[13]);
        PlayerController.Instance.currentBullet = frontBullets_;
        PlayerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateHp();
        CalculateExp();
        AnimatorControl();
    }

    private void FixedUpdate()
    {
        SendPlayerZT();
    }

    private void CalculateExp()
    {
        maxexp = grade * 100;
        ExpBar.fillAmount = exp / maxexp;
        //升级
        if (exp >= maxexp) 
        {
            grade++;
            exp -= maxexp;
            string expmsg = "EXP " + UIManager.account + " " + grade.ToString() + " " + exp.ToString();
            try
            {
                GManager.Instance.SendMessageByTcp(expmsg);
            }
            catch (Exception)
            {
            }
        }
        tx_grade.text = "等级: " + grade;
    }

    private void CalculateHp()
    {
        if (currentHP > 0)
        {
            LifeNum.text = currentHP.ToString();
            LifeBar.fillAmount = currentHP / 100.0f;
        }
        else
        {
            LifeNum.text = "0";
            LifeBar.fillAmount = 0;
            
        }
    }

    public void BeHit(object[] paras) 
    {
        if (currentHP > 0) 
        {
            int BeDamage = (int)paras[1];
            print("Player be hit!");
            currentHP -= BeDamage;
            print("Player HP:" + currentHP.ToString());
        }
        
    }

    //玩家死亡回调
    public void PlayerDeath()
    {
        string DeathMsg = "D1 " + UIManager.account;
        try
        {
            GManager.Instance.SendMessageByTcp(DeathMsg);
        }
        catch (Exception) { }

        UIManager.Instance.DeathPlane.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject.Find("player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("player").GetComponent<PlayerMoveController>().enabled = false;
        GameObject.Find("player").GetComponent<PlayerCameraFollow>().enabled = false;
    }

    //退出时保存的  (帐号，位置，朝向，是否射击，是否射击2，是否换弹，是否使用魔法剑，魔法剑是否CD
    //，当前武器,走路状态,是否跳跃，是否着路,弹1数量，弹2数量,当前弹1数量,当前弹2数量,等级，经验)
    public void SendPlayerInfo()
    {
        string msg = "S2 " + UIManager.account + " " + this.transform.position.x + " "
           + this.transform.position.y + " " + this.transform.position.z + " "
           + this.transform.eulerAngles.x + " " + this.transform.eulerAngles.y + " "
           + this.transform.eulerAngles.z + " " + PlayerController.Instance.shooting.ToString() + " " + PlayerController.Instance.shooting2.ToString()
           + " " + PlayerController.Instance.IsChangeBullet.ToString() + " " + PlayerController.Instance.currentWeapon.ToString() + " "
           + PlayerMoveController.Instance.RunFlag.ToString() + " " + PlayerMoveController.Instance.JumpFlag.ToString() + " "
           + PlayerMoveController.Instance.isOnground.ToString() + " " + PlayerInfo.Instance.Bullets_[0].ToString() + " "
           + PlayerInfo.Instance.Bullets_[1].ToString() + " " + PlayerInfo.Instance.currentHP.ToString() + " "
           + PlayerController.Instance.currentBullet[0].ToString() + " " + PlayerController.Instance.currentBullet[1].ToString() + " " + PlayerInfo.Instance.grade.ToString() + " "
           + PlayerInfo.Instance.exp.ToString();
        try
        {
            GManager.Instance.SendMessageByTcp(msg);
        }
        catch (Exception)
        {
            print("没联网吧");
        }
    }
    //实时信息
    private void SendPlayerZT()
    {
        string msg = "S1 " + UIManager.account + " " + this.transform.position.x + " "
                + this.transform.position.y + " " + this.transform.position.z + " "
                + this.transform.eulerAngles.x + " " + this.transform.eulerAngles.y + " "
                + this.transform.eulerAngles.z + " " + PlayerController.Instance.shooting.ToString() + " " + PlayerController.Instance.shooting2.ToString()
                + " " + PlayerController.Instance.IsChangeBullet.ToString() + " " + PlayerController.Instance.currentWeapon.ToString() + " "
                + PlayerMoveController.Instance.RunFlag.ToString() + " " + PlayerMoveController.Instance.JumpFlag.ToString() + " "
                + PlayerMoveController.Instance.isOnground.ToString() + " " + PlayerMoveController.Instance.IsDeath.ToString();
        
        try
        {
            GManager.Instance.SendMessageByUdp(msg);
            timer -= Time.deltaTime;
            if (timer <= 0 && currentHP > 0)
            {
                string msg1 = "P1 " + UIManager.account + " " + this.transform.position.x + " " + this.transform.position.z;
                GManager.Instance.SendMessageByTcp(msg1);
                timer = 0.6f;
            }
        }
        catch (Exception)
        {
        }
    }

    //动画控制
    private void AnimatorControl()
    {
        if (PlayerAnimator == null) return;

        PlayerAnimator.SetInteger("RunFlag", PlayerMoveController.Instance.RunFlag);
        PlayerAnimator.SetBool("JumpFlag", PlayerMoveController.Instance.JumpFlag);
        PlayerAnimator.SetBool("isOnground", PlayerMoveController.Instance.isOnground);
        PlayerAnimator.SetBool("IsDeath", PlayerMoveController.Instance.IsDeath);
        PlayerAnimator.SetBool("shooting", PlayerController.Instance.shooting);
        PlayerAnimator.SetBool("shooting2", PlayerController.Instance.shooting2);
        PlayerAnimator.SetBool("FireModel_Signal", PlayerController.Instance.FireModel_Signal[PlayerController.Instance.currentWeapon]);
        PlayerAnimator.SetBool("IsChangeBullet", PlayerController.Instance.IsChangeBullet);
        PlayerAnimator.SetInteger("GunFlag", PlayerController.Instance.currentWeapon);
    }
}
