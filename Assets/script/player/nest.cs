using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nest : teststate
{


    float wavetime = 0;
    float maxWaveTime = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        wavetime += Time.deltaTime;
        if (wavetime >= maxWaveTime)
        {
            wave.makeWave(4f, 12, transform.position, wave.waveType.monster,this );
            wavetime = 0;
        }
        timeback();
    }


    void timeback()
    {
        if (wavetime < maxWaveTime)
            wavetime += Time.deltaTime;

    }

    public override void die()
    {
        Debug.Log("die:" + name);
        Destroy(gameObject,3f);
        isdie = true;

        StartCoroutine(att());

    }

    private IEnumerator att()
    {
        yield return new WaitForSeconds(0.5f);
        word.creatWord(transform.position+new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y,-1), "explosion", 2f, 1f);

        yield return new WaitForSeconds(0.5f);
        word.creatWord(transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y,-1), "explosion", 2f, 1f);

        yield return new WaitForSeconds(0.5f);
        word.creatWord(transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y,-1), "explosion", 2f, 1f);

        yield return new WaitForSeconds(0.5f);
        word.creatWord(transform.position + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y-1), "explosion", 2f, 1f);
    }
}
