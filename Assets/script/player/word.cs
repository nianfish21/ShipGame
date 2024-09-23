using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class word : MonoBehaviour
{
    public  float speed=0;
    public float time = 0;
    public float nowtime=0;

    public static void creatWord(Vector3 positon, string contant, float time, float speed)
    {

        GameObject Word = Resources.Load<GameObject>("perfab\\word");
        GameObject Word2 = GameObject.Instantiate(Word);
        Word2.GetComponent<word>().speed = speed;
        Word2.GetComponentInChildren<Text>().text = contant;
        Word2.GetComponentInChildren<word>().time = time;
        Word2.transform.position = positon;
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, time);
        nowtime = time;
    }

    // Update is called once per frame
    void Update()
    {
        nowtime -= Time.deltaTime;
        transform.position += speed * Vector3.up * Time.deltaTime;
        GetComponentInChildren<Text>().color = new Color(1, 1, 1, nowtime / time);
    }
}
