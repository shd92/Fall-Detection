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


namespace Fall_Detection
{ 

    public sealed partial class MainPage : Page
    {

        public static void Main()
        {
        }

				//Variables for use of detection of fall
        private CppAcc myAcc;
        double AccMag;
        double FallThreshold = 0.2;
        double myMinMag = 50;
        bool fallDetected = false;

        private CppAcc FallAcc;
        double FallAccMag;
        double maxFallAccMag = -300;
        double AccUpperThreshold = 2;

        private CppGyro FallGyro;
        double FallGyroMag;
        double maxFallGyroMag = -300;
        double GyroUpperThreshold = 200;
        
        DispatcherTimer timer;

        //Variables for use AFTER detection of fall
        SoundIO sio;
        AudioTool at;
        float[] alarm;
        AreYouOk askOk;
        Popup prompt;


        private Object lockKey = new Object();

       

        public MainPage()
        {
            this.InitializeComponent();

						//For use of detection of fall
            this.NavigationCacheMode = NavigationCacheMode.Required;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            
            //For use AFTER detection of fall
            sio = new SoundIO();
            at = new AudioTool(sio.getOutputNumChannels(), sio.getOutputSampleRate());
            askOk = new AreYouOk();
            prompt = new Popup();
            prompt.Height = 300;
            prompt.Width = 400;
            prompt.VerticalOffset = 100;
            prompt.Child = askOk;
        }

      
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //Used to Alarm
            sio.audioOutEvent += (uint numSamples) =>
            {
                alarm = at.sin(numSamples, 2000);
                alarm = at.convertChannels(alarm, 1);
                return alarm;
            };
            
            askOk.Cancel.Click += (s, ergs) =>
            {
                prompt.IsOpen = false;
	              sio.stop();
            };
            
             askOk.Help.Click += (s, ergs) =>
             {
                prompt.IsOpen = false;
                sio.stop();

                //Send alarm text to contact list
             };

        }

        //Start or reset Accelerometer 
        public void startStopButton_Click(object sender, RoutedEventArgs e)
        {
        	
        	      //SOUND ALARM
                

                //VIBRATE

                //PROMPT USER IF OKAY
                


                

               
        	
        	
            if (startStopButton.Content.Equals("Start"))
            {
                startStopButton.Content = "Stop";

                myAcc = new CppAcc();
                myMinMag = 100;
                fallDetected = false;
                
                //Detect if Acc mag  falls below FallThreshold (may be the fall happening)
                 myAcc.onReadingChanged += (double x, double y, double z) =>
                 {
                     //Calculate magnitude of accelerometer
                     AccMag = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

                     if (AccMag <= myMinMag)
                     {
                         myMinMag = AccMag;
                     }

                     lock (lockKey)
                     {
                         Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                         {
                             this.myAccText.Text = "myAcc Magnitude: " + Math.Round(AccMag, 3) + " Min = " + Math.Round(myMinMag, 3);

                             //Check to see if Fall was detected
                             if (AccMag <= FallThreshold)
                             {
                                 FallIndication.Text = "FALL DETECTED!";
                                 sio.start(); //Alarm with sound
                                 //Alarm with vibrate
                                 prompt.IsOpen = true; //prompt user if okay
                                 myAcc.Dispose();
                             }
                         });
                     }

                     //Detect if fall may be happening - if Acc mag falls below lower fall threshold 
                     
                 };

            }
            else
            {
                startStopButton.Content = "Start";
                FallIndication.Text = "...";
                myAcc.Dispose();
            }
        }

        //When fall detected, prompt user if OK while making sound and vibrate
        private void FallStatus_Change(FrameworkElement sender, DataContextChangedEventArgs args)
        {

        }


        //ADD PHONE NUMBERS
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Popup popup = new Popup();
        //    popup.Height = 300;
        //    popup.Width = 400;
        //    popup.VerticalOffset = 100;
        //    AddContact control = new AddContact();
        //    popup.Child = control;
        //    popup.IsOpen = true;


        //    control.btnOK.Click += (s, args) =>
        //    {
        //        popup.IsOpen = false;
        //        this.text.Text = control.tbx.Text;
        //    };

        //    control.btnCancel.Click += (s, args) =>
        //    {
        //        popup.IsOpen = false;
        //    };
        //}




    }
}







 ////Trigger calculation of acceleration and angular velocity within 0.5s;
                        //// myAcc.Dispose();
                        //FallAcc = new CppAcc();
                        //FallGyro = new CppGyro();
                        //// timer.Start();
                        //timerCount1 = 0;
                        //timerCount2 = 0;

                        //lock (lockKey)
                        //{
                        //    FallAcc.onReadingChanged += (double xF, double yF, double zF) =>
                        //    {
                        //        //Calculate magnitude of accelerometer
                        //        FallAccMag = Math.Sqrt(Math.Pow(xF, 2) + Math.Pow(yF, 2) + Math.Pow(zF, 2));

                        //        if (FallAccMag >= maxFallAccMag)
                        //        {
                        //            maxFallAccMag = FallAccMag;
                        //        }

                        //        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        //        {
                        //            this.FallAccText.Text = "FallAcc Magnitude " + Math.Round(FallAccMag, 3) + " Max = " + Math.Round(maxFallAccMag, 3);
                        //        });

                        //        timerCount1++;
                        //        if (timerCount1 > 50)
                        //        {
                        //          //  FallAcc.Dispose();
                        //        }

                        //    };
                        //}


                        //lock (lockKey)
                        //{
                        //    FallGyro.onReadingChanged += (double xF, double yF, double zF) =>
                        //    {
                        //        //Calculate magnitude of accelerometer
                        //        FallGyroMag = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

                        //        if (FallGyroMag >= maxFallGyroMag)
                        //        {
                        //            maxFallGyroMag = FallGyroMag;
                        //        }

                        //        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        //        {
                        //            this.FallGyroText.Text = "FallGyro Magnitude: " + Math.Round(FallGyroMag, 3) + " Max = " + Math.Round(maxFallGyroMag, 3);
                        //        });

                        //        timerCount2++;
                        //        if (timerCount2 > 50)
                        //        {
                        //           // FallGyro.Dispose();
                        //        }
                        //    };
                        //}

                        //if (FallAccMag >= AccUpperThreshold && FallGyroMag >= GyroUpperThreshold)
                        //{
                        //    FallDetected();
                        //}

//timer.Tick += (object sender, object eb) =>
//   {
//       timerCount++;
//   };  
