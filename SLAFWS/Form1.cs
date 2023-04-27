
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using MapSurfer.IO;
using MapSurfer.IO.FileTypes;
using MapSurfer.Geometries;
using MapSurfer.Reflection;
using MapSurfer.Utilities;
using MapSurfer.Windows.Forms;
using MapSurfer.Rendering;
using System.IO;
using MapSurfer;
using MapSurfer.Styling.Formats.CartoCSS;
using ProjNet.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.CoordinateSystems;
using MapSurfer.Styling;

namespace SLAFWS
{
    public partial class Form1 : Form
    {
        private MapViewer m_mapViewer;
        private Renderer m_renderer;
        // BALParticipants oBALParticipants = new BALParticipants();
        // CMNParticipants oCMNParticipants = new CMNParticipants();
        const int historyLength = 256;
        float[] history = new float[historyLength];
        float temp = 0;
        float humi = 0;
        float pressu = 0;
        float rain = 0;
        float wind = 0;
        float light = 0;
        int nextWrite = 0;
        Graphics bmg;
        Bitmap bm;
        int a = 1;
        int b = 2;
        int c = 0;
        double[] seriesArray= {0 } ;
        double[] seriesArray1 = { 0 };
        double[] seriesArray2 = { 0 };
        double[] seriesArray3 = { 0 };
        double[] seriesArray4 = { 0 };
        double[] seriesArray5 = { 0 };
        public string indata;
        public string message;
        public string Latitude;
        public string Longitude;
        public string gpsdate;
        public string gpssat;
        public string gpsalt;
        public string wins;
        public string batlvl;
        string cardID = "";
        delegate void SetTextCallback(string text);
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        Thread thread;
        public Form1()
        {
            try
            {
                InitializeComponent();
                CoreUtility.Initialize();
                CoreUtility.CheckAndFixCulture();

                m_renderer = (MapSurfer.Rendering.Renderer)MapSurfer.Rendering.RendererManager.CreateDefaultInstance();

                m_mapViewer = new MapViewer();
                m_mapViewer.Dock = DockStyle.None;
                m_mapViewer.Location = new System.Drawing.Point(45, 15);
                m_mapViewer.Size = new Size(300, 400);
                m_mapViewer.RedrawOnAttach = true;
                m_mapViewer.ActiveTool = MapTool.Zoom;
               

                m_mapViewer.RedrawOnResizing = true;
                this.Controls.Add(m_mapViewer);

                // Initialize file types
                //FileTypeManager<Map> ftmMap = FileTypeManagerCache.GetFileTypeManager<Map>();
                //ftmMap.AddSearchPath(typeof(Map).Assembly.GetLocation());
                //ftmMap.AddSearchPath(MSNEnvironment.GetFolderPath(MSNSpecialFolder.StylingFormats));
                //ftmMap.InitializeFileTypes();

                // Load map project
                //string fileName = Path.Combine(MSNUtility.GetMSNInstallPath(), @"Samples\Projects\Bremen.msnpx");
                //if (File.Exists(fileName))
                //{
                //    LoadProject(fileName);

                //    MapSurfer.Geometries.Envelope env = new MapSurfer.Geometries.Envelope(978779.133679862, 6990983.0938755, 990289.718456316, 6997278.67914107); //map.Extent
                //    m_mapViewer.ZoomToEnvelope(env);
                //}
                LoadProject("C:\\Users\\SLAF\\Downloads\\Compressed\\MapSurfer.NET-Examples-master\\MapSurfer.NET-Examples-master\\Projects\\Bremen.msnpx");
                MapSurfer.Geometries.Envelope env1 = new MapSurfer.Geometries.Envelope(978779.133679862, 6990983.0938755, 990289.718456316, 6997278.67914107); //map.Extent
               // Rectangle re = new Rectangle(9, 6, 9, 6);
                m_mapViewer.ZoomToEnvelope(env1);
                //GeoAPI.Geometries.Coordinate PinPnt = new GeoAPI.Geometries.Coordinate();
                //PinPnt.X = 12;
                //PinPnt.Y = 16;
                //PinPnt.Z = 8;
                //m_mapViewer.Map.SetCenter(PinPnt);
              
               
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void LoadProject(string fileName)
        {try { 
            Map map = Map.FromFile(fileName);

            System.Drawing.Point[] myArray =
            {
                 new System.Drawing.Point(20, 20),
                 new System.Drawing.Point(120, 20),
                 new System.Drawing.Point(120, 120),
                 new System.Drawing.Point(20, 120),
                 new System.Drawing.Point(20,20)
             };
           
          
            map.Initialize(Path.GetDirectoryName(fileName), true);
            //CartoPlacemark df = new CartoPlacemark();
            //df.X = 12;
            //df.Y = 20;
           
            m_mapViewer.Map = map;
          
            m_mapViewer.ZoomToFullExtent();
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
}
        public void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[2048];

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(200);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    //   Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine(e.ToString());
            }

            //  Console.WriteLine("\nPress ENTER to continue...");
            //  Console.Read();

        }
        
        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {

                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.Length < 2000)
                {
                    // All the data has been read from the   
                    // client. Display it on the console. 

                    var sb = new StringBuilder(content.Length);

                    foreach (char i in content)
                        if (i != '\n' && i != '\r' && i != '\t')
                            sb.Append(i);

                    content = sb.ToString();
                    string[] lineArr = content.Split(',');
                   

                        try
                        {
                        //Latitude

                        Longitude  = lineArr[0].ToString();

                        //Longitude

                        Latitude = lineArr[1].ToString();
                        temp = (float)Convert.ToDouble(lineArr[15]);
                        humi = (float)Convert.ToDouble(lineArr[14]);
                        pressu = (float)Convert.ToDouble(lineArr[20]);
                        rain = (float)Convert.ToDouble(lineArr[16]);
                        wind = (float)Convert.ToDouble(lineArr[6]);
                        light = (float)Convert.ToDouble(lineArr[22]);
                        gpsdate = lineArr[4].ToString()+":"+ lineArr[5].ToString();
                        gpsalt= lineArr[2].ToString();
                        gpssat= lineArr[3].ToString();
                        wins = lineArr[7].ToString();
                        batlvl = lineArr[21].ToString();
                        seriesArray[0] = temp;
                        seriesArray1[0] = humi;
                        seriesArray2[0] = pressu;
                        seriesArray3[0] = rain;
                        seriesArray4[0] = wind;
                        seriesArray5[0] = light;
                        //Display
                        if (txtLat.InvokeRequired)
                        {
                            txtLat.Invoke(new MethodInvoker(delegate { txtLat.Text = Latitude; }));
                        }
                        if (txtLong.InvokeRequired)
                        {
                            txtLong.Invoke(new MethodInvoker(delegate { txtLong.Text = Longitude; }));
                        }
                        if (label1.InvokeRequired)
                        {
                            label1.Invoke(new MethodInvoker(delegate { label1.Text = temp.ToString(); }));
                        }
                        if (label2.InvokeRequired)
                        {
                            label2.Invoke(new MethodInvoker(delegate { label2.Text = humi.ToString(); }));
                        }
                        if (label3.InvokeRequired)
                        {
                            label3.Invoke(new MethodInvoker(delegate { label3.Text = pressu.ToString(); }));
                        }
                        if (label6.InvokeRequired)
                        {
                            label6.Invoke(new MethodInvoker(delegate { label6.Text = rain.ToString(); }));
                        }
                        if (label10.InvokeRequired)
                        {
                            label10.Invoke(new MethodInvoker(delegate { label10.Text = wind.ToString(); }));
                        }
                        if (label14.InvokeRequired)
                        {
                            label14.Invoke(new MethodInvoker(delegate { label14.Text = gpsdate.ToString(); }));
                        }
                        if (label15.InvokeRequired)
                        {
                            label15.Invoke(new MethodInvoker(delegate { label15.Text = gpssat.ToString(); }));
                        }
                        if (label16.InvokeRequired)
                        {
                            label16.Invoke(new MethodInvoker(delegate { label16.Text = gpsalt.ToString(); }));
                        }
                        if (label17.InvokeRequired)
                        {
                            label17.Invoke(new MethodInvoker(delegate { label17.Text = wins.ToString(); }));
                        }
                        if (label18.InvokeRequired)
                        {
                            label18.Invoke(new MethodInvoker(delegate { label18.Text = light.ToString(); }));
                        }
                        if (label19.InvokeRequired)
                        {
                            label19.Invoke(new MethodInvoker(delegate { label19.Text = batlvl.ToString(); }));
                        }
                       

                    }
                        catch(Exception ex)
                        {
                          
                            
                        }
                    
                    if (lblCardID.InvokeRequired)
                    {
                        lblCardID.Invoke(new MethodInvoker(delegate { lblCardID.Text = content; }));
                    }

                    // Echo the data back to the client.  
                    //Send(handler, content);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    c++;
                }

                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
    

    
        public void frmMap(string lat, string lon)
        {
            

           
            try
            {
                StringBuilder queryAddress = new StringBuilder();
                queryAddress.Append("http://maps.google.com/maps?q=");

                if (lat != string.Empty)
                {
                    queryAddress.Append(lat + "%2C");
                }

                if (lon != string.Empty)
                {
                    queryAddress.Append(lon);
                }

              //  webBrowser1.Navigate(queryAddress.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            thread = new Thread(StartListening);
            thread.Start();
            frmMap(Latitude, Longitude);
            this.chart1.Palette = ChartColorPalette.BrightPastel;
           
            // Set title.
            this.chart1.Titles.Add("Temperature");

            // Add series
            this.chart1.Series.Clear();
            for (int i = 0; i < seriesArray.Length; i++)
            {
                chart1.Series.Add(seriesArray[i].ToString());
            }
            this.chart2.Palette = ChartColorPalette.EarthTones;

            // Set title.
            this.chart2.Titles.Add("Humidity");

            // Add series
            this.chart2.Series.Clear();
            for (int i = 0; i < seriesArray1.Length; i++)
            {
                chart2.Series.Add(seriesArray1[i].ToString());
            }
            this.chart3.Palette = ChartColorPalette.Berry;

            // Set title.
            this.chart3.Titles.Add("Pressure");

            // Add series
            this.chart3.Series.Clear();
            for (int i = 0; i < seriesArray2.Length; i++)
            {
                chart3.Series.Add(seriesArray2[i].ToString());
            }
            this.chart4.Palette = ChartColorPalette.Berry;

            // Set title.
            this.chart4.Titles.Add("Rain");

            // Add series
            this.chart4.Series.Clear();
            for (int i = 0; i < seriesArray3.Length; i++)
            {
                chart4.Series.Add(seriesArray3[i].ToString());
            }
            this.chart5.Palette = ChartColorPalette.Berry;

            // Set title.
            this.chart5.Titles.Add("Wind Direction");

            // Add series
            this.chart5.Series.Clear();
            for (int i = 0; i < seriesArray4.Length; i++)
            {
                chart5.Series.Add(seriesArray4[i].ToString());
            }
            this.chart6.Palette = ChartColorPalette.Berry;

            // Set title.
            this.chart6.Titles.Add("Light Level");

            // Add series
            this.chart6.Series.Clear();
            for (int i = 0; i < seriesArray5.Length; i++)
            {
                chart6.Series.Add(seriesArray5[i].ToString());
            }
            // hook up timer event
            timer1.Tick += timer1_Tick;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            a++;
            b++;

            // Data array
            int[] pointsArray = { a, b };

            for (int i = 0; i < seriesArray.Length; i++)
            {
                // Add point.
                this.chart1.Series[i].ChartType = SeriesChartType.Point;
                chart1.Series[i].Points.Add(seriesArray[i]);
                
            }
            for (int i = 0; i < seriesArray1.Length; i++)
            {
                // Add point.
                chart2.Series[i].Points.Add(seriesArray1[i]);
            }
            for (int i = 0; i < seriesArray2.Length; i++)
            {
                // Add point.
                this.chart3.Series[i].ChartType = SeriesChartType.StepLine;
                chart3.Series[i].Points.Add(seriesArray2[i]);

            }
            for (int i = 0; i < seriesArray3.Length; i++)
            {
                // Add point.
                this.chart4.Series[i].ChartType = SeriesChartType.StepLine;
                chart4.Series[i].Points.Add(seriesArray3[i]);

            }
           
            Series s1 = chart5.Series[0];
            s1.ChartType = SeriesChartType.Polar;
            s1.CustomProperties="PixelPointWidth=20";
            s1.Points.Clear();
            for (int i = 0; i <= 360; i+=15)
            {
               
                s1.Points.AddXY( seriesArray4[0],i);
            }
            for (int i = 0; i < seriesArray5.Length; i++)
            {
                // Add point.
                this.chart6.Series[i].ChartType = SeriesChartType.Renko;
                chart6.Series[i].Points.Add(seriesArray5[i]);

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
