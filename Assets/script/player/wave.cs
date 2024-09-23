using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wave : teststate
{
    public string type = "";
    public Vector3 source;
    public teststate sourcebio;
    public float size;
    public waveType Wtype;


    public enum waveType
    {
        player,
        monster,
        other,
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        size =Vector3.Distance(Vector3.zero , GetComponent<Rigidbody2D>().velocity);
        GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Lerp(0.2f,1, size));
        if (size <= 0.2f) die();
        transform.localScale = new Vector3(size, size, size);
    }


    public virtual void attack(teststate one, int dam)
    {
    }

    public virtual void beattack(int dam)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "wall")
        {
            var rigi = GetComponent<Rigidbody2D>();
            float x = rigi.velocity.x;
            float y = rigi.velocity.y;
            var faces = Tool.JudgeColliderDir(transform, collision);
            foreach(var face in faces)
            {
                if (face.Equals(Tool.DirEnum.Right)) x = Mathf.Abs(x);
                if (face.Equals(Tool.DirEnum.Left)) x = -Mathf.Abs(x);
                if (face.Equals(Tool.DirEnum.Bottom)) y = Mathf.Abs(y);
                if (face.Equals(Tool.DirEnum.Top)) y = -Mathf.Abs(y);
                Debug.Log("waveface:" + face);
            }
            
            rigi.velocity = new Vector2(x, y);
            rigi.transform.LookAt2D(rigi.transform.position + new Vector3(x,y,0));
            
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        
        if(collision.tag!="wave"&& collision.tag != "wall")
        {
            if (collision.GetComponent<teststate>()&& collision.GetComponent<teststate>()!=sourcebio)
            {

                collision.GetComponent<teststate>().overWave(this);
                GetComponent<Rigidbody2D>().velocity *= 0.985f;
            }
        }
        //挪动物体   
    }


    public static void makeWave(float size,int num,Vector3 position,waveType type,teststate sourceBio)
    {
        Debug.Log("makeWave");
        
        GameObject wave= Resources.Load<GameObject>("perfab\\wave"); 
        for (int i = 0; i < num; i++)
        {
            GameObject wave2 = GameObject.Instantiate(wave);
            wave2.GetComponent<wave>().source = position;
            wave2.GetComponent<wave>().Wtype = type;
            wave2.GetComponent<wave>().sourcebio = sourceBio;
            float z = 360 * i / num;
            wave2.transform.rotation = Quaternion.EulerRotation(0, 0, z);
            wave2.GetComponent<Rigidbody2D>().velocity = wave2.transform.TransformVector(Vector3.right ).normalized * size;
            wave2.transform.position = position;
            
        }
    }
}
