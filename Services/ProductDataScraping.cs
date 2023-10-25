using HtmlAgilityPack;
using sensusProducts.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sensusProducts.Service
{
    public class ProductDataScraping
    {
        Product product = new Product();
        ProductServices ProductServices = new ProductServices();

        public async Task<Product> ScrapData(string url)
        {

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string html = await client.GetStringAsync(url);

                    // Load the HTML content into an HtmlDocument
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    //validate the URL
                    var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//section[@class='headline-bluebar']//h1");
                    if (titleNode == null)
                    {
                        System.Windows.Forms.MessageBox.Show("Provide a proper Sensus product URL");
                        return null;
                    }

                    //extract title and check in DB
                    string titleText = titleNode.InnerText;
                    titleText = HtmlEntity.DeEntitize(titleText);

                    //return null if present in DB
                    if (ProductServices.ProductExistsInDB(titleText)) return null;
                    // else continue with the execution
                    product.Name = titleText;

                    GetDescription(htmlDoc);

                    GetFeatures(htmlDoc);

                    GetDocument(htmlDoc);

                    GetImageLinks(htmlDoc);

                    GetUtilityList(htmlDoc);
                                        
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }
                return product;
            }
        }

        #region extraction_methods

        private void GetUtilityList(HtmlDocument htmlDoc)
        {
            //determine the utility type of the product
            var utilityList = new List<UtilityType>();
            var utilitySpanDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='col-auto']//div[@class='icons']");
            var utilitySpanList = utilitySpanDiv.Descendants("span");
            foreach (var utilitySpan in utilitySpanList)
            {
                var utilityType = utilitySpan.GetAttributeValue("data-utility-trans", "");
                utilityList.Add((UtilityType)Enum.Parse(typeof(UtilityType), utilityType));
            }
            string joinedList = string.Join(", ", utilityList);
            product.UtilityTypes = utilityList;
            // Print the joined list
        }

        private void GetImageLinks(HtmlDocument htmlDoc)
        {
            //extract all the images
            var imgHyperLinkList = new List<string>();
            var galleryDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='clearfix']");
            var productImages = galleryDiv.Descendants("a");
            //if the product has a gallery meaning multiple images
            if (productImages.Any())
            {
                foreach (var productImage in productImages)
                {
                    var img = productImage.SelectSingleNode(".//img").GetAttributeValue("src", "");

                    if (!string.IsNullOrEmpty(img))
                    {
                        string pattern = @"-(\d{2,3}x\d{2,3})(\.\w+)$";
                        // Use Regex.Replace to remove the resolution from the URL
                        string fullsizeimage = Regex.Replace(img, pattern, "$2");
                        imgHyperLinkList.Add(fullsizeimage);
                    }
                }
            }

            // single-display-image product
            else
            {
                var productImage = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='product-image']");
                var img = productImage.SelectSingleNode(".//img").GetAttributeValue("src", "");
                imgHyperLinkList.Add(img);
            }
            product.ImgLinks = imgHyperLinkList;
        }

        private void GetDocument(HtmlDocument htmlDoc)
        {
            //Documents

            var docAnchorNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='features']//div[@class='summary']//a");
            // gets the attribute href from the anchor tag linking to document\
            string link = "";
            if (docAnchorNode != null)
            {
                link = docAnchorNode.GetAttributeValue("href", "");
            }

            product.DocLink = link;
        }

        private void GetDescription(HtmlDocument htmlDoc)
        {

            // Extract Description
            var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode(".//div[@id='description']");
            List<string> desc = new List<string>();
            if (descriptionNode != null)
            {
                // Iterate through text nodes within the description node
                foreach (var textNode in descriptionNode.DescendantsAndSelf().Where(n => n.NodeType == HtmlNodeType.Text))
                {
                    string inputText = textNode.InnerText.Trim();

                    // Split the text into sentences using regular expressions
                    string[] sentences = Regex.Split(inputText, @"(?<=[.!?])\s+");

                    // Initialize a StringBuilder to recombine the sentences
                    StringBuilder formattedText = new StringBuilder();

                    // Add a line break after every two sentences
                    for (int i = 0; i < sentences.Length; i++)
                    {
                        formattedText.Append(sentences[i]);

                        // Add a line break after every second sentence
                        if ((i + 1) % 2 == 0)
                        {
                            formattedText.AppendLine(); // Add a line break
                        }
                        else
                        {
                            formattedText.Append(" "); // Add a space between sentences
                        }
                    }

                    string outputText = formattedText.ToString();

                    if (!string.IsNullOrEmpty(outputText))
                    {
                        outputText = HtmlEntity.DeEntitize(outputText);
                        desc.Add(outputText);
                    }
                }
            }

            product.Description = string.Join("\n", desc.ToArray());
        }

        private void GetFeatures(HtmlDocument htmlDoc)
        {
            //extract features

            var featuresNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='features']");
            if (featuresNode != null)
            {
                //get the find documents hyperlink
                var firstChild = featuresNode.SelectSingleNode("./*");
                if (firstChild != null)
                {
                    string findDocLink = firstChild.GetAttributeValue("href", "");
                    featuresNode.RemoveChild(firstChild);
                    product.FindDocLink = findDocLink;
                }
                List<string> features = new List<string>();
                // Iterate through text nodes within the featuresNode
                foreach (var textNode in featuresNode.DescendantsAndSelf().Where(n => n.NodeType == HtmlNodeType.Text))
                {
                    string str = textNode.InnerText.Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = HtmlEntity.DeEntitize(str);
                        features.Add(str);
                    }
                }
                product.Features = string.Join("\n", features.ToArray());
            }
        }
        #endregion

    }
}
