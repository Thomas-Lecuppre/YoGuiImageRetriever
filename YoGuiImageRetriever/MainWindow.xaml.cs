using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            LocalDictionaryPath = localPath + @"\YogiohImageRetriever\dictionary.json";
            UserConfFilePath = localPath + @"\YogiohImageRetriever\.conf";
            CardsFolderPath = localPath + @"\YogiohImageRetriever\Cards";
            Initialization();
        }

        private readonly string LocalDictionaryPath;
        private readonly string UserConfFilePath;
        private readonly string CardsFolderPath;
        private ImageRatio CurrentImageRatio {  get; set; }
        private ImagePreviewWindow _previewWindow;
        private Dictionary<string, int> SuggestionDictionary { get; set; }
        private string CurrentCardName { get; set; }
        private int CurrentCardId { get; set; }
        private string CurrentCardImagePath {  get; set; }
        private int CurrentCardHeight { get; set; }
        private int CurrentCardWidth { get; set; }

        private void Initialization()
        {
            // Local dictionary file already exist ?
            if(!File.Exists(LocalDictionaryPath))
            {
                // Get dictionary from api
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LocalDictionaryPath));
                _ = GetDataFromWebAsync();
            }
            // Parse dictionary in app.
            GetDataLocal();
        }

        #region Non control functions


        /// <summary>
        /// Get data from web API and save dictionary of card name and ID.
        /// </summary>
        private async Task GetDataFromWebAsync()
        {
            // Get full cards dictionary
            string dictionaryUrl = @"https://db.ygoprodeck.com/api/v7/cardinfo.php";
            Root root = await WebLogic.RetrieveDataFromApi(dictionaryUrl);
            if (root == null)
            {
                MessageBox.Show($"Cannot initialize app because api request send nothing back.");
                return;
            }

            // Create dictionary and save it localy.
            Dictionary<string, int> dictionary = [];
            foreach (Card card in root.Data)
            {
                dictionary.Add(card.Name, card.Id);
            }
            LocalLogic.SaveDictionary(LocalDictionaryPath, dictionary);
        }

        private void GetDataLocal()
        {
            // Retrieve local dictionary.
            // Put suggestion in the list.
            SuggestionDictionary = LocalLogic.LoadDictionary(LocalDictionaryPath);
            SuggestionListBox.Items.Clear();
            foreach (KeyValuePair<string, int> entry in SuggestionDictionary)
            {
                SuggestionListBox.Items.Add(entry.Key);
            }

            // Add Combobox items
            ImageRatio[] imageRatios = (ImageRatio[])Enum.GetValues(typeof(ImageRatio));
            int irCount = imageRatios.Length;
            foreach (ImageRatio ir in imageRatios)
            {
                string irDescription = LocalLogic.GetEnumDescription(ir);
                ImageRatioSelector.Items.Add(irDescription);
            }
            // Check what index to choose.
            int index = LocalLogic.GetRatioSelection(UserConfFilePath);
            if (index > irCount - 1 && index < 0) index = 1;
            ImageRatioSelector.SelectedIndex = index;
        }

        private void HandleImageSelectorChange()
        {
            // Looking to find the ImageRatio that fit the combobox selection
            ImageRatio[] imageRatios = (ImageRatio[])Enum.GetValues(typeof(ImageRatio));
            foreach (ImageRatio ir in imageRatios)
            {
                string irDescription = LocalLogic.GetEnumDescription(ir);
                if (irDescription == ImageRatioSelector.SelectedValue.ToString())
                {
                    CurrentImageRatio = ir;
                    CurrentCardWidth = LocalLogic.GetEnumRatioWidth(ir);
                    CurrentCardHeight = LocalLogic.GetEnumRatioHeight(ir);
                    // Save the index selection.
                    LocalLogic.SaveConf(UserConfFilePath, ImageRatioSelector.SelectedIndex);
                    break;
                }
            }

            // Indicate size in main window.
            CustomHeightTextBox.Text = CurrentCardHeight.ToString();
            CustomWidthTextBox.Text = CurrentCardWidth.ToString();

            CustomHeightTextBox.IsEnabled = CurrentImageRatio == ImageRatio.Undefine;
            CustomWidthTextBox.IsEnabled = CurrentImageRatio == ImageRatio.Undefine;


            // Change image size on the preview window.
            // Is there a windows ?
            if (_previewWindow == null) return;

            // Retrieve local image path
            string imgPath = LocalLogic.GetLocalImagePath(CardsFolderPath, CurrentCardId, CurrentImageRatio);
            if (string.IsNullOrEmpty(imgPath)) return;
            if (!File.Exists(imgPath)) return;

            CurrentCardImagePath = imgPath;

            _previewWindow.UpdateImage(CurrentCardImagePath, CurrentCardWidth, CurrentCardHeight);
        }

        private async Task CardFound()
        {
            string selection = SuggestionListBox.SelectedItem.ToString();
            SearchBox.Text = selection;

            // Found the selection in the dictionary
            foreach (KeyValuePair<string, int> pair in SuggestionDictionary)
            {
                if (pair.Key == selection)
                {
                    CurrentCardId = pair.Value;
                    CurrentCardName = pair.Key;

                    // Is there a selected card ?
                    if (CurrentCardId <= 0 && string.IsNullOrEmpty(CurrentCardName)) return;

                    // Is the card stored localy ?
                    bool r = LocalLogic.IsCardOnLocal(CardsFolderPath, CurrentCardId);

                    if (!r)
                    {
                        // Dowload all the images.
                        string imgUrl = WebLogic.GetCardImageUrl(CurrentCardId, ImageRatio.FullCard);
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            Bitmap img = await WebLogic.GetBitmapFromApi(imgUrl);
                            LocalLogic.SaveImageLocaly(CardsFolderPath, img, CurrentCardId, ImageRatio.FullCard);
                        }
                        imgUrl = WebLogic.GetCardImageUrl(CurrentCardId, ImageRatio.SmallCard);
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            Bitmap img = await WebLogic.GetBitmapFromApi(imgUrl);
                            LocalLogic.SaveImageLocaly(CardsFolderPath, img, CurrentCardId, ImageRatio.SmallCard);
                        }
                        imgUrl = WebLogic.GetCardImageUrl(CurrentCardId, ImageRatio.CroppedCard);
                        if (!string.IsNullOrEmpty(imgUrl))
                        {
                            Bitmap img = await WebLogic.GetBitmapFromApi(imgUrl);
                            LocalLogic.SaveImageLocaly(CardsFolderPath, img, CurrentCardId, ImageRatio.CroppedCard);
                        }
                    }

                    if (_previewWindow == null) return;

                    // Retrieve local image path
                    string imgPath = LocalLogic.GetLocalImagePath(CardsFolderPath, CurrentCardId, CurrentImageRatio);
                    if (string.IsNullOrEmpty(imgPath)) return;
                    if (!File.Exists(imgPath)) return;

                    CurrentCardImagePath = imgPath;

                    _previewWindow.UpdateImage(CurrentCardImagePath, CurrentCardWidth, CurrentCardHeight);
                }
            }
        }

        #endregion

        private void OpenClosePreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if(_previewWindow != null)
            {
                _previewWindow.Close();
                _previewWindow = null;
            }
            else
            {
                _previewWindow = new ImagePreviewWindow();
                _previewWindow.Show();
                // Is there a selected card ?
                if (CurrentCardId > 0 && !string.IsNullOrEmpty(CurrentCardName))
                {
                    // Set Image to preview window.

                    string imgPath = LocalLogic.GetLocalImagePath(CardsFolderPath, CurrentCardId, CurrentImageRatio);
                    if (string.IsNullOrEmpty(imgPath)) return;
                    if (!File.Exists(imgPath)) return;

                    CurrentCardImagePath = imgPath;

                    _previewWindow.UpdateImage(CurrentCardImagePath, CurrentCardWidth, CurrentCardHeight);
                }
            }
        }

        private void SuggestionListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = CardFound();
        }


        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SuggestionListBox.Items.Clear();
            foreach (KeyValuePair<string, int> entry in SuggestionDictionary)
            {
                string key = entry.Key;
                if (key.ToLower().Contains(SearchBox.Text.ToLower()))
                {
                    SuggestionListBox.Items.Add(entry.Key);
                }
            }
        }

        private void CustomWidthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_previewWindow == null) return;
            if (CurrentImageRatio != ImageRatio.Undefine) return;
            if(int.TryParse(CustomWidthTextBox.Text, out var width))
            {
                CurrentCardWidth = width;
                _previewWindow.UpdateImage(CurrentCardImagePath, CurrentCardWidth, CurrentCardHeight);
            }
        }

        private void CustomHeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_previewWindow == null) return;
            if (CurrentImageRatio != ImageRatio.Undefine) return;
            if (int.TryParse(CustomHeightTextBox.Text, out var height))
            {
                CurrentCardHeight = height;
                _previewWindow.UpdateImage(CurrentCardImagePath, CurrentCardWidth, CurrentCardHeight);
            }
        }

        private void ImageRatioSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleImageSelectorChange();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(_previewWindow != null)
            {
                _previewWindow.Close();
            }
        }
    }
}