using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wood : teststate
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
            //wave.makeWave(0.3f, 6, transform.position, wave.waveType.other);
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
