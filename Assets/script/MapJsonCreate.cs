using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using LitJson;
using UnityEngine.UI;

public class MapJsonCreate : MonoBehaviour
{
    //public
    public int GridX = 256;//格子X
    public int GridZ = 256;//格子Z
    public Text GridText;
    public float SizeScale = 0.3f;
    public Texture2D import;

    public bool isShowNullGrid=true;
    public bool isShowGrid=true ;
    //private
    private Texture2D texture2D;
    public  Dictionary<int, int> mapDic = new Dictionary<int, int>();
    private int cameraX, cameraY;
    private Transform quaPlane;
    private Camera mainCamera;
    private bool isMousePress;
    private Vector3 mouseLostPos;
    public float GridHeight = 15;
    private float width = 2;
    private int maxCount = 0;

    private int brushSize = 1;

    void Start()
    {
        string currSceneName = SceneManager.GetActiveScene().name;
        if (File.Exists(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt"))    
        {
            string text = File.ReadAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(text))
            {
                NavGridData data = JsonMapper.ToObject<NavGridData>(text);
                GridX = data.width;
                GridZ = data.height;
                for (int i = 0; i < data.SceneInfos.Count; i++)
                {
                    int nClamp = data.SceneInfos[i];
                    if (nClamp == 1)
                    {
                        mapDic[i] = nClamp;
                    }
                }
            }
        }

        if (mainCamera == null){
            mainCamera = new GameObject("mainCamera").AddComponent<Camera>();
            mainCamera.orthographic = true;
            mainCamera.transform.localEulerAngles = Vector3.right * 90;
            mainCamera.transform.position = Vector3.up * 90;
            //+new Vector3(400,0,260);
            mainCamera.tag = "MainCamera";
            mainCamera.orthographicSize = 261;
            GridHeight = 45;
            quaPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            quaPlane.localEulerAngles = Vector3.right * 90;
            quaPlane.position = new Vector3(mainCamera.transform.position.x, 90, mainCamera.transform.position.z);
            quaPlane.gameObject.GetComponent<Renderer>().enabled = false;
            quaPlane.localScale = new Vector3(200, 100, 1);

        }
        texture2D = new Texture2D(GridX, GridZ);
    }
    private void OnDestroy()
    {
        //SaveFile();
    }

    //保存数据
    public void SaveFile()
    {
        NavGridData data = new NavGridData();
        data.width = GridX;
        data.height = GridZ;
        data.SceneInfoSize = GridX * GridZ;
        data.SceneInfos = new List<int>();
        for (int y = 0; y < GridZ; y++)
        {
            for (int x = 0; x < GridX; x++)
            {
                int id = y * GridX + x;
                if (mapDic.ContainsKey(id))
                {
                    data.SceneInfos.Add(1);
                }
                else
                {
                    data.SceneInfos.Add(0);
                }
            }
        }
        string currSceneName = SceneManager.GetActiveScene().name;
        string text = JsonMapper.ToJson(data);
        File.WriteAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", text, System.Text.Encoding.UTF8);
        Debug.Log("<color=#00FF00>保存完毕</color>");
    }

    void Update()
    {
        
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            
            Vector3 mousePos = Input.mousePosition;
            Vector3 cpos= mainCamera.ScreenToWorldPoint(mousePos);
            Debug.Log(cpos);
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000);
            //if (hit.collider == null) return;
            Vector3 pos = cpos;
            int currX = (int)(pos.x / width);
            int currZ = (int)(pos.z / width);
            for (int j = 0; j < brushSize; j++)
                for (int i=0;i<brushSize;i++)
            {
                int id = (currZ) * GridX+i+ currX+j* GridX;
                maxCount = GridX * GridZ;
                if (id < 0 || id >= maxCount) { Debug.Log(id); }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        mapDic[id] = 1;
                            Debug.Log("修改1");
                        }
                    else if (Input.GetMouseButton(1))
                    {
                        if (mapDic.ContainsKey(id))
                        {
                            mapDic.Remove(id);
                                Debug.Log("修改0");
                            }
                    }
                }
            }
            
            //GridText.text = "选择坐标：(" + currX + "," + currZ + ")";
        }
        float xw = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 240;
        if (xw != 0)
        {
            mainCamera.orthographicSize -= xw;
            Vector3 pos = mainCamera.gameObject.transform.localPosition;
            pos.y -= xw * SizeScale;
            GridHeight = pos.y * 0.5f;
            mainCamera.gameObject.transform.localPosition = pos;
        }
        if (Input.GetMouseButtonDown(2))
        {
            mouseLostPos = Input.mousePosition;
            isMousePress = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isMousePress = false;
        }
        if (isMousePress)
        {
            Vector3 dir = Input.mousePosition - mouseLostPos;
            mouseLostPos = Input.mousePosition;
            dir.z = dir.y;
            dir.y = 0;
            mainCamera.transform.position -= dir * Time.deltaTime * 2;
            cameraX = (int)(mainCamera.transform.position.x / width);
            cameraY = (int)(mainCamera.transform.position.z / width);
            quaPlane.position -= dir * Time.deltaTime * 2;
        }
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            brushSize = 1;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            brushSize = 2;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            brushSize = 3;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            brushSize = 4;
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            brushSize = 5;
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            brushSize = 6;
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            brushSize = 7;
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            brushSize = 8;
        }
    }

    private void OnDrawGizmos()
    {
        //Debug.Log(1);
            for (int y = 0; y < GridZ; y++)
            {
                for (int x = 0; x < GridX; x++)
                {
                    int id = y * GridX + x;
                    if (mapDic.ContainsKey(id)&& isShowGrid)
                    {
                        Gizmos.color = new Color(67, 0, 0, 100 / 255f);
                        Gizmos.DrawCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));                     }
                    else if(isShowNullGrid)
                    {
                        Gizmos.color = new Color(1, 1, 1, 10 / 255f);
                        Gizmos.DrawWireCube(new Vector3(width * 0.5f + x * width, GridHeight, width * 0.5f + y * width), new Vector3(width, 0, width));
                    }
                }
            }
    }

    public void imageToList(Texture2D image)
    {
        if(GridX!= import.width|| GridZ != import.height)
        {
            Debug.LogError("图片大小和地图大小不符");
        }
        mapDic.Clear();
        Color[] pixels = image.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            //if (pixels[i].r!=1f && pixels[i].r != 0f)
            //{
            //    mapDic[i]=1;
            //}
            if (pixels[i].r != 1f)
            {
                mapDic[i] = 1;
            }
        }
    }
}
public class NavGridData
{
    public int width { get; set; }
    public int height { get; set; }
    public int SceneInfoSize { get; set; }
    public List<int> SceneInfos { get; set; }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapJsonCreate))]
 
public class MapJsonCreateHelper : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("保存导出"))
        {
            var component = target as MapJsonCreate;
            component.SaveFile();
        }
        if (GUILayout.Button("导入图片"))
        {
            var component = target as MapJsonCreate;
            component.imageToList(component.import);
        }
        if (GUILayout.Button("导入数据"))
        {
            var component = target as MapJsonCreate;

            string currSceneName = SceneManager.GetActiveScene().name;
            if (File.Exists(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt"))
            {
                string text = File.ReadAllText(Application.dataPath + "/Res/NavGridData/" + currSceneName + ".txt", System.Text.Encoding.UTF8);
                if (!string.IsNullOrEmpty(text))
                {
                    NavGridData data = JsonMapper.ToObject<NavGridData>(text);
                    component.GridX = data.width;
                    component.GridZ = data.height;
                    for (int i = 0; i < data.SceneInfos.Count; i++)
                    {
                        int nClamp = data.SceneInfos[i];
                        if (nClamp == 1)
                        {
                            component.mapDic[i] = nClamp;
                        }
                    }
                }
            }
        }
    }
}
#endif