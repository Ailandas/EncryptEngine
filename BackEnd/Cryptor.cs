using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;

namespace EncryptEngine.BackEnd
{
    public static class Cryptor
    {
        public static int ThirdOfTime = 0;
        public static bool Execution;
        public static int HalfTime = 0;
        public static void Encryption(string path,string key,string BackupPathName)
        {
            try
            {

                //encypt backup
                string destPath = @getDestinationPath();
                bool subFolders = false;
                DirectoryInfo DirectInfo = new DirectoryInfo(path);
                foreach (var fi in DirectInfo.EnumerateFiles())
                {
                    string file = fi.FullName;
                    string destination = @destPath + @"BackupEncrypted";
                    if (Directory.Exists(destination))
                    {
                        string FileDestination = @destPath + @"BackupEncrypted\\" + fi.Name;
                        AES.EncryptFile(file, FileDestination, key);

                    }
                    else
                    {
                        Directory.CreateDirectory(destination);
                        string FileDestination = @destPath + @"BackupEncrypted\\" + fi.Name;
                        AES.EncryptFile(file, FileDestination, key);
                    }
                    

                }
                foreach (var fi in DirectInfo.EnumerateDirectories())
                {
                    subFolders = true;
                }
                if(subFolders==true)
                {
                   
                        string ZipFilePath = destPath + BackupPathName + "\\zipas.zip";
                        string ZipFileEncripted = destPath + "BackupEncrypted\\zipas.zip";
                        AES.EncryptFile(ZipFilePath, ZipFileEncripted, key);
                    

                }
                var enviroment = System.Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;
               
                string BackUpEncripted = @projectDirectory+"\\BackEnd\\BackupEncrypted";
                if(Directory.Exists(BackUpEncripted))
                {
                    Console.WriteLine(path);
                    Replace(path, BackUpEncripted);
                    MultiHashCalculator(path);
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message+" Main");
            }
        }
        public static void Encrypt(string path,string key,string BackupPathName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Execution = true;
            CreateBackup(path, BackupPathName);
            Encryption(path,key,BackupPathName);
            string Backup = getDestinationPath();
            Backup = Backup + "//"+ BackupPathName;
            DeleteDirectory(Backup);

            
            
            Execution = false;
            sw.Stop();
            Console.WriteLine("Finish" + sw.ElapsedMilliseconds) ;
        }
        public static void Decrypt(string path,string key)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Execution = true;
            Decryption(path,key);
            Execution = false;
            sw.Stop();
            Console.WriteLine("Finish" + sw.ElapsedMilliseconds);
        }
        public static void Decryption(string path,string key)
        {
            try
            {
              
                string BackUpForDecription = getDestinationPath();
                BackUpForDecription = BackUpForDecription + "\\BackupForDecription";
                Directory.CreateDirectory(BackUpForDecription);
                DirectoryInfo DirectInfo = new DirectoryInfo(path) ;

                List<string> HashesFromDB = Database.GetHashes();

                List<string> HashList = new List<string>();
                List<string> NameList = new List<string>();


                HashList=SplitStringList(HashesFromDB, ',', true);
                NameList=SplitStringList(HashesFromDB, ',', false);
                foreach (var fi in DirectInfo.EnumerateFiles())
                {

                    if (fi.Name.Contains(".zip"))
                    {
                        string SingleHash = HashCalculator(fi.FullName);
                        if (HashList.Contains(SingleHash) && NameList.Contains(fi.Name))
                        {
                            string ZipFilePath = BackUpForDecription + "\\zipas.zip";
                            AES.DecryptFile(@fi.FullName, BackUpForDecription + "\\" + fi.Name, key);
                            ZipFile.ExtractToDirectory(ZipFilePath, BackUpForDecription + "\\");
                            File.Delete(ZipFilePath);
                        }
                        else
                        {
                            File.Copy(fi.FullName,BackUpForDecription+"\\"+fi.Name);
                        }


                    }
                    else
                    {
                        string SingleHash = HashCalculator(fi.FullName);
                        if (HashList.Contains(SingleHash) && NameList.Contains(fi.Name))
                        {
                            AES.DecryptFile(fi.FullName, BackUpForDecription + "\\" + fi.Name, key);
                        }
                        else
                        {
                            File.Copy(fi.FullName, BackUpForDecription + "\\" + fi.Name);
                        }

                    }

                }
                Replace(path, BackUpForDecription);
              
                
                
                
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
            projectDirectory = projectDirectory + "//BackEnd//" ;
            return projectDirectory;
        }
        private static void ZipFoldersCollection(string ZipFilePath)
        {

            List<string> DeletableItems = new List<string>();
            try
            {
                
                    using (ZipArchive archive = ZipFile.OpenRead(ZipFilePath))
                    {
                    
                        foreach (var s in archive.Entries)
                        {

                            if (s.IsFolder())
                            {

                            }
                            else
                            {
                                DeletableItems.Add(s.FullName);
                           
                            }

                        }
                    }
                    DeleteItems(DeletableItems, ZipFilePath);
   
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message+" ZiPCollection");
            }
           
            
        }
        private static bool IsFolder(this ZipArchiveEntry entry)
        {
            return entry.FullName.Contains("/");
        }
        private static void DeleteItems(List<string> items,string ZipFilePath)
        {
            using (Stream stream = File.Open(ZipFilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update, false))
                {
                    foreach(var z in items)
                    {
                        ZipArchiveEntry zp = archive.GetEntry(z);
                        zp.Delete();
                    }
                        
                }
            }
            
        }
   
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        public static void CreateBackup(string path,string BackupPathName)
        {
            //copy to backup
            string target = getDestinationPath();
            target = target + "\\"+ BackupPathName;
            var diSource = new DirectoryInfo(path);
            var diTarget = new DirectoryInfo(target);
            CopyAll(diSource, diTarget);
            ZipFile.CreateFromDirectory(path, target+"\\zipas.zip");
            string ZipFilePath = target + "\\zipas.zip";
            ZipFoldersCollection(ZipFilePath);
            
            //copy to backup
        }
        public static void Replace(string ReplacableDirectoryPath,string CryptedBackup)
        {
            try
            {
                Directory.Delete(ReplacableDirectoryPath, true);
                Directory.Move(CryptedBackup, ReplacableDirectoryPath);

            }
            catch(Exception exc)
            {
                Console.WriteLine("Replacable: "+ ReplacableDirectoryPath);
                Console.WriteLine("Crypted backup: " + CryptedBackup);
                Console.WriteLine(exc.Message + "message");
            }
        }
        private static void MultiHashCalculator(string path)
        {
            List<string> Names = new List<string>();
            List<string> Hashes = new List<string>();
            DirectoryInfo dInfo = new DirectoryInfo(path);
            foreach(var fi in dInfo.EnumerateFiles())
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(fi.FullName))
                    {
                        Names.Add(fi.Name);
                        var hash = md5.ComputeHash(stream);
                        Hashes.Add(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                    }
                }
            }
            Database.SendHashes(Names, Hashes);
        }
        private static string HashCalculator(string path)
        {
            string hashas;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        
                        var hash = md5.ComputeHash(stream);
                        hashas = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }

            return hashas;
        }
        public static void DeleteDirectory(string input)
        {
            Directory.Delete(input, true);
        }
        public static void TimeMeasurment(string path,string BackupPathName)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                
                sw.Start();
                DirectoryInfo source = new DirectoryInfo(path);
                string destination = getDestinationPath() + "\\" + BackupPathName;
                DirectoryInfo dest = new DirectoryInfo(destination);
                CopyAll(source, dest);
                Directory.Delete(destination, true);
                sw.Stop();
                ThirdOfTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                HalfTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                Console.WriteLine("half:"+ HalfTime);
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

        }
        public static void Cancel()
        {
            
        }
    }
}
