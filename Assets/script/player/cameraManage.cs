using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManage : MonoBehaviour
{
    private static cameraManage camaracontrol;
    public static cameraManage camaraControl
    {
        get
        {
            if (camaracontrol == null)
                camaracontrol = Transform.FindObjectOfType<cameraManage>();
            return camaracontrol;
        }
    }

    private bool isShake;
    public Transform player;
    public float speed = 0.003f;

    // Start is called before the first frame update
    void Start()
    {

        //Vector3 PlayerPoint = new Vector3(player.position.x, player.position.y, transform.position.z);
        //transform.position = PlayerPoint;//相机归位

    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }
    private void FixedUpdate()
    {
        
    }
    public void MoveCamera()//相机延迟跟踪
    {

        Vector3 PlayerPoint = new Vector3(player.position.x, player.position.y, -10);
        Vector3 mousePositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 centerPosion = Vector3.Lerp(PlayerPoint, mousePositon, 0.5f);
        transform.position = Vector3.Lerp(transform.position, centerPosion, speed);

    }

    /// <summary>
    /// 时间暂停
    /// </summary>
    /// <param name="duration">60分之秒</param>
    public void Stop(int duration)
    {
        StartCoroutine(Pause(duration));
    }

    IEnumerator Pause(int duration)
    {
        float pauseTime = duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 震动
    /// </summary>
    /// <param name="duration">时间</param>
    /// <param name="strength">强度</param>
    public void CameraShake(float duration, float strength)
    {

        if (!isShake)
        {
            print("抖动1");
            StartCoroutine(Shake(duration, strength));
            
            Invoke("OnBrain", duration);
        }

    }


    IEnumerator ShakeRow(float duration, float strength)
    {

        isShake = true;
        Transform camera = Camera.main.transform;
        Vector3 startPosition = camera.position;
        while (duration > 0)
        {
            camera.position = Random.insideUnitSphere * strength + startPosition;
            duration -= Time.deltaTime;
            yield return null;
        }
        isShake = false;
    }
    IEnumerator Shake(float duration, float strength)
    {
        print("抖动2");
        isShake = true;
        Transform camera = Camera.main.transform;
        Vector3 startPosition = camera.position;
        while (duration > 0)
        {
            //Debug.Log(Mathf.Lerp(0, strength, duration));
            camera.position = Random.insideUnitSphere * Mathf.Lerp(0, strength, duration) + startPosition;
            duration -= Time.deltaTime;
            yield return null;
        }
        isShake = false;
    }

}
