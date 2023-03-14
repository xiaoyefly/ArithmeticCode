# ****GPU Instancing 深入浅出-中级篇（2）****

## **前言**

在使用GPU Instancing技术有一个约束，必须使用相同材质和相同Mesh的对象才能使用GPU Instancing，那都显示一样的对象就显的很无趣，有没有办法能让GPU Instancing中每个Instance有不同的表现呢？那当然有，这一节我会带大家对不同Instance的个性化属性进行学习。

相关测试工程传送门如下：

链接:[https://pan.baidu.com/s/1qoiiHGRbe_skt6Nercub8Q?pwd=9hf0](https://link.zhihu.com/?target=https%3A//pan.baidu.com/s/1qoiiHGRbe_skt6Nercub8Q%3Fpwd%3D9hf0)提取码: 9hf0

## **GPU Instanceing 相关章节传送门**

[梅川依福：GPU Instancing 深入浅出-基础篇（1）](https://zhuanlan.zhihu.com/p/523702434)

[梅川依福：GPU Instancing 深入浅出-基础篇（2）](https://zhuanlan.zhihu.com/p/523765931)

[梅川依福：GPU Instancing 深入浅出-基础篇（3）](https://zhuanlan.zhihu.com/p/523924945)

[梅川依福：GPU Instancing 深入浅出-中级篇（1）](https://zhuanlan.zhihu.com/p/524195324)

[梅川依福：GPU Instancing 深入浅出-中级篇（2）](https://zhuanlan.zhihu.com/p/524285662)

## **一、明确目的**

上一节我们的通过GPU Instancing技术通过4（其中渲染GPU对象的只有2个批次）个批次渲染了512个对象，现在我们要让这些对象变的更有趣起来

![https://pic2.zhimg.com/80/v2-5dfbfc73e3dcdeed1bfb5b59c1b75d45_1440w.webp](https://pic2.zhimg.com/80/v2-5dfbfc73e3dcdeed1bfb5b59c1b75d45_1440w.webp)

当前状态

![https://pic3.zhimg.com/v2-ae0e9ecd431b54b944492dcd9f2b274e_b.jpg](https://pic3.zhimg.com/v2-ae0e9ecd431b54b944492dcd9f2b274e_b.jpg)

目标效果

从两者比对来看，我们需要让每个Cube Instance有自己独立的颜色，同时这些Instance 还能有自己的运动方式

## **二、所要使用的技术**

### **1、什么是MaterialPropertyBlock**

![https://pic3.zhimg.com/80/v2-5a5a7da16784e33fa88c502254ea2842_1440w.webp](https://pic3.zhimg.com/80/v2-5a5a7da16784e33fa88c502254ea2842_1440w.webp)

其实就是可以给每个实例对象通过Render.SetPropertyBlock设置相应的MaterialPropertyBlock

### **2、官方测试代码**

![https://pic3.zhimg.com/80/v2-8503cc647a7d3a6bd61600ebcb7980aa_1440w.webp](https://pic3.zhimg.com/80/v2-8503cc647a7d3a6bd61600ebcb7980aa_1440w.webp)

**`using** UnityEngine;

**public** **class** **MaterialPropertyBlockExample** : MonoBehaviour
{
**public** GameObject[] objects;

    **void** Start()
    {
        *//创建MaterialPropertyBlock*
        MaterialPropertyBlock props = **new** MaterialPropertyBlock();
        MeshRenderer renderer;

        **foreach** (GameObject obj **in** objects)
        {
            **float** r = Random.Range(0.0f, 1.0f);
            **float** g = Random.Range(0.0f, 1.0f);
            **float** b = Random.Range(0.0f, 1.0f);
            *//设置MaterialPropertyBlock所使用的颜色*
            props.SetColor("_Color", **new** Color(r, g, b));
            *//得到MeshRenderer*
            renderer = obj.GetComponent<MeshRenderer>();
            *//设置PropertyBlock*
            renderer.SetPropertyBlock(props);
        }
    }
}`

通过以上测试官方用例我们知道了如何给Render设置颜色

## **三、让我们的Instance变的有意思**

### **1、增加相应C#脚本**

相应CubeCreate传送门：[梅川依福：GPU Instancing 深入浅出-基础篇（2）](https://zhuanlan.zhihu.com/p/523765931)

`using UnityEngine;

public class FunnyGPUInstance : MonoBehaviour
{
[SerializeField]
private GameObject _instanceGo;//初实例化对你
[SerializeField]
private int _instanceCount;//实例化个数
[SerializeField]
private bool _bRandPos = false;

    private MaterialPropertyBlock _mpb = null;//与buffer交换数据
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _instanceCount; i++)
        {
            Vector3 pos = new Vector3(i * 1.5f, 0, 0);
            GameObject pGO = GameObject.Instantiate<GameObject>(_instanceGo);
            pGO.transform.SetParent(gameObject.transform);
            if (_bRandPos)
            {
                pGO.transform.localPosition = Random.insideUnitSphere * 10.0f;
            }
            else
            {
                pGO.transform.localPosition = pos;
            }
            //个性化显示
            SetPropertyBlockByGameObject(pGO);

        }
    }

    //修改每个实例的PropertyBlock
    private bool SetPropertyBlockByGameObject(GameObject pGameObject)
    {
        if(pGameObject == null)
        {
            return false;
        }
        if(_mpb == null)
        {
            _mpb = new MaterialPropertyBlock();
        }

        //随机每个对象的颜色
        _mpb.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1.0f));
        _mpb.SetFloat("_Phi", Random.Range(-40f, 40f));

        MeshRenderer meshRenderer = pGameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            return false;         
        }

        meshRenderer.SetPropertyBlock(_mpb);

        return true;
    }
}`

把脚本挂到

![https://pic1.zhimg.com/80/v2-269db8126e4b64d9b9090b02b6652770_1440w.webp](https://pic1.zhimg.com/80/v2-269db8126e4b64d9b9090b02b6652770_1440w.webp)

在随机对象中我们给“_Color”与“_Phi”设置了两组随机数值

`_mpb.SetColor("_Color", new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1.0f));      
_mpb.SetFloat("_Phi", Random.Range(-40f, 40f));`

从代码中我们可以看到我们给相应的对象设置了MaterialPropertyBlock

`meshRenderer.SetPropertyBlock(_mpb);`

以上的_Color与_Phi是给Material中的Shader设置的参数

### **2、定义Shader的属性**

在上一节中的MyGPUInstance.Shader中增加代码如下

上一节传送门 [梅川依福：GPU Instancing 深入浅出-中级篇（1）](https://zhuanlan.zhihu.com/p/524195324)

定义Shader的属性代码，此代码和C#的代码中的_mpb.SetColor("_Color",X,X,X)，_mpb.SetFloat("_Phi", Random.Range(-40f, 40f));配对使用

`UNITY_INSTANCING_BUFFER_START(Props)
UNITY_DEFINE_INSTANCED_PROP(float4,_Color)
UNITY_DEFINE_INSTANCED_PROP(float, _Phi)
UNITY_INSTANCING_BUFFER_END(Props)`

以上代码定义了一个Props的常量缓冲区，并且定义了float4的_Color的属性与float的_Phi属性

官方手册的解释

![https://pic3.zhimg.com/80/v2-9a0197aed03b17b4a733a8f961ffb5aa_1440w.webp](https://pic3.zhimg.com/80/v2-9a0197aed03b17b4a733a8f961ffb5aa_1440w.webp)

翻译

| 宏的名称 | 描述 |
| --- | --- |
| UNITY_INSTANCING_BUFFER_Start(bufferName) | 在每个实例的开始处声明名为bufferName的常量缓冲区。将此宏与UNITY_NSTANCING_BUFFER_END一起使用，可以包装要对每个实例唯一的属性声明。使用UNITY_DEFINE_INSTANCED_PROP声明缓冲区内的属性。 |
| UNITY_INSTANCING_BUFFER_END(bufferName) | 在每个实例的结尾处声明名为bufferName的常量缓冲区。将此宏与UNITY_INSTANCING_BUFFER_START一起使用，可以包装要对每个实例唯一的属性声明。使用UNITY_DEFINE_INSTANCED_PROP声明缓冲区内的属性。 |
| UNITY)DEFINE_INSTANCED_PROP(type, propertyName) | 使用指定的类型和名称定义每个实例着色器属性。在以下示例中，_Color属性是唯一的。(可以上面的官方测试代码) |
| UNITY_ACCESS_INSTANCED_PROP |  |

### **3、使用属性**

C#的脚本代码通过Setcolor或是SetFloat传递到Shader中，那在Shader中是如何使用的呢

在顶点着色器中通过UNITY_ACCESS_INSTANCED_PROP(Props, _Phi);进行属性访问

![https://pic4.zhimg.com/80/v2-96a430ae84d7dfd230672a4f5fb3c373_1440w.webp](https://pic4.zhimg.com/80/v2-96a430ae84d7dfd230672a4f5fb3c373_1440w.webp)

翻译：在一个实例常量缓冲区中访问每个实例着色器属性。Unity使用Instance ID索引实例数据数组。bufferName必须与包含指定属性的常量缓冲区的名称匹配。此宏的编译方式对于Instance_ON和非Instance变体编译不同。

顶点着色器的代码如下，通过得到 _Phi来让对象有不同的偏移值

`v2f vert (appdata v)
{
v2f o;
//第四步：instanceid在顶点的相关设置  
UNITY_SETUP_INSTANCE_ID(v);
//第五步：传递 instanceid 顶点到片元
UNITY_TRANSFER_INSTANCE_ID(v, o);

float phi = UNITY_ACCESS_INSTANCED_PROP(Props, _Phi);
v.vertex = v.vertex + sin(_Time.y + phi);

o.vertex = UnityObjectToClipPos(v.vertex);
o.uv = TRANSFORM_TEX(v.uv, _MainTex);
UNITY_TRANSFER_FOG(o,o.vertex);
return o;
}`

片元着色器代码如下

得到C#的设置给Shader的UNITY_ACCESS_INSTANCED_PROP(Props, _Color);颜色，并让每个片元显示这个颜色值

`fixed4 frag (v2f i) : SV_Target
{
//第六步：instanceid在片元的相关设置
UNITY_SETUP_INSTANCE_ID(i);

//得到由CPU设置的颜色
float4 col= UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
return col;   
}`

与FunnyGPUInstance的代码

`Shader "Unlit/FunnyGPUInstance"
{
Properties
{
_MainTex ("Texture", 2D) = "white" {}
}
SubShader
{
Tags { "RenderType"="Opaque" }
LOD 100

        Pass
        {
            CGPROGRAM
            //第一步： sharder 增加变体使用shader可以支持instance 
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4,_Color)
	      	    UNITY_DEFINE_INSTANCED_PROP(float, _Phi)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                //第二步：instancID 加入顶点着色器输入结构 
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                //第三步：instancID加入顶点着色器输出结构
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                //第四步：instanceid在顶点的相关设置  
                UNITY_SETUP_INSTANCE_ID(v);
                //第五步：传递 instanceid 顶点到片元
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float phi = UNITY_ACCESS_INSTANCED_PROP(Props, _Phi);
                v.vertex = v.vertex + sin(_Time.y + phi);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

              
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //第六步：instanceid在片元的相关设置
                UNITY_SETUP_INSTANCE_ID(i);

                //得到由CPU设置的颜色
                float4 col= UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                return col;
            }
            ENDCG
        }
    }
}`

### **4、制作FunnyGPUInstanceCube的prefab**

![https://pic3.zhimg.com/80/v2-fe8de41bf603cc647e0a0582c0904c32_1440w.webp](https://pic3.zhimg.com/80/v2-fe8de41bf603cc647e0a0582c0904c32_1440w.webp)

把相应的Prefab拖放到FunnyGPUInstanceCube中

![https://pic4.zhimg.com/80/v2-8cf21e68d5b78fca233aae533116d6af_1440w.webp](https://pic4.zhimg.com/80/v2-8cf21e68d5b78fca233aae533116d6af_1440w.webp)

### **5、测试效果**

![https://pic4.zhimg.com/80/v2-8da3415178cc0be7390b20f8366a70bb_1440w.webp](https://pic4.zhimg.com/80/v2-8da3415178cc0be7390b20f8366a70bb_1440w.webp)

Batches为3，并且动起来了，目标达成

## **四、总结**

通过MatermialPropertyBlock我们通过C#代码给每个实例对象进行了属性设置，把一个死寂沉沉的GPUInstancing用例变的有趣起来。当然所有的这一切还是归功于Shader与C#的代码的配合。