using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : teststate
{
    private static player instance;
    public static player Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<player>();
            return instance;
        }
    }

    Vector3 Pdir;
    float Pspeed;
    float maxSpeed=3; 
    Vector3 POmousePositon;
    public  int boxnum = 0;
    public  int maxBoxnum = 2;

    public GameObject miaozhunIcon;
    public GameObject dirpointIcon;
    public GameObject dirIcon;


    float coldTimeBullet = 2; 
    public float BulletTime = 0;
    float coldTimeDown = 2; //最大
    public float DownTime = 0;
    public float air = 0; //空气

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += speed * dir * Time.deltaTime;

        Control();
        miaozhun();
    }

    private void OnDrawGizmos() {

                Gizmos.color = new Color(1, 0, 0, 1);
                Gizmos.DrawWireSphere(transform .position , miaozhunJULI);
            
    }


    public override void die()
    {
        //base.die();
        isdie = true;
        gameObject.SetActive(false);
    }
    public override void appear()
    {
        base.appear();
    }

    public virtual void beattack(int dam)
    {
        base.beattack(dam);
        cameraManage.camaraControl.CameraShake(0.2f, 0.3f);
    }

    public  void attack(Vector3 target)
    {
        if (BulletTime >= coldTimeBullet)
        {
            BulletTime = 0;
            Debug.Log(BulletTime);
            Debug.Log("Player:attack");
            bullet.SetBullet(target, transform.position);
            cameraManage.camaraControl.CameraShake(0.4f, 0.3f);
            
        }
    }
    
    void timeback()
    {
        if(BulletTime< coldTimeBullet)
        {
            BulletTime += Time.deltaTime;
        }

        if (DownTime < coldTimeDown)
        {
            DownTime += Time.deltaTime;
        }
    }
    

    private void getbox()
    {
        
    }
    private void downBox()
    {

    }

    private Rigidbody2D rb;
    private Vector3 mousePosition;
    private Vector3 objectPosition;
    private float distance;// 声明一个浮点数变量，用于记录鼠标与物体的距离
    private bool isDragging;
    public  float miaozhunJULI=2.2f;//瞄准距离
    //控制炮蛋瞄准发射
    private void miaozhun()
    {
        if( !isDragging)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            bool ismiaozhun = true ;
            if(hits.Length>0)
            foreach (var hitone in hits)
            {
                if (hit.collider.tag == "player" || hit.collider.tag == "select")
                {
                        ismiaozhun = false ;
                    }
            }

            // 如果碰撞到了物体
            if (ismiaozhun)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                Debug.Log("touch:"+hit.collider.name );
                miaozhunIcon.SetActive(true);

                if(Vector2.Distance(transform .position ,mousePosition) < miaozhunJULI)
                {
                    miaozhunIcon.transform.position = mousePosition;
                }else
                {
                    miaozhunIcon.transform.position = transform.position + (mousePosition - transform.position).normalized * miaozhunJULI;
                }
                
            }else
            {
                miaozhunIcon.SetActive(false);
            }

            if(Input.GetMouseButtonDown(1))
            {
                attack(miaozhunIcon.transform .position );
            }
        }
        else
        {
            miaozhunIcon.SetActive(false);
        }

        timeback();
    }

    //控制鼠标拖拽移动
    private void Control()
    {
        var  hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool isplayer = false;
        if(hits.Length>0)
        foreach(var hitone in hits)
        {
            if(hitone.collider.tag == "player" || hitone.collider.tag == "select")
            {
                isplayer = true;
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            // 如果碰撞到了物体
            if (isplayer)
            {
                // 开始拖拽物体
                isDragging = true;
                rb = GetComponent<Rigidbody2D>();
                rb.isKinematic = true;
                // 设置物体为运动学物体，以便拖拽
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                // 记录鼠标位置
                objectPosition = transform.position - mousePosition;
                // 记录物体位置
                distance = Vector3.Distance(transform.position, mousePosition);
                // 记录鼠标与物体的距离
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (rb != null)
            {
                rb.velocity = Pdir * Pspeed - Pdir*boxnum*0.15f;
                isDragging = false;
                rb.isKinematic = false;
                rb = null;
                wave.makeWave(1.3f+ boxnum, 12, transform.position , wave.waveType.player, this);
            }
        }

        if (isDragging)//拖拽力量
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0,0,10);
            distance = Vector3.Distance(transform.position, mousePosition);

            Pspeed = distance;
            if (Pspeed > maxSpeed) Pspeed = maxSpeed;//最大力量限制
            //Debug.Log("Pspeed:" + Pspeed);

            Pdir =(mousePosition- transform.position ).normalized;

            dirpointIcon.SetActive(true);
            dirpointIcon.transform.position = mousePosition;
            dirpointIcon.transform.localScale = new Vector3(Pspeed * 1 + 1, Pspeed * 0.5f + 1, Pspeed + 1);
            dirIcon.SetActive(true);
            dirIcon.transform.LookAt2D(mousePosition);
            dirIcon.transform.localScale = new Vector3(Pspeed * 0.5f + 1, Pspeed * 0.5f + 1, Pspeed + 1);

            
        }
        else
        {
            dirpointIcon.SetActive(false );
            dirIcon.SetActive(false );
        }
        
    }
    
}
