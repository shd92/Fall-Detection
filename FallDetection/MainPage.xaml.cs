using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CppSensors;
using Windows.UI.Core;
using System.Numerics;
using libsound;
using Windows.Phone.Devices.Notification;
using VibrateAlarm;
using SoundAlarm;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using VisualizationTools;

namespace FallDetection
{

    public sealed partial class MainPage : Page
    {

        //Variables used for detecting fall
        private CppAccGyro myAccGyro;
        double AccMag;
        double GyroMag;
        double FallLowThresholdAcc = 0.1; //Low threshold to detect start of fall
        double myMinAccMag = 50;
        double myMaxAccMag = -10;
        double myMaxGyroMag = -10;
        double maxTemp;
        DispatcherTimer timer; //will be used to time 0.5 seconds after fall
        int timerCount;
        double FallHighThresholdGyro = 800; //high threshold to detect impact force
        double FallHighThresholdAcc = 2.7; //high treshold to detect impact force
        bool fallDetected;

        //Variables to record fall information
        float[] myGyroRecording;
        private LineGraph myGyroGraph;
        float[] myAccRecording;
        private LineGraph myAccGraph;
        bool drawGraph = false;
        

        //Variables for use AFTER detection of fall
        Sound sound; //Initiate Sound Alarm
        Vibrator vibrate; //Initiate Vibrate Alarm
        AreYouOk askOk; //Initiates .xaml to be popped up
        Popup prompt; //Initiates a pop up prompt
        Geolocator geolocator; //Initiates a geolocator to determine location
        string lat;
        string lon;
             

        private Object lockKey = new Object();


        public MainPage()
        {
            this.InitializeComponent() ;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            //For use of detection of fall
            this.NavigationCacheMode = NavigationCacheMode.Required;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);

            //For use of recording graph of fall
            
            //Gyroscope Graphing tools
            myGyroGraph = new VisualizationTools.LineGraph((int)GyroGraph_XAML.Width, (int)GyroGraph_XAML.Height); //Fall Event Recording
            myGyroGraph.setYLim(-1, 1500);
            GyroGraph_XAML.Source = myGyroGraph;
            //Accelerometer Graphing tools
            myAccGraph = new VisualizationTools.LineGraph((int)AccGraph_XAML.Width, (int)AccGraph_XAML.Height); //Fall Event Recording
            myAccGraph.setYLim(-1, 10);
            AccGraph_XAML.Source = myAccGraph;

            //For use AFTER detection of fall
            sound = new Sound();
            vibrate = new Vibrator();
            askOk = new AreYouOk();
            prompt = new Popup();
            prompt.Height = 200;
            prompt.Width = 400;
            prompt.VerticalOffset = 100;
            prompt.Child = askOk;
            geolocator = new Geolocator();

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            askOk.ClickedCancel += (s, ergs) =>
            {
                prompt.IsOpen = false;
                sound.stop();
                vibrate.stop();
            };

            askOk.ClickedHelp += (s, ergs) =>
            {
                prompt.IsOpen = false;
                sound.stop();
                vibrate.stop();

                //Send alarm text to contact list
                ComposeSms();
            };

            timer.Tick += (object sender, object eb) =>
            {
                timerCount++;

                if(timerCount == 10)
                {
                    if(drawGraph)
                    {
                        myGyroGraph.setArray(myGyroRecording);
                        myAccGraph.setArray(myAccRecording);
                        drawGraph = false;
                    }
                    timer.Stop();
                }
            }; 

            //For drawing graphs
            CompositionTarget.Rendering += myGyroGraph.Render;
            CompositionTarget.Rendering += myAccGraph.Render;


        }

