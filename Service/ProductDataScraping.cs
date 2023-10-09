using HtmlAgilityPack;
using sensusProducts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sensusProducts.Service
{
    public class ProductDataScraping
    {
        [Obsolete]
        public async Task<Product> ScrapData(string url)
        {
            Product product = new Product();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string html = await client.GetStringAsync(url);

                    // Load the HTML content into an HtmlDocument
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    // Extract product title 
                    var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//section[@class='headline-bluebar']//h1");
                    string titleText = titleNode.InnerText;
                    titleText = HtmlEntity.DeEntitize(titleText);
                    product.Name = titleText;
                    Console.WriteLine("Title: " + titleText);


                    //extract Description
                    var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode(".//div[@id='description']");
                    List<string> desc = new List<string>();
                    if (descriptionNode != null)
                    {
                        // Iterate through text nodes within the descriptionnode
                        foreach (var textNode in descriptionNode.DescendantNodesAndSelf().Where(n => n.NodeType == HtmlNodeType.Text))
                        {
                            string str = textNode.InnerText.Trim();
                            if (!string.IsNullOrEmpty(str))
                            {
                                str = HtmlEntity.DeEntitize(str);
                                desc.Add(str);
                            }
                        }
                    }

                    product.Description = string.Join("\n", desc.ToArray());


                    //extract features

                    var featuresNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='features']");
                    if (featuresNode != null)
                    {
                        //get the find documents hyperlink
                        var firstChild = featuresNode.SelectSingleNode("./*");
                        if (firstChild != null)
                        {
                            string findDocLink = firstChild.GetAttributeValue("href", "");
                            Console.WriteLine("Find Documents Link: " + findDocLink);
                            featuresNode.RemoveChild(firstChild);
                        }
                        List<string> features = new List<string>();
                        // Iterate through text nodes within the featuresNode
                        foreach (var textNode in featuresNode.DescendantNodesAndSelf().Where(n => n.NodeType == HtmlNodeType.Text))
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


                    //Documents

                    var docAnchorNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='features']//div[@class='summary']//a");
                    // gets the attribute href from the anchor tag linking to document\
                    string link = "";
                    if (docAnchorNode != null)
                    {
                        Console.WriteLine("Documents PDF link: ");
                        link = docAnchorNode.GetAttributeValue("href", "");
                    }

                    product.DocLink = link;


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
                                imgHyperLinkList.Add(img);
                            }
                        }

                        //modifying the hyperlink to have high resolution images
                        foreach (var item in imgHyperLinkList)
                        {
                            Console.WriteLine("Multiple Images links: ");
                            var fullsizeimage = item.Substring(0, item.Length - 12) + ".jpg";
                            Console.WriteLine(fullsizeimage);
                        }
                    }

                    // single-display-image product
                    else
                    {
                        Console.WriteLine("Single Image link: ");

                        var productImage = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='product-image']");
                        var img = productImage.SelectSingleNode(".//img").GetAttributeValue("src", "");
                        var fullsizeimage = img.Substring(0, img.Length - 12) + ".jpg";
                        imgHyperLinkList.Add(fullsizeimage);
                        Console.WriteLine(fullsizeimage);
                    }
                    product.ImgLinks = imgHyperLinkList;

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
                    Console.WriteLine(joinedList);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                return product;
            }
        }
    }
}
