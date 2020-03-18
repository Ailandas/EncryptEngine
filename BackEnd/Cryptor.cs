using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading;

namespace EncryptEngine.BackEnd
{
    public static class Cryptor
    {
        public static long ThirdOfTime = 0;
        public static bool Execution;
        public static long HalfTime = 0;
        public static bool InProgress = false;
        public static int FileCount = 0;
        public static void Encryption(string path,string key,string BackupPathName)
        {
            try
            {
                
                    //encypt backup
                    string destPath = @getDestinationPath() + @"\BackEnd\";
                    bool subFolders = false;
                    DirectoryInfo DirectInfo = new DirectoryInfo(path);
                    foreach (var fi in DirectInfo.EnumerateFiles())
                    {
                        string file = fi.FullName;
                        string destination = @destPath + @"BackupEncrypted";
                        if (Directory.Exists(destination))
                        {

                            string FileDestination = @destPath + @"BackupEncrypted\" + fi.Name;
                        
                            AES.EncryptFile(file, FileDestination, key);
                        
                        }
                        else
                        {
                            Directory.CreateDirectory(destination);

                            DirectorySecurity ds = Directory.GetAccessControl(destination);
                            ds.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                            Directory.SetAccessControl(destination, ds);

                            string FileDestination = @destPath + @"BackupEncrypted\\" + fi.Name;
                        
                            AES.EncryptFile(file, FileDestination, key);
                        

                        }


                    }
                    foreach (var fi in DirectInfo.EnumerateDirectories())
                    {
                        subFolders = true;
                    }
                    if (subFolders == true)
                    {

                        string zipName = getZipFileName(destPath + BackupPathName);
                        string ZipFilePath = destPath + BackupPathName + "\\" + zipName;
                        string ZipFileEncripted = destPath + "BackupEncrypted\\" + zipName;
                    InProgress = true;
                        AES.EncryptFile(ZipFilePath, ZipFileEncripted, key);
                    InProgress = false;



                    }
                    

                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message + " Main");

            }

        }
        public static void Encrypt(string path,string key,string BackupPathName, CancellationToken cancelToken, Delegate DelegatePause)
        {
            
                try
                {

                if (cancelToken.IsCancellationRequested == false)
                {
                    bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    while(wait==true)
                    {
                        wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    }
                    InProgress = true;
                    FileDirectoryOperations.CreateBackup(path, BackupPathName);
                    InProgress = false;
                }
                if (cancelToken.IsCancellationRequested == false)
                {
                    bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    while (wait == true)
                    {
                        wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    }
                    InProgress = true;
                    Encryption(path, key, BackupPathName);
                    InProgress = false;
                }
                string ExtraSymbolOnPath = "";
                if (cancelToken.IsCancellationRequested == false)
                {
                    bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    while (wait == true)
                    {
                        wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    }
                    InProgress = true;
                    string BackUpEncripted = getDestinationPath() + @"\BackEnd\BackupEncrypted";
                   ExtraSymbolOnPath= FileDirectoryOperations.Replace(path, BackUpEncripted);
                    InProgress = false;
                }
                if(cancelToken.IsCancellationRequested==false)
                {
                    bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    while (wait == true)
                    {
                        wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    }
                    InProgress = true;
                    FileDirectoryOperations.MultiHashCalculator(path+ExtraSymbolOnPath+@"\");
                    InProgress = false;
                }
                if (cancelToken.IsCancellationRequested == false)
                {
                    bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    while (wait == true)
                    {
                        wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                    }
                    InProgress = true;
                    string Backup = getDestinationPath() + @"\BackEnd\" + BackupPathName;
                    FileDirectoryOperations.DeleteDirectory(Backup);
                    Console.WriteLine("Finish");
                    InProgress = false;
                }


            }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                   
                }
            
            
        }
        public static void Decrypt(string path,string key, CancellationToken cancelToken, Delegate DelegatePause)
        {

                try
                {
                    if(cancelToken.IsCancellationRequested==false)
                    {
                        bool wait=Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        while (wait == true)
                        {
                            wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        }
                        InProgress = true;
                        Decryption(path, key,DelegatePause);
                        InProgress = false;
                    }

                    if (cancelToken.IsCancellationRequested == false)
                    {
                        bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        while (wait == true)
                        {
                            wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        }
                        InProgress = true;
                        FileDirectoryOperations.DeleteDirectory(path);
                        InProgress = false;
                    }

                    if (cancelToken.IsCancellationRequested == false)
                    {
                        bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        while (wait == true)
                        {
                            wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        }
                        InProgress = true;
                        string BackUpForDecription = getDestinationPath() + @"\BackEnd\BackupForDecription\";
                        FileDirectoryOperations.Replace(path, BackUpForDecription);
                        Console.WriteLine("Finish");
                        InProgress = false;
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }

        }
        public static void Decryption(string path,string key, Delegate DelegatePause)
        {
           
                try
                {

                    string projectDirectory = getDestinationPath();
                    string BackUpForDecription = projectDirectory + @"\BackEnd\BackupForDecription";
                    Directory.CreateDirectory(BackUpForDecription);
                    DirectoryInfo DirectInfo = new DirectoryInfo(path);

                    List<string> HashesFromDB = Database.GetHashes();

                    List<string> HashList = new List<string>();
                    List<string> NameList = new List<string>();


                    HashList = SplitStringList(HashesFromDB, ',', true);
                    NameList = SplitStringList(HashesFromDB, ',', false);
                    foreach (var fi in DirectInfo.EnumerateFiles())
                    {
                        bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                        if (wait == true)
                        {
                            while (wait == true)
                            {
                                wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                            }
                        }
                        else
                        {

                            if (fi.Name.Contains(".zip") && fi.Name.Length==18+4)
                            {

                                string SingleHash = FileDirectoryOperations.HashCalculator(fi.FullName);
                                if (HashList.Contains(SingleHash) && NameList.Contains(fi.Name))
                                {

                                    string ZipFilePath = BackUpForDecription + "\\" + fi.Name;
                                    AES.DecryptFile(@fi.FullName, BackUpForDecription + "\\" + fi.Name, key);
                                    ZipFile.ExtractToDirectory(ZipFilePath, BackUpForDecription + "\\");
                                    File.Delete(ZipFilePath);
                                }
                                else
                                {
                                    File.Copy(fi.FullName, BackUpForDecription + "\\" + fi.Name);
                                }


                            }
                            else
                            {
                                string SingleHash = FileDirectoryOperations.HashCalculator(fi.FullName);
                                if (HashList.Contains(SingleHash) && NameList.Contains(fi.Name))
                                {

                                    string outputFile = BackUpForDecription + "\\" + fi.Name;
                                    AES.DecryptFile(fi.FullName, outputFile, key);


                                }
                                else
                                {
                                    File.Copy(fi.FullName, BackUpForDecription + "\\" + fi.Name);
                                }

                            }

                        }
                    }
                    
                    

                    


                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    
                }
            
        }
        public static List<string> SplitStringList(List<string> splitable,char c,bool Hash)
        {
            List<string> hash = new List<string>();
            List<string> name = new List<string>();
            string[] Split = new string[2];
            foreach(string one in splitable)
            {
                Split=one.Split(c);
                name.Add(Split[0]);
                hash.Add(Split[1]);
            }
            if(Hash==true)
            {
                return hash;
            }
            else if(Hash==false)
            {
                return name;
            }
            else
            {
                return null;
            }
        }
        private static string getDestinationPath()
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;
            
            return projectDirectory;
        }
       
       

