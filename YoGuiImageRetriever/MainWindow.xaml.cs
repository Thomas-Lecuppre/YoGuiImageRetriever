using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YoGuiImageRetriever
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _ = GetImageAsync();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                _ = GetImageAsync();
            }
        }

        private async Task GetImageAsync()
        {
            string searchText = StringUtilities.FormatText(SearchBox.Text);
            Root root = await CheckLink(searchText);
            if (root == null)
            {
                SearchBox.BorderBrush = new SolidColorBrush(Colors.IndianRed);
                return;
            }

            SearchBox.BorderBrush = new SolidColorBrush(Colors.Black);
            string url = root.Data.First().CardImages.First().ImageUrl;
            DownloadImage(url);
        }

        private async Task<Root> CheckLink(string cardName)
        {
            string url = $"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={cardName}";
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<Root>(content);
                        }
                        catch { return null; }
                    }
                    else return null;
                }
            }
            catch (HttpRequestException)
            {
                // An exception occurred during the request
                return null;
            }
        }

        private async void DownloadImage(string imageUrl)
        { 
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new System.IO.MemoryStream(imageBytes);
                    bitmapImage.EndInit();

                    YuGiOhImage.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading image: {ex.Message}");
                }
            }
        }
    }
}