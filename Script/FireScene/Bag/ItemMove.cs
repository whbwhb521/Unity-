using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * 移动物品
 */
public class ItemMove : MonoBehaviour, 
    IBeginDragHandler, 
    IDragHandler, 
    IEndDragHandler,
    ICanvasRaycastFilter
{
    public static GameObject NowDropBox;//目前的dropbox
    private Transform BgPanel;//我的背包背景的transform
    private Transform EmenyBgPanel;

    private GameObject MyBag;
    private GameObject EnemyBag;
    private GameObject NowSelectBag;//选择的背包

    private bool isRaycastLocationValid = true;//不能穿透
    private int num;//点击图的数量
    private Sprite sp;//点击的图

    public void Start()
    {
        MyBag = GameObject.Find("Bag");
        EnemyBag = GameObject.Find("Bags").transform.Find("EmenyBag").gameObject;
        BgPanel = GameObject.Find("BgPanel").transform;
        EmenyBgPanel = GameObject.Find("Bags").transform.Find("EmenyBag/Bag1/EmenyBgPanel");

    }
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return isRaycastLocationValid;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        NowSelectBag = transform.parent.parent.parent.gameObject;
        BgPanel.SetAsLastSibling();
        EmenyBgPanel.SetAsLastSibling();
        sp = transform.Find("Image").GetComponent<Image>().sprite;
        num = Convert.ToInt32(transform.Find("Text").GetComponent<Text>().text);
        isRaycastLocationValid = false;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject EndObj = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log("EndObj:"+ EndObj.tag);
        Debug.Log("NowDropBox:"+ NowDropBox);
        Debug.Log("MyBag:"+MyBag);
        if (EndObj != null) 
        {
            //选择的是我的背包中的物体
            if (NowSelectBag.tag.Equals("MyBag"))
            {
                Debug.Log("NowSelectBag:" + NowSelectBag.tag);
                //在GLPlane上抬起鼠标表示把东西扔出去
                if (EndObj.tag.Equals("GLPlane"))
                {
                    if (sp == MyBag.GetComponent<BagItem>().Item_MainBulletSprite)
                    {
                        PlayerInfo.Instance.Bullets_[0] -= num;
                    }
                    else if (sp == MyBag.GetComponent<BagItem>().Item_SecondBulletSprite)
                    {
                        PlayerInfo.Instance.Bullets_[1] -= num;
                    }
                    Debug.Log("MyBag.GetComponent<BagItem>().Item_MainBulletSprite:" + MyBag.GetComponent<BagItem>().Item_MainBulletSprite);
                    Debug.Log("sp:" + sp);
                }
                //在敌方背包处抬起鼠标，把东西扔到敌方背包中
                else if (EndObj.tag.Equals("EnemyBag") ||
                     ((EndObj.transform.parent != null) && EndObj.transform.parent.gameObject.tag.Equals("EnemyBag")))
                {
                     if (sp == MyBag.GetComponent<BagItem>().Item_MainBulletSprite)
                     {
                        NowDropBox.GetComponent<DropBox>().BulletsNum_[0] += num;
                        PlayerInfo.Instance.Bullets_[0] -= num;
                    }
                     else if (sp == MyBag.GetComponent<BagItem>().Item_SecondBulletSprite)
                     {
                        NowDropBox.GetComponent<DropBox>().BulletsNum_[1] += num;
                        PlayerInfo.Instance.Bullets_[0] -= num;
                    }
                    EnemyBag.SendMessage("UpdateBag", NowDropBox.GetComponent<DropBox>().BulletsNum_, SendMessageOptions.RequireReceiver);
                }
            }
            //选择的是敌方背包中的东西
            else if (NowSelectBag.tag.Equals("EnemyBag")) 
            {
                //在自己背包处抬起鼠标，把东西扔到我放背包中
                if (EndObj.tag.Equals("MyBag") ||
                    ((EndObj.transform.parent != null) && EndObj.transform.parent.gameObject.tag.Equals("MyBag"))) 
                {
                    if (sp == MyBag.GetComponent<BagItem>().Item_MainBulletSprite)
                    {
                        NowDropBox.GetComponent<DropBox>().BulletsNum_[0] -= num;
                        PlayerInfo.Instance.Bullets_[0] += num;
                    }
                    else if (sp == MyBag.GetComponent<BagItem>().Item_SecondBulletSprite)
                    {
                        NowDropBox.GetComponent<DropBox>().BulletsNum_[1] -= num;
                        PlayerInfo.Instance.Bullets_[1] += num;
                    } 
                }
                EnemyBag.SendMessage("UpdateBag", NowDropBox.GetComponent<DropBox>().BulletsNum_, SendMessageOptions.RequireReceiver);
            }
            
            MyBag.SendMessage("UpdateBag", PlayerInfo.Instance.Bullets_, SendMessageOptions.RequireReceiver);
        }

        BgPanel.SetAsFirstSibling();
        EmenyBgPanel.SetAsFirstSibling();
        isRaycastLocationValid = true;
    }
}
