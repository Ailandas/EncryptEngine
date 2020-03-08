using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace EncryptEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtKey.MaxLength = 16;
        }
        private static ManualResetEvent mre = new ManualResetEvent(false);
        Thread ProgressBaras1;
        Thread EstimatedTime1;
        Thread EncriptionThread1;
        Thread ProgressBaras2;
        Thread EstimatedTime2;
        Thread DecriptionThread2;
        bool encipher;
        private void btnCipher_Click(object sender, RoutedEventArgs e)
        {
            if(txtKey.Text!="" && txtKey.Text.Length > 15)
            {
                if(txtPath.Text!="")
                {
                    string path = txtPath.Text;
                    string key = txtKey.Text;
                    string Backup = @"BackUp";
                    ProgressBar1.Value = 0;
                    BackEnd.Cryptor.ThirdOfTime = 0;
                    ProgressBar1.Maximum = 100;

                    btnCipher.IsEnabled = false;
                    btnDecipher.IsEnabled = false;
                    btnSelectFolder.IsEnabled = false;

                    btnCancel.IsEnabled = true;
                    btnContinue.IsEnabled = false;
                    btnStop.IsEnabled = true;
                    encipher = true;

                     ProgressBaras1 = new Thread(delegate ()
                    {
                        ProgressBar(0,true);

                    });
                    ProgressBaras1.Start();

                     EstimatedTime1 = new Thread(delegate ()
                    {
                        BackEnd.Cryptor.TimeMeasurment(path, "TempBackup");
                    });
                    EstimatedTime1.Start();
                    

                     EncriptionThread1 = new Thread(delegate ()
                    {
                        BackEnd.Cryptor.Encrypt(path, key, Backup);
                    });
                    EncriptionThread1.Start();
                    
            
                   //System.Windows.MessageBox.Show("Sėkmingai užšifruota", "Sveikiname", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("Nepasirinkote direktorijos", "klaida", MessageBoxButton.OK, MessageBoxImage.Information);
                }
               
            }
            else
            {
                System.Windows.MessageBox.Show("Raktas turi buti sudarytas is 16 simbolių", "klaida", MessageBoxButton.OK, MessageBoxImage.Information);
            }
             

        }

        private void btnDecipher_Click(object sender, RoutedEventArgs e)
        {
            if (txtKey.Text != "" && txtKey.Text.Length>15)
            {
                if (txtPath.Text != "" )
                {
                    string path = txtPath.Text;
                    string key = txtKey.Text;
                    ProgressBar1.Value = 0;
                    BackEnd.Cryptor.HalfTime = 0;
                    ProgressBar1.Maximum = 100;
                    encipher = false;

                    btnCipher.IsEnabled = false;
                    btnDecipher.IsEnabled = false;
                    btnSelectFolder.IsEnabled = false;

                    btnCancel.IsEnabled = true;
                    btnContinue.IsEnabled = false;
                    btnStop.IsEnabled = true;


                    ProgressBaras2 = new Thread(delegate ()
                    {
                        ProgressBar(0,false);

                    });
                    ProgressBaras2.Start();
                    
                     EstimatedTime2 = new Thread(delegate ()
                    {
                        BackEnd.Cryptor.TimeMeasurment(path, "TempBackupas");
                    });
                    EstimatedTime2.Start();

                     DecriptionThread2 = new Thread(delegate ()
                    {
                        BackEnd.Cryptor.Decrypt(path, key);
                    });
                    DecriptionThread2.Start();
                    
                    //System.Windows.MessageBox.Show("Sėkmingai atšifruota", "Sveikiname", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("Nepasirinkote direktorijos", "klaida", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                System.Windows.MessageBox.Show("Raktas turi buti sudarytas is 16 simbolių", "klaida", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtKey_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.lblSymbolCount.Content = this.txtKey.Text.Length.ToString();
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Dialog = new FolderBrowserDialog();
            DialogResult result = Dialog.ShowDialog();
            if (result.ToString() == "OK")
            {
                txtPath.Text = Dialog.SelectedPath;
                
            }
        }
        private void ProgressBar(double third,bool Encryption)
        {

            Stopwatch sw = new Stopwatch();
            bool cicle = true;
            sw.Reset();
            while(cicle==true)
            {
                if (Encryption == true)
                {
                    
                    sw.Start();
                    
                    if (BackEnd.Cryptor.ThirdOfTime == 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            if (ProgressBar1.Value < 30)
                            {
                                ProgressBar1.Value = sw.ElapsedMilliseconds / 200;
                            }

                        });
                    }
                    else if (BackEnd.Cryptor.ThirdOfTime != 0)
                    {
                        this.Dispatcher.Invoke(() =>
                         {
                             if (BackEnd.Cryptor.Execution == true)
                             {
                                 ProgressBar1.Maximum = BackEnd.Cryptor.ThirdOfTime * 100;
                                 ProgressBar1.Value = sw.ElapsedMilliseconds;
                             }
                             else if (BackEnd.Cryptor.Execution == false)
                             {
                                 ProgressBar1.Value = BackEnd.Cryptor.ThirdOfTime * 100;
                                 btnCipher.IsEnabled = true;
                                 btnDecipher.IsEnabled = true;
                                 btnSelectFolder.IsEnabled = true;
                                 btnStop.IsEnabled = false;
                                 btnCancel.IsEnabled = false;
                                 btnContinue.IsEnabled = false;
                                 cicle = false;
                                 sw.Stop();
                             }

                         }
                         );

                    }
                }
                else //Decryption
                {
                    
                    sw.Start();
                      
                        if (BackEnd.Cryptor.HalfTime == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                if (ProgressBar1.Value < 30)
                                {
                                    ProgressBar1.Value = sw.ElapsedMilliseconds / 200;
                                }

                            });
                        }
                        else if (BackEnd.Cryptor.HalfTime != 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                if (BackEnd.Cryptor.Execution == true)
                                {

                                    ProgressBar1.Maximum = BackEnd.Cryptor.HalfTime * 100;
                                    ProgressBar1.Value = sw.ElapsedMilliseconds;
                                }
                                else if (BackEnd.Cryptor.Execution == false)
                                {

                                    ProgressBar1.Value = BackEnd.Cryptor.HalfTime * 100;
                                    btnCipher.IsEnabled = true;
                                    btnDecipher.IsEnabled = true;
                                    btnSelectFolder.IsEnabled = true;
                                    btnStop.IsEnabled = false;
                                    btnCancel.IsEnabled = false;
                                    btnContinue.IsEnabled = false;
                                    cicle = false;
                                    sw.Stop();
                                }

                            }
                             );

                        }
                    
                }
                
                





            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (encipher == true)
            {
                ProgressBaras1.Suspend();

                EncriptionThread1.Suspend();
                btnStop.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnContinue.IsEnabled = true;
            }
            else
            {
                ProgressBaras2.Suspend();

                DecriptionThread2.Suspend();
                btnStop.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnContinue.IsEnabled = true;
            }
            System.Windows.MessageBox.Show("Stopped");
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            if (encipher == true)
            {
                ProgressBaras1.Resume();
                EncriptionThread1.Resume();
                btnStop.IsEnabled = true;
                btnCancel.IsEnabled = true;
            }
            else
            {

                ProgressBaras2.Resume();
                DecriptionThread2.Resume();
                btnStop.IsEnabled = true;
                btnCancel.IsEnabled = true;
            }
            System.Windows.MessageBox.Show("Resumed");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (encipher == true)
            {
                ProgressBaras1.Abort();
                EncriptionThread1.Abort();
                btnStop.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnContinue.IsEnabled = false;
                ProgressBar1.Value = 0;
                BackEnd.Cryptor.HalfTime = 0;
                BackEnd.Cryptor.ThirdOfTime = 0;
                ProgressBar1.Maximum = 100;
                btnCipher.IsEnabled = true;
                btnDecipher.IsEnabled = true;
            }
            else
            {

                ProgressBaras2.Abort();
                DecriptionThread2.Abort();
                btnStop.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnContinue.IsEnabled = false;
                ProgressBar1.Value = 0;
                BackEnd.Cryptor.HalfTime = 0;
                BackEnd.Cryptor.ThirdOfTime = 0;
                ProgressBar1.Maximum = 100;
                btnCipher.IsEnabled = true;
                btnDecipher.IsEnabled = true;
            }
            System.Windows.MessageBox.Show("Cancelled");
        }
    }
}
