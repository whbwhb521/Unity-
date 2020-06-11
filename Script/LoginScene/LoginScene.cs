using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * 登录界面的所有操作
 */
public class LoginScene : MonoBehaviour
{
    public static string msg;
    public InputField Account_Input;//输入账号
    public InputField Password_Input;//输入密码
    public InputField Reg_Password; //注册输入密码
    public InputField Reg_Account; //注册输入账号
    public InputField Reg_PasswordAgain;//注册再次输入密码
    public Text Warning; //警告文本
    public Text Reg_Warning; //注册警告文本

    public Button Login_btn;//登录按钮
    public Button Register_btn;//注册按钮
    public Button Reg_Register_btn; //注册注册按钮
    public Button Reg_Cancle; //注册取消按钮

    public Button MusicVol;//声音按钮
    public AudioSource As_Music;
    public Slider MusicSlider;//音量条
    
    public GameObject Login_plane;//登录面板
    public GameObject Reg_plane;//注册面板

    private EventSystem eventsystem;



    private void Awake()
    {
        Login_btn.onClick.AddListener(LoginCall);
        Register_btn.onClick.AddListener(RegisterCall);
        Reg_Register_btn.onClick.AddListener(Reg_RegisterCall);
        Reg_Cancle.onClick.AddListener(Reg_CancleCall);
        MusicVol.onClick.AddListener(MusicVolCall);
        MusicSlider.onValueChanged.AddListener(MusicSilderListener);
    }

    //调节音量
    private void MusicSilderListener(float arg0)
    {
        As_Music.volume = MusicSlider.value;
    }

    //点击音量按钮
    private void MusicVolCall()
    {//调出MusicSilder
        MusicSlider.gameObject.SetActive(true);
        eventsystem.SetSelectedGameObject(MusicSlider.gameObject);
    }

    //点击取消切换版面
    private void Reg_CancleCall()
    {
        //清空之前注册版面中写过的东西
        Reg_Account.text = "";
        Reg_Password.text = "";
        Reg_PasswordAgain.text = "";
        Reg_plane.SetActive(false);
        Login_plane.SetActive(true);
        Warning.gameObject.SetActive(false);
        Warning.gameObject.SetActive(false);
    }

    //点击登录面板的注册，切换面板
    private void RegisterCall()
    {
        //清空之前登录面板的东西
        Account_Input.text = "";
        Password_Input.text = "";
        Login_plane.SetActive(false);
        Reg_plane.SetActive(true);
        Warning.gameObject.SetActive(false);
        Warning.gameObject.SetActive(false);
    }

    //点击登录
    private void LoginCall()
    {
        Warning.gameObject.SetActive(true);
        Warning.color = Color.red;
        string smessage = "A1" + " " + Account_Input.text + " " + Password_Input.text;
        GManager.Instance.SendMessageByTcp(smessage);
    }

    //点击注册
    private void Reg_RegisterCall()
    {
        Reg_Warning.gameObject.SetActive(true);
        Reg_Warning.color = Color.red;
        Regex Account_reg = new Regex(@"^[a-zA-Z][a-zA-Z0-9_]{4,11}$");
        if (!Reg_Password.text.Equals(Reg_PasswordAgain.text))
        {
            Reg_Warning.text = "2次密码不一致";
        }
        else if (!Account_reg.IsMatch(Reg_Account.text)) 
        {
            Reg_Warning.text = "帐号为字母开头的5到12位字母数字下划线组合!";
        }
        else
        {
            if (Reg_Password.text.Equals(""))
            {
                Reg_Warning.text = "密码不能为空!";
            }
            else 
            {
                //发送消息给服务端进行注册，返回成功或者失败
                string smessage = "A2" + " " + Reg_Account.text + " " + Reg_Password.text;
                print(smessage);
                GManager.Instance.SendMessageByTcp(smessage);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MusicSlider.value = 0.5f;
        eventsystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        MusicSliderActiveChanged();
        disposeLoginAndRegister();
        //GameController();
    }

    private void GameController()
    {

    }

    //点击其他地方使音量条失去焦点
    private void MusicSliderActiveChanged() 
    {
        //当前焦点是MusicSlider是active的
        if (MusicSlider.gameObject.activeSelf &&
            eventsystem.currentSelectedGameObject != MusicSlider.gameObject)
        {
            MusicSlider.gameObject.SetActive(false);
        }
    }

    //根据获取的信息处理注册和登录
    private void disposeLoginAndRegister() 
    {
        
        if (GManager.SceneFlag == 0 && msg != "")
        {
            string[] _Msg = msg.Split(new char[] { ' ' }, 2);
            string flag = _Msg[0];
            Debug.Log("msg： " + msg);
            switch (flag)
            {
                case "A2":
                    Debug.Log("注册成功");
                    Reg_Warning.color = Color.green;
                    Reg_Warning.text = "注册成功!";
                    break;
                case "B2":
                    Debug.Log("注册失败");
                    Reg_Warning.color = Color.red;
                    Reg_Warning.text = "注册失败!";
                    break;
                case "B1":
                    Debug.Log("登录失败");
                    Warning.color = Color.red;
                    Warning.text = "登录失败!";
                    break;
                case "A1":
                    //登录成功
                    Debug.Log("登陆成功");
                    Warning.color = Color.green;
                    Warning.text = "登录成功!";
                    //GManager.Instance.ChangeScene(GManager.MainSceneName);
                    SceneManager.LoadScene(GManager.MainSceneName);
                    UIManager.account = Account_Input.text;
                    PlayerInfo.playerinfo = _Msg[1];
                    break;
                default:
                    Debug.Log("default： "+msg);
                    break;
            }
            msg = "";
        }
    }
}
