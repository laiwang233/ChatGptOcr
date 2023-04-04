using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.LocalV3;

namespace Util
{
    public class Ocr
    {
        public string TextRecognition(Bitmap bitmap)
        {
            var model = LocalFullModels.ChineseV3;
            using var all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
            {
                AllowRotateDetection = true, /* 允许识别有角度的文字 */
                Enable180Classification = false, /* 允许识别旋转角度大于90度的文字 */
            };

            var memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Png);

            using var src = Cv2.ImDecode(memory.GetBuffer(), ImreadModes.Color);
            var result = all.Run(src);

            return result.Text;
        }
    }
}
