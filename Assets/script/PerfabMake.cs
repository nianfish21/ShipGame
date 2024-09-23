
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor.Experimental.SceneManagement;
using System.Threading;

[System.Reflection.Obfuscation(Exclude = true)]

public class PerfabMake :Editor
{
   

    

    static string[] assetGUIDs;
    static string[] assetPaths;
    static string[] allAssetPaths;
    static Thread thread;

    [MenuItem("zpyTools/查找资源引用", false)]
    static void FindAssetRefMenu()
    {
        if (Selection.assetGUIDs.Length == 0)
        {
            Debug.Log("请先选择任意一个组件，再击此菜单");
            return;
        }

        assetGUIDs = Selection.assetGUIDs;

        assetPaths = new string[assetGUIDs.Length];

        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
        }

        allAssetPaths = AssetDatabase.GetAllAssetPaths();

        thread = new Thread(new ThreadStart(FindAssetRef));
        thread.Start();
    }


    static void FindAssetRef()
    {
        Debug.Log(string.Format("开始查找引用{0}的资源。", string.Join(",", assetPaths)));
        List<string> logInfo = new List<string>();
        string path;
        string log;
        for (int i = 0; i < allAssetPaths.Length; i++)
        {
            path = allAssetPaths[i];
            if (path.EndsWith(".prefab") || path.EndsWith(".unity"))
            {
                string content = File.ReadAllText(path);
                if (content == null)
                {
                    continue;
                }

                for (int j = 0; j < assetGUIDs.Length; j++)
                {
                    if (content.IndexOf(assetGUIDs[j]) > 0)
                    {
                        log = string.Format("{0} 引用了 {1}", path, assetPaths[j]);
                        logInfo.Add(log);
                    }
                }
            }
        }

        for (int i = 0; i < logInfo.Count; i++)
        {
            Debug.Log(logInfo[i]);
        }

        Debug.Log("选择对象引用数量：" + logInfo.Count);

        Debug.Log("查找完成");
    }
}



//修改动作窗口
public class MyFirstWindow : EditorWindow
{
    public  string path1 = "";
    public string path2 = "";

    public bool matshengcheng = false;
    public bool jiebaowenjiantihuan = false;
    public bool anifix = false;
    public bool perbafmatChang = false;
    public bool test212 = false;

    public AnimationClip ani;
    MyFirstWindow()
    {
        this.titleContent = new GUIContent("Bug Reporter");
    }

