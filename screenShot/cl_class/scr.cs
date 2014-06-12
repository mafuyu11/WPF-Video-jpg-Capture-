using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace screenShot.cl_class 
{
    public static class scr 
    {
        public static void video_capture(MediaElement me, string fileName)
        {
            var bmp = new RenderTargetBitmap(me.NaturalVideoWidth, me.NaturalVideoHeight, 96, 96, PixelFormats.Default);
            var sb = new VisualBrush(me);
            var dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();

            bmp.Dispatcher.Invoke
                (
                    new Action
                        (
                        () => Thread.Sleep(5)
                        )
                );

            using (dc)
            {
                dc.PushTransform(new ScaleTransform(1, 1));
                dc.DrawRectangle(sb, null,
                                 new Rect(new Point(0, 0), new Point(me.NaturalVideoWidth, me.NaturalVideoHeight)));
                dc.Dispatcher.Invoke
                    (
                        new Action
                            (
                            () => Thread.Sleep(5)
                            )
                    );
            }

            bmp.Render(dv);

            var j = new JpegBitmapEncoder {QualityLevel = 100};
            j.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream s = File.Create(fileName))
            {
                j.Dispatcher.Invoke
                    (
                        new Action
                            (
                            () => Thread.Sleep(5)
                            )
                    );

                j.Save(s);
                s.Dispose();
                bmp.Clear();
            }
        }
    }
}