        //Start or reset Accelerometer 
        public void startStopButton_Click(object sender, RoutedEventArgs e)
        {

            if (startStopButton.Content.Equals("Turn Fall Detection On"))
            {
                startStopButton.Content = "Turn Fall Detection Off";

                myAccGyro = new CppAccGyro();
                myMinAccMag = 100;
                myMaxAccMag = -5;
                myMaxGyroMag = -5;
                maxTemp = -50;

                myGyroRecording = new float[300];
                myAccRecording = new float[300];
                int index = 0;


                //Detect if Acc mag  falls below FallThreshold (may be the fall happening)
                myAccGyro.onReadingChanged += (double aX, double aY, double aZ, double gX, double gY, double gZ) =>
                {
                    //Calculate magnitude of accelerometer
                    AccMag = Math.Sqrt(Math.Pow(aX, 2) + Math.Pow(aY, 2) + Math.Pow(aZ, 2));

                    //Calculate magnitude of gyroscope
                    GyroMag = Math.Sqrt(Math.Pow(gX, 2) + Math.Pow(gY, 2) + Math.Pow(gZ, 2));

                    if(fallDetected)
                    {
                        myGyroRecording[index] = (float)GyroMag;
                        myAccRecording[index] = (float)AccMag;
                        index++;
                    }

                    if (!fallDetected)
                    {

                        if (AccMag <= myMinAccMag)
                        {
                            myMinAccMag = AccMag;
                        }

                        if(GyroMag >= maxTemp)
                        {
                            maxTemp = GyroMag;
                        }

                        lock (lockKey)
                        {
                            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {
                                this.aMag.Text = "myAcc Magnitude: " + Math.Round(AccMag, 3) + " Min = " + Math.Round(myMinAccMag, 3);
                                this.gMag.Text = "myGyro Magnitude: " + Math.Round(GyroMag, 3) + "Max = " + Math.Round(maxTemp, 3);

                                //Check to see if Fall was detected
                                if (AccMag <= FallLowThresholdAcc)
                                {
                                    fallDetected = true;
                                    StartFall.Text = "Starting";
                                    timerCount = 0;
                                    timer.Start();

                                }
                            });
                        }
                    }
                    else if (fallDetected && timerCount > 1 && timerCount < 10)
                    {
                        

                        if (AccMag > myMaxAccMag)
                        {
                            myMaxAccMag = AccMag;
                        }

                        if (GyroMag > myMaxGyroMag)
                        {
                            myMaxGyroMag = GyroMag;
                        }


                        lock (lockKey)
                        {
                            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {

                                this.aMag.Text = "myAcc Magnitude: " + Math.Round(AccMag, 3) + " Max = " + Math.Round(myMaxAccMag, 3);
                                this.gMag.Text = "myGyro Magnitude: " + Math.Round(GyroMag, 3) + " Max = " + Math.Round(myMaxGyroMag, 3);
                                this.fallCount.Text = " " + timerCount;

                                if (myMaxGyroMag >= FallHighThresholdGyro && myMaxAccMag >= FallHighThresholdAcc)
                                {
                                    fallDetected = false;
                                    drawGraph = true;
                                    
                                    sound.start(); //Alarm with sound
                                    vibrate.start(); //Alarm with Vibration
                                    prompt.IsOpen = true; //prompt user if okay
                                }
                            });

                        }

                    }
                    else if(timerCount >=10)
                    {
                        fallDetected = false;
                        timerCount = 0;
                        myGyroRecording = new float[300];
                        myAccRecording = new float[300];
                        index = 0;

                        Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            StartFall.Text = "Reset";
                        });
                        myMaxAccMag = 0;
                        myMaxGyroMag = 0;
                        myMinAccMag = 100;
                        maxTemp = 0;
                    }

                };

            }

            else if (startStopButton.Content.Equals("Turn Fall Detection Off"))
            {
                myAccGyro.Dispose();
                startStopButton.Content = "Turn Fall Detection On";
            }

        }

        private async void ComposeSms()
        {
            //Create new Chat
            Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();

            //Determine Location
            geolocator.DesiredAccuracyInMeters = 50;
            lat = "";
            lon = "";
            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(2), timeout: TimeSpan.FromSeconds(10));

                lat = geoposition.Coordinate.Latitude.ToString("0.00");
                lon = geoposition.Coordinate.Longitude.ToString("0.00");

                //Message of Text
                msg.Body = "This is an automated message from my Fall Detection App. I am experiencing an Emergency, please send help! My location: Latitude = " + lat + ", Longitude = " + lon;
            }
            catch(Exception ex)
            {
                if((uint)ex.HResult == 0x80004004)
                {
                    //Message of Text
                    msg.Body = "This is an automated message from my Fall Detection App. I am experiencing an Emergency, please send help!";
                }
            }

          
            
            //Get Contact number 1
            if(ContactNum1.Text != "Add New Number")
            {
                msg.Recipients.Add(ContactNum1.Text);
            }

            //Get Contact number 2
            if (ContactNum2.Text != "Add New Number")
            {
                msg.Recipients.Add(ContactNum2.Text);
            }

            //Get Contact number 3
            if (ContactNum3.Text != "Add New Number")
            {
                msg.Recipients.Add(ContactNum3.Text);
            }

            //Get contact number 4
            if (ContactNum4.Text != "Add New Number")
            {
                msg.Recipients.Add(ContactNum4.Text);
            }

            //Get Contact number 5
            if (ContactNum5.Text != "Add New Number")
            {
                msg.Recipients.Add(ContactNum5.Text);
            }

            await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
            
        }



    }
}