    [MenuItem("zpyTools/工具窗口")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MyFirstWindow));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        GUI.skin.label.fontSize = 10;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Bug Reporter");


        anifix = GUILayout.Toggle(anifix, "动画修复动作");
        if (anifix)
            AniPerfabFix();

        matshengcheng = GUILayout.Toggle(matshengcheng, "生成材质球");
        if(matshengcheng)
            MatShencheng();

        perbafmatChang = GUILayout.Toggle(perbafmatChang, "带空材质预制体材质替换");
        if(perbafmatChang)
            PerFabAddMaterial();

        test212 = GUILayout.Toggle(test212, "test");
        if (test212)
            test();

        jiebaowenjiantihuan = GUILayout.Toggle(jiebaowenjiantihuan, "文件替换");
        if (jiebaowenjiantihuan)
            XiuGaiWenJian();
    }

   
    //-------------------------动画修改预制体-------------------

    //根据动画第一帧调整预制体动作
    public void  AniPerfabFix()
    {
        //添加动画clip
        GUILayout.Space(10);
        ani = (AnimationClip)EditorGUILayout.ObjectField("animationclip", ani, typeof(AnimationClip));

        if (GUILayout.Button("修改选择的预制体文件:动画第一帧"))
        {
            perfabChangeByAni();
        }

        if (GUILayout.Button("场景预制体尺寸修复"))
        {
            sizeFix();
        }
    }


    string[] assetGUIDs;
    string[] assetPaths;
    string[] allAssetPaths;

    //通过动画控制器修改物体的根骨骼状态
    public void perfabChangeByAni()
    {
        if (Selection.assetGUIDs.Length == 0)
        {
            Debug.Log("请先选择任意一个组件，再击此菜单");
            return;
        }

        assetGUIDs = Selection.assetGUIDs;

        assetPaths = new string[assetGUIDs.Length];

        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
        }

        allAssetPaths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < assetPaths.Length; i++)
        {
            GameObject perfab = PrefabUtility.LoadPrefabContents(assetPaths[i]);//加载
            Debug.Log("加载地址" + assetPaths[i]);

            //修改内容
            {
                var mesh = perfab.GetComponentInChildren<SkinnedMeshRenderer>();
                ani.SampleAnimation(mesh.rootBone.transform.parent.gameObject, 0.1f);
                Debug.Log(perfab.name + "完成");
            }
            //DestroyImmediate( perfab.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);

            PrefabUtility.SaveAsPrefabAsset(perfab, assetPaths[i]);
            Debug.Log("保存地址" + assetPaths[i]);

            PrefabUtility.UnloadPrefabContents(perfab);//卸载
            Debug.Log("卸载地址" + assetPaths[i]);
        }

    }
    /// <summary>
    /// 预制体修复1 尺寸修复
    /// </summary>
    static void sizeFix()
    {
        var a = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
        foreach (var b in a)
        {
            b.rootBone.parent.transform.localScale = new Vector3(1, 1, 1);
            b.transform.localScale = new Vector3(1, 1, 1);
            Debug.Log(b.name);

        }
    }

    //----------------------一些工具----------------

    public void tool()
    {
        GUILayout.Label("工具");

        GUILayout.Space(10);

        if (GUILayout.Button("选择文件夹，修改图片点格式"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }

            string[] files = Directory.GetFiles(assetPaths[0], "*.*", SearchOption.AllDirectories);
            //string[] files = Directory.GetFiles(path2);
            if (files.Length > 0)
            {
                foreach (string path in files)
                {
                    if (path.PathGetFileType() == ".fbx")
                    {
                        GameObject perfab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));//加载
                        Debug.Log("加载地址" + path);
                        GameObject a = GameObject.Instantiate(perfab);
                        a.name = perfab.name;
                    }

                }
            }

        }
    }

    //------------------材质生成-------------------

    Thread thread2;
    public string matguid=""; //替换图片uid
    public string MatShenchengpath1 = ""; //材质地址
    public string MatShenchengpath2 = ""; //生成地址
    /// <summary>
    /// 材质球根据图片名称生成
    /// </summary>
    public void MatShencheng()
    {
        GUILayout.Label("替换原guid：c519d2c2f0d48aa45a8dd6cfa589f3d9");

        GUILayout.Label("1.替换图片的guid");
        EditorGUILayout.TextField(matguid);
        if (GUILayout.Button("选择图片guid"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }
            matguid = assetGUIDs[0];

        }
        GUILayout.Space(10);

        GUILayout.Label("2.原始mat地址");
        EditorGUILayout.TextField(MatShenchengpath1);
        if (GUILayout.Button("选择材质"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }
            MatShenchengpath1 = assetPaths[0];
            
        }
        GUILayout.Space(10);

        GUILayout.Label("3.替换建mat文件夹地址");
        EditorGUILayout.TextField(MatShenchengpath2);
        if (GUILayout.Button("选择文件夹"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }
            MatShenchengpath2 = assetPaths[0];
        }
        GUILayout.Space(10);


        if (GUILayout.Button("创建mat线程"))
        {
            thread2 = new Thread(new ThreadStart(MatCreate));
            thread2.Start();

        }
        if (GUILayout.Button("关闭创建mat线程"))
        {
            if (thread2 != null)
                if (thread2.IsAlive)
                    thread2.Abort();

        }
        GUILayout.Space(10);

    }

    private void MatCreate()
    {

        Debug.Log("创建mat");
        //直接拿到左右文件
        string[] files = Directory.GetFiles(MatShenchengpath2, "*.*", SearchOption.AllDirectories);
        //string[] files = Directory.GetFiles(path2);
        if (files.Length > 0)
        {
            Debug.Log("总文件:" + files.Length);
            int i = 0;
            foreach (string s in files)
            {
                try
                {
                    if (s.PathGetFileType() == ".jpg" || s.PathGetFileType() == ".JPG" || s.PathGetFileType() == ".tga") //是图片
                    {
                        string metaPath = s + ".meta";

                        string metacontent = File.ReadAllText(metaPath); //获取这张图片的guid
                        string guid = metacontent.Substring(metacontent.IndexOf("guid: ") + 6, 32);

                        string matcontent = File.ReadAllText(MatShenchengpath1);
                        //Debug.Log(i + "位置：1");
                        if (matcontent == null)
                        {
                            continue;
                        }
                        if(matguid!= guid) //改guid
                            while (matcontent.IndexOf(matguid) > 0)
                            {

                                matcontent = matcontent.Replace(matguid, guid);
                            }
                        int MatNameNum = 0; //改名字
                        if(MatShenchengpath1.PathGetFileName_NoType()!= s.PathGetFileName_NoType())
                            while (matcontent.IndexOf(MatShenchengpath1.PathGetFileName_NoType()) > 0)
                            {
                                MatNameNum++;
                                matcontent = matcontent.Replace(MatShenchengpath1.PathGetFileName_NoType(), s.PathGetFileName_NoType());
                            }
                        if (MatNameNum > 1)
                        {
                            Debug.LogError("多次找到原始材质名字，请检查错误修改的部分，或者原始材质名字请修改" + s.PathBackLevel(1) + "\\" + s.PathGetFileName_NoType() + ".mat");
                        }
                        File.WriteAllText(s.PathBackLevel(1) + "\\" + s.PathGetFileName_NoType() + ".mat", matcontent);

                        Debug.Log( i+"位置：" + s.PathBackLevel(1) + "\\" + s.PathGetFileName_NoType() + ".mat"+"\n"+guid);
                    }

                }
                catch
                {
                    Debug.LogError("报错：" + s);
                }
                i++;
            }
        }
        Debug.Log("创建mat线程停止");
    }

    //预制体操作
    public void PerFabAddMaterial()
    {

        GUILayout.Space(10);

        if (GUILayout.Button("从文件夹加载预制体"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }

            //加载选择文件夹的所有预制体
            string[] files = Directory.GetFiles(assetPaths[0], "*.*", SearchOption.AllDirectories);
            //string[] files = Directory.GetFiles(path2);
            if (files.Length > 0)
            {
                foreach (string path in files)
                {
                    if (path.PathGetFileType() == ".fbx")
                    {
                        GameObject perfab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));//加载
                        Debug.Log("加载地址" + path);
                        GameObject a = GameObject.Instantiate(perfab);
                        a.name = perfab.name;
                    }

                }
            }

        }


        if (GUILayout.Button("选择文件夹搜索，给场景内物体添加同名材质"))
        {

            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }

            var skins = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
            foreach (var skin in skins)
            {
                skin.rootBone.parent.transform.localScale = new Vector3(1, 1, 1);
                skin.transform.localScale = new Vector3(1, 1, 1);
                //Debug.Log(skin.name);

                Material[] matl = new Material[skin.sharedMaterials.Length];
                for (int i = 0; i < skin.sharedMaterials.Length; i++)
                {
                    //获取名字
                    string matname = skin.sharedMaterials[i].name;
                    if (matname.LastIndexOf('.') > 0)
                        matname = matname.Substring(0, matname.LastIndexOf('.'));

                    Material matReplace = null;

                    //加载选择文件夹的材质
                    string[] files = Directory.GetFiles(assetPaths[0], "*.*", SearchOption.AllDirectories);
                    //string[] files = Directory.GetFiles(path2);
                    if (files.Length > 0)
                    {
                        foreach (string path in files)
                        {
                            if (path.PathGetFileName_NoType() == matname && path.PathGetFileType() == ".mat")
                            {

                                matReplace = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));//加载

                                break;
                            }

                        }
                    }

                    Debug.Log(skin.name + ":");
                    matl[i] = matReplace;


                }
                skin.sharedMaterials = matl;
            }

        }

    }



    //---------------------测试----------------------
    GameObject p1;
    public void test()
    {
        defaultMaterial = (Material)EditorGUILayout.ObjectField("Material", defaultMaterial, typeof(Material));

        if (GUILayout.Button("加载文件.fbx"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("请先选择任意一个文件夹，再击此菜单");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }
            string path = assetPaths[0];

            var importer = (ModelImporter)AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                return;
            }
            //所有renderer
            var sources = new List<Renderer>();
            var assets = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath);
            
            for (var i = 0; i < assets.Length; i++)
            {
                var source = assets[i] as Renderer;
                if (source != null)
                {
                    sources.Add(source);
                }
            }

            importer.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnTextureName, ModelImporterMaterialSearch.RecursiveUp);
            Debug.Log(importer.GetExternalObjectMap().Count);
            //所有material
            var keys = new Dictionary<string, bool>();
            foreach (var render in importer.GetExternalObjectMap())
            {
                if (!keys.ContainsKey(render.Key.name.PathGetFileName_NoType()))
                {
                    keys.Add(render.Key.name, true);
                    Debug.Log(render.Key.name);
                }
            }
            

            //替换为默认material
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            if (keys.Count > 0 || importer.materialLocation != ModelImporterMaterialLocation.InPrefab)
            {
                var newMaterial = DefaultMaterial;
                var kind = typeof(UnityEngine.Material);
                foreach (var it in keys)
                {
                    var id = new AssetImporter.SourceAssetIdentifier();
                    id.name = it.Key;
                    id.type = kind;
                    importer.RemoveRemap(id);
                    importer.AddRemap(id, newMaterial);//核心句子 修改
                }
                importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
                importer.SaveAndReimport();
            }

            //ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(path);
            //importer.materialLocation.
            //importer. = TextureImporterType.Sprite;
            //importer.isReadable = false;
            //importer.SaveAndReimport();
            //EditorUtility.SetDirty(importer);


            //ModelImporterMaterialLocation location;


            //if (path.PathGetFileType() == ".fbx")
            //{
            //    GameObject perfab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));//加载
            //    Debug.Log("加载地址" + path);
            //    GameObject p1 = GameObject.Instantiate(perfab);
            //    p1.name = perfab.name;
            //}
        }
    }

    //DefaultMaterial :指定Material
    private static Material defaultMaterial;
    public  static Material DefaultMaterial
    {
        get
        {
            if (defaultMaterial == null)
            {
                var path = string.Format("Assets/{0}.mat", "DefaultMaterial");
                Material load = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (load == null)
                {
                    return null;
                }
                defaultMaterial = load;
                Resources.UnloadAsset(load);
            }
            return defaultMaterial;
        }
    }


    //筛选 同名 文件类型1 替换文件类型2
    string type1 = "";
    string type2 = "";
    public string XiuGaiWenJianpath1 = "";
    public string XiuGaiWenJianpath2 = "";
    Thread thread;
    //吧ozj 和bmp 解包出的smd和图片文件移动到解包文件夹的位置 第一步，让后删除ozj 和bmp文件 ，好进行下一步转fbx
    public void XiuGaiWenJian()
    {
        GUILayout.Label("后缀名1");
        type1 = EditorGUILayout.TextField(type1);
        GUILayout.Label("文件地址1");
        XiuGaiWenJianpath1 = EditorGUILayout.TextField(XiuGaiWenJianpath1);
        GUILayout.Space(10);
        GUILayout.Label("后缀名2");
        type2 = EditorGUILayout.TextField(type2);
        GUILayout.Label("文件地址2");
        XiuGaiWenJianpath2 = EditorGUILayout.TextField(XiuGaiWenJianpath2);



        
        if (GUILayout.Button("替换文件，第一级文件夹下所有文件"))
        {
            ////直接拿到左右文件
            //string[] files = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories);
            //foreach (string file in files)
            //{    // 处理每个文件}
            //    Debug.Log(file);
            //}

            

            ////获取文件夹
            //foreach (string directory in Directory.GetDirectories(XiuGaiWenJianpath1, "*", SearchOption.AllDirectories))
            //{
            //    Debug.Log(directory);
            //    foreach (string file in Directory.GetFiles(directory))
            //    {
            //        string filetype = file.PathGetFileType();
            //        string filename = file.PathGetFileName_NoType();
            //        if (filetype == type1)
            //        {

            //            string[] files2 = Directory.GetFiles(XiuGaiWenJianpath2, "*.*", SearchOption.AllDirectories);
            //            foreach (string file2 in files2)
            //            {

            //                string file2type = file2.PathGetFileType();
            //                string file2name = file2.PathGetFileName_NoType();

            //                if (filename == file2name && file2type == type2)
            //                {
            //                    //吧文件1位置所有文件复制到文件2位置并且删除文件2
            //                    Delet(file2);
            //                    Debug.Log("删除:" + file2);
            //                    CopyFolder(file.PathBackLevel(1), file2.PathBackLevel(1));
            //                }
            //            }

            //        }
            //    }
            //}
        }



        if (GUILayout.Button("替换文件，所有文件"))
        {
            thread = new Thread(new ThreadStart(TiHuan));
            thread.Start();
            Debug.Log("替换线程开始");
        }

        if (GUILayout.Button("停止替换文件"))
        {
            if(thread !=null)
                if(thread.IsAlive)
                    thread.Abort();
            //调用Thread.Abort方法试图强制终止thread线程

        }

        //if (GUILayout.Button("test删除文件地址1"))
        //{
        //    Delet(XiuGaiWenJianpath1);
        //}
        //if (GUILayout.Button("test复制文件1到文件夹2位置"))
        //{
        //    CopyFile(XiuGaiWenJianpath1, XiuGaiWenJianpath2);
        //}
        //if (GUILayout.Button("test复制文件夹1到文件夹2位置"))
        //{
        //    XiuGaiWenJianpath1 = XiuGaiWenJianpath1.PathGetFileName_NoType();
        //}
    }

    //替换文件
    public void TiHuan()
    {
        //直接拿到左右文件
        string[] files = Directory.GetFiles(XiuGaiWenJianpath1, "*.*", SearchOption.AllDirectories);
        Debug.Log("待筛选文件总数量:"+ files.Length );

        foreach (string file in files)
        {
            string filetype = file.PathGetFileType();
            string filename = file.PathGetFileName_NoType();
            if (filetype == type1)
            {

                string[] files2 = Directory.GetFiles(XiuGaiWenJianpath2, "*.*", SearchOption.AllDirectories);
                foreach (string file2 in files2)
                {

                    string file2type = file2.PathGetFileType();
                    string file2name = file2.PathGetFileName_NoType();

                    if (filename == file2name && file2type == type2)
                    {
                        //吧文件1位置所有文件复制到文件2位置并且删除文件2
                        Delet(file2);
                        Debug.Log("删除:" + file2);
                        CopyFile(file, file2.PathBackLevel(1) + "\\" + file.PathGetFileName());
                    }
                }
            }
        }
        Debug.Log("停止");
    }

    /// <summary>
    /// 复制文件到路径下
    /// </summary>
    /// <param name="dir1"></param>
    /// <param name="dir2"></param>
    public static void CopyFile(string dir1, string dir2)
    {
        dir1 = dir1.Replace("/", "\\");
        dir2 = dir2.Replace("/", "\\");
        if (File.Exists(dir1))
        {
            File.Copy( dir1,dir2, true);
        }
        Debug.Log("复制：" + dir1);
    }

    /// <summary>
    /// 删除路径下文件
    /// </summary>
    /// <param name="path"></param>
    public static  void Delet(string path)
    {
        // 创建对文件对象。
        FileInfo fi = new FileInfo(path);
        // 创建文件
        FileStream fs = fi.Create();
        // 根据需要修改文件，然后关闭文件。
        fs.Close();
        // 删除该文件。
        fi.Delete();
        Debug.Log("删除：" + path);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="varFromDirectory">文件夹内</param>
    /// <param name="varToDirectory">文件夹</param>
    public static  void CopyFolder(string varFromDirectory, string varToDirectory)
    {
        Directory.CreateDirectory(varToDirectory);
        //FuZhiWenJian(varFromDirectory, varToDirectory);
        if (!Directory.Exists(varFromDirectory)) return;

        string[] directories = Directory.GetDirectories(varFromDirectory);

        if (directories.Length > 0)
        {
            foreach (string d in directories)
            {
                CopyFolder(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
            }
        }
        string[] files = Directory.GetFiles(varFromDirectory);
        if (files.Length > 0)
        {
            foreach (string s in files)
            {
                File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), true);
            }
        }
    }

    /// <summary>
    /// 预制体修复2 筛选弓箭
    /// </summary>
    static void set1()
    {
        var a = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
        foreach (var b in a)
        {
            if (b.name.Contains("Bow") || b.name.Contains("bow"))
            {

            }
            else
            {
                //DestroyObject(b.gameObject);
                DestroyImmediate(b.gameObject);
            }

        }
    }

    /// <summary>
    /// 预制体修改阶段3 筛选弓箭骨骼
    /// </summary>
    static void set2()
    {

        var allGos = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        var previousSelection = Selection.objects;
        Selection.objects = allGos;
        var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        Selection.objects = previousSelection;
        foreach (var trans in selectedTransforms)
        {
            if (trans != null)
                if (trans.parent == null)
                    if (trans.name.Contains("Bow") || trans.name.Contains("bow"))
                    {
                        var sket = trans.GetComponentInChildren<SkinnedMeshRenderer>().rootBone.parent;
                        if (sket.Find("Bone01") && sket.Find("Bone08"))// && sket.Find("Bone04")
                        {
                            trans.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        DestroyImmediate(trans.gameObject);
                    }
        }
    }

    static void set3()
    {
        var allGos = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        var previousSelection = Selection.objects;
        Selection.objects = allGos;
        var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        Selection.objects = previousSelection;
        foreach (var trans in selectedTransforms)
        {
            if (trans != null)
                if (trans.parent == null)
                {
                    var mesh = trans.GetComponentInChildren<SkinnedMeshRenderer>();
                    Transform skp = mesh.rootBone.parent;
                    GameObject a = new GameObject("11");
                    a.transform.SetParent(trans);
                    //mesh.rootBone.transform.SetParent(trans);

                    //trans.GetComponent<Animator>().Play("");
                    //mesh.rootBone.transform.SetParent(skp);
                    EditorUtility.SetDirty(trans.gameObject);
                    //AssetDatabase.Refresh();
                }
        }
    }




}

