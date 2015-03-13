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


namespace FallDetection
{

    public sealed partial class MainPage : Page
    {

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
        Sound sound; //Initiate Sound Alarm
        Vibrator vibrate; //Initiate Vibrate Alarm
        AreYouOk askOk; //Initiates .xaml to be popped up
        Popup prompt; //Initiates a pop up prompt


        private Object lockKey = new Object();


        public MainPage()
        {
            this.InitializeComponent() ;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            //For use of detection of fall
            this.NavigationCacheMode = NavigationCacheMode.Required;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);

            //For use AFTER detection of fall
            sound = new Sound();
            vibrate = new Vibrator();
            askOk = new AreYouOk();
            prompt = new Popup();
            prompt.Height = 200;
            prompt.Width = 400;
            prompt.VerticalOffset = 100;
            prompt.Child = askOk;
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

        }

        //Start or reset Accelerometer 
        public void startStopButton_Click(object sender, RoutedEventArgs e)
        {

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
                                sound.start(); //Alarm with sound
                                vibrate.start(); //Alarm with Vibration
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

        private async void ComposeSms()
        {
            Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
            msg.Body = "This is body of demo message.";
            msg.Recipients.Add("3605090277");
            msg.Recipients.Add("3605090396");
            await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {
            EMJ
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}


//How to send text/email/call
//http://stackoverflow.com/questions/23797559/wp-8-1-runtime-code-to-make-phone-call-send-sms-send-email-not-the-silverlig



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
