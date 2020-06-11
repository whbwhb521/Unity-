using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageShow : MonoBehaviour
{
    private float speed = 10.0f;
    private float time = 0.5f;
    private float startTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.up * speed * Time.deltaTime);
        startTime += Time.deltaTime;

        this.GetComponent<Text>().color = new Color(1, 0, 0, 1 - startTime);
        Destroy(gameObject, time);
    }
}
