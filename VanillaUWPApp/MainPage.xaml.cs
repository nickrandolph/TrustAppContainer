using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VanillaUWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void TempWriteButtonClick(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(@"c:\temp\dummyoutput.txt", "File contents");
        }

        private void AppDataWriteButtonClick(object sender, RoutedEventArgs e)
        {
            var file = System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, $"dummyoutput.txt");

            File.WriteAllText(file, "File contents");
        }
    }
}
