using System;
using System.Collections.Generic;
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
using Azure;
using Microsoft.Win32;
using System.IO;

namespace WpfApplicationTestAzure
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            var fileName = FileStorage.UploadOnAzure(txtPath.Text, AzureInformation.Get());
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            FileStorage.DeleteAllFile(AzureInformation.Get());
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                txtPath.Text = openFileDialog.FileName;
        }



    }
}
