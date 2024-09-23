using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepIland
{
    public class BascSubstance : MonoBehaviour
    {
        public delegate void pixelModule(string pixTag);
        //像素物理操作的集合
        public event pixelModule pixelModuleCollection;

        public int[] size = new int[2] { 128, 128 };
        public DirtyRect dirtyRect;
        //public List<string> pixTags;//所有可以识别的像素tag
        public LayerThings texs;
        // Start is called before the first frame update
        void Start()
        {
            #region 初始化，添加一层的像素层和标签层，添加像素层生成的精灵
            texs = new LayerThings()
            {
                layerCount = 1,
                layersTex = new List<Color[]>(),
                TexTag = new List<string[]>(),
                size = size,
            };
            //print(texs.size[0]);

            //print(texs.size[1]);
            int count = size[0] * size[1];
            var c = new Color[count];
            var t = new string[count];

            for (int i = 0; i < count; i++)
            {
                c[i] = Color.clear;
                t[i] = "air";
            }
            texs.layersTex.Add(c);
            texs.TexTag.Add(t);
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(new Texture2D(size[0], size[1]), new Rect(0, 0, size[0], size[1]), new Vector2(0.5f, 0.5f), 10f);
            GetComponent<SpriteRenderer>().sprite.texture.filterMode = FilterMode.Point;
            GetComponent<SpriteRenderer>().sprite.texture.Apply();
            #endregion
            dirtyRect = new DirtyRect();
        }

        // Update is called once per frame
        void Update()
        {
            //print(dirtyRect.max);
            #region 通过鼠标在图像上添加沙子像素
            if (Input.GetMouseButton(0))
            {
                var hit = Tool.Get2DRayFormMouse(10);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<BascSubstance>())
                    {
                        var mousePosition = Tool.GetMouseWorldPosition2D(10);

                        var obj = hit.collider.gameObject;
                        int[] size = new int[2] {  obj.GetComponent<SpriteRenderer>().sprite.texture.width, obj.GetComponent<SpriteRenderer>().sprite.texture.height };

                        //一个是xy坐标，一个是color[]位置
                        var pixXY = Tool.GetSpritPixXYFormPosition(mousePosition, gameObject.transform.position, 10, size);
                        var pixIndex = Tool.GetSpritPixIndexFormPosition(mousePosition, gameObject.transform.position, 10, size);

                        var thing = hit.collider.gameObject.GetComponent<BascSubstance>().texs;
                        #region 改变他的层类
                        int[] newxy = new int[2];
                        for (int x = -2; x < 2; x++)
                        {
                            for (int y = -2; y < 2; y++)
                            {
                                newxy[0] = pixXY[0] + x;
                                newxy[1] = pixXY[1] + x ;
                                SetPixAndRect(texs, 0, newxy, Color.yellow, "Sand", dirtyRect);
                            }
                        }

                        obj.GetComponent<SpriteRenderer>().sprite.texture.SetPixels(thing.GetTex());
                        obj.GetComponent<SpriteRenderer>().sprite.texture.Apply();
                        #endregion

                        //设置颜色
                        //obj.GetComponent<SpriteRenderer>().sprite.texture.SetPixel(pixXY[0], pixXY[1], Color.black);
                        //obj.GetComponent<SpriteRenderer>().sprite.texture.Apply();
                        //print(pixXY[0] + "," + pixXY[1]);
                    }
                }
            }
            #endregion
            //var xy = new int[2] { 0, 0 };
            #region 脏矩形遍历
            var count = texs.size[1] * texs.size[0];
            //当块不脏时
            if (!dirtyRect.isDirty)
            {
                dirtyRect.CreatRect();
            }
            else
            {
                var size = texs.size;
                int[] xy = new int[2];
                int index = 0;
                for (int y = 0; y <= dirtyRect.rect[3]; y++) //？？这里的0 和-1是否对不上
                {
                    for (int x = 0; x <= dirtyRect.rect[2]; x++)
                    {
                        xy[0] = x + dirtyRect.rect[0];
                        xy[1] = y + dirtyRect.rect[1];
                        index = Tool.GetIndexFromXY(xy[0], xy[1], size[0], size[1]);

                        var dirtyPix = Sand(texs, index);

                        if (dirtyPix.istrue)
                        {   //当没有这次的第一个最大最小块
                            dirtyRect.AddDirtyPix(dirtyPix.xy);
                        }
                    }
                }
                //如果遍历完矩形里面的像素，没有发现不稳定像素，就标记没有脏矩形
                if (dirtyRect.DirtyPixAmount==0)
                {
                    dirtyRect.isDirty = false;
                }

                dirtyRect.CreatRect();
            }

            #endregion
            var min = Tool.GetPositionFormSpritPixXY(new int[2] { dirtyRect.min.x, dirtyRect.min.y },transform.position,10,texs.size);
            var max = Tool.GetPositionFormSpritPixXY(new int[2] { dirtyRect.max.x, dirtyRect.max.y }, transform.position, 10, texs.size);
            Tool.DebugBox(new float[4] { min.x, min.y, max.x, max.y },0);
            GetComponent<SpriteRenderer>().sprite.texture.SetPixels(texs.GetTex());
            GetComponent<SpriteRenderer>().sprite.texture.Apply();


        }
        public static void Layer1FallingSand(LayerThings layerThing)
        {

        }
        /// <summary>
        /// 在添加像素的同事更新脏矩形
        /// </summary>
        /// <param name="layerThing"></param>
        /// <param name="index"></param>
        /// <param name="pixColor"></param>
        /// <param name="pixTag"></param>
        /// <param name="dirtyRect"></param>
        public static void SetPixAndRect(LayerThings layerThing,int layer, int index,Color pixColor,string pixTag,DirtyRect dirtyRect)
        {

            layerThing.layersTex[layer][index] = pixColor;
            layerThing.TexTag[layer][index] = pixTag;
            var pixPosition = Tool.GetXYFormIndex(index, layerThing.size[0]);
            dirtyRect.AddDirtyPix(new Vector2Int(pixPosition[0], pixPosition[1]));

        }
        public static void SetPixAndRect(LayerThings layerThing, int layer, int[]xy, Color pixColor, string pixTag, DirtyRect dirtyRect)
        {
            if (xy[0] < layerThing.size[0] & xy[0] >= 0 & xy[1] < layerThing.size[1] & xy[1] >= 0)
            {
                var index = Tool.GetIndexFromXY(xy, layerThing.size);
                layerThing.layersTex[layer][index] = pixColor;
                layerThing.TexTag[layer][index] = pixTag;
                var pixPosition = Tool.GetXYFormIndex(index, layerThing.size[0]);
                dirtyRect.AddDirtyPix(new Vector2Int(pixPosition[0], pixPosition[1]));
            }


        }
        /// <summary>
        /// 沙子的行为
        /// </summary>
        /// <param name="layerThing"></param>
        /// <param name="xy"></param>
        public static void Sand(LayerThings layerThing, int[] xy)
        {
            //宽高
            var size = layerThing.size;
            int[,] go = new int[3, 2] { { 0, -1 }, { -1, -1 }, { 1, -1 } };
            var xygo = new int[2];
            var indexXY = Tool.GetIndexFromXY(xy, size);
            if (layerThing.TexTag[0][indexXY] == "Sand")
            {
                for (int i = 0; i < 3; i++)
                {
                    xygo[0] = xy[0] + go[i, 0];
                    xygo[1] = xy[1] + go[i, 1];
                    if (xygo[0] < layerThing.size[0] & xygo[0] >= 0 & xygo[1] < layerThing.size[1] & xygo[1] >= 0)
                    {
                        var indexXYGo = Tool.GetIndexFromXY(xygo, layerThing.size);
                        if (layerThing.TexTag[0][indexXYGo] == "air")
                        {
                            Displace(layerThing, xy, xygo);
                            break;
                        }
                        if (layerThing.TexTag[0][indexXYGo] == "Water")
                        {
                            Displace(layerThing, xy, xygo);
                            break;
                        }
                    }
                }
            }
        }
        public static BoolAndVector2Int Sand(LayerThings layerThing, int index)
        {
            if (layerThing.TexTag[0][index] == "Sand")
            {
                var xy = Tool.GetXYFormIndex(index, layerThing.size[0]);
                int[,] go = new int[3, 2] { { 0, -1 }, { -1, -1 }, { 1, -1 } };
                var xygo = new int[2];
                for (int i = 0; i < 3; i++)
                {
                    xygo[0] = xy[0] + go[i, 0];
                    xygo[1] = xy[1] + go[i, 1];
                    if (xygo[0] < layerThing.size[0] & xygo[0] >= 0 & xygo[1] < layerThing.size[1] & xygo[1] >= 0)
                    {
                        var indexXYGo = Tool.GetIndexFromXY(xygo, layerThing.size);
                        if (layerThing.TexTag[0][indexXYGo] == "air")
                        {
                            Displace(layerThing, xy, xygo);
                            return new BoolAndVector2Int(true,new Vector2Int(xygo[0],xygo[1]));
                            //break;
                        }
                        if (layerThing.TexTag[0][indexXYGo] == "Water")
                        {
                            Displace(layerThing, xy, xygo);
                            break;
                        }
                    }
                }
            }
            return new BoolAndVector2Int(false, new Vector2Int());
        }

        /// <summary>
        /// 水的行为
        /// </summary>
        /// <param name="layerThing"></param>
        /// <param name="xy"></param>
        public static void Water(LayerThings layerThing, int[] xy)
        {
            //宽高
            var size = layerThing.size;
            int[,] go = new int[5, 2] { { 0, -1 }, { -1, -1 }, { 1, -1 },{ -1,0},{1,0 } };
            var indexXY = Tool.GetIndexFromXY(xy, size);
            int random = UnityEngine.Random.Range(0, 2);
            
            if (layerThing.TexTag[0][indexXY] == "Water")
            {
                //print(1);
                for (int i = 0; i < 5; i++)
                {
                    
                    int a = i;
                    
                    if (i == 2 & random == 1) a = 1;
                    if (i == 1 & random == 1) a = 2;
                    if (i == 3 & random == 1) a = 4;
                    if (i == 4 & random == 1) a = 3;
                    //print(a);
                    var xygo = new int[2] { xy[0] + go[a, 0], xy[1] + go[a, 1] };
                    if (xygo[0] < layerThing.size[0] & xygo[0] >= 0 & xygo[1] < layerThing.size[1] & xygo[1] >= 0)
                    {
                        var indexXYGo = Tool.GetIndexFromXY(xygo, layerThing.size);
                        if (layerThing.TexTag[0][indexXYGo] == "air")
                        {
                            Displace(layerThing, xy, xygo);
                            break;
                        }
                        if (layerThing.TexTag[0][indexXYGo] == "Sand")
                        {
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 两个像素位置调换
        /// </summary>
        /// <param name="layerThing"></param>
        /// <param name="xy"></param>
        /// <param name="xyGo"></param>
        public static void Displace(LayerThings layerThing, int[] xy, int[] xyGo)
        {
            var indexXY = Tool.GetIndexFromXY(xy, layerThing.size);
            var indexXYGo = Tool.GetIndexFromXY(xyGo, layerThing.size);
            var a = layerThing.TexTag[0][indexXY];
            layerThing.TexTag[0][indexXY] = layerThing.TexTag[0][indexXYGo];
            layerThing.TexTag[0][indexXYGo] = a;
            var b = layerThing.layersTex[0][indexXY];
            layerThing.layersTex[0][indexXY] = layerThing.layersTex[0][indexXYGo];
            layerThing.layersTex[0][indexXYGo] = b;


        }
        public static void Displace(LayerThings layerThing, int index1, int index2)
        {
            
            var a = layerThing.TexTag[0][index1];
            layerThing.TexTag[0][index1] = layerThing.TexTag[0][index2];
            layerThing.TexTag[0][index2] = a;
            var b = layerThing.layersTex[0][index1];
            layerThing.layersTex[0][index1] = layerThing.layersTex[0][index2];
            layerThing.layersTex[0][index2] = b;


        }



        //老旧的东西
        /// <summary>
        /// 输入9个图片，然后执行中心的
        /// </summary>
        /// <param name="things"></param>
        public void PixelPhysicsWorkLayer(LayerThings[] things)
        {
            for (int y = 0; y < things[5].size[1]; y++)
            {
                for (int x = 0; x < things[5].size[0]; x++)
                {
                    PixelWaterModuleLayer(things, x,y,0);
                }
            }
        }
        public void PixelWaterModuleLayer(LayerThings[] thing, int x, int y,int layer)
        {
            int[] down = { 0, -1 };
            int[,] downside = { { -1, -1 }, { 1,- 1 } };
            int[,] side = { {1, 0 },{ -1, 0 } };

            int index = Tool.GetIndexFromXY(x, y, thing[5].size[0], thing[5].size[1]);
            if (thing[5].TexTag[layer][index] == "water")
            {
                bool isWork = false;
                if(!isWork&thing[4].TexTag[layer][ Tool.GetIndexFromXY(x+down[0], y+down[1], thing[4].size[0], thing[4].size[1])] == "air")
                {
                    isWork = true;
                    PixelDisPlace(thing, index, x + down[0], y + down[1], layer);
                }
                for(int i=0;i<2;i++)
                {
                    var x2 = x + downside[i, 0];
                    var y2 = y + downside[i, 1];
                    if (!isWork & thing[4].TexTag[layer][ Tool.GetIndexFromXY(x2, y2 ,thing[4].size[0], thing[4].size[1])] == "air")
                    {
                        isWork = true;
                        PixelDisPlace(thing,index, x2, y2, layer);
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    var x2 = x + side[i, 0];
                    var y2 = y + side[i, 1];
                    if (!isWork & thing[4].TexTag[layer][ Tool.GetIndexFromXY(x2, y2, thing[4].size[0], thing[4].size[1])] == "air")
                    {
                        isWork = true;
                        PixelDisPlace(thing, index, x2, y2, layer);
                    }
                }
            }
        }
        public void PixelDisPlace(LayerThings[] thing,int index1,int x2,int y2,int layer)
        {
            int thingnumber = 4;
            if (x2 > thing[5].size[0])
            {
                x2 = x2 - thing[5].size[0];
                if (y2 > thing[4].size[1])
                {
                    y2 = y2 - thing[5].size[1];
                    thingnumber = 2;
                } 
                if (y2 < thing[4].size[1])
                {
                    y2 = y2 + thing[5].size[1];
                    thingnumber = 8;
                }
                thingnumber = 5;
            }
            else if(x2 < thing[5].size[0])
            {
                x2 = x2 + thing[5].size[0];
                if (y2 > thing[4].size[1])
                {
                    y2 = y2 - thing[5].size[1];
                    thingnumber = 0;
                }
                if (y2 < thing[4].size[1])
                {
                    y2 = y2 + thing[5].size[1];
                    thingnumber = 6;
                }
                thingnumber = 3;
            }
            else
            {
                if (y2 > thing[4].size[1])
                {
                    y2 = y2 - thing[5].size[1];
                    thingnumber = 1;
                }
                if (y2 < thing[4].size[1])
                {
                    y2 = y2 + thing[5].size[1];
                    thingnumber = 7;
                }
                thingnumber = 4;
            }

            int index2 = Tool.GetIndexFromXY(x2, y2, thing[thingnumber].size[0], thing[thingnumber].size[1]);

            var c = thing[5].TexTag[layer][ index1];
            thing[5].TexTag[layer][ index1] = thing[thingnumber].TexTag[layer][ index2];
            thing[thingnumber].TexTag[layer][ index2] = c;

            var d = thing[5].layersTex[layer][ index1];
            thing[5].layersTex[layer][ index1] = thing[thingnumber].layersTex[layer][ index2];
            thing[thingnumber].layersTex[layer][ index2] = d;
        }
    }

    public class DirtyRect
    {

        //x,y,sizex,sizey
        public bool isDirty=false;
        //public bool haveFirstDirtyPix = false;
        public int DirtyPixAmount = 0;
        public Vector2Int min=new Vector2Int();
        public Vector2Int max=new Vector2Int();
        public int[] rect=new int[4] { 0,0,0,0};

        public void AddDirtyPix(Vector2Int xy)
        {
            if (DirtyPixAmount == 0)
            {
                max = xy;
                min = xy;
                DirtyPixAmount++;
                isDirty = true;
            }
            else
            {
                DirtyPixAmount++;
                if (xy.x < min.x) min.x = xy.x;
                if (xy.y < min.y) min.y = xy.y;
                if (xy.x > max.x) max.x = xy.x;
                if (xy.y > max.y) max.y = xy.y;
            }
        }
        public void CreatRect()
        {
            rect[0] = min.x;
            rect[1] = min.y;
            rect[2] = max.x - min.x;
            rect[3] = max.y - min.y;
            DirtyPixAmount = 0;
        }
        /// <summary>
        /// 遍历法1
        /// </summary>
        /// <param name="layerThing"></param>
        public void bianli1(LayerThings layerThing)
        {
            var size = layerThing.size;
            int[] xy = new int[2];
            int index=0;
            for(int x = 0; x < rect[2]; x++)
            {
                for (int y = 0; y < rect[3]; y++)
                {
                    xy[0] = x + rect[0];
                    xy[1] = y + rect[1];
                    index = Tool.GetIndexFromXY(xy[0], xy[1], size[0], size[1]);
                }
            }
        }
        public void RectACT(LayerThings layerThing)
        {

            var size = layerThing.size;
            //脏像素
            Vector2Int xy = new Vector2Int();
            Vector2Int xymin = new Vector2Int();
            Vector2Int xymax = new Vector2Int();
            int index = 0;
            for (int x = 0; x < size[0]; x++)
            {
                for (int y = 0; y < size[1]; y++)
                {
                    xy.x= x + size[0];
                    xy.y = y + size[1];
                    if (xy.x < xymin.x) xymin.x = xy.x;
                    if (xy.y < xymin.y) xymin.y = xy.y;
                    if (xy.x > xymax.x) xymax.x = xy.x;
                    if (xy.y > xymax.y) xymax.y = xy.y;
                    index = Tool.GetIndexFromXY(xy[0], xy[1], size[0], size[1]);
                    
                }
            }
        }

    }

    public struct BoolAndVector2Int
    {
        public bool istrue;
        public Vector2Int xy;
        public BoolAndVector2Int(bool istrue ,Vector2Int xy)
        {
            this.istrue = istrue;
            this.xy = xy;
        }
    }
}
