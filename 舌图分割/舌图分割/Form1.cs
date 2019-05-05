using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Windows.Forms;

namespace 舌图分割
{
    public partial class Form1 : Form
    {
        public static int ITER_COUNT = 5; //grabcut迭代次数
        public static GrabCutModes GRABCUTMODE = GrabCutModes.InitWithRect;  //grabcut的mode
        public static int MEADIANBlUR_KSIZE = 15;//分水岭
        public static Size ELEMENT_SIZE = new Size(20, 20);//分水岭
        public static int MEANSHIFT_SP = 120; //均值漂移
        public static int MEANSHIFT_SR = 50;//均值漂移
        public static Scalar LODIFF = new Scalar(5, 5, 5, 0); //Floodfill
        public static Scalar UPDIFF = new Scalar(5, 5, 5, 0);//Floodfill
        public static ThresholdTypes CONTOUR_TYPE = ThresholdTypes.Otsu;//通过轮廓
        public static int SIGMAX = 2; //通过轮廓
        public int saveCount = 1;

        public Form1()
        {
            DoubleBuffered = true;
            InitializeComponent();
        }

        //载入窗体
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox2.MouseWheel += new MouseEventHandler(pictureBox2_MouseWheel);
            skinEngine1.SkinFile = Environment.CurrentDirectory + "\\RealOne.ssk";
            value1.Hide();
            value2.Hide();
            textBox5.Hide();
            textBox6.Hide();
            panel1.Hide();
            button4.Hide();
            comboBox4.Hide();
        }

