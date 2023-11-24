using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace YoGuiImageRetriever
{
    public static class LocalLogic
    {
        public static void SaveDictionary(string filePath, Dictionary<string, int> dic)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true, // Optional: for a more human-readable format
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            string jsonString = JsonSerializer.Serialize(dic, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static Dictionary<string, int> LoadDictionary(string filePath)
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString);
            }

            return new Dictionary<string, int>();
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static int GetEnumRatioWidth(Enum value)
        {
            RatioAttribute ratioAttribute = GetEnumRatioAttribute(value);
            return ratioAttribute?.Width ?? 0;
        }

        public static int GetEnumRatioHeight(Enum value)
        {
            RatioAttribute ratioAttribute = GetEnumRatioAttribute(value);
            return ratioAttribute?.Height ?? 0;
        }

        static RatioAttribute GetEnumRatioAttribute(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            return (RatioAttribute)Attribute.GetCustomAttribute(field, typeof(RatioAttribute));
        }

        /// <summary>
        /// Retrieve the last ratio selection made by the user.
        /// </summary>
        /// <param name="localConfPath">Path to local conf path.</param>
        /// <returns>An index of xhat to select. Return 1 if error.</returns>
        public static int GetRatioSelection(string localConfPath)
        {
            if (!File.Exists(localConfPath)) return 1;

            string content = File.ReadAllText(localConfPath);
            if(int.TryParse(content, out int ratio)) return ratio;
            return 1;
        }

        public static void SaveConf(string confPath, int index)
        {
            File.WriteAllText(confPath, $"{index}");
        }

        /// <summary>
        /// Tell if the card is already stored localy
        /// </summary>
        /// <param name="saveFolder">Local folder path where card image are stored.</param>
        /// <param name="cardId">The card ID to check.</param>
        /// <returns>True is card is localy stored, else false.</returns>
        public static bool IsCardOnLocal(string saveFolder, int cardId)
        {
            if (!Directory.Exists(saveFolder)) return false;
            DirectoryInfo dir = new DirectoryInfo(saveFolder);
            FileInfo[] fileInfos = dir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);

            return fileInfos.Count(x => Path.GetFileNameWithoutExtension(x.FullName).Contains($"{cardId}")) > 0;
        }

        public static string GetLocalImagePath(string saveFolder, int cardId, ImageRatio imageRatio)
        {
            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
            DirectoryInfo dir = new DirectoryInfo(saveFolder);
            FileInfo[] fileInfos = dir.GetFiles($"*{cardId}.jpg", SearchOption.TopDirectoryOnly);

            switch(imageRatio)
            {
                case ImageRatio.SmallCard:
                    {
                        return fileInfos.FirstOrDefault(x => x.Name.StartsWith("sc_"))?.FullName;
                    }
                case ImageRatio.CroppedCard:
                    {
                        return fileInfos.FirstOrDefault(x => x.Name.StartsWith("cc_"))?.FullName;
                    }
                case ImageRatio.FullCard:
                case ImageRatio.Undefine:
                    {
                        return fileInfos.FirstOrDefault(x => x.Name.StartsWith("fc_"))?.FullName;
                    }
                default: return string.Empty;
            }
        }

        public static void SaveImageLocaly(string saveFolder,Bitmap image , int cardId, ImageRatio imageRatio)
        {
            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
            string imagePath;
            switch (imageRatio)
            {
                case ImageRatio.SmallCard:
                    {
                        imagePath = Path.Combine(saveFolder, $"sc_{cardId}.jpg");
                        break;
                    }
                case ImageRatio.CroppedCard:
                    {
                        imagePath = Path.Combine(saveFolder, $"cc_{cardId}.jpg");
                        break;
                    }
                case ImageRatio.FullCard:
                case ImageRatio.Undefine:
                default:
                    {
                        imagePath = Path.Combine(saveFolder, $"fc_{cardId}.jpg");
                        break;
                    }
            }

            if (string.IsNullOrEmpty(imagePath)) return;
            try
            {
                image.Save(imagePath);
            }
            catch( Exception ex )
            {
                MessageBox.Show(ex.Message, "Cannot save image", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
