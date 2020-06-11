using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
/*
 UDP连接
     */
public class ClientUDP
{
    private static Socket clientSocket; //客户端套接字
    public static Thread thread;
    public static ThreadStart ts;
    public static byte[] ReadBuffer;
    public static int MaxBuffer = 10240;
    public static bool isRun = false;
    public static int Alread = 0;

    public ClientUDP()
    {

    }

    public static void StartThread()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9998);

        try
        {
            clientSocket.Connect(ip);
        }
        catch (Exception e)
        {
            Debug.Log("连接服务器失败:" + e.ToString());
            return;
        }
        Debug.Log("开始接受");
        ClientUDP.isRun = true;
        ClientUDP.ReadBuffer = new byte[MaxBuffer];
        ClientUDP.ts = new ThreadStart(run);
        thread = new Thread(ts);
        thread.IsBackground = true;
        thread.Start();
    }

    public static void run()
    {
        Debug.Log("run");
        if (ClientUDP.isRun == false) return;
        try
        {
            ReadBuffer = new byte[MaxBuffer];
            clientSocket.BeginReceive(ReadBuffer, 0, MaxBuffer, 0, new AsyncCallback(receive), null);
            Debug.Log("clientSocket.BeginReceive");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void receive(IAsyncResult ar)
    {
        int lenth = 0;
        try
        {
            lenth = clientSocket.EndReceive(ar);
            if (lenth > 0)
            {
                byte[] data = new byte[lenth];
                Array.Copy(ReadBuffer, 0, data, 0, lenth);
                string msg = Encoding.UTF8.GetString(data, 0, lenth);
                GManager.Instance.AddMessageUDP(msg);
            }
            clientSocket.BeginReceive(ReadBuffer, 0, MaxBuffer, 0, new AsyncCallback(receive), null);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public static void Close()
    {
        isRun = false;
        ClientUDP.thread.Abort();
        ClientUDP.clientSocket.Close();
    }

    public static void SendMsg(string msg)
    {
        byte[] bs = Encoding.UTF8.GetBytes(msg);
        clientSocket.Send(bs);
    }
}