        public static void TimeMeasurment(string path)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                
                sw.Start();

                long size = 0;
                int count = 0;
                DirectoryInfo dinfo = new DirectoryInfo(path);
                foreach (var fi in dinfo.EnumerateFiles())
                {

                    long length = new System.IO.FileInfo(fi.FullName).Length;
                    size = size + length;
                    count++;
                }

                sw.Stop();
                ThirdOfTime = size;
                HalfTime = size;
                FileCount = count;
                Console.WriteLine("half:"+ HalfTime);
                Console.WriteLine("third:" + ThirdOfTime);

            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                
            }

        }
        public static void Cancel()
        {

            
            

                string path = getDestinationPath() + @"\BackEnd\";
                string backup = path + @"BackUp\";
                string timeMeassurment1 = path + @"TempBackupas\";
                string timeMeassurment2 = path + @"TempBackup\";
                string BackupEncrypted = path + @"BackupEncrypted\";
                string BackupForEncriptio = path + @"BackupForDecription\";
            while (true)
            {
                if (InProgress == false)
                {
                    if (Directory.Exists(backup))
                    {
                        Directory.Delete(backup, true);
                    }
                    if (Directory.Exists(timeMeassurment1))
                    {
                        Directory.Delete(timeMeassurment1, true);
                    }
                    if (Directory.Exists(timeMeassurment2))
                    {
                        Directory.Delete(timeMeassurment2, true);
                    }
                    if (Directory.Exists(BackupEncrypted))
                    {
                        Directory.Delete(BackupEncrypted, true);
                    }
                    if (Directory.Exists(BackupForEncriptio))
                    {

                        Directory.Delete(BackupForEncriptio, true);
                    }
                    break;
                }
                
            }
           
        }
       
        public static string getZipFileName(string path)
        {
            string zipname = "";
            DirectoryInfo dinfo = new DirectoryInfo(path);
            foreach(var fi in dinfo.EnumerateFiles())
            {
                if(fi.Name.Contains(".zip"))
                {
                    zipname = fi.Name;
                    break;
                }
            }
            return zipname;
        }

    }
}
