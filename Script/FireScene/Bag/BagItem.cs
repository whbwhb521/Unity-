using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *  背包
 */
public class BagItem : MonoBehaviour
{

    public GameObject[] blocks;
    public Sprite Item_MainBulletSprite;
    public Sprite Item_SecondBulletSprite;
    private int MainWeaponBullet;
    private int SecondWeaponBullet;


    //获取空格子
    public Transform GetEmptyGrid() 
    {
        for (int i = 0; i < blocks.Length; i++) 
        {
            if (blocks[i].activeSelf == false) 
            {
                return blocks[i].transform;
            }
        }
        return null;
    }

    private void Start()
    {
    }
    private void Update()
    {
    }

    //更新背包
    public void UpdateBag(int[] BulletsNum_) 
    {
        ClearBag();
        MainWeaponBullet = BulletsNum_[0];
        SecondWeaponBullet = BulletsNum_[1];

        Transform emptyTS = GetEmptyGrid();
        while (MainWeaponBullet > 30 && emptyTS != null) 
        {
            emptyTS.Find("Image").GetComponent<Image>().sprite = Item_MainBulletSprite;
            emptyTS.Find("Text").GetComponent<Text>().text = "30";
            emptyTS.gameObject.SetActive(true);
            MainWeaponBullet -= 30;
            emptyTS = GetEmptyGrid();
        }
        if (emptyTS != null && MainWeaponBullet > 0) 
        {
            emptyTS.Find("Image").GetComponent<Image>().sprite = Item_MainBulletSprite;
            emptyTS.Find("Text").GetComponent<Text>().text = MainWeaponBullet.ToString();
            emptyTS.gameObject.SetActive(true);
            emptyTS = GetEmptyGrid();
        }
        while (SecondWeaponBullet > 30 && emptyTS != null) 
        {
            emptyTS.Find("Image").GetComponent<Image>().sprite = Item_SecondBulletSprite;
            emptyTS.Find("Text").GetComponent<Text>().text = "30";
            emptyTS.gameObject.SetActive(true);
            SecondWeaponBullet -= 30;
            emptyTS = GetEmptyGrid();
        }
        if (emptyTS != null && SecondWeaponBullet > 0)
        {
            emptyTS.Find("Image").GetComponent<Image>().sprite = Item_SecondBulletSprite;
            emptyTS.Find("Text").GetComponent<Text>().text = SecondWeaponBullet.ToString();
            emptyTS.gameObject.SetActive(true);
            emptyTS = GetEmptyGrid();
        }
    }

    //清空背包
    public void ClearBag() 
    {
        Debug.Log("ClearBag");
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].activeSelf == true)
            {
                blocks[i].transform.Find("Image").GetComponent<Image>().sprite = null;
                blocks[i].transform.Find("Text").GetComponent<Text>().text = "";
                blocks[i].SetActive(false);
            }
        }
    }
}

