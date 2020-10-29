using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace WeatherSvgRipper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting!");

            string html = "https://www.iconfinder.com/iconsets/good-weather-1";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(html);

            List<(string, string)> assets = new List<(string, string)>();
            string assetXPath = "/html/body/main/div/div/div[{0}]";
            string nameXPath = "/html/body/main/div/div/div[{0}]/div/a";

            for (int i = 1; i <= 70; i++)
            {
                HtmlNode assetNode = htmlDoc.DocumentNode.SelectSingleNode(string.Format(assetXPath, i));
                string assetId = assetNode.Attributes.FirstOrDefault(x => x.Name == "data-asset-id").Value;

                HtmlNode nameNode = htmlDoc.DocumentNode.SelectSingleNode(string.Format(nameXPath, i));
                string name = nameNode.Attributes.FirstOrDefault(x => x.Name == "title").Value;
                name = name.Replace(",", "_").Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(assetId))
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = assetId;
                    }

                    assets.Add((assetId, name+ ".svg"));
                }
            }

            string downloadUrl = "https://www.iconfinder.com/icons/{0}/download/svg/512";
            string imagesFolder = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "weather-svgs");

            if (Directory.Exists(imagesFolder))
            {
                Directory.Delete(imagesFolder, true);
            }
            Directory.CreateDirectory(imagesFolder);

            using (var client = new WebClient())
            {
                foreach (var asset in assets)
                {
                    string url = string.Format(downloadUrl, asset.Item1);
                    string filePath = Path.Join(imagesFolder, asset.Item2);

                    Console.WriteLine($"url: {url}");
                    Console.WriteLine($"filePath: {filePath}");
                    Console.WriteLine();

                    client.DownloadFile(url, filePath);
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
