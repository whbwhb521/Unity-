using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 UI管理，主要用作Canvas中物体的控制，例如：背包的显示，设置版面的显示，FPS的显示等，
 一些按键的操作，后来把服务端数据的接收也写在了这里（其实应该分开）。
     */
public class UIManager : MonoBehaviour
{
    private static int NowUI = 0; // 0:游戏UI   1. 背包UI  2.聊天UI  3.设置 
    public static UIManager Instance;
    public static string account;
    public static string msgtcp = "";
    public static string msgudp = "";
    public GameObject Player;
    public GameObject bag;
    private GameObject MyBag;
    public GameObject GLPanel;
    public InputField ChatInputField;
    public GameObject ChatPanel;
    private bool CanSeeBag = false;
    private string smessage;
    public Text ChatText;
    private GameObject ChatScrollBar;
    public GameObject OPScrollBar;
    public Button Exit_btn;  //退出按钮
    public GameObject GameSetPanel;//设置面板
    public Slider MusicSlider;//音量调
    public static float vol = 0.8f;//全局音量
    public Button Operator_btn;//操作按钮
    public GameObject panel1;
    public GameObject panel2;
    public Button return_btn;
    public Text OP_Text;
    public GameObject DeathPlane;//死亡后的面板
    public Button Death_Plane_ReLifeBtn;
    public Button Death_Plane_ExitBtn;
    private float Gtime;
    private int frames;
    public Text FPSText;

    private static string OPText = "操作说明\n 鼠标左键：开火\n W、A、S、D键：前后左右移动\n " +
        "鼠标滚轮：切枪\n Q键：切换为主武器\n R键：换子弹\n P键：增加一名AI敌人\n " +
        "空格键：跳跃\n Tab键：打开背包\n Enter键：开启聊天\n ";

