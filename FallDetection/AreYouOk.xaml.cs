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


namespace FallDetection
{
    public partial class AreYouOk : UserControl
    {

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler ClickedCancel;
        public event ChangedEventHandler ClickedHelp;
        
        public AreYouOk()
        {
            this.InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancelClick(EventArgs.Empty);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            OnHelpClick(EventArgs.Empty);
        }

        protected virtual void OnCancelClick(EventArgs e)
        {
            ClickedCancel(this, e);
        }

        protected virtual void OnHelpClick(EventArgs e)
        {
            ClickedHelp(this, e);
        }
    }
}
