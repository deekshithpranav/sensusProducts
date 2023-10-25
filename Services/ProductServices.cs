using sensusProducts.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace sensusProducts.Service
{
    public class ProductServices : IProductService
    {
        public void AddProduct(Product product)
        {

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";
            string query = @"
                 SELECT TOP 1 PID
                 FROM productIDs
                 ORDER BY PID DESC;
            ";
            int productID=0;
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
                        object str = command.ExecuteScalar() == null? 0: command.ExecuteScalar();
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
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        public void DeleteProductInDB(int PID)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";
            string query;
            
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    query = "DELETE FROM PDetails WHERE PID = @IntValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", PID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }
                    
                    query = "DELETE FROM ProductImgLinks WHERE PID = @IntValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", PID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                    query = "DELETE FROM productIDs WHERE PID = @IntValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", PID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                    query = "DELETE FROM PUtility WHERE PID = @IntValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", PID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }

        public List<Product> LoadProductsFromDB()
        {
            List<Product> products = new List<Product>();

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";
            string query = @"
                 SELECT * FROM productIDs;
            ";
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int _id = int.Parse(reader["PID"].ToString());
                                string _Name = reader["PName"].ToString();
                                


                                Product product = new Product
                                    {
                                        // Map database columns to Product properties
                                        Id = _id,
                                        Name = _Name,
                                        
                                        // Map other properties here...
                                    };
                                    products.Add(product);
                                }
                            }
                        foreach(Product product in products)
                        {
                            int _id = product.Id;
                            string _PFeatures = "";
                            string _PDesc = "";
                            string _DocLink = "";
                            string _FindDocLink = "";
                            List<string> _ImgLinks = new List<string>();
                            List<UtilityType> _utilityTypes = new List<UtilityType>();

                            query = @"SELECT * FROM PDetails WHERE PID = @IntValue;";

                            using (SqlCommand command2 = new SqlCommand(query, connection))
                            {
                                // Provide parameter values
                                command2.Parameters.AddWithValue("@IntValue", _id);

                                using (SqlDataReader reader2 = command2.ExecuteReader())
                                {
                                    while (reader2.Read())
                                    {
                                        _PDesc = reader2["PDesc"].ToString();
                                        _PFeatures = reader2["PFeatures"].ToString();
                                        _DocLink = reader2["DocLink"].ToString();
                                        _FindDocLink = reader2["FindDocLink"].ToString();
                                    }
                                }
                            }

                            query = @"SELECT * FROM ProductImgLinks WHERE PID = @IntValue;";

                            using (SqlCommand command2 = new SqlCommand(query, connection))
                            {
                                // Provide parameter values
                                command2.Parameters.AddWithValue("@IntValue", _id);

                                using (SqlDataReader reader2 = command2.ExecuteReader())
                                {
                                    while (reader2.Read()) // Check if there is data to read
                                    {
                                        string temp = reader2["ImgLink"].ToString();
                                        _ImgLinks.Add(temp);
                                    }
                                }
                            }

                            query = @"SELECT * FROM PUtility WHERE PID = @IntValue;";

                            using (SqlCommand command2 = new SqlCommand(query, connection))
                            {
                                // Provide parameter values
                                command2.Parameters.AddWithValue("@IntValue", _id);
                                using (SqlDataReader reader2 = command2.ExecuteReader())
                                {
                                    while (reader2.Read()) // Check if there is data to read
                                    {
                                        Enum.TryParse(reader2["Utility"].ToString(), out UtilityType temp);
                                        _utilityTypes.Add(temp);
                                    }
                                }
                            }    

                            product.Description = _PDesc;
                            product.Features = _PFeatures;
                            product.DocLink = _DocLink;
                            product.FindDocLink = _FindDocLink;
                            product.ImgLinks = _ImgLinks;
                            product.UtilityTypes = _utilityTypes;
                        }
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return products;
        }

        public void UpdateProduct(Product product, int OldProductID)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";
            string query = @"
                UPDATE productIDs
                SET PName = @StringValue
                WHERE PID = @IntValue;
            ";
            try
            {
                // Create a SqlConnection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a SqlCommand
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@IntValue", OldProductID);
                        command.Parameters.AddWithValue("@StringValue", product.Name);
                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                    query = @"
                        UPDATE PDetails
                        SET PDesc = @StringValue,
                        PFeatures = @StringValue2,
                        DocLink = @StringValue3,
                        FindDocLink = @StringValue4
                        WHERE PID = @IntValue;
                    ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IntValue", OldProductID);
                        command.Parameters.AddWithValue("@StringValue", product.Name);
                        command.Parameters.AddWithValue("@StringValue2", product.Features);
                        command.Parameters.AddWithValue("@StringValue3", product.DocLink);
                        command.Parameters.AddWithValue("@StringValue4", product.FindDocLink);
                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }


                    //delete the image data in DB
                    query = "DELETE FROM ProductImgLinks WHERE PID = @IntValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Provide parameter values
                        command.Parameters.AddWithValue("@IntValue", OldProductID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                    //delete the utility data in DB
                    query = "DELETE FROM PUtility WHERE PID = @IntValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IntValue", OldProductID);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.Write(rowsAffected);
                    }

                    query = "INSERT INTO PUtility (PID, Utility) VALUES (@IntValue, @StringValue)";
                    foreach (var utility in product.UtilityTypes)
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {

                            command.Parameters.AddWithValue("@IntValue", OldProductID);
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
                            command.Parameters.AddWithValue("@IntValue", OldProductID);
                            command.Parameters.AddWithValue("@StringValue", imgLink.ToString());
                            int rowsAffected = command.ExecuteNonQuery();
                            Debug.Write(rowsAffected);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool ProductExistsInDB(string productTitle)
        {
            //check if the product is already present in db

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=sensus;Trusted_Connection=True;";

            // SQL query to check if the title exists in the database
            string query = "SELECT COUNT(*) FROM productIDs WHERE PName = @Title";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", productTitle);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Product is already present in the database
                            System.Windows.Forms.MessageBox.Show("Product is already present in the database");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Debug.WriteLine("Error: " + ex.Message);
            }
            return false;
        }
    }

}