    private void Awake()
    {
        Exit_btn.onClick.AddListener(ExitGame);
        MusicSlider.onValueChanged.AddListener(MusicSilderListener);
        Operator_btn.onClick.AddListener(Operator);
        return_btn.onClick.AddListener(return_op);
        Death_Plane_ReLifeBtn.onClick.AddListener(ReLife);
        Death_Plane_ExitBtn.onClick.AddListener(Exit);
        Instance = this;
        try 
        {
            GManager.Instance.SendMessageByTcp("M2 " + account + " 加入游戏");
        } catch (Exception) 
        {
            print("请联网");
        }
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void ReLife()
    {
        PlayerInfo.Instance.currentHP = 100;
        GameObject.Find("player").GetComponent<PlayerController>().enabled = true;
        GameObject.Find("player").GetComponent<PlayerMoveController>().enabled = true;
        GameObject.Find("player").GetComponent<PlayerCameraFollow>().enabled = true;
        PlayerMoveController.Instance.IsDeath = false;
        DeathPlane.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void return_op()
    {
        OPScrollBar.GetComponent<Scrollbar>().value = 0;
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    private void Operator()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        OP_Text.text = OPText;
    }

    private void MusicSilderListener(float arg0)
    {
        vol = MusicSlider.value;
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GLPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        bag.transform.localPosition = new Vector3(1000, 1000, 0);
        MyBag = bag.transform.Find("Bag").gameObject;
        ChatScrollBar = ChatPanel.transform.Find("ChatScrollBar").gameObject;
        MusicSlider.value = 0.8f;
        StartCoroutine(FPS());
    }

    IEnumerator FPS()
    {
        while (true) 
        {
            float fps = frames/ Gtime;
            FPSText.text ="FPS:" + fps.ToString();
            frames = 0;
            Gtime = 0;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        BagControll();
        EscController();
        ChatController();
        CursorController();
        DisPoseMessage();
        AddEnemy();
        Gtime += Time.deltaTime;
        ++frames;
    }



    private void AddEnemy()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            try
            {
                GManager.Instance.SendMessageByTcp("P2 xx 0");
            }
            catch (Exception) 
            {               
            }
            
        }
    }

    private void FixedUpdate()
    {       
    }

    private void CursorController()
    {   if (PlayerInfo.Instance.currentHP > 0) 
        {
            if (NowUI == 1)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameObject.Find("player").GetComponent<PlayerController>().enabled = false;
            }
            else if (NowUI == 2)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameObject.Find("player").GetComponent<PlayerController>().enabled = false;
                GameObject.Find("player").GetComponent<PlayerMoveController>().enabled = false;
            }
            else if (NowUI == 3)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameObject.Find("player").GetComponent<PlayerController>().enabled = false;
                GameObject.Find("player").GetComponent<PlayerMoveController>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                GameObject.Find("player").GetComponent<PlayerController>().enabled = true;
                GameObject.Find("player").GetComponent<PlayerMoveController>().enabled = true;
            }
        }
        
    }



    //点击回车键
    private void ChatController()
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            //已经打开了,发送并关闭
            if (NowUI == 2)
            {
                ChatInputField.gameObject.SetActive(false);
                ChatPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
                if (ChatInputField.text != "") 
                {
                    //点击发送，发送到服务端
                    smessage = "M1" + " " + account + " " + ChatInputField.text;
                    try
                    {
                        GManager.Instance.SendMessageByTcp(smessage);
                    }
                    catch (Exception)
                    {
                        ChatText.text += "无法连接服务器\n";
                        ChatScrollBar.GetComponent<Scrollbar>().value = 0;
                    }
                    smessage = "";
                }
                
                if (CanSeeBag) 
                {
                    NowUI = 1;
                } 
                else 
                {
                    NowUI = 0;
                } 
            }
            //打开
            else 
            {
                ChatInputField.gameObject.SetActive(true);
                ChatPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);                
                NowUI = 2;
                ChatInputField.text = "";
                //获取焦点
                ChatInputField.ActivateInputField();
            }
            print("NowUI:" + NowUI);
        }
        
    }

    private void EscController()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (NowUI == 1)
            {
                closeBag();
            }
            //调出设置
            else if (NowUI == 0)
            {
                GameSetPanel.SetActive(true);
                NowUI = 3;
            }
            else if (NowUI == 2)
            {
                closeChat();
            }
            else if (NowUI == 3) 
            {
                GameSetPanel.SetActive(false);
                NowUI = 0;
            }
        }
    }


    //关闭聊天框
    private void closeChat()
    {
        ChatPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
        ChatInputField.gameObject.SetActive(false);
        ChatInputField.text = "";
        if (CanSeeBag) NowUI = 1;
        else NowUI = 0;
    }

    private void BagControll()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && NowUI != 2) 
        {
            //bag处于打开状态，则关闭
            if (NowUI == 1)
            {
                closeBag();
            }
            //bag处于关闭状态，则打开
            else 
            {
                bag.transform.localPosition = new Vector3(0, 0, 0);
                //更新包裹
                MyBag.SendMessage("UpdateBag", Player.GetComponent<PlayerInfo>().Bullets_, SendMessageOptions.RequireReceiver);
                NowUI = 1;
                CanSeeBag = true;
            }
        }      
    }

    //关闭背包
    private void closeBag() 
    {
        bag.transform.localPosition = new Vector3(1000, 1000, 0);
        NowUI = 0;
        CanSeeBag = false;
    }

    

    private void DisPoseMessage()
    {
        if (GManager.SceneFlag == 1 && !msgtcp.Equals(""))
        {
            string[] args = msgtcp.Split(new char[] { ' ' }, 3);
            switch (args[0])
            {
                //发送的消息
                case "M1":
                    ChatText.text += args[1] + ": " + args[2] + "\n";
                    ChatScrollBar.GetComponent<Scrollbar>().value = 0;
                    break;
                //有人加入游戏
                case "M2":
                    bool successAdd = PlayerEnemyManager.Instance.AddPlayerEnemy(args[1]);
                    if (successAdd)
                    {
                        ChatText.text += args[1] + ": " + args[2] + "\n";
                        ChatScrollBar.GetComponent<Scrollbar>().value = 0;
                    }
                    break;
                //寻路
                case "P1":

                    EmenyManager.Instance.AddEnemy(args[1], args[2]);
                    try 
                    {
                        EmenyManager.Instance.UpDatePlayerEnemyPos(args[1], args[2]);
                    }
                    catch (Exception)
                    {
                        print(args[2]);
                    }
                    break;
                //更新敌人血量
                case "H1":
                    string[] args2 = args[2].Split(new char[] { ' ' },2);
                    try
                    {
                        EmenyManager.Instance.UpDatePlayerEnemyBlood(args[1], Convert.ToInt32(args2[0]), args2[1]);
                    }
                    catch (Exception)
                    {
                    }
                    break;
                //玩家死亡
                case "D1":
                    ChatText.text += "玩家 " + args[1] + "死亡\n";
                    break;
                //玩家退出
                case "E1":
                    ChatText.text += "玩家 " + args[1] + "退出游戏\n";
                    PlayerEnemyManager.Instance.RemovePlayerEnemy(args[1]);
                    break;
            }
            msgtcp = "";
        } 
        if (GManager.SceneFlag == 1 && !msgudp.Equals(""))
        {
            string[] args = msgudp.Split(new char[] { ' ' }, 3);
            switch (args[0]) 
            { 
                //更新玩家信息
                case "S1":
                    //(帐号，位置，朝向，是否射击，是否射击2，是否换弹，是否使用魔法剑，魔法剑是否CD
                    //，当前武器,是否死亡)
                    if (!UIManager.account.Equals(args[1]))
                    {
                        PlayerEnemyManager.Instance.AddPlayerEnemy(args[1]);
                        string[] args1 = args[2].Split(new char[] { ' ' });
                        try
                        {
                            Vector3 pos = new Vector3(Convert.ToSingle(args1[0]), Convert.ToSingle(args1[1]), Convert.ToSingle(args1[2]));
                            Vector3 rot = new Vector3(Convert.ToSingle(args1[3]), Convert.ToSingle(args1[4]), Convert.ToSingle(args1[5]));
                            bool shooting = Convert.ToBoolean(args1[6]);
                            bool shooting2 = Convert.ToBoolean(args1[7]);
                            bool isChangeBullet = Convert.ToBoolean(args1[8]);
                            int currentWeapon = Convert.ToInt32(args1[9]);
                            int RunFlag = Convert.ToInt32(args1[10]);
                            bool JumpFlag = Convert.ToBoolean(args1[11]);
                            bool isOnground = Convert.ToBoolean(args1[12]);
                            bool isDeath = Convert.ToBoolean(args1[13]);
                            PlayerEnemyManager.Instance.UpDatePlayerEnemy(args[1], pos, rot, shooting, shooting2,
                                isChangeBullet, false, false, currentWeapon, RunFlag, JumpFlag, isOnground, isDeath);
                        }
                        catch (Exception)
                        {
                            print(args[2]);
                        }
                    }
                    break;                                
            }
            msgudp = "";
        }
    }
}
