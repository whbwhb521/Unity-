using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 角色控制器，主要控制除移动外的所有操作
 */

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private Animator PlayerAnimator;     //Player的动画
    private float RotateLevel = 3.0f;    //旋转的力度
    private Transform m_Transform;       //Player的tranform
    //换子弹动画
    private string[] reloads_anim = { "AnyState_Player_Shooting.infantry_combat_reload", "AnyState_Player_Shooting.handgun_combat_reload" };

    //动画参数
    public bool shooting = false;      //对应动画中的参数-是否射击
    public bool shooting2 = false;     //对应动画中的参数-是否射击2
    public bool IsChangeBullet = false;//对应动画中的参数-是否换弹
    public bool isMagic = false;       //对应动画中的参数-是否使用魔法剑
    public bool isMagicCD = false;     //魔法剑是否CD

    //子弹相关
    public bool[] FireModel_Signal;  //是否单发
    public int[] currentBullet = { 30,30,0};      //当前数量初始30发子弹
    private int[] maxBullet;          //最大容量
    private int[] Damages;            //子弹威力

    //声音相关
    public AudioSource m_AudioSource;     //音源
    private AudioClip[] shot_hand_gun_sound;    //枪声
    private AudioClip[] ChangeBullet_Sound;     //换子弹

    //特效相关
    private ParticleSystem[] ps;

    //射线相关
    public LineRenderer lineRender;         //射线

    //武器相关
    public GameObject[] Weapons_;          //3把武器集合
    public int currentWeapon;             //当前武器 ----也对应着动画中的参数GunFlag
    private float change_time;             //切换武器时间>一个数值才切换

    //UI相关
    private Text BulletNum;                //子弹UI
    private RawImage[] hert;               //准心颜色
    private RawImage weapon;               //武器-组件
    private Texture2D[] weapons_texture;   //武器纹理
    private Text weapon_text;              //武器名-组件
    private string[] weapon_names;         //武器名
    private Text FireModel_text;           //开火模式

    

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        PlayerAnimator = GetComponent<Animator>();
        BulletNum = GameObject.Find("BulletNum").GetComponent<Text>();
        
        currentWeapon = 0;
        
        FireModel_Signal = new bool[] {  Weapons_[0].GetComponent<MainWeaponHit>().FireModel_Signal,
        Weapons_[1].GetComponent<SecondWeaponHit>().FireModel_Signal,
        Weapons_[2].GetComponent<ThridWeaponHit>().FireModel_Signal};
        
        maxBullet = new int[] { Weapons_[0].GetComponent<MainWeaponHit>().maxBullet,
        Weapons_[1].GetComponent<SecondWeaponHit>().maxBullet,
        Weapons_[2].GetComponent<ThridWeaponHit>().maxBullet};

        shot_hand_gun_sound = new AudioClip[] { Weapons_[0].GetComponent<MainWeaponHit>().shot_hand_gun,
        Weapons_[1].GetComponent<SecondWeaponHit>().shot_hand_gun};

        ChangeBullet_Sound = new AudioClip[] { Weapons_[0].GetComponent<MainWeaponHit>().changeBullet,
        Weapons_[1].GetComponent<SecondWeaponHit>().changeBullet};

        ps = new ParticleSystem[] { Weapons_[0].GetComponent<MainWeaponHit>().ps,
        Weapons_[1].GetComponent<SecondWeaponHit>().ps};

        Damages = new int[] { Weapons_[0].GetComponent<MainWeaponHit>().Damage,
        Weapons_[1].GetComponent<SecondWeaponHit>().Damage,
        Weapons_[2].GetComponent<ThridWeaponHit>().Damage};

        BulletNum.text = currentBullet[0].ToString();
        hert = new RawImage[]{ GameObject.Find("hert0").GetComponent<RawImage>(),
            GameObject.Find("hert1").GetComponent<RawImage>(),
            GameObject.Find("hert2").GetComponent<RawImage>(),
            GameObject.Find("hert3").GetComponent<RawImage>()};

        weapon = GameObject.Find("weapon_pic").GetComponent<RawImage>();
        weapon_text = GameObject.Find("weapon_text").GetComponent<Text>();
        weapon_names = new string[] { Weapons_[0].GetComponent<MainWeaponHit>().weaponName,
            Weapons_[1].GetComponent<SecondWeaponHit>().weaponName,
            Weapons_[2].GetComponent<ThridWeaponHit>().weaponName};

        weapons_texture = new Texture2D[] { Weapons_[0].GetComponent<MainWeaponHit>().AK_texture,
        Weapons_[1].GetComponent<SecondWeaponHit>().Glock_texture,
        Weapons_[2].GetComponent<ThridWeaponHit>().Kinfe_texture};
        FireModel_text = GameObject.Find("FireModel_text").GetComponent<Text>();
        m_AudioSource.volume = UIManager.vol;
    }

    // Update is called once per frame
    void Update()
    {
        ShootingControl();
        ChangeBullet();
        ChangeWeapon();
        if (m_AudioSource.volume != UIManager.vol)
        {
            m_AudioSource.volume = UIManager.vol;
            m_AudioSource.volume = UIManager.vol;
        }
    }

    private void FixedUpdate()
    {
        Turn();
    }

    //换弹控制
    void ChangeBullet()
    {
        if (maxBullet[currentWeapon] != 0) 
        {
            //子弹不足最大容量，且子弹大于0，且点击了R键，则换弹
            if ((Input.GetKeyDown(KeyCode.R) && currentBullet[currentWeapon] < maxBullet[currentWeapon] && this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] > 0) || IsChangeBullet)
            {
                IsChangeBullet = true;
                if (PlayerAnimator.GetCurrentAnimatorStateInfo(1).IsName(reloads_anim[currentWeapon])
                        && PlayerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.5f)
                {
                    print("换弹1");
                    //后备子弹+现有子弹小于弹夹容量
                    if (this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] + currentBullet[currentWeapon] < maxBullet[currentWeapon])
                    {
                        this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] = 0;
                        currentBullet[currentWeapon] += this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon];
                    }
                    else 
                    {
                        this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] -= (maxBullet[currentWeapon] - currentBullet[currentWeapon]);                    
                        currentBullet[currentWeapon] = maxBullet[currentWeapon];

                    }
                    BulletNum.text = currentBullet[currentWeapon].ToString();
                    IsChangeBullet = false;
                }

            }
            //弹夹子弹打光，后备子弹大于0
            else if (currentBullet[currentWeapon] <= 0 && this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] > 0)
            {
                IsChangeBullet = true;
                if (PlayerAnimator.GetCurrentAnimatorStateInfo(1).IsName(reloads_anim[currentWeapon])
                        && PlayerAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.5f)
                {
                    print("换弹2");
                    //后备子弹大于弹夹容量
                    if (this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] >= maxBullet[currentWeapon])
                    {
                        this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] -= maxBullet[currentWeapon];
                        currentBullet[currentWeapon] = maxBullet[currentWeapon];
                    }
                    else 
                    {                        
                        this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon] = 0;
                        currentBullet[currentWeapon] = this.gameObject.GetComponent<PlayerInfo>().Bullets_[currentWeapon];
                    }
                    BulletNum.text = currentBullet[currentWeapon].ToString();
                    IsChangeBullet = false;
                }
            }
        }
    }
    //开火控制
    void ShootingControl()
    {
        //是否显示射线
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            lineRender.enabled = !lineRender.enabled;
            EmenyInfo.Instance.lineRender.enabled = !EmenyInfo.Instance.lineRender.enabled;
        }
        //可以选择开火模式且点击 了B
        if (Input.GetKeyDown(KeyCode.B) && currentWeapon==0) 
        {
            //按B切换开火。
            FireModel_Signal[0] = !FireModel_Signal[0];
            if (FireModel_Signal[0])
            {
                FireModel_text.text = "单发";
            }
            else 
            {
                FireModel_text.text = "连发";
            }
        }
        
        //当前武器发射模式单发
        if (FireModel_Signal[currentWeapon])
        {
            if (Input.GetMouseButtonDown(0) && !IsChangeBullet && currentBullet[currentWeapon] > 0)
            {
                shooting = true;
            }
            if (Input.GetMouseButtonDown(1) && !isMagicCD) 
            {
                isMagic = true;
            }
        }
        else //包括了连发武器和刀
        {
            if (Input.GetMouseButton(0) && !IsChangeBullet && (currentWeapon == 2 || currentBullet[currentWeapon] > 0))
            {
                shooting = true;
            }
            
            if (Input.GetMouseButton(1) && !IsChangeBullet)//刀的右击，或者连发武器的右击操作
            {
                shooting2 = true;
            } 
        }
    }
    //射击动画中的脚本回调函数
    public void GetShootEndTime()
    {
        if (!IsChangeBullet)
        {
            PlayGunSound();
            currentBullet[currentWeapon]--;
            BulletNum.text = currentBullet[currentWeapon].ToString();
            ShowRay(1000);
            shooting = false;
        }
    }
    //射击动画结束的脚本
    public void ShootEndTime() 
    {
        hert[0].color = Color.green;
        hert[1].color = Color.green;
        hert[2].color = Color.green;
        hert[3].color = Color.green;
    }
    //开枪声音和开枪特效
    private void PlayGunSound()
    {
        m_AudioSource.clip = shot_hand_gun_sound[currentWeapon];
        m_AudioSource.Play();
        ps[currentWeapon].Play();
    }
    //换子弹动画回调
    public void ChangeBulletStartTime() 
    {
        PlayChangeBulletSound();
    }
    //换子弹声音
    private void PlayChangeBulletSound() 
    {
        m_AudioSource.clip = ChangeBullet_Sound[currentWeapon];
        m_AudioSource.Play();
    }

    //转身
    private void Turn()
    {
        float KeyMouseX = Input.GetAxis("Mouse X");
        //人左右转
        m_Transform.Rotate(Vector3.up, RotateLevel * KeyMouseX);
    }

    //刀左击结束
    public void OnKinfeAttackEnd() 
    {
        shooting = false;
        ShowRay(2.0f);
    }
    //刀右击结束
    public void OnKinfeAttackBEnd() 
    {
        shooting2 = false;
        ShowRay(2.0f);

    }
    //换武器
    private void ChangeWeapon() 
    {
        change_time += Time.deltaTime;
        //切换成主武器
        if (Input.GetKeyDown(KeyCode.Q) && currentWeapon != 0) 
        {
            Weapons_[currentWeapon].SetActive(false);
            Weapons_[0].SetActive(true);
            currentWeapon = 0;
            BulletNum.text = currentBullet[currentWeapon].ToString();
            weapon_text.text = weapon_names[currentWeapon];
            weapon.texture = weapons_texture[currentWeapon];
            IsChangeBullet = false;         
            m_AudioSource.Stop();
        }
        //向上盘
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && change_time > 0.2) 
        {
            Weapons_[currentWeapon].SetActive(false);
            currentWeapon = (currentWeapon + 1) % 3;
            Weapons_[currentWeapon].SetActive(true);         
            change_time = 0;
            BulletNum.text = currentBullet[currentWeapon].ToString();
            weapon_text.text = weapon_names[currentWeapon];
            weapon.texture = weapons_texture[currentWeapon];
            IsChangeBullet = false;
            m_AudioSource.Stop();
        }
        //向下盘
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && change_time > 0.2) 
        {
            Weapons_[currentWeapon].SetActive(false);
            currentWeapon = (currentWeapon - 1) % 3;
            if (currentWeapon < 0) 
            {
                currentWeapon += 3;
            }
            Weapons_[currentWeapon].SetActive(true);
            change_time = 0;
            BulletNum.text = currentBullet[currentWeapon].ToString();
            weapon_text.text = weapon_names[currentWeapon];
            weapon.texture = weapons_texture[currentWeapon];
            IsChangeBullet = false;
            m_AudioSource.Stop();
        }
        if (currentWeapon == 0)
        {
            FireModel_text.gameObject.SetActive(true);
        }
        else 
        {
            FireModel_text.gameObject.SetActive(false);
        }
    }
    //武器的射线
    private void ShowRay(float distance)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        LayerMask mask = ~(1 << 9); //打开除9之外的层
        if (Physics.Raycast(ray, out Hit, distance, mask))
        {
            lineRender.SetPosition(0, Weapons_[currentWeapon].transform.position);
            lineRender.SetPosition(1, Hit.point);
            object[] paras = { Hit.collider, Damages[currentWeapon] };
            GameObject.Find(Hit.collider.name).SendMessage("BeHit", paras, SendMessageOptions.DontRequireReceiver);
            if (Hit.transform.tag == "enemy")
            {
                hert[0].color = Color.red;
                hert[1].color = Color.red;
                hert[2].color = Color.red;
                hert[3].color = Color.red;
            }
        }
    }

    
}
