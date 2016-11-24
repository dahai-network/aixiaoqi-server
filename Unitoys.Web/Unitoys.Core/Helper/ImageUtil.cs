using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing;
using System.IO;

namespace Unitoys.Core
{
    public static class ImageUtil
    {
       /// <summary>
       /// 生成缩略图
       /// </summary>
       /// <param name="originalImagePath">源图片地址</param>
       /// <param name="thumNailPath">缩略图图片地址</param>
       /// <param name="width">宽度</param>
       /// <param name="height">高度</param>
       /// <param name="model">缩放处理方式 HW W H Cut</param>
        public static void MakeThumNail(string originalImagePath, string thumNailPath, int width, int height, string model)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
            int thumWidth = width;        //缩略图的宽度    
            int thumHeight = height;      //缩略图的高度     
            int x = 0;
            int y = 0;
            int originalWidth = originalImage.Width;    //原始图片的宽度  
            int originalHeight = originalImage.Height;  //原始图片的高度       
            switch (model)
            {
                case "HW":      //指定高宽缩放,可能变形         
                    break;
                case "W":       //指定宽度,高度按照比例缩放    
                    if (originalWidth >= 650)
                        thumHeight = originalImage.Height * width / originalImage.Width;
                    else
                    {
                        thumWidth = originalWidth;
                        thumHeight = originalHeight;
                    }
                    break;
                case "H":       //指定高度,宽度按照等比例缩放         
                    thumWidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut":
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)thumWidth / (double)thumHeight)
                    {
                        originalHeight = originalImage.Height;
                        originalWidth = originalImage.Height * thumWidth / thumHeight;
                        y = 0;
                        x = (originalImage.Width - originalWidth) / 2;
                    }
                    else
                    {
                        originalWidth = originalImage.Width;
                        originalHeight = originalWidth * height / thumWidth;
                        x = 0;
                        y = (originalImage.Height - originalHeight) / 2;
                    }

                    break;
                default:
                    break;
            }
            //新建一个bmp图片 
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(thumWidth, thumHeight);        //新建一个画板  
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(bitmap);        //设置高质量查值法               
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;        //设置高质量，低速度呈现平滑程度      
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;        //清空画布并以透明背景色填充    
            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            graphic.DrawImage(originalImage, 
                new Rectangle(0, 0, thumWidth, thumHeight), 
                new Rectangle(x, y, originalWidth, originalHeight),
                GraphicsUnit.Pixel); 
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            using (var encoderParameters = new EncoderParameters(1))
            {
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                try
                {
                    bitmap.Save(thumNailPath, jgpEncoder, encoderParameters);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    graphic.Dispose();
                }
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
       
        #region 图片压缩(降低质量)Compress
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitmap">传入的Bitmap对象</param>
        /// <param name="destStream">压缩后的Stream对象</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitmap, Stream destStream, long level)
        {
            ImageCodecInfo myImageCodecInfo;
            Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            // Create an Encoder object based on the GUID

            // for the Quality parameter category.
            myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one

            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with 给定的 quality level
            myEncoderParameter = new EncoderParameter(myEncoder, level);
            myEncoderParameters.Param[0] = myEncoderParameter;
            srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcBitMap">传入的Bitmap对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Bitmap srcBitMap, string destFile, long level)
        {
            Stream s = new FileStream(destFile, FileMode.Create);
            Compress(srcBitMap, s, level);
            s.Close();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">传入的Stream对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(Stream srcStream, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcStream);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">传入的Image对象</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(System.Drawing.Image srcImg, string destFile, long level)
        {
            Bitmap bm = new Bitmap(srcImg);
            Compress(bm, destFile, level);
            bm.Dispose();
        }
        /// <summary>
        /// 图片压缩(降低质量以减小文件的大小)
        /// </summary>
        /// <param name="srcFile">待压缩的BMP文件名</param>
        /// <param name="destFile">压缩后的图片保存路径</param>
        /// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        public static void Compress(string srcFile, string destFile, long level)
        {
            // Create a Bitmap object based on a BMP file.
            Bitmap bm = new Bitmap(srcFile);
            Compress(bm, destFile, level);
            bm.Dispose();
        }

        #endregion 图片压缩(降低质量)

        /// <summary>
        /// 传入头像url相对路径返回外网绝对路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUserHeadCompleteUrl(this string url)
        {
            return UTConfig.SiteConfig.ImageHandleHost + url + "@!head-100";
        }
        /// <summary>
        /// 传入套餐图片Url相对路径返回外网绝对路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetPackageCompleteUrl(this string url)
        {
            return UTConfig.SiteConfig.ImageHandleHost + url;
        }
        /// <summary>
        /// 传入图片Url相对路径返回外网绝对路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCompleteUrl(this string url)
        {
            return UTConfig.SiteConfig.ImageHandleHost + url;
        }

        
        /// <summary>
        /// 传入国家表的图片Url相对路径返回外网绝对路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCountryPicCompleteUrl(this string url)
        {
            return UTConfig.SiteConfig.ImageHandleHost + url + "?x-oss-process=image/resize,m_fill,w_360,h_172,limit_0/auto-orient,0/format,jpg/interlace,1";
        }
    }
}
