using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCover : MonoBehaviour
{
    public GameObject DamageText;

    public void GenerateText(int damage) 
    {
        GameObject Damage = Instantiate(DamageText, transform) as GameObject;
        Damage.GetComponent<Text>().text = damage.ToString();
    }

}
