using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using SkiaSharp;

namespace RegistrationForm.Utils
{
    public static class Captcha
    {
        public static (string code, byte[] image) GenerateCaptcha()
        {
            var rnd = new Random();
            var code = rnd.Next(1000, 9999).ToString();

            using var bmp = new SKBitmap(100, 40);
            using var canvas = new SKCanvas(bmp);
            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 24,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Arial")
            };

            canvas.DrawText(code, 10, 30, paint);
            canvas.Flush();

            using var image = SKImage.FromBitmap(bmp);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            byte[] imageBytes = data.ToArray();

            string path = GetViewPath("captcha.png");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, imageBytes);

            return (code, imageBytes);
        }
        
        private static string GetViewPath(string filename)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDir, "..", "..", "..", "Views", filename);
            return Path.GetFullPath(filePath);
        } 
    }

    public static class CaptchaStorage
    {
        public static string CurrentCode { get; set; } = "";

    }

        
}
