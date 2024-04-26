//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using HtmlAgilityPack;
// URL of the PakWheels page containing car listings
var carModelsDictionary = new Dictionary<string, List<string>>();
carModelsDictionary["honda"] = new List<string> { "city", "civic", "br-v", "cr-v", "accord" };
carModelsDictionary["toyota"] = new List<string> { "corolla", "yaris", "fortuner", "hilux", "prius", "land-cruiser", "camry" };
carModelsDictionary["suzuki"] = new List<string> { "cultus", "swift", "alto", "wagon-r", "bolan", "ciaz", "jimny", "vitara", "apv", "ravi", "mega-carry" };
carModelsDictionary["kia"] = new List<string> { "sportage", "picanto", "rio", "sorento", "grand-carnival" };
carModelsDictionary["hyundai"] = new List<string> { "tucson", "elantra", "ioniq", "santa-fe", "grand-starex" };
carModelsDictionary["proton"] = new List<string> { "saga", "x70" };
carModelsDictionary["united"] = new List<string> { "bravo", "alpha" };
carModelsDictionary["changan"] = new List<string> { "karvaan", "alsvin", "oshan-x7" };
carModelsDictionary["prince"] = new List<string> { "pearl" };
carModelsDictionary["haval"] = new List<string> { "h6", "jolion" };

foreach (var a in carModelsDictionary)
{
    var carModels = carModelsDictionary[a.Key];
    for (int b = 0; b < carModels.Count; b++)
    {
        string url = "https://www.pakwheels.com/new-cars/"+a.Key+"/" + carModels[b] + "/";//"https://www.pakwheels.com/new-cars/honda/city/";// toyota/corolla/
        try
        {
            HttpClient httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            // Specify XPath to select car elements
            List<CarInfo> cars = new List<CarInfo>();
            // Select all <p> elements with class 'city-name' inside <li> elements
            //HtmlNodeCollection carNodesName = doc.DocumentNode.SelectNodes("//li[@class='col-md-3']/div[@class='panel-130-100-make']/div[@class='cards']/a/div/h3[@class='nomargin truncate']");
            //HtmlNodeCollection carNodesprices = doc.DocumentNode.SelectNodes("//li[@class='col-md-3']/div[@class='panel-130-100-make']/div[@class='cards']/div/div[@class='generic-green truncate fs14']");
            //class='table table-bordered mb0'
            HtmlNodeCollection carVariants = doc.DocumentNode.SelectNodes("//section[@id='price-block']/div[@class='container']/table/tr/td[@class='pos-rel']/a/h3[@class='nomargin w-65']");
            HtmlNodeCollection carPrices = doc.DocumentNode.SelectNodes("//section[@id='price-block']/div[@class='container']/table/tr/td/div[@class='generic-green fs16 mb5']");
            HtmlNodeCollection carSpecs = doc.DocumentNode.SelectNodes("//section[@id='price-block']/div[@class='container']/table/tr/td[@class='pos-rel']/p[@class='mb0']");
            HtmlNodeCollection carVariantInfo = doc.DocumentNode.SelectNodes("//section[@id='price-block']/div[@class='container']/table/tr/td[@class='pos-rel']/a");
            HtmlNode carPicture = doc.DocumentNode.SelectSingleNode("//section[@id='main-banner']/div[@class='container']/span[@id='product']/div/div[@class='row']/div[@class='col-md-8 vehicle-intro generation-slider pos-rel']/div[@id='myCarousel']/ul[@id='image-gallery']/li[1]");
            
            if (carVariants != null)
            {
                string carPictureUrl = carPicture.GetAttributeValue("data-thumb", "");
                // Iterate through each <p> element and extract the inner text (car name)
                for (int i = 0; i < carVariants.Count; i++)
                {
                    string carColorOptions = "";
                    string carName = carVariants[i].InnerText.Trim();
                    string carPrice = carPrices[i].InnerText.Trim();
                    string carSpec = carSpecs[i].InnerText.Trim();
                    string carVariantInfoUrl = carVariantInfo[i].GetAttributeValue("href", "");

                    HttpClient httpClient2 = new HttpClient();
                    string html2 = await httpClient2.GetStringAsync(carVariantInfoUrl);
                    HtmlDocument doc2 = new HtmlDocument();
                    doc2.LoadHtml(html2);
                    HtmlNodeCollection carColors = doc2.DocumentNode.SelectNodes("//section[@id='version-colors']/div[@class='container']/div[@class='well']/ul/li/p");
                    for (int j = 0; j < carColors.Count; j++)
                    {
                        var color = carColors[j].InnerText.Trim();
                        carColorOptions += color + ", ";
                    }

                    carPrice = carPrice.Split("\n")[0];
                    CarInfo carInfo = new CarInfo();
                    carInfo.Name = carName;
                    carInfo.Price = carPrice;
                    carInfo.Specs = carSpec;
                    carInfo.Colors = carColorOptions;
                    carInfo.PictureUrl = carPictureUrl;

                    cars.Add(carInfo);
                }
            }
            //if (carNodesName != null)
            //{
            //    // Iterate through each <p> element and extract the inner text (car name)
            //    for (int i = 0; i < carNodesName.Count; i++)
            //    {
            //        string carName = carNodesName[i].InnerText.Trim();
            //        string carPrice = carNodesprices[i].InnerText.Trim();
            //        CarInfo carInfo = new CarInfo();
            //        carInfo.Name = carName;
            //        carInfo.Price = carPrice;
            //        cars.Add(carInfo);
            //    }
            //}
            // Display extracted car names
            Console.WriteLine("Extracted Car Names:");
            foreach (CarInfo car in cars)
            {
                Console.WriteLine(car.Name);
                Console.WriteLine(car.Price);
                Console.WriteLine(car.Specs);
                Console.WriteLine(car.Colors);
                Console.WriteLine(car.PictureUrl);
                Console.WriteLine("\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
// Class to represent car information
public class CarInfo
{
    public string Name { get; set; }
    public string Price { get; set; }
    public string Specs { get; set; }
    public string Colors { get; set; }
    public string PictureUrl { get; set; }
}
