using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class scenceManage : MonoBehaviour
{
    public Canvas ui;

    public Text monsternum;
    public Text nestnum;
    public Text health;
    public Text box;


    public GameObject button;
    public GameObject gaunkaxuanze;//关卡选择
    public GameObject gamestate; //游戏内状态
    public Button startButton;
    public Button exitButton;

    public int guankaId=0;
    public int maxguanka=3;
    bool isstart;
    bool isover;
    // Start is called before the first frame update
    void Start()
    {
        Object.DontDestroyOnLoad(gameObject);

        startButton.onClick.AddListener(()=>loadscence(1));
        exitButton.onClick.AddListener(() => exitGmae());
        Debug.Log("按钮初始化");
    }

    // Update is called once per frame
    void Update()
    {
        if (isstart)
        {
            monsternum.text = FindObjectsOfType<Monster>().Length.ToString();
            nestnum.text = FindObjectsOfType<nest>().Length.ToString();

            health.text = player.Instance.health.ToString();
            if (!isover)
                if (FindObjectsOfType<nest>().Length == 0 && FindObjectsOfType<Monster>().Length == 0)
                {
                    win();
                    isover = true;
                }
                else if (player.Instance.isdie)
                {
                    lose();
                    isover = true;
                }
            //如果怪物数量为0巢穴数量为0 玩家出现迷雾进入下一关
        }
    }

    void start()
    {
        if(player.Instance == null)
        {
            GameObject playerPerfab = Resources.Load<GameObject>("perfab\\player");
            GameObject player = Instantiate(playerPerfab);
            player.GetComponent<player>().health = 3;
            player.name = "player";
        }
    }

    void win()
    {
        if (guankaId < maxguanka)
        {
            CreatButton("you win", () => {
                CreatButton("next", () => {
                    loadscence(guankaId += 1);
                },0.8f, 0.5f);
            },0.8f,0.5f);
            
        }
        else
        {
            CreatButton("you win All", () => {
                iswinAll = true;
                StartCoroutine(winall());
                CreatButton("thank you play!", () => {
                    CreatButton("back manu", () => {
                        loadscence(0);
                    },0.2f, 0.2f);
                },0.5f, 0.5f);
            }, 0.5f, 0.5f);
        }
    }

    void lose()
    {
        CreatButton("you die", () => {
            CreatButton("retry", () => {
                loadscence(guankaId);
            }, 0.8f, 0.5f);
        }, 0.8f, 0.5f);
        
    }

    void exitGmae()
    {
        Application.Quit();
    }

    bool iswinAll;
    private IEnumerator winall()
    {
        Debug.Log("winall");
        while (iswinAll)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);

            word.creatWord(mousePosition + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, -1)*0.5f, "❥(>.<)", 2f, 1f);

            yield return new WaitForSeconds(0.2f);
        }

    }

    public void CreatButton(string content,UnityAction a, float Position_x = 0.5f, float Position_y = 0.5f)
    {
        GameObject bu = Instantiate(button);
        bu.transform.SetParent(ui.transform );
        bu.SetActive(true);
        bu.GetComponentInChildren<Text>().text = content;
        bu.GetComponent<Button>().onClick.AddListener(a);
        bu.transform.position = new Vector3(Camera.main.pixelRect.xMax * Position_x, Camera.main.pixelRect.yMax* Position_y);
        bu.GetComponent<Button>().onClick.AddListener(()=> { Destroy(bu);});//点击后销毁
    }

    public  void loadscence(int id)
    {
        SceneManager.LoadScene(id.ToString());
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode sceneType) => {
            
            guankaId = id;
            isover = false;

            if (id == 0)
            {
                isstart = false ;
                gaunkaxuanze.SetActive(true);
                gamestate.SetActive(false);
            }
            else
            {
                isstart = true;
                gaunkaxuanze.SetActive(false);
                gamestate.SetActive(true);
            }
        };
        
        

    }
}
public class  scenceDate
{
    public string name;
    public Vector3 StartPositon;
}
