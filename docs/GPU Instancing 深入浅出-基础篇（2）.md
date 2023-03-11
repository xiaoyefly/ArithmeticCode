# ****GPU Instancing 深入浅出-基础篇（2）****

## **一、什么是Draw Call**

### **1、官方手册解读**

老样子先看相应[Optimizing draw calls - Unity 手册](https://link.zhihu.com/?target=https%3A//docs.unity.cn/cn/current/Manual/optimizing-draw-calls.html)

![https://pic1.zhimg.com/80/v2-c63f1d4a1bc3f386bcb7e710975d4f1c_1440w.webp](https://pic1.zhimg.com/80/v2-c63f1d4a1bc3f386bcb7e710975d4f1c_1440w.webp)

优化Draw calls

要在屏幕上绘制几何图形，unity会调用图形API进行处理。一个Draw call会告诉图形API需要绘制什么以及使用什么方式进行绘制。每个Draw call包含了图形API所需要的纹理，阴影以及缓冲区的绘制信息。大量的Draw call会消耗大量的资源，但Draw call的准备通阶段要比Draw call本身消耗更多的资源。

## **二、一次DrawCall在做什么**

以上是为自己的翻译，我们可以通过以下的Gif来感受一下GPU与CPU的通信，简单的来说Draw call其实是CPU与GPU的通信方式，他们通过CommandBuffer作为通信的“信道”，其实每一次GPU与CPU的通信并没有我们想得的那么简单叫一个Draw Call其中的过程有很多步骤，所以Unity3D又叫作Batch（批次）。

如下图所示：

![https://pic3.zhimg.com/v2-9382428960e7d1c30b9babc5e64257c2_b.jpg](https://pic3.zhimg.com/v2-9382428960e7d1c30b9babc5e64257c2_b.jpg)

CPU与GPU的一次Draw Call

GPU渲染速度远远高于CPU提交命令的速度，如果一帧中间DrawCall数量太多，CPU就会在设置渲染状态-提交drawcall上花费大量时间，造成性能问题，这里的性能问题其实是GPU在等待CPU的处理。

![https://pic2.zhimg.com/80/v2-c53d4e0dae648e87ba38df2d6e049dbd_1440w.webp](https://pic2.zhimg.com/80/v2-c53d4e0dae648e87ba38df2d6e049dbd_1440w.webp)

### **1、举个例子（测试用例1）**

我们可以通过Unity3D的一个案例来说明

想要用例下载见百度网盘：

链接: [https://pan.baidu.com/s/1qoiiHGRbe_skt6Nercub8Q?pwd=9hf0](https://link.zhihu.com/?target=https%3A//pan.baidu.com/s/1qoiiHGRbe_skt6Nercub8Q%3Fpwd%3D9hf0) 提取码: 9hf0

创建Unity工程，在场景中创建对象，并使用如下代码

**`using** UnityEngine;
**public** **class** **CreateCube** : MonoBehaviour
{
[SerializeField]
**private** GameObject _instanceGo;*//需要实例化对象*
[SerializeField]
**private** **int** _instanceCount;*//需要实例化个数*
[SerializeField]
**private** **bool** _bRandPos = **false**;*//是否随机的显示对象*
*// Start is called before the first frame update*
**void** Start()
{
**for** (**int** i = 0; i < _instanceCount; i++)
{
Vector3 pos = **new** Vector3(i * 1.5f, 0, 0);
GameObject pGO = GameObject.Instantiate<GameObject>(_instanceGo);
pGO.transform.SetParent(gameObject.transform);
**if**(_bRandPos)
{
pGO.transform.localPosition = Random.insideUnitSphere * 10.0f;
}
**else**{
pGO.transform.localPosition = pos;
}          
}
}
}`

创建一个NormalCubeCreate的空节点挂上以上代码

并创建一个Cube把Cube制作成prefab后拖放到NormalCubeCreate中的Instance Go属性上中

![https://pic3.zhimg.com/80/v2-4797e1f69678f6c711091e7634550b12_1440w.webp](https://pic3.zhimg.com/80/v2-4797e1f69678f6c711091e7634550b12_1440w.webp)

运行后的效果

![https://pic1.zhimg.com/80/v2-f468cb33852e977364c6b10e9898a204_1440w.webp](https://pic1.zhimg.com/80/v2-f468cb33852e977364c6b10e9898a204_1440w.webp)

感谢使用腾讯文档，您粘贴的区域不支持图片插入。

### **2、用例分析**

通过Statistics我们可以看到Batches为12这里的Batches为Draw Call的次数

![https://pic1.zhimg.com/80/v2-c2a58065e6a7c0800a71abbc42bb6aec_1440w.webp](https://pic1.zhimg.com/80/v2-c2a58065e6a7c0800a71abbc42bb6aec_1440w.webp)

打开Frame Debug: Frame Debug可以显示每一帧渲染时CPU与GPU的一些绘制信息

![https://pic2.zhimg.com/80/v2-af8d49f06fcd53be2e8dc24804165d19_1440w.webp](https://pic2.zhimg.com/80/v2-af8d49f06fcd53be2e8dc24804165d19_1440w.webp)

相应 的Frame Debug下的数据显示

![https://pic1.zhimg.com/80/v2-5e6aa8b0212b9d019c9514deb8b11794_1440w.webp](https://pic1.zhimg.com/80/v2-5e6aa8b0212b9d019c9514deb8b11794_1440w.webp)

通过以上的测试我们在RenderForward.RenderLoopJob中看到10个Draw Mesh NormalCube(Clone)的提交，相当于在同一帧中CPU与GPU提交了10次的绘制（Draw call），每画一个Cube就有一次DrawCall的调用。

那有没有更优的解决方案呢，如一次就把这十个对象都绘制上，当然有的，但是是有前提的那就是我们说的材质和网格需要相同（但不完全如此总归是有此约束）。

## **三、更优的Draw Call处理**

### **1、优化方案**

![https://pic3.zhimg.com/v2-4f3264fcb24b258424f37adf9c5979ee_b.jpg](https://pic3.zhimg.com/v2-4f3264fcb24b258424f37adf9c5979ee_b.jpg)

通过以上的GIF我们可以这么理解，我们可以把需要绘制的相同内容同时放到CommandBuffer中再通知GPU进行绘制，这样可以有效的优化每绘制一个对象就调用一个转态转换让GPU进行显示效率要高。

![https://pic1.zhimg.com/80/v2-ac750113acbfd5e8fe82850f59a48af4_1440w.webp](https://pic1.zhimg.com/80/v2-ac750113acbfd5e8fe82850f59a48af4_1440w.webp)

### **2、举个例子**

我们可以通过Unity3D的一个案例来说明

创建Unity工程，在场景中创建对象，并使用测试用例1的代码

重新创建一个Prefab命名InstanceCube,在所使用的Material中选择Enable GPU Instancing

![https://pic2.zhimg.com/80/v2-268034b127890f6f91708dbe33f49609_1440w.webp](https://pic2.zhimg.com/80/v2-268034b127890f6f91708dbe33f49609_1440w.webp)

创建一个GameObject重名称为InstanceCubeCreate挂上CreateCube组件，把InstanceCube

![https://pic3.zhimg.com/80/v2-738281c8a48998ab0c28c09f693fb566_1440w.webp](https://pic3.zhimg.com/80/v2-738281c8a48998ab0c28c09f693fb566_1440w.webp)

运行效果如下

![https://pic2.zhimg.com/80/v2-e08ac71468aa52b3c1d45ff115b967ad_1440w.webp](https://pic2.zhimg.com/80/v2-e08ac71468aa52b3c1d45ff115b967ad_1440w.webp)

### **2、用例分析**

从Statistics中我们可以看到Batches变成了3，而RenderForward.RenderLoop.Job为1，我们仅仅只是在Cube的材质中打开了Enable GPU Instancing就达到了我们想要的效果（Batch从12变成了3）。其中Frame Debug中显示的Draw Mesh(Instanced) 和官方文档中提到的

![https://pic2.zhimg.com/80/v2-bed6024a78c6b8bd0d5b2c355ba608c1_1440w.webp](https://pic2.zhimg.com/80/v2-bed6024a78c6b8bd0d5b2c355ba608c1_1440w.webp)

表现完全一至看来是GPU Instancing起到了正向的作用。

## **四、总结**

经过以上的使用我们初小掌握了GPU Instancing的使用，在Material中只要把开“Enale GPU Instancing”就有如上的运行效果，原来GPU Instancing如此的简单。似乎到此咱们就结束了相应的课程。

![https://pic2.zhimg.com/80/v2-92c3adc4c1311bec66d99add409ea6d5_1440w.webp](https://pic2.zhimg.com/80/v2-92c3adc4c1311bec66d99add409ea6d5_1440w.webp)

咱只是入了个门

咱只是入了个门，路漫漫兮......

在下面的章节中我会给大家介绍，GPU Instancing中的一些限制，这些限制我们要如何绕过去？当然方法很多。