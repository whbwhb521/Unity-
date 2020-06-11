using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/*
 TCP连接
     */
public class ClientTcp
{
    private static Socket clientSocket; //客户端套接字

    public static Thread thread;
    public static ThreadStart ts;
    public static byte[] ReadBuffer;
    public static int MaxBuffer = 30000;
    public static bool isRun = false;
    public static int Alread = 0;
    
    public ClientTcp() 
    {
        
    }

    public static void StartThread() 
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);

        try
        {
            clientSocket.Connect(ip);
        }
        catch (Exception e) 
        {
            Debug.Log("连接服务器失败:"+e.ToString());
            return;
        }
        Debug.Log("开始接受");
        ClientTcp.isRun = true;
        ClientTcp.ReadBuffer = new byte[MaxBuffer];
        ClientTcp.ts = new ThreadStart(run);
        thread = new Thread(ts);
        thread.IsBackground = true;
        thread.Start();

    }

    public static void run() 
    {
        Debug.Log("run");
        if (ClientTcp.isRun == false) return;
        try
        {
            ReadBuffer = new byte[MaxBuffer];
            clientSocket.BeginReceive(ReadBuffer, 0, MaxBuffer, 0, new AsyncCallback(receive),null);
            Debug.Log("clientSocket.BeginReceive");
        }
        catch(Exception e) 
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
                string msg = Encoding.UTF8.GetString(data,0,lenth);
                GManager.Instance.AddMessageTCP(msg);
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
        ClientTcp.thread.Abort();
        ClientTcp.clientSocket.Close();
    }

    public static void SendMsg(string msg) 
    {
        byte[] bs = Encoding.UTF8.GetBytes(msg);
        clientSocket.Send(bs);
    }
}
