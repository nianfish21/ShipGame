using LitJson;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Tool
{

    public static void LookAt2D(this Transform self, Vector3 positon)
    {
        Vector2 direction = positon - self.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        self.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="path"></param>
    /// <param name="RE">正则表达式</param>
    public static void JsonSave<T>(T obj, string path, string RE)
    {
        //string pattern = @"D:\W(\w*\W)*block";
        var directorypath = Regex.Matches(path, RE);
        //Regex.Matches(input, pattern)
        //print(directorypath.Count);
        //print(Directory.Exists(directorypath[0].Value));
        if (directorypath.Count > 0)
        {
            if (!Directory.Exists(directorypath[0].Value))
            {
                Directory.CreateDirectory(directorypath[0].Value);
            }
        }
        string Date = JsonMapper.ToJson(obj);
        File.WriteAllText(path, Date);
    }

    public static T JsonRead<T>(string path)
    {
        var Date = File.ReadAllText(path);
        return JsonMapper.ToObject<T>(Date);
    }

    public static Vector3 getMousePositon2D()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
    }

    /// <summary>
    /// 返回相机的正交视界
    /// </summary>
    /// <returns></returns>
    public static Rect CamareArea()
    {
        Vector3 camaer_maxpoint = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 10));
        Vector3 camaer_minpoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector2 rect_maxpoint = new Vector2(camaer_maxpoint.x, camaer_maxpoint.y);
        Vector2 rect_minpoint = new Vector2(camaer_minpoint.x, camaer_minpoint.y);
        Debug.DrawLine(rect_maxpoint, rect_minpoint);
        //print(rect_minpoint);
        //print(rect_maxpoint);
        Rect rect_to_obj = new Rect(rect_minpoint, rect_maxpoint - rect_minpoint);
        return rect_to_obj;
    }

    public static Rect CamareArea(Camera camarein)
    {
        Vector3 camaer_maxpoint = camarein.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 10));
        Vector3 camaer_minpoint = camarein.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector2 rect_maxpoint = new Vector2(camaer_maxpoint.x, camaer_maxpoint.y);
        Vector2 rect_minpoint = new Vector2(camaer_minpoint.x, camaer_minpoint.y);
        Debug.DrawLine(rect_maxpoint, rect_minpoint);
        Rect rect_to_obj = new Rect(rect_minpoint, rect_maxpoint - rect_minpoint);
        return rect_to_obj;
    }

    /// <summary>
    /// 只能识别0和1
    /// </summary>
    /// <param name="intin"></param>
    /// <returns></returns>
    public static string[] Int1DToString1D(int[] intin)
    {
        string[] typeout = new string[intin.Length];
        for (int i = 0; i < intin.Length; i++)
        {
            typeout[i] = intin[i] == 0 ? "none" : "ground";
        }
        return typeout;
    }

    public static string[] String2DToString1D(string[,] stringin, int width, int height)
    {
        string[] outs = new string[width * height];
        int a = 0;
        for (int y = 0; y < height; y++)//高度 Y   就只负责运行中间部分
        {
            for (int x = 0; x < width; x++)//宽度，列数 X
            {

                outs[a++] = stringin[x, y];//只取中心区域
            }
        }
        return outs;
    }

    public static string[,] String1DToString2D(string[] stringin, int width, int height)
    {
        string[,] outs = new string[width, height];
        int a = 0;
        for (int y = 0; y < height; y++)//高度 Y   就只负责运行中间部分
        {
            for (int x = 0; x < width; x++)//宽度，列数 X
            {

                outs[x, y] = stringin[a++];//只取中心区域
            }
        }
        return outs;
    }

    public static int[] Int2DToInt1D(int[,] in1, int width, int height)
    {
        int[] outs = new int[width * height];
        int a = 0;
        for (int y = 0; y < height; y++)//高度 Y   就只负责运行中间部分
        {
            for (int x = 0; x < width; x++)//宽度，列数 X
            {

                outs[a++] = in1[x, y];//只取中心区域
            }
        }
        return outs;
    }

    public static int[,] Int1DToInt2D(int[] in1, int width, int height)
    {
        int[,] outs = new int[width, height];
        int a = 0;
        for (int y = 0; y < height; y++)//高度 Y   就只负责运行中间部分
        {
            for (int x = 0; x < width; x++)//宽度，列数 X
            {

                outs[x, y] = in1[a++];//只取中心区域
            }
        }
        return outs;
    }

    /// <summary>
    /// 从2维索引获得1维索引
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static int GetIndexFromXY(int x, int y, int width, int height)
    {
        int indexout = -1;
        if (x >= width | x < 0 | y >= height | y < 0) return indexout;
        indexout = y * width + x;
        return indexout;
    }
    /// <summary>
    /// 从2维索引获得1维索引
    /// </summary>
    /// <param name="xy">位置</param>
    /// <param name="wh">宽高</param>
    /// <returns></returns>
    public static int GetIndexFromXY(int[] xy, int[] wh)
    {
        int indexout = -1;
        if (xy[0] >= wh[0] | xy[0] < 0 | xy[1] >= wh[1] | xy[1] < 0) return indexout;
        indexout = xy[1] * wh[0] + xy[0];
        return indexout;
    }
    /// <summary>
    /// 从1维索引获得2维索引
    /// </summary>
    /// <param name="index"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static int[] GetXYFormIndex(int index, int width)
    {
        var x = index % width;
        var y = (index - x) / width;
        return new int[2] { x, y };
    }

    /// <summary>
    /// 判断方框1和方框2是否相交
    /// </summary>
    /// <param name="rect1">方框1</param>
    /// <param name="rect2">方框2</param>
    /// <returns></returns>
    public static bool Intersect(Rect rect1, Rect rect2)
    {
        bool outb = !((rect1.xMax < rect2.xMin) | (rect1.yMax < rect2.yMin) | (rect1.xMin > rect2.xMax) | (rect1.yMin > rect2.yMax));
        return outb;
    }

    /// <summary>
    /// 判断点targetposition是否在这rect内
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="targetposition"></param>
    /// <returns></returns>
    public static bool Inside(Rect rect, Vector2 targetposition)
    {
        bool outa = targetposition.x < rect.xMax & targetposition.x > rect.xMin & targetposition.y < rect.yMax & targetposition.y > rect.yMin;
        return outa;
    }

    /// <summary>
    /// double 的rect转Rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Rect RectToUnityRect(double[] rect)
    {
        if (rect.Length != 4)
        {
            Debug.Log("ERROR");
            return new Rect();
        }
        return new Rect(new Vector2((float)rect[0], (float)rect[1]), new Vector2((float)rect[2] - (float)rect[0], (float)rect[3] - (float)rect[1]));
    }


    /// <summary>
    /// string转图片
    /// </summary>
    /// <param name="date"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture2D StringToTexture(string date, int width, int height)
    {

        Texture2D texout = new Texture2D(width, height);
        texout.LoadImage(Convert.FromBase64String(date));
        return texout;

    }

    /// <summary>
    /// 图片转string
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static string TextureToString(Texture2D a)
    {
        string strout = Convert.ToBase64String(a.EncodeToPNG());
        return strout;

    }

    /// <summary>
    /// 相机旋转都为0时使用，获得鼠标点击的位置上z轴距离相机正面为z单位的第一个物体
    /// </summary>
    /// <param name="z">2d透视检测的z轴坐标</param>
    /// <returns></returns>
    public static RaycastHit2D Get2DRayFormMouse(float z)
    {
        var ray = GetMouseWorldPosition2D(z);
        var hit = Physics2D.Raycast(new Vector3(ray.x, ray.y, 0), Vector2.zero);
        Debug.DrawRay(new Vector3(ray.x, ray.y, 0), Vector2.up);
        if (Camera.main.transform.rotation == new Quaternion(0, 0, 0, 0))
        { Debug.Log("camaer:rotation error"); }
        return hit;
    }

    public static Vector3 GetMouseWorldPosition2D(float z)
    {
        var onScreenPosition = Input.mousePosition;
        var positonOut = Camera.main.ScreenToWorldPoint(new Vector3(onScreenPosition.x, onScreenPosition.y, z));
        return positonOut;
    }

    /// <summary>
    /// 获得从世界坐标获得像素在图像内的位置二维位置
    /// </summary>
    /// <param name="pointPosition">查询的位置</param>
    /// <param name="spritPosition">精灵的位置，精灵位置在图片中心的情况</param>
    /// <param name="unit">几个像素一个单位</param>
    /// <param name="size">精灵的尺寸</param>
    /// <returns></returns>
    public static int[] GetSpritPixXYFormPosition(Vector2 pointPosition, Vector2 spritPosition, float unit, int[] size)
    {
        var spritToPoint = pointPosition - spritPosition;
        //如果是10个像素一个单元，那么1个距离就包含了10个像素，所以乘以unit就是一个距离包含一个像素，用来把这个距离变成和像素一样大
        spritToPoint *= unit;
        int x = size[0] / 2 + (int)spritToPoint.x;
        int y = size[1] / 2 + (int)spritToPoint.y;
        return new int[2] { x, y };
    }
    /// <summary>
    /// 获得一维向量图像内的像素坐标
    /// </summary>
    /// <param name="pointPosition"></param>
    /// <param name="spritPosition"></param>
    /// <param name="unit"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static int GetSpritPixIndexFormPosition(Vector2 pointPosition, Vector2 spritPosition, float unit, int[] size)
    {
        var spritToPoint = pointPosition - spritPosition;
        //如果是10个像素一个单元，那么1个距离就包含了10个像素，所以乘以unit就是一个距离包含一个像素，用来把这个距离变成和像素一样大
        spritToPoint *= unit;
        int x = size[0] / 2 + (int)spritToPoint.x;
        int y = size[1] / 2 + (int)spritToPoint.y;

        return Tool.GetIndexFromXY(x, y, size[0], size[1]);
    }

    /// <summary>
    /// 从像素坐标到世界坐标
    /// </summary>
    /// <param name="pixPositon"></param>
    /// <param name="spritPosition"></param>
    /// <param name="unit"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Vector2 GetPositionFormSpritPixXY(int[] pixPositon, Vector2 spritPosition, float unit, int[] size)
    {
        //出错了
        var spritToPix = new int[2] { pixPositon[0] - size[0] / 2, pixPositon[1] - size[1] / 2 };
        //如果是10个像素一个单元，那么1个距离就包含了10个像素，所以乘以unit就是一个距离包含一个像素，用来把这个距离变成和像素一样大
        Vector2 pixWorldPositon = new Vector2((float)spritToPix[0] / unit, (float)spritToPix[1] / unit);
        pixWorldPositon += spritPosition;
        return pixWorldPositon;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect">minx miny maxx maxy</param>
    public static void DebugBox(float[] rect, float z)
    {

        Debug.DrawLine(new Vector3((float)rect[0], (float)rect[1], z), new Vector3((float)rect[2], (float)rect[1], z));
        Debug.DrawLine(new Vector3((float)rect[0], (float)rect[1], z), new Vector3((float)rect[0], (float)rect[3], z));
        Debug.DrawLine(new Vector3((float)rect[0], (float)rect[3], z), new Vector3((float)rect[2], (float)rect[3], z));
        Debug.DrawLine(new Vector3((float)rect[2], (float)rect[1], z), new Vector3((float)rect[2], (float)rect[3], z));
    }


    public enum DirEnum
    {
        None,
        Right,
        Left,
        Top,
        Bottom,
        Front,
        Back
    }


    /// <summary>
    /// 判断触发的方向
    /// </summary>
    public static DirEnum[] JudgeColliderDir(Transform transform, Collider2D other)
    {
        Vector3 transPos = transform.position;
        Vector3 closePos = other.ClosestPoint(transPos);
        Vector3 dir = (transPos - closePos).normalized;
        List<DirEnum> dirEnum = new List<DirEnum>();
        if (dir.x >= 1)
        {
            dirEnum.Add( DirEnum.Right);
        }
        if (dir.x <= -1)
        {
            dirEnum.Add(DirEnum.Left);
        }
        if (dir.y >= 1)
        {
            dirEnum.Add(DirEnum.Bottom);
        }
        if (dir.y <= -1)
        {
            dirEnum.Add(DirEnum.Top);
        }
        if (dir.z >= 1)
        {
            dirEnum.Add(DirEnum.Front);
        }
        if (dir.z <= -1)
        {
            dirEnum.Add(DirEnum.Back);
        }
        return dirEnum.ToArray();
    }
    ///// <summary>
    ///// 判断碰撞的方向
    ///// </summary>
    //public static  DirEnum JudgeCollisionDir(Collider2D other)
    //{
    //    DirEnum dirEnum = DirEnum.None;
    //    Vector3 dir = other.contacts[0].normal;
    //    if (dir.x >= 1)
    //    {
    //        dirEnum = DirEnum.Right;
    //    }
    //    if (dir.x <= -1)
    //    {
    //        dirEnum = DirEnum.Left;
    //    }
    //    if (dir.y >= 1)
    //    {
    //        dirEnum = DirEnum.Bottom;
    //    }
    //    if (dir.y <= -1)
    //    {
    //        dirEnum = DirEnum.Top;
    //    }
    //    if (dir.z >= 1)
    //    {
    //        dirEnum = DirEnum.Front;
    //    }
    //    if (dir.z <= -1)
    //    {
    //        dirEnum = DirEnum.Back;
    //    }
    //    return dirEnum;
    //}





}
    