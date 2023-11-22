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
            bool r = await CheckLink(searchText);
            if (!r)
            {
                UrlBlock.Foreground = new SolidColorBrush(Colors.IndianRed);
                return;
            }

            UrlBlock.Foreground = new SolidColorBrush(Colors.Black);
        }

        private async Task<bool> CheckLink(string cardName)
        {
            string url = $"https://www.yugiohcardguide.com/single/{cardName}.html";
            UrlBlock.Text = url;

            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Check if the response body contains specific content indicating an error
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (responseBody.Contains("missing.html"))
                        {
                            return false;
                        }

                        // The link is available
                        return true;
                    }
                    else if (IsRedirect(response.StatusCode))
                    {
                        // The link is redirected
                        return false;
                    }
                    else
                    {
                        // The link is not available or other error
                        return false;
                    }
                }
            }
            catch (HttpRequestException)
            {
                // An exception occurred during the request
                return false;
            }
        }

        static bool IsRedirect(System.Net.HttpStatusCode statusCode)
        {
            return statusCode == System.Net.HttpStatusCode.MovedPermanently ||
                   statusCode == System.Net.HttpStatusCode.Found ||
                   statusCode == System.Net.HttpStatusCode.SeeOther ||
                   statusCode == System.Net.HttpStatusCode.TemporaryRedirect ||
                   statusCode == System.Net.HttpStatusCode.PermanentRedirect;
        }
    }
}