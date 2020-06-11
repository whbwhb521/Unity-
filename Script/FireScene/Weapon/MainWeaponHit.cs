using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 主武器一些信息，武器按照类型分为了主武器，副武器和刀。
 （其实应该按照每种武器分类，时间问题没写）
     */
public class MainWeaponHit : MonoBehaviour
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
    public int Damage = 50;

    public bool hasFireModel = true; //是否有开火模式选择

    public bool FireModel_Signal = true; //默认单发开火

    public string weaponName = "AK47";   //枪的名字

    public Texture2D AK_texture;

}
