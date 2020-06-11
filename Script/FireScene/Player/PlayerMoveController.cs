using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 角色移动控制，控制角色的移动
 */
public class PlayerMoveController : MonoBehaviour
{

    public static PlayerMoveController Instance;
    //移动参数
    private Transform m_Transform;       //Player的tranform
    private Vector3 Player_Movement;     //角色移动距离
    private float runspeed = 0.04f;       //移动速度

    public bool IsDeath=false;                //对应动画中的参数-是否死亡
    public bool isOnground = true;     //对应动画中的参数-是否落地
    public int RunFlag = -1;           //对应动画中的参数-跑步状态
    public bool JumpFlag = false;      //对应动画中的参数-是否起跳

    //声音相关
    public AudioSource m_AudioSource;     //音源
    public AudioClip[] FootStep_Sound;          //脚步声
    public AudioClip JumpStart_Sound;           //起跳声
    public AudioClip JumpLand_Sound;            //落地声

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        m_AudioSource.volume = UIManager.vol;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInfo.Instance.currentHP <= 0)
        {
            IsDeath = true;
        }
        if (m_AudioSource.volume != UIManager.vol)
        {
            m_AudioSource.volume = UIManager.vol;
            m_AudioSource.volume = UIManager.vol;
        }
    }

    private void FixedUpdate()
    {
        MoveControl();
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
    //起跳回调
    public void JumpStartTime()
    {
        m_AudioSource.clip = JumpStart_Sound;
        m_AudioSource.Play();
    }
    //落地回调
    public void JumpLandTime()
    {
        m_AudioSource.clip = JumpLand_Sound;
        m_AudioSource.Play();
    }
    //脚步的声音
    private void PlayFootStepSound()
    {
        if (isOnground)
        {
            int n = UnityEngine.Random.Range(1, FootStep_Sound.Length);
            m_AudioSource.clip = FootStep_Sound[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            FootStep_Sound[n] = FootStep_Sound[0];
            FootStep_Sound[0] = m_AudioSource.clip;
        }
    }
    //移动控制逻辑
    private void MoveControl()
    {
        /*float KeyVertical = Input.GetAxis("Vertical");
        float KeyHorizontal = Input.GetAxis("Horizontal");*/
        float KeyVertical = Input.GetAxisRaw("Vertical");
        float KeyHorizontal = Input.GetAxisRaw("Horizontal");

        RunFlag = -1;
        Player_Movement.Set(KeyHorizontal, 0.0f, KeyVertical);
        Player_Movement.Normalize();
        m_Transform.Translate(Player_Movement * runspeed);
        //移动逻辑
        if (KeyVertical == 1.0f)
            RunFlag = 0;
        if (KeyHorizontal == -1.0f)
            RunFlag = 2;
        if (KeyHorizontal == 1.0f)
            RunFlag = 3;
        if (KeyVertical == -1.0f)
            RunFlag = 1;

        //跳跃逻辑
        if (Input.GetKey(KeyCode.Space) && JumpFlag == false && isOnground)
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 5, 0);
            this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0));
            JumpFlag = true;
            isOnground = false;
        }
    }
    //判断是否落地
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "")
        {
            isOnground = true;
            JumpFlag = false;
        }
    }
    
}
