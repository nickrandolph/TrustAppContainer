using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Storage;

namespace PartialTrustWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
