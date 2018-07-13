using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace WpfApp3
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private long progress1;

        public long Progress1
        {
            get { return progress1; }
            set
            {
                progress1 = value;
                NotifyPropertyChanged("Progress1");
            }
        }

        private long progress2;

        public long Progress2
        {
            get { return progress2; }
            set
            {
                progress2 = value;
                NotifyPropertyChanged("Progress2");
            }
        }

        private long progress3;

        public long Progress3
        {
            get { return progress3; }
            set
            {
                progress3 = value;
                NotifyPropertyChanged("Progress3");
            }
        }

        private long progress4;

        public long Progress4
        {
            get { return progress4; }
            set
            {
                progress4 = value;
                NotifyPropertyChanged("Progress4");
            }
        }

        private long progress5;

        public long Progress5
        {
            get { return progress5; }
            set
            {
                progress5 = value;
                NotifyPropertyChanged("Progress5");
            }
        }

        private long maxValue1 = 100;

        public long MaxValue1
        {
            get { return maxValue1; }
            set { maxValue1 = value; NotifyPropertyChanged("MaxValue1"); }
        }

        private long maxValue2 = 100;

        public long MaxValue2
        {
            get { return maxValue2; }
            set { maxValue2 = value; NotifyPropertyChanged("MaxValue2"); }
        }
        private long maxValue3 = 100;

        public long MaxValue3
        {
            get { return maxValue3; }
            set { maxValue3 = value; NotifyPropertyChanged("MaxValue3"); }
        }

        private long maxValue5 = 100;

        public long MaxValue5
        {
            get { return maxValue5; }
            set { maxValue5 = value; NotifyPropertyChanged("MaxValue5"); }
        }

        private long maxValue4 = 100;

        public long MaxValue4
        {
            get { return maxValue4; }
            set { maxValue4 = value; NotifyPropertyChanged("MaxValue4"); }
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

           bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                Path.Text = filename;
            }
        }
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void AddProgressValue(int position, long value)
        {
            switch (position)
            {
                case 0: Progress1 = value; break;
                case 1: Progress2 = value; break;
                case 2: Progress3 = value; break;
                case 3: Progress4 = value; break;
                case 4: Progress5 = value; break;
                default:
                    break;
            }
        }



        public void ChangeMaxValue(int position)
        {
            switch (position)
            {
                case 0: MaxValue1 = listBytes[position].Length; break;
                case 1: MaxValue2 = listBytes[position].Length; break;
                case 2: MaxValue3 = listBytes[position].Length; break;
                case 3: MaxValue4 = listBytes[position].Length; break;
                case 4: MaxValue5 = listBytes[position].Length; break;
                default:
                    break;
            }
        }


        byte[] data;
        Semaphore sem = new Semaphore(1, 1);
        List<byte[]> listBytes = new List<byte[]>();
        Dictionary<int, byte[]> listZipBytes = new Dictionary<int, byte[]>();

        public int howmany(int length)
        {
            for (int i = 10; i > 0; i--)
            {
                if(length % i == 0)
                {
                    return i;
                }
            }
            return 1;
        }

        public void CompressEvent(int x)
        {
            int sms = x;
            using (MemoryStream comp = new MemoryStream(listBytes[sms]))
            {
                using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
                {
                    ChangeMaxValue(listBytes[sms].Length);
                    //int ha = howmany(listBytes[sms].Length);
                    for (int i = 0; i < listBytes[sms].Length; i++ )
                    {
                        inStream.WriteByte(listBytes[sms][i]);
                        AddProgressValue(sms, i);
                    }
                }
                listZipBytes.Add(sms, comp.ToArray());
            }
        }

        Dictionary<int, List<byte[]>> decompresslistbytes = new Dictionary<int, List<byte[]>>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void DecompressEvent(int x)
        {
            int sms = x;
            List<byte[]> listGrb = new List<byte[]>();
            using (MemoryStream fileStream = new MemoryStream(listBytes[sms]))
            {
                using (GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    byte[] bytes = new byte[10];
                    var bytesCount = 0;
                    while ((bytesCount = gZipStream.Read(bytes, 0, 10)) != 0)
                    {
                        listGrb.Add(bytes.Take(bytesCount).ToArray());
                    }
                    decompresslistbytes.Add(sms, listGrb);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ResetAll();
            status.Text = "Working...";
            
            string inFileName = Path.Text;
            FileStream inFile = new FileStream(inFileName, FileMode.Open);
            try
            {
                int ss = (int)(inFile.Length / int.Parse(CountsThreads.Text));
                data = new byte[inFile.Length];
                inFile.Read(data, 0, data.Length);

                for (int i = 0; i < int.Parse(CountsThreads.Text); i++)
                {
                    listBytes.Add(data.Skip(ss * i).Take(ss).ToArray());
                }
                listBytes.Add(data.Skip(ss * int.Parse(CountsThreads.Text)).Take(data.Length - ss).ToArray());
                Window1 window1 = new Window1();
                window1.ShowDialog();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                List<Task> listThread = new List<Task>();

                for (int i = 0; i < listBytes.Count; i++)
                {
                    listThread.Add(new Task(() => { CompressEvent(i); }));
                    listThread[i].Start();
                }
                Task.WaitAll(listThread.ToArray());
                string pth = inFileName.Substring(0, inFileName.LastIndexOf('\\') + 1) + window1.Filename;
                using (FileStream fileStreamResult = new FileStream(pth, FileMode.Create))
                {
                    for (int i = 0; i < listBytes.Count; i++)
                    {
                        fileStreamResult.Write(listZipBytes[i], 0, listZipBytes[i].Length);
                    }
                }
                //view
                stopwatch.Stop();
                times.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";
                status.Text = "Completed!!";
                test.Text = "Infile Bytes count:" + inFile.Length.ToString();
                test1.Text = "Thread1 : " + listBytes[0].Length.ToString();
                if (listBytes.Count > 1)
                    test2.Text = "Thread2 : " + listBytes[1].Length.ToString();
                if (listBytes.Count > 2)
                    test3.Text = "Thread3 : " + listBytes[2].Length.ToString();
                if (listBytes.Count > 3)
                    test4.Text = "Thread4 : " + listBytes[3].Length.ToString();
                if(listBytes.Count  > 4)
                    test5.Text = "Thread5 : " + listBytes[4].Length.ToString();
                inFile.Dispose();

                
                listBytes.Clear();
                listZipBytes.Clear();
                data = null;
            }
            catch (Exception error)
            {
                status.Text = error.Message;
            }
            inFile.Dispose();
        }

        public void ResetAll()
        {
            Progress1 = 0;
            Progress2 = 0;
            Progress3 = 0;
            Progress4 = 0;
            Progress5 = 0;
            MaxValue1 = 100;
            MaxValue2 = 100;
            MaxValue3 = 100;
            MaxValue4 = 100;
            MaxValue5 = 100;
            test.Text = "Infile Bytes count:";
            test1.Text = "Thread1 : ";
            test2.Text = "Thread2 : ";
            test3.Text = "Thread3 : ";
            test4.Text = "Thread4 : ";
        }
        public byte[] StringToByte(string str)
        {
            var list = str.Split(' ').ToList();
            list.RemoveAt(list.Count - 1);
            var byts = new byte[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                byts[i] = byte.Parse(list[i]);
            }
            return byts;
        }


        public List<byte[]> SplitByte(byte[] data)
        {
            StringBuilder datas = new StringBuilder();
            StringBuilder header = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                if(i<10)
                    header.Append(data[i].ToString() + " ");
                datas.Append(data[i]);
                if (i + 1 != data.Length)
                    datas.Append(" ");
            }
            var lists = datas.ToString().Split(new string[] { header.ToString() }, StringSplitOptions.None).ToList();
            List<byte[]> bytes = new List<byte[]>();
            lists.RemoveAt(0);
            for (int i = 0; i < lists.Count; i++)
            {
                lists[i] = header + lists[i];
                bytes.Add(StringToByte(lists[i]));
            }
            return bytes;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ResetAll();
            status.Text = "Working...";
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string inFileName = Path.Text;
            FileStream inFile = new FileStream(inFileName, FileMode.Open);
            try
            {
                data = new byte[inFile.Length];
                inFile.Read(data, 0, data.Length);
                listBytes = SplitByte(data);
                List<Task> listThread = new List<Task>();

                for (int i = 0; i < listBytes.Count; i++)
                {
                    listThread.Add(new Task(() => { DecompressEvent(i); }));
                    listThread[i].Start();
                    listThread[i].Wait();
                }
                Task.WaitAll(listThread.ToArray());
                using (FileStream fileStreamResult = new FileStream(inFileName.Substring(0, inFileName.LastIndexOf('.')), FileMode.Create))
                {
                    for (int i = 0; i < decompresslistbytes.Count; i++)
                    {
                        List<byte[]> bytes = decompresslistbytes[i];
                        foreach (var byteinone in bytes)
                        {
                            fileStreamResult.Write(byteinone, 0, byteinone.Length);
                        }
                    }
                }
                stopwatch.Stop();
                times.Text = stopwatch.ElapsedMilliseconds.ToString() + "ms";
                status.Text = "Completed!!";
                test.Text = "Infile Bytes count:" + inFile.Length.ToString();
                test1.Text = "Thread1 : " + listBytes[0].Length.ToString();
                if (listBytes.Count > 1)
                    test2.Text = "Thread2 : " + listBytes[1].Length.ToString();
                if (listBytes.Count > 2)
                    test3.Text = "Thread3 : " + listBytes[2].Length.ToString();
                if (listBytes.Count > 3)
                    test4.Text = "Thread4 : " + listBytes[3].Length.ToString();
                if (listBytes.Count > 4)
                    test5.Text = "Thread5 : " + listBytes[4].Length.ToString();
                inFile.Dispose();
                
                listBytes.Clear();
                decompresslistbytes.Clear();
                data = null;
            }
            catch (Exception error)
            {
                status.Text = error.Message;
            }
            inFile.Dispose();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                MessageBox.Show(files[0]);
                Path.Text = files[0];
            }
        }
    }
}
