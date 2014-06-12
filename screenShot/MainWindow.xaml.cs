using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using screenShot.cl_class;
using System.IO;
using System.Threading;

namespace screenShot
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow
    {
        public string PositionTime { get; set; }
        public string FilePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            btn_ev();
            DragFile();
            SliderEvents();
        }

        public bool IsOpenedWindow { get; set; }

        public void btn_chg()
        {
            if (Pmed.Source != null)
            {
                var be = new BitmapImage();
                if (Pmed.LoadedBehavior == MediaState.Play)
                {
                    be.BeginInit();
                    be.UriSource = new Uri("pack://application:,,,/img/appbar.transport.play.rest.png");
                    be.EndInit();
                    PPlay.Source = be;
                    Pmed.LoadedBehavior = MediaState.Pause;
                }
                else
                {
                    be.BeginInit();
                    be.UriSource = new Uri("pack://application:,,,/img/appbar.transport.pause.rest.png");
                    be.EndInit();
                    PPlay.Source = be;
                    Pmed.LoadedBehavior = MediaState.Play;
                }
            }
        }

        private void btn_ev()
        {
            PPlay.MouseLeftButtonDown += (s, e) =>
                {
                    btn_chg();
                };

            IsOpenedWindow = false;
            PList.MouseLeftButtonDown += (s, e) =>
                {
                    var pl = new list { Owner = this };
                    if (IsOpenedWindow == false)
                    {
                        pl.Show();
                        IsOpenedWindow = true;
                    }
                };

            PScr.MouseLeftButtonDown += (s, e) =>
                {
                    if (Pmed.HasVideo)
                    {
                        Pmed.LoadedBehavior = MediaState.Pause;
                        using (var sv = new SaveFileDialog())
                        {
                            var fi = new FileInfo(FilePath);
                            sv.FileName = fi.Name + " - " + PositionTime;
                            sv.Filter = "이미지 파일|*.jpg";
                            if (sv.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                try
                                {
                                    scr.video_capture(Pmed, sv.FileName);
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.Forms.MessageBox.Show("캡쳐 하는 도중 오류가 났습니다." + ex.Message, "캡쳐 실패");
                                }
                            }
                        }
                        Pmed.LoadedBehavior = MediaState.Play;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("해당 파일은 동영상 파일이 아닙니다.", "캡쳐 실패");
                    }
                };

            Pmed.MediaOpened += (s, e) =>
                {
                    var fi = new FileInfo(FilePath);
                    Title = "동영상 스크린샷 APP / " + fi.Name;
                    PlayerTime();
                };
        }


        private void SliderEvents()
        {
            PProg.BeginInit();
            PProg.IsMoveToPointEnabled = true;
            PProg.ValueChanged += (s, e) => // 진행 바 값 변경 시 이벤트
            {
                try
                {
                    var v = Pmed.Position.TotalSeconds / Pmed.NaturalDuration.TimeSpan.TotalSeconds;
                    if (Math.Abs(e.NewValue - v) > 1.0 / Pmed.NaturalDuration.TimeSpan.TotalSeconds)
                    {
                        Pmed.Position = TimeSpan.FromSeconds(Pmed.NaturalDuration.TimeSpan.TotalSeconds * e.NewValue);
                    }
                }
                catch (Exception)
                {
                } 
            };
            PProg.EndInit();

            //PVol.BeginInit();
            PVol.ValueChanged += (s, e) => // 볼륨 바 값 변경 시 이벤트
            {
                PVol.ToolTip = "볼륨 : " + Convert.ToString((int)(PVol.Value * 200));
            };
            //PVol.EndInit();
        }

        private void DragFile()
        {
            Drop += (s, e) => 
            {
                var str = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop, false);
                var fi = new FileInfo(str[0]);
                
                if (fi.Extension.Equals(@".mp4"))
                {
                    Pmed.Source = new Uri(str[0]);
                    Pmed.LoadedBehavior = MediaState.Pause;
                    FilePath = str[0];
                    btn_chg();
                }
                else
                {
                    Pmed.LoadedBehavior = MediaState.Close;
                    System.Windows.Forms.MessageBox.Show("mp4 형식의 동영상 파일이 아닙니다.", "재생 불가");
                }
            };

            DragEnter += (s, e) => 
            {
                e.Effects = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop) ? 
                    System.Windows.DragDropEffects.All : System.Windows.DragDropEffects.None;
            };
        }

        private void PlayerTime()
        {
            var t = new System.Windows.Forms.Timer();
            t.Dispose();
            t.Tick += (s1, e1) => // 틱 당 이벤트
                {
                    Thread.Sleep(5);
                    try
                    {
                        var h = Pmed.Position.Hours.ToString("00");
                        var m = Pmed.Position.Minutes.ToString("00");
                        var se = Pmed.Position.Seconds.ToString("00");
                        PProg.ToolTip = h + ":" + m + ":" + se + " / " + DurationTimer();
                        PositionTime = h + "시간 " + m + "분 " + se + "초";

                        PProg.IsHitTestVisible = true;
                        PProg.Value = Pmed.Position.TotalSeconds / Pmed.NaturalDuration.TimeSpan.TotalSeconds;
                    }
                    catch (InvalidOperationException)
                    {
                        Thread.Sleep(10);
                    }
                };
            t.Start();
        }

        public string DurationTimer() // 재생 진행 시간
        {
            var n = "00:00:00";
            try
            {
                var h = Pmed.NaturalDuration.TimeSpan.Hours.ToString("00");
                var m = Pmed.NaturalDuration.TimeSpan.Minutes.ToString("00");
                var se = Pmed.NaturalDuration.TimeSpan.Seconds.ToString("00");
                n = h + ":" + m + ":" + se;
            }
            catch (InvalidOperationException)
            {
                return n;
            }

            return n;
        }
    }
}
