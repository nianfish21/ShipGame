using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teststate: MonoBehaviour
{
    
    //public string name;
    public int health=1;
    public  int quality=1;
    public bool isdie = false;
    
    public virtual void attack(teststate one,int dam)
    {
        if (one!=null)
            one.beattack(dam);
    }

    public virtual void beattack(int dam)
    {
        word.creatWord(transform.position, "-" +dam.ToString (), 1, 0.5f);
        Debug.Log(name + "BeAttack");
        health -= dam;
        cameraManage.camaraControl.CameraShake(0.2f, 0.3f);
        StartCoroutine(beattacking(gameObject));
        if (health <= 0)
        {
            die();
        }
    }

    private IEnumerator beattacking(GameObject a)
    {
        yield return new WaitForSeconds(0.1f);
        a.transform.position += Vector3.right * 0.1f;
        yield return new WaitForSeconds(0.1f);
        a.transform.position -= Vector3.right * 0.2f;
        yield return new WaitForSeconds(0.1f);
        a.transform.position += Vector3.right * 0.1f;
        
    }

    public virtual void overWave(wave waveone)
    {

        var rigiSelf = GetComponent<Rigidbody2D>();
        var rigiWave = waveone.GetComponent<Rigidbody2D>();
        float selfV = Vector2.Distance(Vector2.zero, rigiSelf.velocity);
        float waveV = Vector2.Distance(Vector2.zero, rigiWave.velocity);
        if (selfV < waveV*0.5)
        {
            rigiSelf.AddForce( rigiWave.velocity * 0.4f);
        }
        
    }
    public virtual void die()
    {
        Debug.Log("die:" + name);
        Destroy (gameObject);
        isdie = true;
    }
    public virtual void appear()
    {
        isdie = false;
    }
}
