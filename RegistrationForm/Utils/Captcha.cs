using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RegistrationForm.Utils
{
        public static class Captcha
        {
            public static (string code, byte[] image) GenerateCaptcha()
            {
                var rnd = new Random();
                var code = rnd.Next(1000, 9999).ToString();

                using var bmp = new Bitmap(100, 40);
                using var g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.DrawString(code, new System.Drawing.Font("Arial", 20), Brushes.Black, 10, 5);

                byte[] imageBytes;
                
                using (var ms  = new MemoryStream()) 
                {
                    bmp.Save(ms, ImageFormat.Png);
                string path = @"D:\\C#\\RegistrationForm\\RegistrationForm\\Views\\captcha.png";
                Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure folder exists
                bmp.Save(path, ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                return (code, imageBytes);
            }
        }

        public static class CaptchaStorage
        {
            public static string CurrentCode { get; set; } = "";
        }
}
