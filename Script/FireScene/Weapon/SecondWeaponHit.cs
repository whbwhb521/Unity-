using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 副武器的一些信息
     */
public class SecondWeaponHit : MonoBehaviour
{
    

    //子弹相关
    public int maxBullet = 30;            //最大容量
    //声音相关
    public AudioClip shot_hand_gun;       //枪声

    public AudioClip changeBullet;        //换弹声
    //特效相关
    public ParticleSystem ps;

    //换弹的一些参数，暂时没有

    //
    public int Damage = 60;

    public bool hasFireModel = false; //是否有开火模式选择

    public bool FireModel_Signal = true; //默认单发开火

    public string weaponName = "格洛克";  //枪的名字

    public Texture2D Glock_texture;
}
