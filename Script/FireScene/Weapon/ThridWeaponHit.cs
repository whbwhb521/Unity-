using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 刀类武器的一些信息
     */
public class ThridWeaponHit : MonoBehaviour
{
    //换弹的一些参数，暂时没有

    //
    public int Damage = 200;

    public int maxBullet = 0;            //手枪无子单

    public bool hasFireModel = false; //是否有开火模式选择

    public bool FireModel_Signal = false; //默认单发开火

    public string weaponName = "蝴蝶刀";  //刀的名字

    public Texture2D Kinfe_texture;
}
