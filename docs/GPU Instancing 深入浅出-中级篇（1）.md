# ****GPU Instancing 深入浅出-中级篇（1）****

在初期篇中我们对GPU Instancing的使用有了一个基础的了解，知道了其简单的原理，以及简单的应用，甚至是一些在使用过程中的约束。

进入中级篇，之前是写给小白们看的，当前一些老司机可以通过中级篇开始上路了。

在中级篇中我们将会看

1、创建带有Enable GPU Instancing的Shader

2、使用MaterialPropertyBlock让GPU Instancing变的更有趣

3、使用Graphic.DrawMeshInstanced进行无GameObject的Instance对象创建

相关测试工程传送门如下：

链接:[https://pan.baidu.com/s/1qoiiHGRbe_skt6Nercub8Q?pwd=9hf0](https://link.zhihu.com/?target=https%3A//pan.baidu.com/s/1qoiiHGRbe_skt6Nercub8Q%3Fpwd%3D9hf0)提取码: 9hf0

跟着文章由浅入深，相信只要看完整个系列的文章就能轻松搞定在大规模的程序开发中使用GPU Instancing的问题，本文主要来源一个小众《元宇宙》项目。

## **GPU Instanceing 相关章节传送门**

[梅川依福：GPU Instancing 深入浅出-基础篇（1）](https://zhuanlan.zhihu.com/p/523702434)

[梅川依福：GPU Instancing 深入浅出-基础篇（2）](https://zhuanlan.zhihu.com/p/523765931)

[梅川依福：GPU Instancing 深入浅出-基础篇（3）](https://zhuanlan.zhihu.com/p/523924945)

[梅川依福：GPU Instancing 深入浅出-中级篇（1）](https://zhuanlan.zhihu.com/p/524195324)

[梅川依福：GPU Instancing 深入浅出-中级篇（2）](https://zhuanlan.zhihu.com/p/524285662)

## **一、目的**

### **1、Unity默认支持**

其实在Unity的默认渲染管线中的Standard Shader中默认都支持了Enable GPU Instancing

![https://pic4.zhimg.com/80/v2-a69d7282bacb111f6d545a5bd1cc96e3_1440w.webp](https://pic4.zhimg.com/80/v2-a69d7282bacb111f6d545a5bd1cc96e3_1440w.webp)

那为什么我们需要还需要自己在Shader中重新支持Enable GPU Instancing呢？其实在SRP中或是URP中或是自己定义的Shader中其实默认都不支持Enable GPU Instancing，所以在写Shader的时候我们都会自己加入GPU Instacing的支持。对于GPU Instancing的支持Unity提供了一系列的宏，让用户自定义的Shader可以轻松支持GPU Instancing。

### **2、Unity手册描述**

[Creating shaders that support GPU instancing - Unity 手册](https://link.zhihu.com/?target=https%3A//docs.unity.cn/cn/current/Manual/gpu-instancing-shader.html)

![https://pic2.zhimg.com/80/v2-d48e9a3dd39563b44db3b10c4a9a5c79_1440w.webp](https://pic2.zhimg.com/80/v2-d48e9a3dd39563b44db3b10c4a9a5c79_1440w.webp)

翻译

创建支持GPU Instancing的Shader

本节内容包括了如何给用户自定义的Shader增加支持GPU Instancing的功能。本文首先介绍了自定义Unity着色器支持GPU实例化所需的Shader关键字、变量和函数。然后，本文内容还包括如何向曲面着色器和顶点/片段着色器添加逐实例数据的示例。

所以建议感兴趣的也可以先看看官方手册。

## **一、创建Unlit的自定义Shader材质球**

下载测试工程的话可以在3_MyInstanceShader中进行学习

![https://pic3.zhimg.com/80/v2-f1776ade228478e82a63ad904cf33e06_1440w.webp](https://pic3.zhimg.com/80/v2-f1776ade228478e82a63ad904cf33e06_1440w.webp)

### **1、创建Unlit Shader**

![https://pic4.zhimg.com/80/v2-05cabb44c6ffd6d16f955537f642ca63_1440w.webp](https://pic4.zhimg.com/80/v2-05cabb44c6ffd6d16f955537f642ca63_1440w.webp)

然后左键NewUnlitShader

![https://pic2.zhimg.com/80/v2-7755ed0da2e4b9b2e73e395d74e2ace9_1440w.webp](https://pic2.zhimg.com/80/v2-7755ed0da2e4b9b2e73e395d74e2ace9_1440w.webp)

### **2、创建材质Unlit材质球**

如下创建使用NewUnlitShader右键创建Material

![https://pic3.zhimg.com/80/v2-dab01faa92345aa182b8091b3e52c252_1440w.webp](https://pic3.zhimg.com/80/v2-dab01faa92345aa182b8091b3e52c252_1440w.webp)

创建出的Material

![https://pic4.zhimg.com/80/v2-d14221c6e033271ec04fb5c379c8ccff_1440w.webp](https://pic4.zhimg.com/80/v2-d14221c6e033271ec04fb5c379c8ccff_1440w.webp)

没有相应的Enable GPU Instancing的选项。

那要如何才能快速支持Enable GPU Instancing呢？

## **二、让材质球支持GPU Instancing**

### **1、给我们的Shader命个名**

需要给自定义的Shader定义一个自己的名字，双击Shader后对开始Shader的编写

![https://pic4.zhimg.com/80/v2-c844de75711adf62b89a6763e10dd1e7_1440w.webp](https://pic4.zhimg.com/80/v2-c844de75711adf62b89a6763e10dd1e7_1440w.webp)

`Shader "Unlit/MyGPUInstance"`

### **2、第一步：增加变体让Shader支持instance**

增加变体使用Shader可以支持Instance

![https://pic4.zhimg.com/80/v2-4a021dafea2d50fec8c9139ca2401f1f_1440w.webp](https://pic4.zhimg.com/80/v2-4a021dafea2d50fec8c9139ca2401f1f_1440w.webp)

*`//第一步： sharder 增加变体使用shader可以支持instance*  
**#pragma multi_compile_instancing**`

以上代码将使Unity生成着色器的两个变体，一个具有GPU实例化支持，一个不具有GPU实例化支持。

到我们的材质球上看看有什么变化

![https://pic4.zhimg.com/80/v2-3b9ea370e7d36ca2012999e908bdf2b7_1440w.webp](https://pic4.zhimg.com/80/v2-3b9ea370e7d36ca2012999e908bdf2b7_1440w.webp)

是不是很神奇

官方手册说明

![https://pic1.zhimg.com/80/v2-dce90cd75d615e350c011cc48a94e0cc_1440w.webp](https://pic1.zhimg.com/80/v2-dce90cd75d615e350c011cc48a94e0cc_1440w.webp)

翻译：生成instance变体。这对于片段和顶点着色器是必需增加的。对于曲面着色器，它是可选的。

### **3、第二步-添加顶点着色器输入宏**

instancID 加入顶点着色器输入结构

![https://pic1.zhimg.com/80/v2-d8115d33ee6285b2c5b65f5916308db4_1440w.webp](https://pic1.zhimg.com/80/v2-d8115d33ee6285b2c5b65f5916308db4_1440w.webp)

`//第二步：instancID 加入顶点着色器输入结构
UNITY_VERTEX_INPUT_INSTANCE_ID`

宏翻译后如下其实就是增加了一个SV_InstanceID语义的instanceID变量

#define UNITY_VERTEX_INPUT_INSTANCE_ID unit instanceID : SV_InstanceID

instanceID主要作用是使用GPU实例化时，用作顶点属性的索引。

官方手册说明

![https://pic4.zhimg.com/80/v2-f62d906e31d302ad8f73d8e7ef8608ab_1440w.webp](https://pic4.zhimg.com/80/v2-f62d906e31d302ad8f73d8e7ef8608ab_1440w.webp)

翻译：在顶点着色器输入/输出结构体中定义instance ID。要使用此宏，请启用IINSTANCING_ON关键字。否则，Unity不会设置instance ID。要访问instance ID，请使用#ifdef INSTANCING_ON中的 vertexInput.instanceID 。如果不使用此块，变体将无法编译。

### **4、第三步-添加顶点着色器输出宏**

instancID加入顶点着色器输出结构

![https://pic1.zhimg.com/80/v2-f063453d7dd6788bafbe44279724f0f8_1440w.webp](https://pic1.zhimg.com/80/v2-f063453d7dd6788bafbe44279724f0f8_1440w.webp)

如第二步一样目的是增加一个SV_InstanceID语义的nstanceID变量，用作顶点属性的索引。

`//第三步：instancID 加入顶点着色器输出结构
UNITY_VERTEX_INPUT_INSTANCE_ID`

### **5、第四步-得到instanceid顶点的相关设置**

![https://pic1.zhimg.com/80/v2-de3eac98a561ff23a158367bc978f988_1440w.webp](https://pic1.zhimg.com/80/v2-de3eac98a561ff23a158367bc978f988_1440w.webp)

`//第四步：instanceid在顶点的相关设置  
UNITY_SETUP_INSTANCE_ID(v);`

#define UNITY_SETUP_INSTANCE_ID(input) \

unity_InstanceID = input.instanceID + unity_BaseInstanceID;

官方文档

![https://pic1.zhimg.com/80/v2-cc50211ad9cf5c464ba93de84d5fe6d8_1440w.webp](https://pic1.zhimg.com/80/v2-cc50211ad9cf5c464ba93de84d5fe6d8_1440w.webp)

翻译：允许着色器函数访问实例ID。对于顶点着色器，开始时需要此宏。对于片段着色器，此添加是可选的。有关示例，请参见顶点和片段着色器。

### **6、第五步-传递instanceID顶点到片元角色器**

![https://pic3.zhimg.com/80/v2-cce93160c60c535b51525526fa425b0a_1440w.webp](https://pic3.zhimg.com/80/v2-cce93160c60c535b51525526fa425b0a_1440w.webp)

`//第五步：传递 instanceid 顶点到片元
UNITY_TRANSFER_INSTANCE_ID(v, o);`

官方手册

![https://pic4.zhimg.com/80/v2-b78eb31e2e2bd1a4ea63f89cc87d32fb_1440w.webp](https://pic4.zhimg.com/80/v2-b78eb31e2e2bd1a4ea63f89cc87d32fb_1440w.webp)

翻译：在顶点着色器中将InstanceID从输入结构复制到输出结构。如果需要访问片段着色器中的每个实例数据，请使用此宏。

### **7、第六步 instanceID在片元的相关设置**

![https://pic2.zhimg.com/80/v2-ba77d546a9acf0c7a38833218568fca1_1440w.webp](https://pic2.zhimg.com/80/v2-ba77d546a9acf0c7a38833218568fca1_1440w.webp)

`//第六步：instanceid在片元的相关设置
UNITY_SETUP_INSTANCE_ID(i);`

### **8、代码全展示**

经过以上六个步骤后我们写成了自己带GPU Instancing的Shader(MyGPUInstance)

`Shader "Unlit/MyGPUInstance"
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

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //第六步：instanceid在片元的相关设置
                UNITY_SETUP_INSTANCE_ID(i);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}`

## **三、一个带有GPU instancing的测试**

### **1、创建Prefab并使用相应材质**

命名：Prefba为MyGPUInstanceCube

![https://pic4.zhimg.com/80/v2-b6a177adfa43e52e23fc02fff5bb9c77_1440w.webp](https://pic4.zhimg.com/80/v2-b6a177adfa43e52e23fc02fff5bb9c77_1440w.webp)

按以上截图创建相应的Prefab

### **2、挂上之前的CreateCube脚本**

并在Instance GO中使用MyGPUInstanceCube的Prefba

![https://pic3.zhimg.com/80/v2-d22e0706bdb271ced022adaf124582f6_1440w.webp](https://pic3.zhimg.com/80/v2-d22e0706bdb271ced022adaf124582f6_1440w.webp)

CreatCube代码传送门：[梅川依福：GPU Instancing 深入浅出-基础篇（2）](https://zhuanlan.zhihu.com/p/523765931)

### **3、来测试一下**

![https://pic3.zhimg.com/80/v2-d9cbcc981e0f2ca611b28e489c6fcaa2_1440w.webp](https://pic3.zhimg.com/80/v2-d9cbcc981e0f2ca611b28e489c6fcaa2_1440w.webp)

达成目标，批次为4个批次，优化了510个Batcing.

## **四、总结**

本节我们使用Unity的Ulit的Shader创建了我们自定义的MyGPUInstance.shader，并通过六步依次添加了宏完成了自己定义的Shadr支持GPU instancing的制作。然后通过创建一个材质并在Cube中使用了本材质，通过createCube批量创建了512个Cube只使用了2个批次渲染了512个Cube对象。

但是我们测试用例真的比较丑，那我们是不是需要让这些白盒看起来好看一些呢？下一节我们会让我们的白盒子变的更有趣一些，不然我自己都看不下去了。