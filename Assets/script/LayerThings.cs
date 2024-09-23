using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DeepIland
{

    /// <summary>
    /// 控制物体一层层像素身体
    /// </summary>
    public class LayerThings
    {
        public string name;
        //[0]x,[1],y
        public int[] size;
        public List<Color[]> layersTex;
        public List<string[]> TexTag;
        public int layerCount;
        public Rect rect;
        public Vector2 positon;

        /// <summary>
        /// 输出图片
        /// </summary>
        /// <returns></returns>
        public Color[] MixTex()
        {
            Color[] texOut = new Color[size[0] * size[1]];
            for (int y = 0; y < size[1]; y++)
            {
                for (int x = 0; x < size[0]; x++)
                {
                    int index = Tool.GetIndexFromXY(x, y, size[0], size[1]);
                    texOut[index]=GetPix(index);
                }
            }
            return texOut;
        }

        public LayerThings() { }

        public LayerThings(List<Texture2D> layersTex,int []size,int layerCount)
        {
            for(int i=0;i< layersTex.Count; i++)
            {
                this.layersTex[i] = layersTex[i].GetPixels();
            }
            this.size = size;
            this.layerCount = layerCount;
        }

        public LayerThings(List<Color[]> layersTex, int[] size, int layerCount)
        {
                this.layersTex = layersTex;
            
            this.size = size;
            this.layerCount = layerCount;
        }
        public void LayersUpdate(int layerNumber)
        {

        }
        
        /// <summary>
        /// 获得像素，按照layer排序，输出最前面的像素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPix(int x,int y)
        {
            for(int i =0;i< layersTex.Count; i++)
            {
                int index= Tool.GetIndexFromXY(x, y, size[0], size[1]);
                if (layersTex[i][index] != Color.clear)
                {
                    return layersTex[i][index];
                }
            }
            return Color.clear;
        }
        public Color GetPix(int index)
        {
            for (int i = 0; i < layersTex.Count; i++)
            {
                if (layersTex[i][ index] != Color.clear)
                {
                    return layersTex[i][ index];
                }
            }
            return Color.clear;
        }
        public Color[] GetTex()
        {
            int len = size[0] * size[1];
            Color[] texOut = new Color[len];
            
            for (int i=0;i< len; i++)
            {
                texOut[i] = GetPix(i);
            }

            return texOut;
        }

        /// <summary>
        /// 实例到数据
        /// </summary>
        /// <param name="layerThingsIn"></param>
        /// <returns></returns>
        public static LayerSaveType LayerThingsToLayerSaveType(LayerThings layerThingsIn)
        {
            //Texture2D tex = new Texture2D(layerThingsIn.size[0], layerThingsIn.size[1]);
            List<string> texs = new List<string>();
            Texture2D image = new Texture2D(layerThingsIn.size[0], layerThingsIn.size[1]);
            for(int i = 0; i < layerThingsIn.layerCount - 1; i++)
            {
                image.SetPixels(layerThingsIn.layersTex[i]);
                texs.Add(Convert.ToBase64String(image.EncodeToPNG()));
            }

            return new LayerSaveType(texs, layerThingsIn.size, layerThingsIn.layerCount);
        }
        /// <summary>
        /// 数据到实例
        /// 输出texture版本初始化实例
        /// </summary>
        /// <param name="layerSaveTypeIn"></param>
        /// <returns></returns>
        public static LayerThings LayerSaveTypeToLayerThings2(LayerSaveType layerSaveTypeIn)
        {
            //Texture2D tex = new Texture2D(layerSaveTypeIn.size[0], layerSaveTypeIn.size[1]);
            List<Texture2D> texs = new List<Texture2D>();
            for (int i = 0; i < layerSaveTypeIn.layerCount - 1; i++)
            {
                texs.Add(new Texture2D(layerSaveTypeIn.size[0], layerSaveTypeIn.size[1]));
                texs[texs.Count - 1].LoadImage(Convert.FromBase64String(layerSaveTypeIn.layersTex[i]));
                //texs.Add(Convert.ToBase64String(tex.EncodeToPNG()));
            }
            return new LayerThings(texs, layerSaveTypeIn.size, layerSaveTypeIn.layerCount);
        }
        /// <summary>
        /// 数据到实例
        /// 少量内存转换
        /// </summary>
        /// <param name="layerSaveTypeIn"></param>
        /// <returns></returns>
        public static LayerThings LayerSaveTypeToLayerThings(LayerSaveType layerSaveTypeIn)
        {

            //Texture2D tex = new Texture2D(layerSaveTypeIn.size[0], layerSaveTypeIn.size[1]);
            Texture2D tex =new Texture2D(layerSaveTypeIn.size[0], layerSaveTypeIn.size[1]);
            List<Color[]> layerTexOut=new List<Color[]>();
            Debug.Log(33);

            for (int i = 0; i < layerSaveTypeIn.layerCount; i++)
            {
                Debug.Log(22);
                tex.LoadImage(Convert.FromBase64String(layerSaveTypeIn.layersTex[i]));
                layerTexOut.Add(tex.GetPixels());
                //texs.Add(Convert.ToBase64String(tex.EncodeToPNG()));
            }
            return new LayerThings(layerTexOut, layerSaveTypeIn.size, layerSaveTypeIn.layerCount);
        }


        /// <summary>
        /// 存储json用的类
        /// </summary>
        public class LayerSaveType
        {
            public string name;
            public List<string> layersTex;
            public List<string[]> pixTag;
            public int[] size;
            public int layerCount;
            public double[] rect;
            public double[] positon;
            public LayerSaveType()
            {

            }
            public  LayerSaveType(List< string> layersTex,int[] size,int layerCount)
            {
                this.layersTex = layersTex;
                this.size = size;
                this.layerCount = layerCount;
            }


        }
    }

}