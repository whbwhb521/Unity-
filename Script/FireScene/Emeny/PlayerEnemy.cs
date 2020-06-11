
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 其他的客户端，即队友
 */
public class PlayerEnemy : MonoBehaviour
{
    private string _acco;

    private Animator PlayerAnimator;     //Player的动画
    //动画参数
    private bool shooting = false;      //对应动画中的参数-是否射击
    private bool shooting2 = false;     //对应动画中的参数-是否射击2
    private bool IsChangeBullet = false;//对应动画中的参数-是否换弹
    private bool isMagic = false;       //对应动画中的参数-是否使用魔法剑
    private bool isMagicCD = false;     //魔法剑是否CD
    private bool IsDeath = false;       //对应动画中的参数-是否死亡
    private int RunFlag = -1;
    private bool JumpFlag = false;
    private bool isOnground = true;
    //声音相关
    public AudioSource[] m_AudioSource;     //音源
    private AudioClip[] shot_hand_gun_sound;    //枪声
    private AudioClip[] ChangeBullet_Sound;     //换子弹
    public AudioClip[] FootStep_Sound;          //脚步声
    public AudioClip JumpStart_Sound;           //起跳声
    public AudioClip JumpLand_Sound;            //落地声
    //子弹相关
    private bool[] FireModel_Signal;  //是否单发
    //特效相关
    private ParticleSystem[] ps;
    //武器相关
    public GameObject[] Weapons_;          //3把武器集合
    private int currentWeapon;             //当前武器 ----也对应着动画中的参数GunFlag

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = 0;
        PlayerAnimator = GetComponent<Animator>();
        ps = new ParticleSystem[] { Weapons_[0].GetComponent<MainWeaponHit>().ps,
        Weapons_[1].GetComponent<SecondWeaponHit>().ps};
        shot_hand_gun_sound = new AudioClip[] { Weapons_[0].GetComponent<MainWeaponHit>().shot_hand_gun,
        Weapons_[1].GetComponent<SecondWeaponHit>().shot_hand_gun};

        ChangeBullet_Sound = new AudioClip[] { Weapons_[0].GetComponent<MainWeaponHit>().changeBullet,
        Weapons_[1].GetComponent<SecondWeaponHit>().changeBullet};
      
        FireModel_Signal = new bool[] {  Weapons_[0].GetComponent<MainWeaponHit>().FireModel_Signal,
        Weapons_[1].GetComponent<SecondWeaponHit>().FireModel_Signal,
        Weapons_[2].GetComponent<ThridWeaponHit>().FireModel_Signal};
        m_AudioSource[0].volume = UIManager.vol;
        m_AudioSource[1].volume = UIManager.vol;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AudioSource[0].volume != UIManager.vol) 
        {
            m_AudioSource[0].volume = UIManager.vol;
            m_AudioSource[1].volume = UIManager.vol;
        }
    }
    
    //设置info
    public void SetAnimatorInfo(bool _shooting,bool _shooting2, bool _IsChangeBullet ,int cWeapon, int _RunFlag, bool _JumpFlag, bool _isOnground, bool _isDeath) 
    {
        currentWeapon = cWeapon;
        shooting = _shooting;
        shooting2 = _shooting2;
        IsChangeBullet = _IsChangeBullet;
        RunFlag = _RunFlag;
        JumpFlag = _JumpFlag;
        isOnground = _isOnground;
        IsDeath = _isDeath;
        SetGun();

    }

    //设置枪支
    private void SetGun() 
    {
        foreach (GameObject Weapon in Weapons_) 
        {
            Weapon.SetActive(false);
        }
        Weapons_[currentWeapon].SetActive(true);
    }
        
    //设置帐号
    public void SetAcco(string acco) 
    {
        _acco = acco;
    }
    //销毁
    public void Destory() 
    {
        GameObject.Destroy(this.gameObject);
    }
    //移动
    public void Move(Vector3 pos, Vector3 rot)
    {
        if (pos != transform.position)
        {
            transform.position = pos;
        }
        else 
        {
            
        }

        transform.eulerAngles = rot;
    }

    //射击动画中的脚本回调函数
    public void GetShootEndTime()
    {
        PlayGunSound();
    }
    //开枪声音和开枪特效
    private void PlayGunSound()
    {
        m_AudioSource[0].clip = shot_hand_gun_sound[currentWeapon];
        m_AudioSource[0].Play();
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
        m_AudioSource[0].clip = ChangeBullet_Sound[currentWeapon];
        m_AudioSource[0].Play();
    }
    //左脚踩地回调
    public void LeftFootStepTime()
    {
        PlayFootStepSound();
    }
    //右脚踩地回调
    public void RightFootStepTime()
    {
        PlayFootStepSound();
    }
    //脚步的声音
    private void PlayFootStepSound()
    {
        if (isOnground)
        {
            int n = Random.Range(1, FootStep_Sound.Length);
            m_AudioSource[1].clip = FootStep_Sound[n];
            m_AudioSource[1].PlayOneShot(m_AudioSource[1].clip);
            // move picked sound to index 0 so it's not picked next time
            FootStep_Sound[n] = FootStep_Sound[0];
            FootStep_Sound[0] = m_AudioSource[1].clip;
        }
    }

    //起跳回调
    public void JumpStartTime()
    {
        m_AudioSource[1].clip = JumpStart_Sound;
        m_AudioSource[1].Play();
    }
    //落地回调
    public void JumpLandTime()
    {
        m_AudioSource[1].clip = JumpLand_Sound;
        m_AudioSource[1].Play();
    }

    public void ShootEndTime() { }

    //动画控制
    public void AnimatorControl()
    {
        if (PlayerAnimator == null) return;

        PlayerAnimator.SetBool("shooting", shooting);
        PlayerAnimator.SetBool("shooting2", shooting2);
        PlayerAnimator.SetBool("FireModel_Signal", FireModel_Signal[currentWeapon]);
        PlayerAnimator.SetBool("IsChangeBullet", IsChangeBullet);
        PlayerAnimator.SetInteger("GunFlag", currentWeapon);

        PlayerAnimator.SetInteger("RunFlag", RunFlag);
        PlayerAnimator.SetBool("JumpFlag", JumpFlag);
        PlayerAnimator.SetBool("isOnground", isOnground);
        PlayerAnimator.SetBool("IsDeath", IsDeath);
    }
}
