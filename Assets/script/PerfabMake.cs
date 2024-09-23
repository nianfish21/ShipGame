
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

    [MenuItem("zpyTools/������Դ����", false)]
    static void FindAssetRefMenu()
    {
        if (Selection.assetGUIDs.Length == 0)
        {
            Debug.Log("����ѡ������һ��������ٻ��˲˵�");
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
        Debug.Log(string.Format("��ʼ��������{0}����Դ��", string.Join(",", assetPaths)));
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
                        log = string.Format("{0} ������ {1}", path, assetPaths[j]);
                        logInfo.Add(log);
                    }
                }
            }
        }

        for (int i = 0; i < logInfo.Count; i++)
        {
            Debug.Log(logInfo[i]);
        }

        Debug.Log("ѡ���������������" + logInfo.Count);

        Debug.Log("�������");
    }
}



//�޸Ķ�������
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

    [MenuItem("zpyTools/���ߴ���")]
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


        anifix = GUILayout.Toggle(anifix, "�����޸�����");
        if (anifix)
            AniPerfabFix();

        matshengcheng = GUILayout.Toggle(matshengcheng, "���ɲ�����");
        if(matshengcheng)
            MatShencheng();

        perbafmatChang = GUILayout.Toggle(perbafmatChang, "���ղ���Ԥ��������滻");
        if(perbafmatChang)
            PerFabAddMaterial();

        test212 = GUILayout.Toggle(test212, "test");
        if (test212)
            test();

        jiebaowenjiantihuan = GUILayout.Toggle(jiebaowenjiantihuan, "�ļ��滻");
        if (jiebaowenjiantihuan)
            XiuGaiWenJian();
    }

   
    //-------------------------�����޸�Ԥ����-------------------

    //���ݶ�����һ֡����Ԥ���嶯��
    public void  AniPerfabFix()
    {
        //��Ӷ���clip
        GUILayout.Space(10);
        ani = (AnimationClip)EditorGUILayout.ObjectField("animationclip", ani, typeof(AnimationClip));

        if (GUILayout.Button("�޸�ѡ���Ԥ�����ļ�:������һ֡"))
        {
            perfabChangeByAni();
        }

        if (GUILayout.Button("����Ԥ����ߴ��޸�"))
        {
            sizeFix();
        }
    }


    string[] assetGUIDs;
    string[] assetPaths;
    string[] allAssetPaths;

    //ͨ�������������޸�����ĸ�����״̬
    public void perfabChangeByAni()
    {
        if (Selection.assetGUIDs.Length == 0)
        {
            Debug.Log("����ѡ������һ��������ٻ��˲˵�");
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
            GameObject perfab = PrefabUtility.LoadPrefabContents(assetPaths[i]);//����
            Debug.Log("���ص�ַ" + assetPaths[i]);

            //�޸�����
            {
                var mesh = perfab.GetComponentInChildren<SkinnedMeshRenderer>();
                ani.SampleAnimation(mesh.rootBone.transform.parent.gameObject, 0.1f);
                Debug.Log(perfab.name + "���");
            }
            //DestroyImmediate( perfab.GetComponentInChildren<SkinnedMeshRenderer>().gameObject);

            PrefabUtility.SaveAsPrefabAsset(perfab, assetPaths[i]);
            Debug.Log("�����ַ" + assetPaths[i]);

            PrefabUtility.UnloadPrefabContents(perfab);//ж��
            Debug.Log("ж�ص�ַ" + assetPaths[i]);
        }

    }
    /// <summary>
    /// Ԥ�����޸�1 �ߴ��޸�
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

    //----------------------һЩ����----------------

    public void tool()
    {
        GUILayout.Label("����");

        GUILayout.Space(10);

        if (GUILayout.Button("ѡ���ļ��У��޸�ͼƬ���ʽ"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
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
                        GameObject perfab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));//����
                        Debug.Log("���ص�ַ" + path);
                        GameObject a = GameObject.Instantiate(perfab);
                        a.name = perfab.name;
                    }

                }
            }

        }
    }

    //------------------��������-------------------

    Thread thread2;
    public string matguid=""; //�滻ͼƬuid
    public string MatShenchengpath1 = ""; //���ʵ�ַ
    public string MatShenchengpath2 = ""; //���ɵ�ַ
    /// <summary>
    /// ���������ͼƬ��������
    /// </summary>
    public void MatShencheng()
    {
        GUILayout.Label("�滻ԭguid��c519d2c2f0d48aa45a8dd6cfa589f3d9");

        GUILayout.Label("1.�滻ͼƬ��guid");
        EditorGUILayout.TextField(matguid);
        if (GUILayout.Button("ѡ��ͼƬguid"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
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

        GUILayout.Label("2.ԭʼmat��ַ");
        EditorGUILayout.TextField(MatShenchengpath1);
        if (GUILayout.Button("ѡ�����"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
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

        GUILayout.Label("3.�滻��mat�ļ��е�ַ");
        EditorGUILayout.TextField(MatShenchengpath2);
        if (GUILayout.Button("ѡ���ļ���"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
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


        if (GUILayout.Button("����mat�߳�"))
        {
            thread2 = new Thread(new ThreadStart(MatCreate));
            thread2.Start();

        }
        if (GUILayout.Button("�رմ���mat�߳�"))
        {
            if (thread2 != null)
                if (thread2.IsAlive)
                    thread2.Abort();

        }
        GUILayout.Space(10);

    }

    private void MatCreate()
    {

        Debug.Log("����mat");
        //ֱ���õ������ļ�
        string[] files = Directory.GetFiles(MatShenchengpath2, "*.*", SearchOption.AllDirectories);
        //string[] files = Directory.GetFiles(path2);
        if (files.Length > 0)
        {
            Debug.Log("���ļ�:" + files.Length);
            int i = 0;
            foreach (string s in files)
            {
                try
                {
                    if (s.PathGetFileType() == ".jpg" || s.PathGetFileType() == ".JPG" || s.PathGetFileType() == ".tga") //��ͼƬ
                    {
                        string metaPath = s + ".meta";

                        string metacontent = File.ReadAllText(metaPath); //��ȡ����ͼƬ��guid
                        string guid = metacontent.Substring(metacontent.IndexOf("guid: ") + 6, 32);

                        string matcontent = File.ReadAllText(MatShenchengpath1);
                        //Debug.Log(i + "λ�ã�1");
                        if (matcontent == null)
                        {
                            continue;
                        }
                        if(matguid!= guid) //��guid
                            while (matcontent.IndexOf(matguid) > 0)
                            {

                                matcontent = matcontent.Replace(matguid, guid);
                            }
                        int MatNameNum = 0; //������
                        if(MatShenchengpath1.PathGetFileName_NoType()!= s.PathGetFileName_NoType())
                            while (matcontent.IndexOf(MatShenchengpath1.PathGetFileName_NoType()) > 0)
                            {
                                MatNameNum++;
                                matcontent = matcontent.Replace(MatShenchengpath1.PathGetFileName_NoType(), s.PathGetFileName_NoType());
                            }
                        if (MatNameNum > 1)
                        {
                            Debug.LogError("����ҵ�ԭʼ�������֣���������޸ĵĲ��֣�����ԭʼ�����������޸�" + s.PathBackLevel(1) + "\\" + s.PathGetFileName_NoType() + ".mat");
                        }
                        File.WriteAllText(s.PathBackLevel(1) + "\\" + s.PathGetFileName_NoType() + ".mat", matcontent);

                        Debug.Log( i+"λ�ã�" + s.PathBackLevel(1) + "\\" + s.PathGetFileName_NoType() + ".mat"+"\n"+guid);
                    }

                }
                catch
                {
                    Debug.LogError("����" + s);
                }
                i++;
            }
        }
        Debug.Log("����mat�߳�ֹͣ");
    }

    //Ԥ�������
    public void PerFabAddMaterial()
    {

        GUILayout.Space(10);

        if (GUILayout.Button("���ļ��м���Ԥ����"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
                return;
            }

            assetGUIDs = Selection.assetGUIDs;

            assetPaths = new string[assetGUIDs.Length];

            for (int i = 0; i < assetGUIDs.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
            }

            //����ѡ���ļ��е�����Ԥ����
            string[] files = Directory.GetFiles(assetPaths[0], "*.*", SearchOption.AllDirectories);
            //string[] files = Directory.GetFiles(path2);
            if (files.Length > 0)
            {
                foreach (string path in files)
                {
                    if (path.PathGetFileType() == ".fbx")
                    {
                        GameObject perfab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));//����
                        Debug.Log("���ص�ַ" + path);
                        GameObject a = GameObject.Instantiate(perfab);
                        a.name = perfab.name;
                    }

                }
            }

        }


        if (GUILayout.Button("ѡ���ļ������������������������ͬ������"))
        {

            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
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
                    //��ȡ����
                    string matname = skin.sharedMaterials[i].name;
                    if (matname.LastIndexOf('.') > 0)
                        matname = matname.Substring(0, matname.LastIndexOf('.'));

                    Material matReplace = null;

                    //����ѡ���ļ��еĲ���
                    string[] files = Directory.GetFiles(assetPaths[0], "*.*", SearchOption.AllDirectories);
                    //string[] files = Directory.GetFiles(path2);
                    if (files.Length > 0)
                    {
                        foreach (string path in files)
                        {
                            if (path.PathGetFileName_NoType() == matname && path.PathGetFileType() == ".mat")
                            {

                                matReplace = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));//����

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



    //---------------------����----------------------
    GameObject p1;
    public void test()
    {
        defaultMaterial = (Material)EditorGUILayout.ObjectField("Material", defaultMaterial, typeof(Material));

        if (GUILayout.Button("�����ļ�.fbx"))
        {
            if (Selection.assetGUIDs.Length == 0 || Selection.assetGUIDs.Length > 1)
            {
                Debug.Log("����ѡ������һ���ļ��У��ٻ��˲˵�");
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
            //����renderer
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
            //����material
            var keys = new Dictionary<string, bool>();
            foreach (var render in importer.GetExternalObjectMap())
            {
                if (!keys.ContainsKey(render.Key.name.PathGetFileName_NoType()))
                {
                    keys.Add(render.Key.name, true);
                    Debug.Log(render.Key.name);
                }
            }
            

            //�滻ΪĬ��material
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
                    importer.AddRemap(id, newMaterial);//���ľ��� �޸�
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
            //    GameObject perfab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));//����
            //    Debug.Log("���ص�ַ" + path);
            //    GameObject p1 = GameObject.Instantiate(perfab);
            //    p1.name = perfab.name;
            //}
        }
    }

    //DefaultMaterial :ָ��Material
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


    //ɸѡ ͬ�� �ļ�����1 �滻�ļ�����2
    string type1 = "";
    string type2 = "";
    public string XiuGaiWenJianpath1 = "";
    public string XiuGaiWenJianpath2 = "";
    Thread thread;
    //��ozj ��bmp �������smd��ͼƬ�ļ��ƶ�������ļ��е�λ�� ��һ�����ú�ɾ��ozj ��bmp�ļ� ���ý�����һ��תfbx
    public void XiuGaiWenJian()
    {
        GUILayout.Label("��׺��1");
        type1 = EditorGUILayout.TextField(type1);
        GUILayout.Label("�ļ���ַ1");
        XiuGaiWenJianpath1 = EditorGUILayout.TextField(XiuGaiWenJianpath1);
        GUILayout.Space(10);
        GUILayout.Label("��׺��2");
        type2 = EditorGUILayout.TextField(type2);
        GUILayout.Label("�ļ���ַ2");
        XiuGaiWenJianpath2 = EditorGUILayout.TextField(XiuGaiWenJianpath2);



        
        if (GUILayout.Button("�滻�ļ�����һ���ļ����������ļ�"))
        {
            ////ֱ���õ������ļ�
            //string[] files = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories);
            //foreach (string file in files)
            //{    // ����ÿ���ļ�}
            //    Debug.Log(file);
            //}

            

            ////��ȡ�ļ���
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
            //                    //���ļ�1λ�������ļ����Ƶ��ļ�2λ�ò���ɾ���ļ�2
            //                    Delet(file2);
            //                    Debug.Log("ɾ��:" + file2);
            //                    CopyFolder(file.PathBackLevel(1), file2.PathBackLevel(1));
            //                }
            //            }

            //        }
            //    }
            //}
        }



        if (GUILayout.Button("�滻�ļ��������ļ�"))
        {
            thread = new Thread(new ThreadStart(TiHuan));
            thread.Start();
            Debug.Log("�滻�߳̿�ʼ");
        }

        if (GUILayout.Button("ֹͣ�滻�ļ�"))
        {
            if(thread !=null)
                if(thread.IsAlive)
                    thread.Abort();
            //����Thread.Abort������ͼǿ����ֹthread�߳�

        }

        //if (GUILayout.Button("testɾ���ļ���ַ1"))
        //{
        //    Delet(XiuGaiWenJianpath1);
        //}
        //if (GUILayout.Button("test�����ļ�1���ļ���2λ��"))
        //{
        //    CopyFile(XiuGaiWenJianpath1, XiuGaiWenJianpath2);
        //}
        //if (GUILayout.Button("test�����ļ���1���ļ���2λ��"))
        //{
        //    XiuGaiWenJianpath1 = XiuGaiWenJianpath1.PathGetFileName_NoType();
        //}
    }

    //�滻�ļ�
    public void TiHuan()
    {
        //ֱ���õ������ļ�
        string[] files = Directory.GetFiles(XiuGaiWenJianpath1, "*.*", SearchOption.AllDirectories);
        Debug.Log("��ɸѡ�ļ�������:"+ files.Length );

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
                        //���ļ�1λ�������ļ����Ƶ��ļ�2λ�ò���ɾ���ļ�2
                        Delet(file2);
                        Debug.Log("ɾ��:" + file2);
                        CopyFile(file, file2.PathBackLevel(1) + "\\" + file.PathGetFileName());
                    }
                }
            }
        }
        Debug.Log("ֹͣ");
    }

    /// <summary>
    /// �����ļ���·����
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
        Debug.Log("���ƣ�" + dir1);
    }

    /// <summary>
    /// ɾ��·�����ļ�
    /// </summary>
    /// <param name="path"></param>
    public static  void Delet(string path)
    {
        // �������ļ�����
        FileInfo fi = new FileInfo(path);
        // �����ļ�
        FileStream fs = fi.Create();
        // ������Ҫ�޸��ļ���Ȼ��ر��ļ���
        fs.Close();
        // ɾ�����ļ���
        fi.Delete();
        Debug.Log("ɾ����" + path);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="varFromDirectory">�ļ�����</param>
    /// <param name="varToDirectory">�ļ���</param>
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
    /// Ԥ�����޸�2 ɸѡ����
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
    /// Ԥ�����޸Ľ׶�3 ɸѡ��������
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
    /// �ӻ�ȡ�ļ�����.����
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string PathGetFileName(this string self)
    {
        self = self.Replace("/", "\\");
        if (self.LastIndexOf("\\") > -1)
            self = self.Substring(self.LastIndexOf("\\") + 1, self.Length - 1 - self.LastIndexOf("\\"));
        else
            Debug.Log("�ַ�����·��");
        return self;
    }

    /// <summary>
    /// �ӻ�ȡ�ļ�����.����
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string PathGetFileName_NoType(this string self)
    {
        self = self.PathGetFileName();
        if (self.LastIndexOf('.') > -1)
            self = self.Remove(self.LastIndexOf('.') , self.Length - self.LastIndexOf('.'));
        else 
            Debug.Log("�ַ������ļ�");
        return self;
    }

    /// <summary>
    /// ���ļ����͵����ֻ�ȡ�ļ�����
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