using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EncryptEngine.BackEnd
{
    public static class FileDirectoryOperations
    {
        public static void CreateBackup(string path, string BackupPathName)
        {
            
                //copy to backup
                string target = getDestinationPath() + @"\BackEnd\" + BackupPathName;
                var diSource = new DirectoryInfo(path);
                var diTarget = new DirectoryInfo(target);
                CopyAll(diSource, diTarget);
                string zipName = DateTime.Now.Ticks.ToString();
                ZipFile.CreateFromDirectory(path, target + @"\" + zipName + ".zip");
                string ZipFilePath = target + @"\" + zipName + ".zip";
                ZipFoldersCollection(ZipFilePath);
            
            //copy to backup
        }
        private static string getDestinationPath()
        {
            var enviroment = System.Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(enviroment).Parent.FullName;

            return projectDirectory;
        }
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {


            System.IO.Directory.CreateDirectory(target.FullName);


            foreach (FileInfo fi in source.GetFiles())
            {

                fi.CopyTo(System.IO.Path.Combine(@target.FullName, @fi.Name), true);

            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(@diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }

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
                Console.WriteLine(exc.Message + " ZiPCollection");

            }


        }
        private static bool IsFolder(this ZipArchiveEntry entry)
        {
            return entry.FullName.Contains("/");
        }
        private static void DeleteItems(List<string> items, string ZipFilePath)
        {
            using (Stream stream = File.Open(ZipFilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update, false))
                {
                    foreach (var z in items)
                    {
                        ZipArchiveEntry zp = archive.GetEntry(z);
                        zp.Delete();
                    }

                }
            }

        }
        public static void MultiHashCalculator(string path )
        {
            
                List<string> Names = new List<string>();
                List<string> Hashes = new List<string>();
                DirectoryInfo dInfo = new DirectoryInfo(path);
                foreach (var fi in dInfo.EnumerateFiles())
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
        public static string HashCalculator(string path)
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
        public static string Replace(string ReplacableDirectoryPath, string CryptedBackup)
        {
           
                if (Directory.Exists(CryptedBackup))
                {
                    if (Directory.Exists(ReplacableDirectoryPath))
                    {
                        /*  Directory.Delete(ReplacableDirectoryPath);
                          Console.WriteLine("Existing still:");
                          DirectoryInfo dest = new DirectoryInfo(CryptedBackup);
                          Console.WriteLine(ReplacableDirectoryPath);
                          Console.WriteLine(CryptedBackup);
                          dest.MoveTo(ReplacableDirectoryPath);*/
                        Console.WriteLine("1");
                        Directory.Delete(ReplacableDirectoryPath,true);
                        Random random = new Random();
                        int r=random.Next(1, 10);
                        Directory.Move(CryptedBackup, ReplacableDirectoryPath+r);
                        return r.ToString();
                    
                    }
                    else
                    {

                        Console.WriteLine("2");
                        Random random = new Random();
                        int r= random.Next(1, 10);
                        Directory.Move(CryptedBackup, ReplacableDirectoryPath /*+ $"{r.ToString()}"*/);
                         return r.ToString();
                        /* Console.WriteLine("Already deleted");
                         DirectoryInfo dest = new DirectoryInfo(CryptedBackup);
                         Console.WriteLine(ReplacableDirectoryPath);
                         Console.WriteLine(CryptedBackup);
                         dest.MoveTo(ReplacableDirectoryPath);*/
                    }
                }
            return "";
            
        }

        public static void DeleteDirectory(string input)
        {
            
                try
                {
                    Console.WriteLine("Deleting:" + input);
                    Directory.Delete(input, true);
                    
                 
                }
                
               
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    
                }
            
        }


    }
}
