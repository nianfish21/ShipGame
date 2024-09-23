using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionBullet : teststate
{
    float wavetime = 0;
    float maxWaveTime = 4;
    float rang=1.3f;
    // Start is called before the first frame update
    void Start()
    {

    }



    public override void beattack(int dam)
    {
        base.beattack(dam);
        exprosion();
    }

    public void exprosion()
    {
        for(int i = 0; i < 6; i++)
        {
            word.creatWord(transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, -1) * rang, "explosion", 1f, 0.3f);
        }

        var hit = Physics2D.CircleCastAll(transform.position, rang, Vector2.zero);
        foreach (var h in hit)
        {
            if(h.collider.GetComponent<teststate>()&& h.collider.tag != "wall")
            {
                attack(h.collider.GetComponent<teststate>(), 4);
            }
        }

        die();
    }

    // Update is called once per frame
    void Update()
    {
        wavetime += Time.deltaTime;
        if (wavetime >= maxWaveTime)
        {
            wave.makeWave(0.3f, 12, transform.position, wave.waveType.other,this );
            wavetime = 0;
        }
        timeback();
    }

    void timeback()
    {
        if (wavetime < maxWaveTime)
            wavetime += Time.deltaTime;

    }
}
