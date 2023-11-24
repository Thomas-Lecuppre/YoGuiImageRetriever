using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace YoGuiImageRetriever
{
    public static class WebLogic
    {
        /// <summary>
        /// Retrieve data from API call
        /// </summary>
        /// <param name="url">API url to call</param>
        /// <returns>An object that represent the retrieved data.</returns>
        public async static Task<Root> RetrieveDataFromApi(string url)
        {
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
                        catch {}
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                // An exception occurred during the request
                MessageBox.Show(hre.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Download the image from api link
        /// </summary>
        /// <param name="url">Link to the image</param>
        /// <returns>The image.</returns>
        public async static Task<Bitmap> GetBitmapFromApi(string url)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(url);
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new System.IO.MemoryStream(imageBytes);
                    bitmapImage.EndInit();

                    Bitmap btm = BitmapImage2Bitmap(bitmapImage);
                    return btm;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading image: {ex.Message}");
                    return null;
                }
            }
        }

        public static string GetCardUrl(string cardName)
        {
            return $"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={cardName}";
        }

        public static string GetCardImageUrl(int cardId, ImageRatio imageRatio)
        {
            switch (imageRatio)
            {
                case ImageRatio.SmallCard:
                    {
                        return $"https://images.ygoprodeck.com/images/cards_small/{cardId}.jpg";
                    }
                case ImageRatio.CroppedCard:
                    {
                        return $"https://images.ygoprodeck.com/images/cards_cropped/{cardId}.jpg";
                    }
                case ImageRatio.FullCard:
                case ImageRatio.Undefine:
                default:
                    {
                        return $"https://images.ygoprodeck.com/images/cards/{cardId}.jpg";
                    }
            }
        }

        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
