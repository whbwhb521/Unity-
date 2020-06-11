using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()   //where T : new()为泛型约束，通俗来说就是确保T类型是可以被new的
{
    private static T _instance;     //私有的T类型的静态变量
    public static T GetInstance()       //获取实例的函数
    {
        if (_instance == null)      //判断实例是否已存在
        {
            _instance = new T();    //不存在则创建新的实例
        }
        return _instance;   //返回实例
    }
}
