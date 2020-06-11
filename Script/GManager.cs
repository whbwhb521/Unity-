using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 消息的管理器，用来收发消息
     */
public class GManager : MonoBehaviour
{
    public static GManager Instance;
    public static int SceneFlag = -1;
    public static string LoginSceneName = "Login";
    public static string MainSceneName = "SampleScene";

    private Queue<string> messageQueueTCP = new Queue<string>();
    private Queue<string> messageQueueUDP = new Queue<string>();

    void Awake()
    {
        Instance = this;
        Screen.fullScreen = false;
        DontDestroyOnLoad(this.gameObject);
        ClientTcp.StartThread();
        ClientUDP.StartThread();
    }
    public void SendMessageByTcp(string msg) 
    {
        ClientTcp.SendMsg(msg);
    }

    public void SendMessageByUdp(string msg)
    {
        ClientUDP.SendMsg(msg);
    }

    void Update()
    {
        if (messageQueueTCP.Count > 0) 
        {
            string msg = messageQueueTCP.Dequeue();
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name.Equals(LoginSceneName))
            {
                LoginScene.msg = msg;
                //场景为登录
                SceneFlag = 0;
            }
            else if (scene.name.Equals(MainSceneName)) 
            {
                UIManager.msgtcp = msg;
                //场景为战斗
                SceneFlag = 1;
            }
        }
        if (messageQueueUDP.Count > 0)
        {
            string msg = messageQueueUDP.Dequeue();
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name.Equals(MainSceneName))
            {
                UIManager.msgudp = msg;
                //场景为战斗
                SceneFlag = 1;
            }
        }
    }

    public void AddMessageTCP(string msg) 
    {
        messageQueueTCP.Enqueue(msg);
    }

    public void AddMessageUDP(string msg)
    {
        messageQueueUDP.Enqueue(msg);
    }
    //关闭服务
    private void OnApplicationQuit()
    {
        print("OnApplicationQuit");
        if (SceneFlag == 1) 
        {
            PlayerInfo.Instance.SendPlayerInfo();
            GManager.Instance.SendMessageByTcp("E1 " + UIManager.account + " ");
            GManager.Instance.SendMessageByUdp("E1 " + UIManager.account + " ");
        }
        
        ClientTcp.Close();
        ClientUDP.Close();
    }
}