        //图像分割操作
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            value1.Hide();
            value2.Hide();
            textBox5.Hide();
            textBox6.Hide();
            button4.Hide();
            comboBox4.Hide();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            if (comboBox1.Text == "")
            {
                MessageBox.Show("请先选择您所需要的算法。");
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "图片文件|*.bmp;*.ico;*.jpeg;*.jpg;*.png;*.tif;*.tiff";
                openFileDialog.ShowDialog();
                string fileName1 = openFileDialog.FileName;
                if (fileName1 != "")
                {
                    button4.Show();
                    pictureBox1.ImageLocation = fileName1;
                    Mat image = Cv2.ImRead(fileName1);
                    Mat grayImage = new Mat();
                    Mat binImage = new Mat();
                    //自适应阈值分割函数
                    if (comboBox1.Text == "adaptiveThreshold")
                    {
                        if (comboBox2.Text == "THRESH_BINARY")
                        {
                            if (comboBox3.Text == "ADAPTIVE_THRESH_MEAN_C")
                            {
                                double startTime = Cv2.GetTickCount();
                                AdaptiveThreshold(image, grayImage, binImage, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary);//自适应阈值分割函数
                                double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                                showImage(binImage, duration);
                            }
                            if (comboBox3.Text == "ADAPTIVE_THRESH_GAUSSIAN_C")
                            {
                                double startTime = Cv2.GetTickCount();
                                AdaptiveThreshold(image, grayImage, binImage, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary);//自适应阈值分割函数
                                double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                                showImage(binImage, duration);
                            }
                        }
                        if (comboBox2.Text == "THRESH_BINARY_INV")
                        {
                            if (comboBox3.Text == "ADAPTIVE_THRESH_MEAN_C")
                            {
                                double startTime = Cv2.GetTickCount();
                                AdaptiveThreshold(image, grayImage, binImage, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv);//自适应阈值分割函数
                                double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                                showImage(binImage, duration);
                            }
                            if (comboBox3.Text == "ADAPTIVE_THRESH_GAUSSIAN_C")
                            {
                                double startTime = Cv2.GetTickCount();
                                AdaptiveThreshold(image, grayImage, binImage, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv);//自适应阈值分割函数                                                                                                    //算法结束                                                                                                               
                                double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                                showImage(binImage, duration);
                            }
                        }
                        else if (comboBox2.Text == "" || comboBox3.Text == "")
                        {
                            button4.Hide();
                            MessageBox.Show("请选择参数一和参数二再进行分割");
                        }
                        panel1.Hide();
                        value1.Hide();
                        value2.Hide();
                        textBox5.Hide();
                        textBox6.Hide();
                        button4.Hide();
                    }
                    //grabcut函数迭代五次
                    else if (comboBox1.Text == "grabcut")
                    {
                        comboBox2.Text = "";
                        comboBox3.Text = "";
                        try
                        {
                            double startTime = Cv2.GetTickCount();
                            Mat res = grabCut(image, ITER_COUNT, GRABCUTMODE, 20, 30, image.Cols - (20 << 1), image.Rows - 30);
                            double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                            showImage(res, duration);
                            showParameters("迭代次数：", ITER_COUNT.ToString(), "分割算子：", GRABCUTMODE.ToString());
                            textBox6.Hide();
                            comboBox4.Show();
                            comboBox4.Items.Clear();
                            comboBox4.Text = GRABCUTMODE.ToString();
                            comboBox4.Items.Add("InitWithRect");
                            comboBox4.Items.Add("InitWithMask");
                            comboBox4.Items.Add("Eval");
                        }
                        catch (Exception err)
                        {
                            if (err.ToString().Contains("totalSampleCount"))
                            {
                                MessageBox.Show("您选择的区域无法分割出前景，无法分割，请重新选择区域大小");
                            }
                            else
                            {
                                MessageBox.Show(err.ToString());
                            }
                        }
                    }
                    //分水岭
                    else if (comboBox1.Text == "watershed")
                    {
                        comboBox2.Text = "";
                        comboBox3.Text = "";
                        try
                        {
                            double startTime = Cv2.GetTickCount();
                            Mat result = waterShed(image, MEADIANBlUR_KSIZE, ELEMENT_SIZE);
                            double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                            showImage(result, duration);
                            showParameters("中值滤波内核：", MEADIANBlUR_KSIZE.ToString(), "形态学卷积核：", "(" + ELEMENT_SIZE.Width.ToString() + "," + ELEMENT_SIZE.Height.ToString() + ")");
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.ToString());
                        }
                    }
                    //均值漂移
                    else if (comboBox1.Text == "meanshift")
                    {
                        comboBox2.Text = "";
                        comboBox3.Text = "";
                        try
                        {
                            double startTime = Cv2.GetTickCount();
                            Mat res = meanShift(image, MEANSHIFT_SP, MEANSHIFT_SR);
                            double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                            showImage(res, duration);
                            showParameters("颜色域半径：", MEANSHIFT_SP.ToString(), "空间域半径：", MEANSHIFT_SR.ToString());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.ToString());
                        }
                    }
                    //漫水填充分割 
                    else if (comboBox1.Text == "floodfill")
                    {
                        comboBox2.Text = "";
                        comboBox3.Text = "";
                        try
                        {
                            double startTime = Cv2.GetTickCount();
                            Mat res = floodFill(image, LODIFF, UPDIFF, 20, 30, image.Cols - (20 << 1), image.Rows - 30);
                            double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                            showImage(res, duration);
                            showParameters("像素最大下行差异值：", LODIFF.ToString(), "像素最大上行差异值：", UPDIFF.ToString());
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.ToString());
                        }
                    }
                    //通过轮廓分割图像
                    else if (comboBox1.Text == "contour")
                    {
                        comboBox2.Text = "";
                        comboBox3.Text = "";
                        try
                        {
                            double startTime = Cv2.GetTickCount();
                            Mat res = contourSeg(image, CONTOUR_TYPE, SIGMAX);
                            double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                            showImage(res, duration);
                            showParameters("高斯核x方向标准差:", SIGMAX.ToString(), "二值化算子:", CONTOUR_TYPE.ToString());
                            textBox6.Hide();
                            comboBox4.Show();
                            comboBox4.Items.Clear();
                            comboBox4.Text = CONTOUR_TYPE.ToString();
                            comboBox4.Items.Add("Binary");
                            comboBox4.Items.Add("BinaryInv");
                            comboBox4.Items.Add("Trunc");
                            comboBox4.Items.Add("Tozero");
                            comboBox4.Items.Add("TozeroInv");
                            comboBox4.Items.Add("Mask");
                            comboBox4.Items.Add("Otsu");
                            comboBox4.Items.Add("Triangle");
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.ToString());
                        }
                    }
                    //异常处理
                    else
                    {
                        MessageBox.Show("算法选择有误，请重新选择算法和正确的参数");
                    }
                }
            }
        }

        //自适应阈值分割函数封装
        private Mat AdaptiveThreshold(Mat image, Mat grayImage, Mat binImage, AdaptiveThresholdTypes adaptiveThresholdTypes, ThresholdTypes thresholdTypes)
        {
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);//色彩空间转换
            Cv2.AdaptiveThreshold(grayImage, binImage, 255, adaptiveThresholdTypes, thresholdTypes, 7, 1);
            return binImage;
        }

        //Grabcut分割函数封装
        private Mat grabCut(Mat image, int ITER_COUNT, GrabCutModes GRABCUTMODE, int x, int y, int w, int h)
        {
            var bgModel = new Mat();
            var fgdModel = new Mat();
            var mask = new Mat();
            const int GC_PR_FGD = 3;
            Rect rect = new Rect();
            rect.X = x;
            rect.Y = y;
            rect.Width = w;
            rect.Height = h;
            Cv2.GrabCut(image, mask, rect, bgModel, fgdModel, ITER_COUNT, GRABCUTMODE);
            Cv2.Compare(mask, GC_PR_FGD, mask, CmpTypes.EQ);
            Mat foreground = new Mat(image.Size(), MatType.CV_8UC3, new Scalar(0, 0, 0));
            image.CopyTo(foreground, mask);
            showRect(rect.X, rect.Y, rect.Width, rect.Height);
            showImagePara(image);
            return foreground;
        }

        //分水岭分割函数封装
        private Mat waterShed(Mat src, int MEADIANBlUR_KSIZE, Size ELEMENT_SIZE)
        {
            var imageGray = new Mat();
            var thresh = new Mat();
            var fg = new Mat();
            var bgt = new Mat();
            var bg = new Mat();
            var marker = new Mat();
            var marker32 = new Mat();
            var m = new Mat();
            var res = new Mat();
            var threshOpen = new Mat();
            var threshClose = new Mat();
            Cv2.CvtColor(src, imageGray, ColorConversionCodes.BGR2GRAY);
            Cv2.EqualizeHist(imageGray, imageGray);//直方图均衡化
            Cv2.MedianBlur(imageGray, imageGray, MEADIANBlUR_KSIZE);//中值滤波
            Cv2.Threshold(imageGray, thresh, 0, 255, ThresholdTypes.Otsu);
            Cv2.Erode(thresh, fg, 0, null, 2);
            Cv2.Dilate(thresh, bgt, 0, null, 3);
            Cv2.Threshold(bgt, bg, 1, 128, ThresholdTypes.BinaryInv);
            marker = fg + bg;
            marker.ConvertTo(marker32, MatType.CV_32SC1);
            Cv2.Watershed(src, marker32);
            Cv2.ConvertScaleAbs(marker32, m);
            Cv2.Threshold(m, thresh, 0, 255, ThresholdTypes.Otsu);
            var element = Cv2.GetStructuringElement(MorphShapes.Rect, ELEMENT_SIZE);//获取自定义核
            Cv2.MorphologyEx(thresh, threshOpen, MorphTypes.Open, element);//开运算
            Cv2.MorphologyEx(threshOpen, threshClose, MorphTypes.Close, element);//闭运算
            Cv2.BitwiseAnd(src, src, res, threshClose);
            return res;
        }

        //均值漂移函数封装
        private Mat meanShift(Mat image, int MEANSHIFT_SP, int MEANSHIFT_SR)
        {
            Mat res = new Mat();
            Cv2.PyrMeanShiftFiltering(image, res, MEANSHIFT_SP, MEANSHIFT_SR, 2);
            Mat hsv = new Mat();
            Mat[] hsvChannels;
            Cv2.CvtColor(res, hsv, ColorConversionCodes.BGR2HSV);
            Cv2.Split(hsv, out hsvChannels);
            Mat imgH = hsvChannels[0];
            Mat imgS = hsvChannels[1];
            Mat imgV = hsvChannels[2];
            //对H和V分量进行二值化
            Mat imgHh = new Mat();
            Mat imgVv = new Mat();
            Cv2.Threshold(imgH, imgHh, 0, 255, ThresholdTypes.Otsu);
            Cv2.Threshold(imgV, imgVv, 0, 255, ThresholdTypes.Otsu);
            Mat imgHV = imgHh & imgVv;//合并
            //去噪点
            Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(10, 10));
            Cv2.MorphologyEx(imgHV, imgHV, MorphTypes.Open, element);
            //闭操作，连接一些连通域
            Cv2.MorphologyEx(imgHV, imgHV, MorphTypes.Close, element);
            Mat src = new Mat();
            //！！！重要！！！
            //8UC1 => 8UC3
            Cv2.CvtColor(imgHV, src, ColorConversionCodes.GRAY2RGB);
            Mat result = image & src;
            return result;
        }

        //FloodFill函数封装
        private Mat floodFill(Mat image, Scalar LODIFF, Scalar UPDIFF, int x, int y, int w, int h)
        {
            Rect rect = new Rect();
            rect.X = x;
            rect.Y = y;
            rect.Width = w;
            rect.Height = h;
            Cv2.FloodFill(image, new Point(10, 10), new Scalar(0, 0, 0), out rect, LODIFF, UPDIFF, 4);
            textBox7.Text = x.ToString();
            textBox8.Text = y.ToString();
            textBox9.Text = w.ToString();
            textBox10.Text = h.ToString();
            showImagePara(image);
            panel1.Show();
            return image;
        }

        //通过轮廓分割图像
        private Mat contourSeg(Mat src, ThresholdTypes CONTOUR_TYPE, int SIGMAX)
        {
            Mat imageGray = new Mat();
            Mat img = new Mat();
            Cv2.CvtColor(src, imageGray, ColorConversionCodes.RGB2GRAY);
            Cv2.GaussianBlur(imageGray, imageGray, new Size(5, 5), SIGMAX);
            Cv2.Threshold(imageGray, img, 0, 255, CONTOUR_TYPE);
            Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(10, 10));
            Cv2.MorphologyEx(img, img, MorphTypes.Open, element);
            Cv2.MorphologyEx(img, img, MorphTypes.Close, element);
            Mat[] contours;
            var hierarchy = new Mat();
            Cv2.FindContours(img, out contours, hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            //查找最大轮廓
            double maxarea = 0;
            int maxAreaIdx = 0;
            for (int index = contours.Length - 1; index >= 0; index--)
            {
                double tmparea = Math.Abs(Cv2.ContourArea(contours[index]));
                if (tmparea > maxarea)
                {
                    maxarea = tmparea;
                    maxAreaIdx = index; //记录最大轮廓的索引号
                }
            }
            Mat[] contourMax = new Mat[1];
            var maxContour = contours[maxAreaIdx];
            contourMax.SetValue(maxContour, 0);
            Mat mask = Mat.Zeros(src.Size(), MatType.CV_8UC1);
            //掩模上填充轮廓
            Cv2.DrawContours(mask, contourMax, -1, Scalar.All(255), -1);
            Mat real = src.Clone();
            var result = new Mat();
            Cv2.CvtColor(mask, mask, ColorConversionCodes.GRAY2RGB);
            Cv2.BitwiseAnd(real, mask, result);
            return result;
        }

        //操作：添加到算法评价表
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("尚未对算法进行完整评价。");
            }
            else
            {
                if (comboBox1.Text == "adaptiveThreshold")
                {
                    MessageBox.Show("该算法不参与评价。");
                }
                else if (comboBox1.Text == "grabcut")
                {
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = "GrabCut分割算法";
                    this.dataGridView1.Rows[index].Cells[1].Value = textBox3.Text;
                    this.dataGridView1.Rows[index].Cells[2].Value = imageLike(textBox2.Text);
                    this.dataGridView1.Rows[index].Cells[3].Value = imageEvaluation(textBox1.Text);
                }
                else if (comboBox1.Text == "watershed")
                {
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = "分水岭分割算法";
                    this.dataGridView1.Rows[index].Cells[1].Value = textBox3.Text;
                    this.dataGridView1.Rows[index].Cells[2].Value = imageLike(textBox2.Text);
                    this.dataGridView1.Rows[index].Cells[3].Value = imageEvaluation(textBox1.Text);
                }
                else if (comboBox1.Text == "floodfill")
                {
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = "漫水填充分割算法";
                    this.dataGridView1.Rows[index].Cells[1].Value = textBox3.Text;
                    this.dataGridView1.Rows[index].Cells[2].Value = imageLike(textBox2.Text);
                    this.dataGridView1.Rows[index].Cells[3].Value = imageEvaluation(textBox1.Text);
                }
                else if (comboBox1.Text == "meanshift")
                {
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = "均值漂移分割算法";
                    this.dataGridView1.Rows[index].Cells[1].Value = textBox3.Text;
                    this.dataGridView1.Rows[index].Cells[2].Value = imageLike(textBox2.Text);
                    this.dataGridView1.Rows[index].Cells[3].Value = imageEvaluation(textBox1.Text);
                }
                else if (comboBox1.Text == "contour")
                {
                    int index = this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[index].Cells[0].Value = "轮廓分割算法";
                    this.dataGridView1.Rows[index].Cells[1].Value = textBox3.Text;
                    this.dataGridView1.Rows[index].Cells[2].Value = imageLike(textBox2.Text);
                    this.dataGridView1.Rows[index].Cells[3].Value = imageEvaluation(textBox1.Text);
                }
                else if (comboBox1.Text == "")
                {
                    MessageBox.Show("未知算法");
                }
            }
        }

        //计算原图像非黑像素的个数 
        private double countNotBlackPixel(Mat image)
        {
            int count = 0;
            int rows = image.Rows;
            int cols = image.Cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Vec3b value = image.At<Vec3b>(i, j);
                    if (value == new Vec3b(0, 0, 0))
                    {
                        count++;
                    }
                }
            }
            return rows * cols - count;
        }

        //通过得分评价性能
        private string imageEvaluation(string text)
        {
            text = textBox1.Text.ToString();
            double ev = Convert.ToDouble(text);
            if (ev >= 80 && ev <= 100)
            {
                return "效果较佳";
            }
            if (ev >= 50 && ev < 80)
            {
                return "效果一般";
            }
            else
            {
                return "效果较差";
            }
        }

        //计算两张图片的相似度SSIM指标
        private double countSsim(Mat i1, Mat i2)
        {
            const double C1 = 6.5025, C2 = 58.5225;
            int d = MatType.CV_32F;
            var I1 = new Mat();
            var I2 = new Mat();
            i1.ConvertTo(I1, d);
            i2.ConvertTo(I2, d);
            Mat I1_2 = I1.Mul(I1);
            Mat I2_2 = I2.Mul(I2);
            Mat I1_I2 = I1.Mul(I2);
            var mu1 = new Mat();
            var mu2 = new Mat();
            Cv2.GaussianBlur(I1, mu1, new Size(11, 11), 1.5);
            Cv2.GaussianBlur(I2, mu2, new Size(11, 11), 1.5);
            Mat mu1_2 = mu1.Mul(mu1);
            Mat mu2_2 = mu2.Mul(mu2);
            Mat mu1_mu2 = mu1.Mul(mu2);
            var sigam1_2 = new Mat();
            var sigam2_2 = new Mat();
            var sigam12 = new Mat();
            Cv2.GaussianBlur(I1_2, sigam1_2, new Size(11, 11), 1.5);
            sigam1_2 -= mu1_2;
            Cv2.GaussianBlur(I2_2, sigam2_2, new Size(11, 11), 1.5);
            sigam2_2 -= mu2_2;
            Cv2.GaussianBlur(I1_I2, sigam12, new Size(11, 11), 1.5);
            sigam12 -= mu1_mu2;
            Mat t1, t2, t3;
            t1 = 2 * mu1_mu2 + C1;
            t2 = 2 * sigam12 + C2;
            t3 = t1.Mul(t2);
            t1 = mu1_2 + mu2_2 + C1;
            t2 = sigam1_2 + sigam2_2 + C2;
            t1 = t1.Mul(t2);
            Mat ssim_map = new Mat();
            Cv2.Divide(t3, t1, ssim_map);
            Scalar mssim = Cv2.Mean(ssim_map);
            double ssim = (mssim.Val0 + mssim.Val1 + mssim.Val2) / 3;
            return ssim;
        }

        //通过SSIM评价性能
        private string imageLike(string text)
        {
            text = textBox2.Text.ToString();
            double ev = Convert.ToDouble(text);
            if (ev >= 0.42 && ev <= 1)
            {
                return "相似度较高";
            }
            if (ev >= 0.2 && ev < 0.42)
            {
                return "相似度较低";
            }
            else
            {
                return "相似度极低";
            }
        }

        //计算两张图片的峰值信噪比PSNR
        private double countPsnr(Mat I1, Mat I2)
        {
            Mat s1 = new Mat();
            Cv2.Absdiff(I1, I2, s1);
            s1.ConvertTo(s1, MatType.CV_32F);//转换为32位的float类型，8位不能计算平方
            s1 = s1.Mul(s1);
            Scalar s = Cv2.Sum(s1);  //计算每个通道的和
            double sse = s.Val0 + s.Val1 + s.Val2;
            double mse = sse / (I1.Channels() * I1.Total()); //  sse/(w*h*3)
            double psnr = 10.0 * Math.Log10((255 * 255) / mse);
            return psnr;
        }

        //导入理想分割图片并进行算法评价
        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null)
            {
                MessageBox.Show("理想图片背景应为黑色，大小应与待评价分割图片一致。");
                OpenFileDialog openFileDialog2 = new OpenFileDialog();
                openFileDialog2.Filter = "图片文件|*.bmp;*.ico;*.jpeg;*.jpg;*.png;*.tif;*.tiff";
                openFileDialog2.ShowDialog();
                string fileName = openFileDialog2.FileName;
                if (fileName == null)
                {
                    pictureBox1.ImageLocation = null;
                    pictureBox2.ImageLocation = null;
                }
                else if (fileName != "")
                {
                    if (comboBox1.Text == "adaptiveThreshold")
                    {
                        MessageBox.Show("该算法不参与算法评价");
                    }
                    else
                    {
                        Mat imageIdeal = Cv2.ImRead(fileName);

                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox2.Image;
                        Mat imageOrigin = BitmapConverter.ToMat(bitmap);
                        if (imageOrigin.Size() != imageIdeal.Size() || imageOrigin == null)
                        {
                            MessageBox.Show("分割后的图片和理想分割图像大小不一致，请重新选择。");
                        }
                        else
                        {
                            Cv2.ImShow("理想图片", imageIdeal);
                            double countOrigin = countNotBlackPixel(imageOrigin);
                            double countIdeal = countNotBlackPixel(imageIdeal);
                            if (countOrigin > countIdeal)
                            {
                                double score = (countIdeal / countOrigin) * 100;
                                textBox1.Text = score.ToString();
                            }
                            else
                            {
                                double score = (countOrigin / countIdeal) * 100;
                                textBox1.Text = score.ToString();
                            }
                            double ssim = countSsim(imageOrigin, imageIdeal);
                            textBox2.Text = ssim.ToString();
                            double psnr = countPsnr(imageOrigin, imageIdeal);
                            textBox4.Text = psnr.ToString();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("尚未进行图像分割，不能导入理想图像。");
            }
        }

        //显示图片并显示运行时间
        private void showImage(Mat image, double duration)
        {
            Cv2.ImShow("舌图分割结果", image);
            System.Drawing.Bitmap bitmap = BitmapConverter.ToBitmap(image); //转 Scalar为C#中Bitmap
            pictureBox2.Image = bitmap;
            textBox3.Text = duration.ToString();
        }

        //显示算法的重要参数
        private void showParameters(string valueText1, string boxText5, string valueText2, string boxText6)
        {
            value1.Text = valueText1;
            value2.Text = valueText2;
            textBox5.Text = boxText5;
            textBox6.Text = boxText6;
            value1.Show();
            textBox5.Show();
            value2.Show();
            textBox6.Show();
        }

        //grabcut show rect
        private void showRect(int x, int y, int w, int h)
        {
            textBox7.Text = x.ToString();
            textBox8.Text = y.ToString();
            textBox9.Text = w.ToString();
            textBox10.Text = h.ToString();
            panel1.Show();
        }

        private void showImagePara(Mat image)
        {
            textBox12.Text = image.Width.ToString();
            textBox11.Text = image.Height.ToString();
        }

        //保存分割后的图片到本地
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("图片为空,无法保存");
            }
            else
            {
                System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox2.Image;
                Mat mat = BitmapConverter.ToMat(bitmap);
                if (MessageBox.Show("是否保存图片？", "保存分割舌图", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (comboBox1.Text == null)
                    {
                        MessageBox.Show("未找到对应的分割算法,无法保存");
                    }
                    else
                    {
                        string count = saveCount++.ToString();
                        string text = comboBox1.Text;
                        Cv2.ImWrite("D:\\" + text + count + ".png", mat);
                        MessageBox.Show("保存成功！图片默认保存为D盘路径下的" + text + count + ".png");
                    }
                }
            }
        }

        //鼠标滚轮事件
        private void pictureBox2_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) //放大图片
            {
                pictureBox2.Size = new System.Drawing.Size(pictureBox2.Width + 20, pictureBox2.Height + 20);
            }
            else
            {  //缩小图片
                pictureBox2.Size = new System.Drawing.Size(pictureBox2.Width - 20, pictureBox2.Height - 20);
            }
        }

        //更改参数再次分割图片
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "grabcut" && value1.Text == "迭代次数：" && value2.Text == "分割算子：")
            {
                try
                {
                    int iter_count = int.Parse(textBox5.Text);
                    GrabCutModes grabcutmode = (GrabCutModes)(Enum.Parse(typeof(GrabCutModes), comboBox4.Text));
                    int x = int.Parse(textBox7.Text);
                    int y = int.Parse(textBox8.Text);
                    int w = int.Parse(textBox9.Text);
                    int h = int.Parse(textBox10.Text);
                    if (iter_count > 20 || iter_count < 0)
                    {
                        MessageBox.Show("迭代次数于0-20次之内较为合适");
                    }
                    else
                    {
                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox1.Image;
                        Mat image = BitmapConverter.ToMat(bitmap);
                        double startTime = Cv2.GetTickCount();
                        Mat res = grabCut(image, iter_count, grabcutmode, x, y, w, h);
                        double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                        showImage(res, duration);
                        showParameters("迭代次数：", iter_count.ToString(), "分割算子：", grabcutmode.ToString());
                        textBox6.Hide();
                        showRect(x, y, w, h);
                    }
                }
                catch (Exception err)
                {
                    if (err.ToString().Contains("totalSampleCount"))
                    {
                        MessageBox.Show("您选择的区域无法分割出前景，无法分割，请重新选择区域大小");
                    }
                    else if (err.ToString().Contains("mask"))
                    {
                        MessageBox.Show("掩模mask为空，无法分割");
                    }
                    else if (err.ToString().Contains("Format"))
                    {
                        MessageBox.Show("输入的字符串格式不正确，请重新输入");
                    }
                }
            }
            if (comboBox1.Text == "watershed" && value1.Text == "中值滤波内核：" && value2.Text == "形态学卷积核：")
            {
                try
                {
                    if (int.Parse(textBox5.Text) % 2 == 0)
                    {
                        MessageBox.Show("中值滤波内核应为奇数，请重新输入。");
                    }
                    else
                    {
                        int meadianblur_ksize = int.Parse(textBox5.Text);
                        if (meadianblur_ksize > 1000 || meadianblur_ksize < 0 )
                        {
                            MessageBox.Show("数值不合理，请重新输入适当的数值");
                        }
                        else
                        {
                            string text = textBox6.Text;
                            string diff11 = text.Replace("(", "");
                            string diff12 = diff11.Replace(")", "");
                            string[] str = diff12.Split(',');
                            Size element_size = new Size(int.Parse(str[0]), int.Parse(str[1]));
                            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox1.Image;
                            Mat image = BitmapConverter.ToMat(bitmap);
                            double startTime = Cv2.GetTickCount();
                            Mat result = waterShed(image, meadianblur_ksize, element_size);
                            double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                            showImage(result, duration);
                            showParameters("中值滤波内核：", meadianblur_ksize.ToString(), "形态学卷积核：", "(" + element_size.Width.ToString() + "," + element_size.Height.ToString() + ")");
                        }          
                    }
                }
                catch (Exception err)
                {
                    if (err.ToString().Contains("Format"))
                    {
                        MessageBox.Show("输入的字符串格式不正确，请重新输入");
                    }
                    else if (err.ToString().Contains("索引超出了数组界限"))
                    {
                        MessageBox.Show("形态学卷积内核应用英文括号隔开");
                    }
                    else
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
            }
            if (comboBox1.Text == "meanshift" && value1.Text == "颜色域半径：" && value2.Text == "空间域半径：")
            {
                try
                {
                    int meanshift_sp = int.Parse(textBox5.Text);
                    int meanshift_sr = int.Parse(textBox6.Text);
                    if (meanshift_sp < 0 || meanshift_sr < 0 || meanshift_sp>1000 || meanshift_sr>1000)
                    {
                        MessageBox.Show("参数有误或数值过大，请重新输入数据");
                    }
                    else
                    {
                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox1.Image;
                        Mat image = BitmapConverter.ToMat(bitmap);
                        double startTime = Cv2.GetTickCount();
                        Mat res = meanShift(image, meanshift_sp, meanshift_sr);
                        double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                        showImage(res, duration);
                        showParameters("颜色域半径：", meanshift_sp.ToString(), "空间域半径：", meanshift_sr.ToString());
                    }
                }
                catch (Exception err)
                {
                    if (err.ToString().Contains("Format"))
                    {
                        MessageBox.Show("输入的字符串格式不正确，请重新输入");
                    }
                    else
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
            }
            if (comboBox1.Text == "floodfill" && value1.Text == "像素最大下行差异值：" && value2.Text == "像素最大上行差异值：")
            {
                try
                {
                    string diff1 = textBox5.Text;
                    string diff11 = diff1.Replace("[", "");
                    string diff12 = diff11.Replace("]", "");
                    string[] str1 = diff12.Split(',');
                    Scalar lodiff = new Scalar(int.Parse(str1[0]), int.Parse(str1[1]), int.Parse(str1[2]), int.Parse(str1[3]));
                    string diff2 = textBox6.Text;
                    string diff21 = diff2.Replace("[", string.Empty);
                    string diff22 = diff21.Replace("]", string.Empty);
                    string[] str2 = diff22.Split(',');
                    int x = int.Parse(textBox7.Text);
                    int y = int.Parse(textBox8.Text);
                    int w = int.Parse(textBox9.Text);
                    int h = int.Parse(textBox10.Text);
                    Scalar updiff = new Scalar(int.Parse(str2[0]), int.Parse(str2[1]), int.Parse(str2[2]), int.Parse(str2[3]));
                    System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox1.Image;
                    Mat image = BitmapConverter.ToMat(bitmap);
                    double startTime = Cv2.GetTickCount();
                    Mat res = floodFill(image, lodiff, updiff, x, y, w, h);
                    double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                    showImage(res, duration);
                    showRect(x, y, w, h);
                    showParameters("像素最大下行差异值：", lodiff.ToString(), "像素最大上行差异值：", updiff.ToString());
                }
                catch (Exception err)
                {
                    if (err.ToString().Contains("Format"))
                    {
                        MessageBox.Show("输入的字符串格式不正确，请重新输入(应输入英文逗号)");
                    }
                    else if (err.ToString().Contains("索引超出了数组界限"))
                    {
                        MessageBox.Show("像素最大下(上)行差异值应用英文括号隔开");
                    }
                    else
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
            }
            if (comboBox1.Text == "contour" && value1.Text == "高斯核x方向标准差:" && value2.Text == "二值化算子:")
            {
                try
                {
                    ThresholdTypes contour_type = (ThresholdTypes)(Enum.Parse(typeof(ThresholdTypes), comboBox4.Text));
                    int sigmax = int.Parse(textBox5.Text);
                    System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)pictureBox1.Image;
                    Mat image = BitmapConverter.ToMat(bitmap);
                    double startTime = Cv2.GetTickCount();
                    Mat res = contourSeg(image, contour_type, sigmax);
                    double duration = (Cv2.GetTickCount() - startTime) / (Cv2.GetTickFrequency());
                    showImage(res, duration);
                    showParameters("高斯核x方向标准差:", sigmax.ToString(), "二值化算子:", contour_type.ToString());
                    textBox6.Hide();
                }
                catch (Exception err)
                {
                    if (err.ToString().Contains("Format"))
                    {
                        MessageBox.Show("输入的字符串格式不正确，请重新输入");
                    }
                    else if (err.ToString().Contains("索引超出了数组界限"))
                    {
                        MessageBox.Show("索引超出了数组界限，无法分割");
                    }
                    else
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
            }
        }
    }
}
