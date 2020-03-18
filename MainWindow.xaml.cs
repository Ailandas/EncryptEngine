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
        Thread ProgressBaras1;
        Thread EstimatedTime1;
        Thread EncriptionThread1;
        Thread ProgressBaras2;
        Thread EstimatedTime2;
        Thread DecriptionThread2;
        bool encipher;
        CancellationTokenSource cancellationTokenSource=new CancellationTokenSource( );
        public delegate bool IsPaused();
        public bool Paused = false;
        private void btnCipher_Click(object sender, RoutedEventArgs e)
        {
      

            if(txtKey.Text!="" && txtKey.Text.Length > 15)
            {
                if(txtPath.Text!="")
                {
                    if (Directory.Exists(txtPath.Text))
                    {
                        BackEnd.Cryptor.Cancel(); //Pasalina aplankalus senus pradedant

                        Paused = false;
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
                        btnStopnPlay.IsEnabled = true;
                        encipher = true;

                        BackEnd.Cryptor.Execution = false;
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = new CancellationTokenSource();


                        IsPaused DelegatePause = this.IsPausedMRE;

                        ProgressBaras1 = new Thread(delegate ()
                        {

                            ProgressBar(true, cancellationTokenSource.Token, DelegatePause);

                        });
                        ProgressBaras1.IsBackground = true;
                        ProgressBaras1.Start();

                        EstimatedTime1 = new Thread(delegate ()
                       {

                           BackEnd.Cryptor.TimeMeasurment(path);
                       });
                        EstimatedTime1.IsBackground = true;
                        EstimatedTime1.Start();


                        EncriptionThread1 = new Thread(delegate ()
                       {

                           BackEnd.Cryptor.Execution = true;
                           BackEnd.Cryptor.Encrypt(path, key, Backup, cancellationTokenSource.Token, DelegatePause);
                           BackEnd.Cryptor.Execution = false;

                       });
                        EncriptionThread1.IsBackground = true;
                        EncriptionThread1.Start();

                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Tokia direktorija neegzistuoja", "Klaida", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                   
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
                    if (Directory.Exists(txtPath.Text))
                    {
                        BackEnd.Cryptor.Cancel(); //Pasalina aplankalus senus pradedant

                        Paused = false;
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
                        btnStopnPlay.IsEnabled = true;

                        BackEnd.Cryptor.Execution = false;
                        cancellationTokenSource.Dispose();
                        cancellationTokenSource = new CancellationTokenSource();

                        IsPaused DelegatePause = this.IsPausedMRE;

                        ProgressBaras2 = new Thread(delegate ()
                        {

                            ProgressBar(false, cancellationTokenSource.Token, DelegatePause);

                        });
                        ProgressBaras2.IsBackground = true;

                        ProgressBaras2.Start();

                        EstimatedTime2 = new Thread(delegate ()
                       {

                           BackEnd.Cryptor.TimeMeasurment(path);
                       });
                        EstimatedTime2.IsBackground = true;
                        EstimatedTime2.Start();

                        DecriptionThread2 = new Thread(delegate ()
                       {
                           BackEnd.Cryptor.Execution = true;
                           BackEnd.Cryptor.Decrypt(path, key, cancellationTokenSource.Token, DelegatePause);
                           BackEnd.Cryptor.Execution = false;

                       });
                        DecriptionThread2.IsBackground = true;
                        DecriptionThread2.Start();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Tokia direktorija neegzistuoja", "Klaida", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
        private void ProgressBar(bool Encryption,CancellationToken cancelToken,Delegate DelegatePause)
        {

            Stopwatch sw = new Stopwatch();
            bool cicle = true;
            sw.Reset();
            while(cicle==true && cancelToken.IsCancellationRequested!=true)
            {
                bool wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                sw.Stop();
                while (wait == true)
                {
                    wait = Convert.ToBoolean(DelegatePause.DynamicInvoke());
                }
                sw.Start();
                if (Encryption == true)
                {
                   
                    sw.Start();
                    
                    if (BackEnd.Cryptor.ThirdOfTime == 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            if (ProgressBar1.Value < 30)
                            {
                                ProgressBar1.Value = sw.ElapsedMilliseconds;
                            }

                        });
                    }
                    else if (BackEnd.Cryptor.ThirdOfTime != 0)
                    {
                        this.Dispatcher.Invoke(() =>
                         {
                             if (BackEnd.Cryptor.Execution == true)
                             {
                                 ProgressBar1.Maximum = BackEnd.Cryptor.ThirdOfTime;
                                 ProgressBar1.Value = sw.ElapsedMilliseconds*8000;
                             }
                             else if (BackEnd.Cryptor.Execution == false)
                             {
                                 ProgressBar1.Value = BackEnd.Cryptor.ThirdOfTime;
                                 btnCipher.IsEnabled = true;
                                 btnDecipher.IsEnabled = true;
                                 btnSelectFolder.IsEnabled = true;
                                 btnStopnPlay.IsEnabled = false;
                                 btnCancel.IsEnabled = false;
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
                                    ProgressBar1.Value = sw.ElapsedMilliseconds;
                                }

                            });
                        }
                        else if (BackEnd.Cryptor.HalfTime != 0)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                if (BackEnd.Cryptor.Execution == true)
                                {

                                    ProgressBar1.Maximum = BackEnd.Cryptor.HalfTime;
                                    ProgressBar1.Value = sw.ElapsedMilliseconds*8000;
                                }
                                else if (BackEnd.Cryptor.Execution == false)
                                {

                                    ProgressBar1.Value = BackEnd.Cryptor.HalfTime;
                                    btnCipher.IsEnabled = true;
                                    btnDecipher.IsEnabled = true;
                                    btnSelectFolder.IsEnabled = true;
                                    btnStopnPlay.IsEnabled = false;
                                    btnCancel.IsEnabled = false;

                                    cicle = false;
                                    sw.Stop();
                                  
                                    

                                }

                            }
                             );

                        }
                }
            }
        }

  

    

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            btnDecipher.IsEnabled = true;
            btnCipher.IsEnabled = true;
            btnStopnPlay.IsEnabled = false;
            btnSelectFolder.IsEnabled = true;
            cancellationTokenSource.Cancel();
            Thread th = new Thread(BackEnd.Cryptor.Cancel);
            th.Start();
            System.Windows.MessageBox.Show("Cancelled");
            
        }
        public bool IsPausedMRE()
        {
            if(Paused==true)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private void btnStopnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(Paused==false)
            {
                Paused = true;
                System.Windows.MessageBox.Show("Sustabdyta");
            }
           else if(Paused==true)
            {
                Paused = false;
                System.Windows.MessageBox.Show("Pratesta");
               
            }
        }
    }
}
