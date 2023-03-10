# ****GPU Instancing 深入浅出-基础篇（1）****

## **一、GPU Instancing定义**

### **1、对官网手册的解读**

[https://docs.unity.cn/cn/current/Manual/GPUInstancing.html](https://link.zhihu.com/?target=https%3A//docs.unity.cn/cn/current/Manual/GPUInstancing.html)

![https://pic4.zhimg.com/80/v2-64245c5160684b76894281bf2e72873f_1440w.webp](https://pic4.zhimg.com/80/v2-64245c5160684b76894281bf2e72873f_1440w.webp)

GPU-Instancing在unity中的手册解释

翻译：GPU Instancing是一种Draw call的优化方案，使用一个Draw call就能渲染具有多个相同材质的网格对象。而这些网格的每个copy称为一个实例。此技术在一个场景中对于需要绘制多个相同对象来说是一个行之有效办法，例如树木或灌木丛的绘制。

GPU Instancing 在同一个Draw call中渲染完全相同的网格。可以通过添加变量来减少重复的外观，每个实例可以具有不同的属性，例如颜色或缩放。在Frame Debugger中如有Draw calls显示多个实例时会显示“Darw Mesh(Instanced)”。

![https://pic1.zhimg.com/80/v2-d2935ae274e78dc102990edeaff64dc0_1440w.webp](https://pic1.zhimg.com/80/v2-d2935ae274e78dc102990edeaff64dc0_1440w.webp)

可达到的效果

以上是GPU Instancing的官方说明，翻译时加入了自己的理解，不到位之处尽请谅解。

### **2、苍白文章的感知**

从官方手册的解读中，我们似乎可以这么理解：即只有使用相同网格和相同材质的物体渲染时，才可以使用GPU Instancing技术， 这样看来，我们使用 GPU Instancing 似乎只能渲染出来一堆网格和材质都一样的物体？那除了告诉用户 “看！我能渲染出这么多一样的东西，厉不厉害？！” 之外，似乎毫无乐趣可言。事实如此么？

那具体是什么效果还是用一些用例来看看吧。

## **二、GPU Instancing可达到的效果**

### **1、相同Mesh个性化显示**

![https://pic4.zhimg.com/80/v2-13864fd0e45f837a82954b761c4e5bf7_1440w.webp](https://pic4.zhimg.com/80/v2-13864fd0e45f837a82954b761c4e5bf7_1440w.webp)

一个批次，使用不同属性来控制cube的显示效果

### **2、多个Mesh Instancing效果**

![https://pic1.zhimg.com/80/v2-f4a2d6b6fafc23ec789f9b53ceffc658_1440w.webp](https://pic1.zhimg.com/80/v2-f4a2d6b6fafc23ec789f9b53ceffc658_1440w.webp)

插件制作的海洋鱼群效果

### **3、大型效果场景的显示**

![https://pic2.zhimg.com/v2-96bd22e15583a785540b4bd7b50bffc9_b.jpg](https://pic2.zhimg.com/v2-96bd22e15583a785540b4bd7b50bffc9_b.jpg)

GPU Instancer插件的效果

### **4、GPU Instancing的高级动作应用**

![https://pic2.zhimg.com/v2-9137389cf1e8d0734988708ed70979dd_b.jpg](https://pic2.zhimg.com/v2-9137389cf1e8d0734988708ed70979dd_b.jpg)

Mesh Animator 插件GPU的实例对象可以播放动作

### **5、GPU在植被中的应用**

![https://pic1.zhimg.com/v2-56e21fd18b79a0708653f09f1555314c_b.jpg](https://pic1.zhimg.com/v2-56e21fd18b79a0708653f09f1555314c_b.jpg)

GPU Instancer插件的效果植被处理

看完以上效果不知道有没有想试试手的感觉，GPU Instancing技术的应用其实很广，所以不要小看官网中那些苍白的文字描述，到AssetStore中看看。

说了这么多展示了这么多，不防继续来看官方手册关于GPU Instancing的一些约束，

## **三、需要注意的事**

### **1、在SRP Batcher时如何使用GPU Instancing技术**

其实不是不支持而是SRP有SRP Batcher，以下官网说明，官网偷偷的告诉我们如果非要在SRP下使用GPU instance的话那可以使用Graphics.DrawMeshInstanced，其实是直接draw a mesh on screen。

![https://pic3.zhimg.com/80/v2-b793446682e21bc2b8c43be51d12bc7e_1440w.webp](https://pic3.zhimg.com/80/v2-b793446682e21bc2b8c43be51d12bc7e_1440w.webp)

使用Graphics.DrawMeshInstanced

### **2、SkinnedMeshRenderers不支持GPU Instancing技术**

![https://pic2.zhimg.com/80/v2-8ebd7919e0be0095b417b76fcd83e969_1440w.webp](https://pic2.zhimg.com/80/v2-8ebd7919e0be0095b417b76fcd83e969_1440w.webp)

其实本质资源是GPU Instancing仅支持MeshRender，不直接支持SkinnedMeshRender的Instance，想想如果支持了那不是所有蒙皮动画都可以Instance，正常来说对于SkinnedMeshRender其实有办法支持，以下是使用GPU的顶点动画的方案进行的支持。

[https://github.com/Unity-Technologies/Animation-Instancing](https://link.zhihu.com/?target=https%3A//github.com/Unity-Technologies/Animation-Instancing)

当然支持Animation-Instancing是有一定牺牲。

### **3、Lighting对GPU Instance是支持的**

说了很多不可以，同时Unity抛砖引玉的说到了Lighting对GPU instanceing对象的不离不弃

![https://pic2.zhimg.com/80/v2-442cd22242704fdb5770447ebdb2fe11_1440w.webp](https://pic2.zhimg.com/80/v2-442cd22242704fdb5770447ebdb2fe11_1440w.webp)

从以上的信息可以看出GPU Instancing出的对象可以受光照的影响，看起来不错，只要设置一下就好了。

## **四、总结**

看完以上的介绍，我们对GPU instancing有一个大体的了解，从苍白的文档中看不出什么效果，后面的章节我们会由浅入深的对GPU Instancing从基础篇一直讲到高级的应用。