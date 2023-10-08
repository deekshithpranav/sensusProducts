using HtmlAgilityPack;
using sensusProducts.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sensusProducts.Service
{
    public class ProductServices : IProductService
    {
        public void AddProduct(Product product)
        {

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";
            string query = @"
                 SELECT COUNT(*) FROM productIDs;
            ";
            int productID;
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                        
                    // Create a SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the SQL command to create the table
                        object str = command.ExecuteScalar();
                        productID = (int)str + 1;

                    }

                    query = "INSERT INTO productIDs (PID, PName) VALUES (@IntValue, @StringValue)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", productID);
                        command.Parameters.AddWithValue("@StringValue", product.Name);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                    

                    query = "INSERT INTO PUtility (PID, Utility) VALUES (@IntValue, @StringValue)";
                    foreach (var utility in product.UtilityTypes)
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.AddWithValue("@IntValue", productID);
                            command.Parameters.AddWithValue("@StringValue", utility.ToString());
                            int rowsAffected = command.ExecuteNonQuery();
                            Debug.Write(rowsAffected);
                        }
                    }
                    query = "INSERT INTO ProductImgLinks (PID, ImgLink) VALUES (@IntValue, @StringValue)";
                    foreach (var imgLink in product.ImgLinks)
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                    {
                            command.Parameters.AddWithValue("@IntValue", productID);
                            command.Parameters.AddWithValue("@StringValue", imgLink.ToString());
                            int rowsAffected = command.ExecuteNonQuery();
                            Debug.Write(rowsAffected);
                        }
                    }
                    query = "INSERT INTO PDetails (PID, PDesc, PFeatures, DocLink, FindDocLink) VALUES (@IntValue, @StringValue1, @StringValue2, @StringValue3, @StringValue4)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", productID);
                        command.Parameters.AddWithValue("@StringValue1", product.Description);
                        command.Parameters.AddWithValue("@StringValue2", product.Features);
                        command.Parameters.AddWithValue("@StringValue3", product.DocLink);
                        command.Parameters.AddWithValue("@StringValue4", product.FindDocLink == null ? "":product.FindDocLink);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void DeleteProduct()
        {
            throw new NotImplementedException();
        }

        public void LoadProduct()
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct()
        {
            throw new NotImplementedException();
        }
    }

}