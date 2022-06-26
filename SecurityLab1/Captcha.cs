using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.Windows.Interop;
using System.Drawing.Drawing2D;
using System.Windows;

namespace SecurityLab1
{
    class Captcha
    {
        public string Text { get; private set; }
        private Random _rand = new Random();

        public BitmapSource GenerateCaptcha(int wid = 250, int hgt = 130)
        {
            Bitmap bm = new Bitmap(wid, hgt);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                //Adding background (random-colored)
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.Clear(Color.FromArgb(120, _rand.Next(255), _rand.Next(255), _rand.Next(255)));

                //Generating CAPTCHA text
                string txt = string.Empty;
                string ALF = "23456789QWERTYUPASDFGHJKZXCVBNM";
                for (int i = 0; i < _rand.Next(4, 7); ++i)
                    txt += ALF[_rand.Next(ALF.Length)];

                //Storing CAPTHCA text
                Text = txt.ToLower();

                //Calculating free space for each character
                int ch_wid = wid / txt.Length;

                //Adding lines below text
                for (int i = 0; i < 20; i++)
                {
                    if (_rand.Next() % 2 == 0)
                        gr.DrawLine(Pens.Black,
                           new System.Drawing.Point(_rand.Next(wid), _rand.Next(hgt)),
                           new System.Drawing.Point(_rand.Next(wid), _rand.Next(hgt)));
                }

                //Drawing text char-by-char
                for (int i = 0; i < txt.Length; i++)
                {
                    float font_size = _rand.Next(60, 80);
                    using (Font the_font = new Font("Times New Roman",
                        font_size, System.Drawing.FontStyle.Bold))
                    {
                        DrawCharacter(txt.Substring(i, 1), gr,
                            the_font, i * ch_wid, ch_wid, wid, hgt);
                    }
                }

                //Adding lines above text
                for (int i = 0; i < 20; i++)
                {
                    if (_rand.Next() % 2 == 0)
                        gr.DrawLine(Pens.Black,
                           new System.Drawing.Point(_rand.Next(wid), _rand.Next(hgt)),
                           new System.Drawing.Point(_rand.Next(wid), _rand.Next(hgt)));
                }

                //Adding colorful dots
                for (int i = 0; i < wid; ++i)
                    for (int j = 0; j < hgt; ++j)
                        if (_rand.Next() % 10 == 0)
                            bm.SetPixel(i, j, Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255)));
            }

            return Bitmap2BitmapImage(bm);
        }

        private int PreviousAngle = 0;
        private void DrawCharacter(string txt, Graphics gr, Font the_font, int X, int ch_wid, int wid, int hgt)
        {
            // Центрируем текст.
            using (StringFormat string_format = new StringFormat())
            {
                string_format.Alignment = StringAlignment.Center;
                string_format.LineAlignment = StringAlignment.Center;
                RectangleF rectf = new RectangleF(X, 0, ch_wid, hgt);

                // Преобразование текста в путь.
                using (GraphicsPath graphics_path = new GraphicsPath())
                {
                    graphics_path.AddString(txt,
                        the_font.FontFamily, (int)the_font.Style,
                        the_font.Size, rectf, string_format);

                    // Произвольные случайные параметры деформации.
                    float x1 = X + _rand.Next(ch_wid) / 2;
                    float y1 = _rand.Next(hgt) / 2;
                    float x2 = X + ch_wid / 2 +
                        _rand.Next(ch_wid) / 2;
                    float y2 = hgt / 2 + _rand.Next(hgt) / 2;
                    PointF[] pts = {
            new PointF(
                X + _rand.Next(ch_wid) / 4,
                _rand.Next(hgt) / 4),
            new PointF(
                X + ch_wid - _rand.Next(ch_wid) / 4,
                _rand.Next(hgt) / 4),
            new PointF(
                X + _rand.Next(ch_wid) / 4,
                hgt - _rand.Next(hgt) / 4),
            new PointF(
                X + ch_wid - _rand.Next(ch_wid) / 4,
                hgt - _rand.Next(hgt) / 4)
        };
                    Matrix mat = new Matrix();
                    graphics_path.Warp(pts, rectf, mat,
                        WarpMode.Perspective, 0);

                    // Поворачиваем бит случайным образом.
                    float dx = X + ch_wid / 2;
                    float dy = hgt / 2;
                    gr.TranslateTransform(-dx, -dy, MatrixOrder.Append);
                    int angle = PreviousAngle;
                    do
                    {
                        angle = _rand.Next(-30, 30);
                    } while (Math.Abs(angle - PreviousAngle) < 20);
                    PreviousAngle = angle;
                    gr.RotateTransform(angle, MatrixOrder.Append);
                    gr.TranslateTransform(dx, dy, MatrixOrder.Append);

                    gr.FillPath(new SolidBrush(Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255))), graphics_path);
                    gr.ResetTransform();
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        private BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }
    }
}
