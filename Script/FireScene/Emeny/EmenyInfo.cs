using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 敌人基本信息
 */
public class EmenyInfo : MonoBehaviour
{
    public static EmenyInfo Instance;
    public int currentHP = 100;          //当前血量
    private float HarmPercent;           //伤害减免百分比
    private int BeDamage;                  //受到的伤害
    Collider[] colliders;                //物体的collider
    private Animator EnemyAnimator;      //Enemy的动画

    //枪械相关-掉落物品
    public GameObject Weapon;        //武器
    public int[] WeaponBullets ;        //后备子弹数量
    private int Damage;            //子弹威力


    //动画相关
    private bool IsDeath;                //对应动画中的参数-是否死亡
    private bool IstakeDamage;           //对应动画中的参数-是否被击
    private bool IsWalk = false;       //对应移动参数----是否移动
    private bool IsAttack = false;     //对应动画参数----是否攻击
    public GameObject DropBox;          //盒子
    private GameObject DropBoxes;        //盒子容器

    //经验
    private int Allexp = 5;             //全局经验
    private int Personexp = 15;         //个人经验

    private string Enemyname;
    private Rigidbody rd;
    private float walkspeed = 0.03f;       //移动速度

    //目的地
    public Vector3 targetPos;
    public Vector3 PlayerPos;
    private Quaternion rotation;
    //击杀者
    private string Killer;
    //射线相关
    public LineRenderer lineRender;         //射线

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IsDeath = false;
        IstakeDamage = false;
        colliders = this.GetComponents<Collider>();
        EnemyAnimator = this.GetComponent<Animator>();
        WeaponBullets = new int[] { 30, 30 };
        rd = this.GetComponent<Rigidbody>();
        DropBoxes = GameObject.Find("Dropboxes");
        Damage = Weapon.GetComponent<ThridWeaponHit>().Damage;
        targetPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentHP <= 0)
        {
            IsDeath = true;
            IsAttack = false;
            IstakeDamage = false;
            IsWalk = false;
        }
        else
        {
        }

        AnimatorController();
    }
    private void FixedUpdate()
    {
        if (!IsDeath) 
        {
            Move();
        }
        
    }

    //受击
    public void BeHit(object[] paras) 
    {
        if ((Collider)paras[0] == colliders[0])
        {
            HarmPercent = 0.5f;
        } else if ((Collider)paras[0] == colliders[1]) 
        {
            HarmPercent = 2.0f;
        }
        BeDamage = (int)(HarmPercent * (int)paras[1]);
        IstakeDamage = true;
        currentHP -= BeDamage;
        print("IstakeDamage:"+ IstakeDamage);
        //发送伤害
        string msg ="H1 " + Enemyname + " " + BeDamage.ToString();
        try
        {
            GManager.Instance.SendMessageByTcp(msg);
        }
        catch (Exception)
        {
        }
        GameObject.Find("DamageShow").GetComponent<DamageCover>().GenerateText(BeDamage);
    }

    //enemy的动画控制
    private void AnimatorController() 
    {
        if (EnemyAnimator == null) return;

        EnemyAnimator.SetBool("IsDeath",IsDeath);
        EnemyAnimator.SetBool("IstakeDamage", IstakeDamage);
        EnemyAnimator.SetBool("IsWalk", IsWalk);
        EnemyAnimator.SetBool("IsAttack", IsAttack);

        if (EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Knife.knife_take_damage"))
        {
            IstakeDamage = false;
        }
    }

    //死亡动画播放完毕回调
    public void DeathEnd() 
    {   
        //盒子掉落
        GameObject GO_DropBox = Instantiate(DropBox, 
            new Vector3(this.transform.position.x, this.transform.position.y+0.5f, this.transform.position.z),
            Quaternion.identity);
        GO_DropBox.transform.parent = DropBoxes.transform;
        GO_DropBox.name = this.Enemyname + "Drop";
        GO_DropBox.gameObject.GetComponent<DropBox>().BulletsNum_ = WeaponBullets;
        /*//通知服务端删除
        string msg = "RM1 " + Enemyname + " ";
        try
        {
            GManager.Instance.SendMessageByTcp(msg);
        }
        catch (Exception)
        {
        }*/
        GameObject.Destroy(this.gameObject);
    }

    public void setName(string _name) 
    {
        Enemyname = _name;
    }

    //移动
    public void Move()
    {       
        if (transform.position.x != targetPos.x || transform.position.z != targetPos.z)
        {
            IsWalk = true;
            Vector3 direction = new Vector3(targetPos.x - transform.position.x, 0, targetPos.z - transform.position.z) ;
            rotation = Quaternion.LookRotation(direction);
            rd.MovePosition(Vector3.MoveTowards(transform.position, targetPos, walkspeed));
            this.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 6.0f);
        }
        else 
        {
            IsWalk = false;
            this.transform.LookAt(PlayerPos);
            IsAttack = true;  
        }  
    }

    public void SetBlood(int blood, string _Killer) 
    {
        currentHP = blood;
        Killer = _Killer;
        if (blood <= 0) 
        {
            UIManager.Instance.ChatText.text += (Enemyname + " killed by " + _Killer);
            PlayerInfo.Instance.exp += Allexp;
            if (Killer.Equals(UIManager.account))
            {
                PlayerInfo.Instance.exp += Personexp;
            }
        }
    }

    //武器的射线
    private void ShowRay(float distance)
    {
        RaycastHit Hit;        
        LayerMask mask = ~(1 << 10); //打开除9之外的层
        Vector3 StartPos = new Vector3(this.transform.position.x, 0.7f, this.transform.position.z);
        Ray ray = new Ray(StartPos, transform.forward * 100);
        if (Physics.Raycast(ray, out Hit, distance, mask))
        {
            lineRender.SetPosition(0, Weapon.transform.position);
            lineRender.SetPosition(1, Hit.point);
            object[] paras = { Hit.collider, Damage };
            GameObject.Find(Hit.collider.name).SendMessage("BeHit", paras, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void ShootEndTime()
    {
    }

    public void OnKinfeAttackEnd()
    {
        IsAttack = false;
        ShowRay(2.0f);
    }
}
