# ****GPU Instancing 深入浅出-基础篇（3）****

## **前言**

前几个章节大家对Unity3d中的GPU-Instancing的官方手册，原理，以及如何打开GPU Instnacing有了一个整体的了解，当前我们还是有不少的疑问？

1、GPU Instancing对渲染批次的影响会带来什么效果呢？

2、GPU Instancing一个批次是否可以显示无数的对象而没有限制呢？

3、为什么我们需要把一些CPU的处理放到GPU中去处理呢？

本节为基础篇的最后一篇，把以上问题搞明白就可以进到“中级篇”的学习。

跟着文章由浅入深，相信只要看完整个系列的文章就能轻松搞定在大规模的程序开发中使用GPU Instancing的问题，本文主要来源一个小众《元宇宙》项目。

## **GPU Instanceing 相关章节传送门**

[梅川依福：GPU Instancing 深入浅出-基础篇（1）](https://zhuanlan.zhihu.com/p/523702434)

[梅川依福：GPU Instancing 深入浅出-基础篇（2）](https://zhuanlan.zhihu.com/p/523765931)

[梅川依福：GPU Instancing 深入浅出-基础篇（3）](https://zhuanlan.zhihu.com/p/523924945)

[梅川依福：GPU Instancing 深入浅出-中级篇（1）](https://zhuanlan.zhihu.com/p/524195324)

[梅川依福：GPU Instancing 深入浅出-中级篇（2）](https://zhuanlan.zhihu.com/p/524285662)

## **一、使用GPU与CPU的性能比对**

在项目工程中我们可以通过生成10000个对象进行runtime的性能比对

![https://pic4.zhimg.com/80/v2-850ad02a0a6bfec4379904477f9ed9eb_1440w.webp](https://pic4.zhimg.com/80/v2-850ad02a0a6bfec4379904477f9ed9eb_1440w.webp)

![https://pic4.zhimg.com/80/v2-608204fda81181f38c9a9b832420ff13_1440w.webp](https://pic4.zhimg.com/80/v2-608204fda81181f38c9a9b832420ff13_1440w.webp)

10000个没有打开GPU Instancing的帧率显示

![https://pic2.zhimg.com/80/v2-749b0fe125081aaac2e8d7110c0a25a5_1440w.webp](https://pic2.zhimg.com/80/v2-749b0fe125081aaac2e8d7110c0a25a5_1440w.webp)

10000个打开GPU Instancing的帧率显示

从性能比对上来看CPU的帧率在打开GPU Instancing相差至少在一倍，由此可见使用GPU Instacing的性能有较强的优势，从Batches来看Batches相差了近500倍。从中也可以看到GPU Instancing每个批次的Instance数量是有上限的，10000 /22 约有500个对象可以合成一个批次。这是为什么呢？

## **二、单批次最多可支持511个Instance的显示**

### **1、FrameDebug中的数据**

在10000个对象显示的时候被分成了20个批次Draw Mesh(instanced)进行显示，且显示Instances 511个，为什么会是511的对象呢？

![https://pic1.zhimg.com/80/v2-1d64fb70753d9c0c819012c87656f37c_1440w.webp](https://pic1.zhimg.com/80/v2-1d64fb70753d9c0c819012c87656f37c_1440w.webp)

### **2、查阅资料**

在Unity5.5的手册中对以上问题进行的回复 ，但是在后面的版本手册中并没有提及

![https://pic3.zhimg.com/80/v2-ad0b3e65dc97082842429af3492ec22e_1440w.webp](https://pic3.zhimg.com/80/v2-ad0b3e65dc97082842429af3492ec22e_1440w.webp)

意思：D3D的常量缓存区(constant buffers)最大值为64KB，对于OpenGL 通常只有16KB。如果你试图定义过多的属性每个instance属性就将达到这个上限值。Shaders可能会发生编译错误或更多问题， Shader编译器可能会崩溃。所以我们需要去平衡每个instance的属性和渲染batch。（不知道为什么只是Unity5.5有这样的提示，大约早期GPU Instance技术还不成熟），但这个constant buffers的限制到现在也没有变化，差别在于不会崩溃会自动分批渲染。

### **3、对Shader中的宏定义分析**

PS:这里的内容看不明白没有关系，在中级篇中我们还会细化分析。

以下是在Shader中翻译出的宏，可以看到在Shader中CBUFFER_START与CBUFFER_END中定义了int unity_BaseInstanceID 以及 UNITY_INSTANCING_BUFFER_START中有两个4X4的float, unity_ObjectToWorldArray和unity_WorldToObjectArray

![https://pic3.zhimg.com/80/v2-5e9248e36d15b7a66b3ce52b7051a822_1440w.webp](https://pic3.zhimg.com/80/v2-5e9248e36d15b7a66b3ce52b7051a822_1440w.webp)

以上两个矩阵，每个矩阵使用4*(float的size)*4(四行)*4（四列） = 64byte 两个矩阵为128byte

constant Buffer 为64KB = 65536 byte

65536btte/128byte = 512个对象

但一个Unity_BaseInstanceID为4个byte, 所以512个对象少一个刚好511

以上是为什么显示511个对象的问题，其实在OpenGL上constant Buff只是16K， 这样推理少了4倍那手机应该为127个对象。

## **三、让GPU动起来**

为什么要让GPU动起来呢？GPU和CPU到底有什么本质上的差异？

### **1、GPU vs CPU**

首先我们来看到二者在架构上的差异

![https://pic3.zhimg.com/80/v2-8c4ea7e128ea9abfe67681d2bb0fd65a_1440w.webp](https://pic3.zhimg.com/80/v2-8c4ea7e128ea9abfe67681d2bb0fd65a_1440w.webp)

CPU与GPU架构

ALU:是专门执行算术和逻辑运算的数字电路

可以通过图看到CPU与GPU的最大差别就是CPU的ALU远远小于GPU的ALU，也就是为什么我们会在区域链中使用GPU进行挖矿的原因。

### **2、属性比对**

以下是两者的一些性能比对

![https://pic2.zhimg.com/80/v2-cde6305320d34d12ac99b13ff882186d_1440w.webp](https://pic2.zhimg.com/80/v2-cde6305320d34d12ac99b13ff882186d_1440w.webp)

总的来说，如果我们的电脑里面有两个计算单元可以为我们服务，为什么不让这两个计算单元都运行起来，更好的支持应用或是游戏的开发呢？

## **四、总结**

本节主要比对了使用GPU Instancing与没有使用GPU Instancing的性能比对，因为我使用的是mac，所以在性能上只有两倍的差距，事实在Android或是windows上GPU Instancing会有更优异的表现。同时在本章中我们也说明了GPU与CPU的架构的差异性，同时发挥两个计算单元的优势更能让应用或是游戏运行的更顺畅。

但我们也要知道GPU Instancing的一些限制性问题，单个批次显示511个对象，通过计算我们知道显示对象的个数的多少主要还是和Instance对象在Shader中所使用的参数有关，这块需要上层业务进一步权衡。