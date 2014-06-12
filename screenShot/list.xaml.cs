using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace screenShot
{
    /// <summary>
    /// list.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class list
    {
        public list()
        {
            InitializeComponent();
            IsXML();
            btn_ev();
        }

        private static string XMLFilePath
        {
            get
            {
                return System.Windows.Forms.Application.StartupPath + @"\list.xml";
            }
        }

        private void IsXML()
        {
            var fi = new FileInfo(XMLFilePath);
            if (!fi.Exists)
            {
                var doc = new XDocument();
                var root = new XElement("root", "");
                doc.Add(root);
                doc.Save(XMLFilePath);
                doc = null;
            }
            else
            {
                var doc = XDocument.Load(XMLFilePath);
                if (doc.Root.FirstNode != null)
                {
                    foreach (var file in doc.Root.Descendants("file"))
                    {
                        var fd = new FileInfo(file.Value);
                        PList.Items.Add(fd.Name);
                    }
                }
                doc = null;
            }
        }

        private void PlayOn()
        {
            var doc = XDocument.Load(XMLFilePath);
            var path = string.Empty;
            foreach (var sel in doc.Root.Descendants("file"))
            {
                var fi = new FileInfo(sel.Value);
                if (fi.Name == PList.SelectedItem.ToString())
                {
                    path = sel.Value;
                }
            }
            var main = (MainWindow)Owner;
            main.Pmed.Source = new Uri(path);
            main.Pmed.LoadedBehavior = MediaState.Pause;
            main.FilePath = path;
            main.btn_chg();
            doc = null;
        }

        private void btn_ev()
        {
            /*Loaded += (s1, e1) =>
                {
                var mainp = (MainWindow)Owner;

                mainp.PPrev.MouseLeftButtonDown += (s, e) =>
                    {
                        if (PList.SelectedIndex > 0)
                        {
                            PList.SelectedIndex--;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox
                                .Show("리스트 처음 부분 이거나 리스트에서 선택한 것이 없습니다.", "리스트 상태");
                            return;
                        }

                        PlayOn();
                    };

                mainp.PNext.MouseLeftButtonDown += (s, e) =>
                    {
                        if (PList.SelectedIndex > PList.Items.Count - 1)
                        {
                            PList.SelectedIndex++;
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox
                                .Show("리스트 마지막 부분 입니다.", "리스트 상태");
                            return;
                        }

                        PlayOn();
                    };
                };*/

            PList.MouseDoubleClick += (s, e) =>
                {
                    PlayOn();
                };

            PAdd.MouseLeftButtonDown += (s, e) =>
                {
                    using (var op = new OpenFileDialog())
                    {
                        op.Multiselect = true;
                        op.Filter = "동영상 파일|*.mp4";
                        if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            foreach (var file in op.FileNames)
                            {
                                var fi = new FileInfo(file);
                                PList.Items.Add(fi.Name);
                                var doc = XDocument.Load(XMLFilePath);
                                doc.Root.Add(new XElement("file", file));
                                doc.Save(XMLFilePath);
                                doc = null;
                            }
                        }
                    }
                };

            PDir.MouseLeftButtonDown += (s, e) =>
                {
                    using (var fo = new FolderBrowserDialog())
                    {
                        fo.Description = "mp4 동영상 파일이 있는 폴더를 추가 합니다.";
                        if (fo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            var dir = new DirectoryInfo(fo.SelectedPath);
                            var files = new List<string>();
                            foreach (var file in dir.GetFiles())
                            {
                                if (file.Extension.Equals(".mp4"))
                                {
                                    files.Add(file.FullName);
                                }
                            }

                            foreach (var file in files)
                            {
                                var fi = new FileInfo(file);
                                PList.Items.Add(fi.Name);
                                var doc = XDocument.Load(XMLFilePath);
                                doc.Root.Add(new XElement("file", file));
                                doc.Save(XMLFilePath);
                                doc = null;
                            }
                        }
                    }
                };

            PDel.MouseLeftButtonDown += (s, e) =>
                {
                    var doc = XDocument.Load(XMLFilePath);

                    var selDir = new List<string>();
                    foreach (var dir in doc.Root.Descendants("file"))
                    {
                        var di = new FileInfo(dir.Value);
                        foreach (var sel in PList.SelectedItems)
                        {
                            if (di.Name == sel.ToString())
                            {
                                selDir.Add(di.FullName);
                            }
                        }
                    }

                    foreach (var sel in selDir)
                    {
                        doc.Root.Descendants("file")
                            .Where(c => c.Value.Equals(sel)).Remove();
                    }
                    doc.Save(XMLFilePath);
                    doc = null;

                    PList.Items.Clear();
                    IsXML();
                };


            Closed += (s, e) =>
                {
                    var main = (MainWindow)Owner;
                    main.IsOpenedWindow = false;
                };
        }
    }
}
