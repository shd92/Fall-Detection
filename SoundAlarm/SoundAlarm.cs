using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libsound;
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

namespace SoundAlarm
{
    public sealed class Sound
    {
        private SoundIO sio;
        private AudioTool at;
        private float[] alarm;

        private DispatcherTimer timer;
        private int count;

        public Sound()
        {
            sio = new SoundIO();
            at = new AudioTool(sio.getOutputNumChannels(), sio.getOutputSampleRate());
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200); //1 second

            //Timer Tick Event
            timer.Tick += (object sender, object eb) =>
            {
                if (count % 2 == 0)
                {
                    sio.start();
                }
                else
                {
                    sio.stop();
                }
                count++;
            };

            //Sound Event
            sio.audioOutEvent += (uint numSamples) =>
            {
                alarm = at.sin(numSamples, 2500);
                alarm = at.convertChannels(alarm, 1);

                return alarm;
            };

        }

        public void start()
        {
            timer.Start();
        }

        public void stop()
        {
            timer.Stop();
            sio.stop();
        }


    }
}
