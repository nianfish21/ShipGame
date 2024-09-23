using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : teststate
{
    public  float speed = 2.5f;

    public GameObject prompt;

    public ActType actType = ActType.idle;

    // Start is called before the first frame update
    void Start()
    {
        prompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeBack();

        switch (actType)
        {
            case ActType.idle:
                idle();
                break;
            case ActType.patrol:
                patrol();
                break;
            case ActType.chase:
                Chase();
                break;
            case ActType.attack:
                attack();
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {

        //画线
        //if(IsClosest()!= new Vector3())Gizmos.DrawLine(IsClosest() + transform.position, SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(Yminus( Event.current.mousePosition)));
        if (patrolPoint != null)
            foreach (var p in patrolPoint)
            {
                Gizmos.color = new Color(1, 0, 0, 1);
                Gizmos.DrawSphere(p, 0.1f);
            }

        
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawWireSphere(transform.position, alertDis);

        Gizmos.color = new Color(0, 1, 0, 1);
        Gizmos.DrawWireSphere(transform.position, attackDis);
    }


    void timeBack()
    {
        if (moveColdTime < MaxmoveColdTime)
            moveColdTime += Time.deltaTime;
        if (AttackColdTime < MaxAttackColdTime)
            AttackColdTime += Time.deltaTime;
    }

    /// <summary>
    /// 待机
    /// </summary>
    float idleTime = 0;
    float maxIdleTime = 2;
    void idle()
    {
        idleTime += Time.deltaTime;
        if (idleTime > maxIdleTime)
        {
            actType = ActType.patrol;
            idleTime = 0;
        }
        alert();
    }

    //警戒
    public float alertDis = 1f;
    public void alert()
    {
        float disTarget = Vector3.Distance(player.Instance.transform.position, transform.position);

        GetComponent<Collider2D>().enabled = false;
        var info = Physics2D.Raycast(transform .position , (player.Instance.transform .position  -transform .position).normalized ,alertDis, 1 << 0);
        if (info.collider == null) return;
        
        Debug.Log("monsterRay:" + info.collider.name);
        if (info.collider.tag == "player")
        {
                actType = ActType.attack;
        }
        Debug.DrawRay(transform.position, (player.Instance.transform.position - transform.position).normalized*alertDis);
        GetComponent<Collider2D>().enabled = true ;


    }

    //碰到浪花
    public override void overWave(wave waveone)
    {
        base.overWave(waveone);
        if (waveone.Wtype != wave.waveType.monster)
        {
            if (actType != ActType.attack)
            {
                target = waveone.source;

                actType = ActType.chase;
            }
                
        }
    }

    //追赶一个点
    public Vector3 target;
    void Chase()
    {
        if (moveColdTime >= MaxmoveColdTime)
        {

            float disTarget = Vector3.Distance(target, transform.position);
            if (disTarget < attackDis) actType=  ActType.idle;
            if (disTarget < attackDis+0.5f)//靠近
            {//减少速度
                moveTo(target, 1.5f);
                moveColdTime = 0;
            }
            else
            {
                moveTo(target, speed);
                moveColdTime = 0;
            }
        }
        alert();
    }

    //巡逻
    public List<Vector3> patrolPoint;
    
    public bool sortdown = true;//从前到后
    
    int nowPoint = 0;
    void patrol()
    {
        float disTarget = Vector3.Distance(patrolPoint[nowPoint], transform.position);
        //Debug.Log(disTarget+ "/" + attackDis);
        if (disTarget <= attackDis)//靠近
        {
            Debug.Log("MonsterNextPoint");
            if (nowPoint <= 0) { sortdown = true ; }
            if (nowPoint >= patrolPoint.Count - 1) sortdown = false ;

            nowPoint += sortdown ? 1 : -1;
            actType = ActType.idle;//回到idle

        }
        if (moveColdTime >= MaxmoveColdTime)//追踪下一个点
        {
            if (disTarget < attackDis +0.5f)//靠近
            {//减少速度
                moveTo(patrolPoint[nowPoint], 1.5f);
                moveColdTime = 0;
            }
            else
            {
                moveTo(patrolPoint[nowPoint], speed);
                moveColdTime = 0;
            }
        }
        alert();


    }

    //追击玩家
    public  float attackDis = 0.4f;
    float AttackColdTime = 0;
    float MaxAttackColdTime = 2f;
    public bool isattack = false; 
    void attack()
    {
        if (player.Instance.isdie == true)
        {
            actType = ActType.idle;
            prompt.SetActive(false);
            return;
        }
        float disTarget = Vector3.Distance(player.Instance.transform.position, transform.position);

        if (disTarget < alertDis)
        {
            if (moveColdTime >= MaxmoveColdTime)
            {
                prompt.SetActive(true);
                isattack = true ;
                    moveTo(player.Instance.transform.position, speed);
                    moveColdTime = 0;
                StartCoroutine(att());

            }
        }
        else
        {
            actType = ActType.idle;
        }
    }
    private IEnumerator att()
    {
        yield return new WaitForSeconds(0.5f);
        moveColdTime = 0;
        prompt.SetActive(false );
    }



    float moveColdTime = 0;
    float MaxmoveColdTime = 0.8f;
    void moveTo(Vector3 target, float speed)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = (target - transform.position).normalized * speed;
            //rb.isKinematic = false;
            wave.makeWave(0.5f, 12, transform.position, wave.waveType.monster,this);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "player" && isattack)
        {
            isattack = false ;
            attack(player.Instance, 1);
            GetComponent<Rigidbody2D>().velocity = (transform.position - player.Instance.transform.position).normalized * 1f;
            moveColdTime = -1;
            
        }
    }

    public override void die()
    {
        base.die();

    }
    public override void appear()
    {
        base.appear();
    }
}



public enum ActType
{
    idle,
    patrol,
    chase,
    attack,
}