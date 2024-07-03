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
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using ImageMagick;
using System.IO;


namespace HEICConvector
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<FileItem> Files { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Files = new ObservableCollection<FileItem>();
            FileDataGrid.ItemsSource = Files;
        }



        private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.heic)|*.jpg;*.jpeg;*.png;*.heic";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        Files.Add(new FileItem { FileName = filename });
                    }
                }
            }
        }


        private void BrowseOutputPathButton_Click(object sender, RoutedEventArgs e)
        {

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select the directory to save converted files";
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OutputPathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void ConvertFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Files.Count == 0)
            {
                System.Windows.MessageBox.Show("Please select files to convert.", "No Files Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string outputDirectory = OutputPathTextBox.Text;

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                System.Windows.MessageBox.Show("Please enter a valid output directory path.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(outputDirectory))
            {
                System.Windows.MessageBox.Show("The specified output directory does not exist.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedFormat = (FormatComboBox.SelectedItem as ComboBoxItem).Content.ToString().Split(' ')[0].ToLower();

            foreach (var fileItem in Files)
            {
                using (MagickImage image = new MagickImage(fileItem.FileName))
                {
                    string outputFileName = System.IO.Path.GetFileNameWithoutExtension(fileItem.FileName) + "." + selectedFormat;
                    string outputPath = System.IO.Path.Combine(outputDirectory, outputFileName);

                    image.Write(outputPath);
                }
            }

            System.Windows.MessageBox.Show("Files converted successfully.", "Conversion Complete", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public class FileItem
        {
            public string FileName { get; set; }
        }
    }
}
