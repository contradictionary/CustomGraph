using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Net;
using System.Text;

namespace ChartFinal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {


        static System.Windows.Data.Binding indi, dep;
        static Timer RefreshCounter,timerFTP;
        static ObservableCollection<GraphDataModel>[] itemSourceBDS;
        static string InFile;
        private static bool flag;
        private static int reftime = 10;
        static int timeLeftSecs = reftime*60;
        static string[] ftpsettings;
        static string[] queues;
        public MainWindow()
        {
            DateTime ExpiryDate = new DateTime(2016, 12, 1);

            if (DateTime.Now > ExpiryDate)
            {
                MessageBox.Show(this, "The Application has Expired!!\nKindly contact developer for More Info.", "Application Expired", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

            }
            else
            {
                #region Timers
                //Timer  :  To update the Data with specific time interval 
                Timer timer = new System.Timers.Timer();
                timer.Interval = reftime * 60 * 1000 + 5000;
                timer.Elapsed += timer_Elapsed;
                timer.Start();

                //Timer  RefreshCounter
                RefreshCounter = new System.Timers.Timer();
                RefreshCounter.Interval = 5000;
                RefreshCounter.Elapsed += RefreshCounter_Elapsed;
                RefreshCounter.Start();

                //Timer  FTP Timer 

                timerFTP = new System.Timers.Timer();
                timerFTP.Interval = reftime * 60 * 1000;
                timerFTP.Elapsed += timerFTP_Elapsed;
                timerFTP.Start();

               
                try
                {
                    ftpsettings = ReadCSV.getDataFromFile(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.conf")[0], ";");
                    //set title
                    if (ftpsettings[5]!=null)
                    {
                       this. Title = "Graph Monitoring For "+ ftpsettings[5]+" Application";
                    }

                    GetFileFromFTP
                                   (
                                   ftpsettings[0],
                                   AppDomain.CurrentDomain.BaseDirectory,
                                   ftpsettings[1],
                                   ftpsettings[2],
                                   ftpsettings[3],
                                   ftpsettings[4]
                                   );
                }
                catch (Exception ftpex)
                {
                   
                    MessageBox.Show(ftpex.Message, "FTP Error");
                }
                try
                {
                    InFile = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.log")[0];
                }
                catch (Exception pex)
                {

                    MessageBox.Show(pex.Message, "Local file Error");
                }
                #endregion

                InitializeComponent();

                //read Queue names from "*.q" file 
                try
                {
                   queues = ReadCSV.getDataFromFile(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.q")[0],";");
                    
                }
                catch (Exception)
                {

                    throw;
                }

               
                try
                {
                    MainFunct();
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Message : " + ex.Message + "\nStack trace :-\n\n" + ex.StackTrace, "Runtime Error( MainFunct())");
                   // MessageBox.Show(this, "Application will now Exit", "Error");

                    //this.Close();
                }

               

                DisplayQueueSelectors();

            }
        }

        private void DisplayQueueSelectors()
        {
            foreach (LineSeries item in ItsAchart.Series)
            {
                CheckBox cb = new CheckBox();
                cb.Content = item.Title;
                cb.IsChecked = true;  
                cb.Checked += CheckBox_Checked;
                cb.Unchecked += CheckBox_unChecked;
                queueselector.Children.Add(cb);
            }
        }

       

        ///<summary>
        ///It will clear and Intialize our main data object (i.e.itemSourceBDS[]) which we are going to bind to the chart
        ///</summary>
        /// 
        private void InitItemSource()
        {
                             
         
          
            itemSourceBDS = new ObservableCollection<GraphDataModel>[queues.Length];

            for (int i = 0; i < queues.Length; i++)
            {
                itemSourceBDS[i] = new ObservableCollection<GraphDataModel>();
            }

        }

        private void timerFTP_Elapsed(object sender, ElapsedEventArgs e)
        {
          
                try
                {
                GetFileFromFTP
           (
           ftpsettings[0],
           AppDomain.CurrentDomain.BaseDirectory,
           ftpsettings[1],
           ftpsettings[2],
           ftpsettings[3],
           ftpsettings[4]
           );
            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"FTP Error");
                }

           
        }

        private void RefreshCounter_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timeLeftSecs > 0) { 
                this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    timeLeftSecs = timeLeftSecs - 5;
                    lable1.Content = "Refresh in - " + new TimeSpan(0, 0, timeLeftSecs).ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }

            }));
            }
            else
            {
                //  timerFTP = new System.Timers.Timer();
                RefreshCounter.Start();
                timeLeftSecs = reftime * 60;
            }
           

        }

        private static void GetFileFromFTP(string ip, string lcd, string remotedir, string user, string pass,string fname)
        {
            string filename = fname + DateTime.Now.ToString("ddMMyyy") + ".log";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://"+ip+ remotedir + filename);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

       
            request.Credentials = new NetworkCredential(user, pass);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + filename, reader.ReadToEnd());
            
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Ftp"+ DateTime.Now.ToString("ddMMyyyy")+".log", ""+DateTime.Now.ToShortTimeString()+"=\nDownload Complete;\nFileName : "+filename+";\nFTP Path : "+remotedir+";\nFTP Server:"+ip+",;\nStatus " + response.StatusDescription, Encoding.ASCII);
              

            reader.Close();
            response.Close();

        }


        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    MainFunct();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }

            }));
        }

        private void InitAxeses()
        {
            ItsAchart.Axes.Clear();
            DateTimeAxis dta = new DateTimeAxis();
         
            dta.IntervalType = DateTimeIntervalType.Minutes;
            dta.Title = "Time";
            dta.Orientation = AxisOrientation.X;
            dta.ShowGridLines = true;
             dta.Minimum = itemSourceBDS.Min(r => r.Min(t => t.Time));
             dta.Maximum = itemSourceBDS.Max(r => r.Max(t => t.Time)); ;
            LinearAxis yaxis = new LinearAxis();
            yaxis.Title = "Case";
            yaxis.Orientation = AxisOrientation.Y;
            yaxis.ShowGridLines = true; 
            ItsAchart.Axes.Clear();
            ItsAchart.Axes.Add(dta);
            ItsAchart.Axes.Add(yaxis);
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFunct();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace,ex.Message);
            }

        }

        private void InitSerieses(string[] queueNames)
        {
            ItsAchart.Series.Clear();

            for (int i = 0; i < queueNames.Length; i++)
            {
                LineSeries lineseries = new LineSeries();

                if (queueNames[i].Equals("Ready4ImageRepository"))
                {
                    lineseries.Title = "Ready4ImgRep";
                }
                else
                {
                    lineseries.Title = queueNames[i];
                }
                
                indi = new System.Windows.Data.Binding("Case");
                dep = new System.Windows.Data.Binding("Time");
                lineseries.IndependentValueBinding = dep;
                lineseries.DependentValueBinding = indi;
                lineseries.ItemsSource = itemSourceBDS[i];
                ItsAchart.Series.Add(lineseries);

            }





            //   Ready4SMESeries.ItemsSource = Ready4SMESeriesBDS;



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int flag1;

            if (recordtype.Text.Equals("Current"))
            {
                flag1 = 1;

                Console.WriteLine("Flag set to 1 as values matched as 'Current' for RecordType");
            }
           
            if (recordtype.Text.Equals("Processed"))
            {
                flag1 = 2;
                Console.WriteLine("Flag set to 2 as values  matched as 'Processed' for RecordType");
            }
            else
            {
                flag1 = 1;
                Console.WriteLine("Flag set to 1 as no values matched for RecordType");
            }
            try
            {
                DateTime now = new DateTime();
                foreach (var item in itemSourceBDS)
                {
                    now= item.Min(r => r.Time);
                    break;
                }
                DateTime fromTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(FromHours.Text), int.Parse(FromMinutes.Text), 0);
                DateTime toTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(ToHours.Text), int.Parse(Tominutes.Text), 0);

                InitItemSource();
                LoadSeriesData(fromTime, toTime, flag1);
                InitSerieses(queues);
                InitAxeses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
             
            }

        }

        private void LoadSeriesData(DateTime fromTime, DateTime toTime, int optn)
        {
            switch (optn)
            {
                case 1:  //for current count and current time

                                                for (int i = 0; i < queues.Length; i++)
                                                {

                                                    ObservableCollection<APISTATDataModel> data = APISTATDataModel.GetFromFile(InFile, new string[] { "|" }, queues[i]);

                                                    var filteredData = data.Where(d => d.Current_Time >= fromTime).Where(s => s.Current_Time <= toTime);

                                                    foreach (var item in filteredData)
                                                    {

                                                        itemSourceBDS[i].Add(new GraphDataModel() { Case = item.Current_Count - item.Suspend_Count, Time = item.Current_Time });

                                                    } /// foreach loop  

                                                }   /// for loop  

                    break;

                    case 2: //for decrease count  in current time field // will show processed cases.


                                                for (int i = 0; i < queues.Length; i++)
                                                {

                                                    ObservableCollection<APISTATDataModel> data = APISTATDataModel.GetFromFile(InFile, new string[] { "|" }, queues[i]);

                                                    var filteredData = data.Where(d => d.Current_Time >= fromTime).Where(s => s.Current_Time <= toTime);

                                                    foreach (var item in filteredData)
                                                    {

                                                        itemSourceBDS[i].Add(new GraphDataModel() { Case = item.Decrease_Count, Time = item.Current_Time });

                                                    } /// foreach loop  

                                                }   /// for loop

                    break;

                default:
                    throw new NotImplementedException();

            }
        }
        private void LoadSeriesData(int lastn, int optn,string[] queues)
        {
           

            switch (optn)
            {
                case 1:  //for current count and current time


                    for (int i = 0; i < queues.Length; i++)
                    {
                        ObservableCollection<APISTATDataModel> data = APISTATDataModel.GetFromFile(InFile, new string[] { "|" }, lastn, queues[i]);
                        foreach (var item in data)
                        {

                            itemSourceBDS[i].Add(new GraphDataModel() { Case = item.Current_Count - item.Suspend_Count, Time = item.Current_Time });

                        } /// foreach loop  

                    }   /// for loop  

                    break;

                case 2: // for Decrease count and Current_Time
                    for (int i = 0; i < queues.Length; i++)
                    {
                        ObservableCollection<APISTATDataModel> data = APISTATDataModel.GetFromFile(InFile, new string[] { "|" }, lastn, queues[i]);
                        foreach (var item in data)
                        {

                            itemSourceBDS[i].Add(new GraphDataModel() { Case = item.Decrease_Count, Time = item.Current_Time });

                        } /// foreach loop  

                    }   /// for loop  
                    break;
         
                default:
                    throw new NotImplementedException();

            }


        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveFileDialog1.Title = "Save an Image File";
                saveFileDialog1.ShowDialog();
                string filepath = saveFileDialog1.FileName;


                FileStream fs = new FileStream(filepath, FileMode.Create);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)ItsAchart.ActualWidth,
                                        (int)ItsAchart.ActualHeight,
                                        96,
                                        96,
                                        PixelFormats.Default);
                rtb.Render(ItsAchart);

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(fs);
                fs.Flush();
                fs.Close();
                MessageBox.Show("File saved to : " + filepath, "Success");

            }
            catch (Exception fex)
            {
                MessageBox.Show(fex.Message, "File save Error");

            }
           

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var s = sender as CheckBox;
            foreach (LineSeries item in ItsAchart.Series)
            {
                if (item.Title.Equals(s.Content.ToString()))
                {
                    item.Visibility = Visibility.Visible;
                }
   
            }
        }
        private void CheckBox_unChecked(object sender, RoutedEventArgs e)
        {
            var s = sender as CheckBox;
            foreach (LineSeries item in ItsAchart.Series)
            {
                if (item.Title.Equals(s.Content.ToString()))
                {
                    item.Visibility = Visibility.Hidden;
                }




            }
        }
        

        public void MainFunct()
        {
            InitItemSource();
            LoadSeriesData(12,1,queues);
            InitSerieses(queues);
            InitAxeses();
        }

       
    }
}

