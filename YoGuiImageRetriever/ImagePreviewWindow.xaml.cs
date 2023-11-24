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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace YoGuiImageRetriever
{
    /// <summary>
    /// Interaction logic for ImagePreviewWindow.xaml
    /// </summary>
    public partial class ImagePreviewWindow : Window
    {
        private string LastImagePath;
        private int LastHeight;
        private int LastWidth;

        public ImagePreviewWindow()
        {
            InitializeComponent();
        }

        public void UpdateImage(string imagePath,int width, int height)
        {
            LastHeight = height;
            LastWidth = width;
            LastImagePath = imagePath;
            UpdateImage();
        }

        private void UpdateImage()
        {
            if (string.IsNullOrEmpty(LastImagePath) || LastHeight <= 0 || LastWidth <= 0) return;
            if (!File.Exists(LastImagePath)) return;
            try
            {
                // Create a BitmapImage
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(LastImagePath, UriKind.RelativeOrAbsolute);
                bitmap.EndInit();

                // Set the BitmapImage as the source of the Image control

                bitmap = ResizeBitmapImage(bitmap, LastWidth, LastHeight);

                ImageBox.Source = bitmap;

                // Manually adjust the window size
                this.Width = LastWidth + 35;
                this.Height = LastHeight + 35;
                this.UpdateLayout();
            }
            catch (Exception ex)
            {
                // Handle exception, e.g., file not found or invalid image format
                MessageBox.Show($"Error loading image: \n{LastImagePath}\n\nError message :{ex.Message}","Image introuvable! ");
            }
        }

        public static BitmapImage ResizeBitmapImage(BitmapImage originalImage, int newWidth, int newHeight)
        {
            // Create the new bitmap and associated encoder
            BitmapImage resizedImage = new BitmapImage();
            resizedImage.BeginInit();
            resizedImage.DecodePixelWidth = newWidth;
            resizedImage.DecodePixelHeight = newHeight;

            // Set the image source using a dummy stream (to force the decoding)
            resizedImage.UriSource = originalImage.UriSource;
            resizedImage.EndInit();

            return resizedImage;
        }
    }
}
