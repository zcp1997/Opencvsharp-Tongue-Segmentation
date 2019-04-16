## 数字化舌图分割系统

#### OpenCvSharp在.net端的使用
>opencvsharp版本号:4.0.0

添加opencvsharp引用仅需添加:
```
using OpenCvSharp;
using OpenCvSharp.Extensions;
```

***分割算法如下***
* Grabcut
* Watershed
* MeanShift
* FloodFill
* 轮廓分割

***评价效果指标***
* 黑色像素点占比
* SSIM图像相似度
* PSNR峰值信噪比

>这是鄙人的毕设，是基于交互式的自动化舌象分割算法，从肉眼来看GrabCut算法效果最为出色。

