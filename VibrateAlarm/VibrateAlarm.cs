using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.Devices.Notification;
using System.Numerics;
using Windows.UI.Core;
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


namespace VibrateAlarm
{
    public sealed class Vibrator
    {

        private VibrationDevice vibrator;
        private DispatcherTimer timer;
        private int count;

        public Vibrator()
        {
            vibrator = VibrationDevice.GetDefault();  
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(310); //0.31 seconds
            count++;

            timer.Tick += (object sender, object eb) =>
            {
                if (count % 2 == 0)
                {
                    vibrator.Vibrate(TimeSpan.FromSeconds(0.13));
                }
                else
                {
                    vibrator.Vibrate(TimeSpan.FromSeconds(0.3));
                }
                count++;
            };  
        }

        public void start()
        {
            timer.Start(); 
            
        }

        public void stop()
        {
            timer.Stop();
        }


    }
}
