using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : teststate
{
    public Vector3 target;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance( transform.position, target) < 0.1f)
        {
            die();
            wave.makeWave(2, 12, transform.position, wave.waveType.other,this);//造波
        }
    }


    

    public static void SetBullet(Vector3 targetone,Vector3 start)
    {
        GameObject bulletPerfab= Resources.Load<GameObject>("perfab\\bullet");
        GameObject bullet = Instantiate(bulletPerfab);
        bullet.transform.position = start;
        bullet.GetComponent<bullet>().target = targetone;
        bullet.GetComponent<Rigidbody2D>().velocity = (targetone - start).normalized * 3;
        bullet.GetComponent<Rigidbody2D>().drag = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "monster")
        {
            attack(collision.GetComponent<teststate>(),1);
            wave.makeWave(2, 12, transform.position, wave.waveType.other, this);
            die();
        }
        if (collision.tag == "other")
        {
            if (collision.GetComponent<Rigidbody2D>())
            {
                collision.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity.normalized * 30);
            }
            attack(collision.GetComponent<teststate>(), 0);
            die();
        }
        if (collision.tag == "wall")
        {
            die();
        }
    }
}