public static class filename
{

    /// <summary>
    /// \ => //
    /// </summary>
    /// <param name="self"></param>
    /// <param name="backlevel"></param>
    /// <returns></returns>
    public static string PathBackLevel(this string self, int backlevel)
    {
        self = self.Replace("/", "\\");
        for (int i = 0; i < backlevel; i++)
        {
            self= self.Remove(self.LastIndexOf("\\"), self.Length  - self.LastIndexOf("\\"));
            //Debug.Log(self);
        }
        return self;
    }

    /// <summary>
    /// 从获取文件名字.类型
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string PathGetFileName(this string self)
    {
        self = self.Replace("/", "\\");
        if (self.LastIndexOf("\\") > -1)
            self = self.Substring(self.LastIndexOf("\\") + 1, self.Length - 1 - self.LastIndexOf("\\"));
        else
            Debug.Log("字符串非路径");
        return self;
    }

    /// <summary>
    /// 从获取文件名字.类型
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string PathGetFileName_NoType(this string self)
    {
        self = self.PathGetFileName();
        if (self.LastIndexOf('.') > -1)
            self = self.Remove(self.LastIndexOf('.') , self.Length - self.LastIndexOf('.'));
        else 
            Debug.Log("字符串非文件");
        return self;
    }

    /// <summary>
    /// 带文件类型的名字获取文件类型
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string PathGetFileType(this string self)
    {
        if (self.LastIndexOf("\\") != -1)
            self=self.PathGetFileName();
        if (self.LastIndexOf('.') == -1) return "";
        self = self.Substring(self.LastIndexOf("."), self.Length  - self.LastIndexOf("."));
        return self;
    }

}
#endif