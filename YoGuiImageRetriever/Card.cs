using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YoGuiImageRetriever
{
    public class CardSet
    {
        [JsonProperty("set_name")]
        public string SetName { get; set; }

        [JsonProperty("set_code")]
        public string SetCode { get; set; }

        [JsonProperty("set_rarity")]
        public string SetRarity { get; set; }

        [JsonProperty("set_rarity_code")]
        public string SetRarityCode { get; set; }

        [JsonProperty("set_price")]
        public string SetPrice { get; set; }
    }

    public class CardImage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("image_url_small")]
        public string ImageUrlSmall { get; set; }

        [JsonProperty("image_url_cropped")]
        public string ImageUrlCropped { get; set; }
    }

    public class CardPrice
    {
        [JsonProperty("cardmarket_price")]
        public string CardmarketPrice { get; set; }

        [JsonProperty("tcgplayer_price")]
        public string TcgplayerPrice { get; set; }

        [JsonProperty("ebay_price")]
        public string EbayPrice { get; set; }

        [JsonProperty("amazon_price")]
        public string AmazonPrice { get; set; }

        [JsonProperty("coolstuffinc_price")]
        public string CoolstuffincPrice { get; set; }
    }

    public class Card
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("frameType")]
        public string FrameType { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("atk")]
        public int Attack { get; set; }

        [JsonProperty("def")]
        public int Defense { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("attribute")]
        public string Attribute { get; set; }

        [JsonProperty("archetype")]
        public string Archetype { get; set; }

        [JsonProperty("card_sets")]
        public List<CardSet> CardSets { get; set; } = new List<CardSet>();

        [JsonProperty("card_images")]
        public List<CardImage> CardImages { get; set; } = new List<CardImage>();

        [JsonProperty("card_prices")]
        public List<CardPrice> CardPrices { get; set; } = new List<CardPrice>();
    }

    public class Root
    {
        [JsonProperty("data")]
        public List<Card> Data { get; set; }
    }
}
