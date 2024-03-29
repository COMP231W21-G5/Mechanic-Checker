﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MechanicChecker.Models
{
    public class SellerContext
    {
        public string ConnectionString { get; set; }
        public SellerContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        public void DeleteSellerByCompanyName(string companyName)
        {
            string command = "DELETE FROM Seller WHERE CompanyName = '" + companyName + "';";

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();
            myCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB
            myCommand.Connection.Close();

        }

        public bool saveSeller(Seller seller)
        {
            bool isPassed = true;
            try
            {
                // Note: The Application date can be null because it is being generated in the DB
                string stringCmd =
                    "INSERT INTO Seller(Username, FirstName, LastName, Email, PasswordHash, AccountType, IsApproved, CompanyName, BusinessPhone, CompanyLogoUrl, WebsiteUrl, Application, ApplicationDate, ActivationCode) " +
                    "VALUES ('"
                    + seller.UserName.Replace("'", "''") + "', '"
                    + seller.FirstName.Replace("'", "''") + "', '"
                    + seller.LastName.Replace("'", "''") + "', '"
                    + seller.Email.Replace("'", "''") + "', '"
                    + seller.PasswordHash + "', '"
                    + seller.AccountType + "', "
                    + Convert.ToInt32(seller.IsApproved) + ", '"
                    + seller.CompanyName.Replace("'", "''") + "', '"
                    + seller.BusinessPhone + "', '"
                    + seller.CompanyLogoUrl + "', '"
                    + seller.WebsiteUrl + "', '"
                    + seller.Application.Replace("'", "''") + "', '"
                    /*
                     * Even though application date is automatically generated via the database the database operates with a different timezone.
                     * Better to manually set the date dynamically on the website.
                     */
                    + seller.ApplicationDate.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                    + seller.ActivationCode + "')";

                MySqlConnection myConnection = GetConnection();
                MySqlCommand myCommand = new MySqlCommand(stringCmd);
                myCommand.Connection = myConnection;
                myConnection.Open();
                myCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB
                myCommand.Connection.Close();

            }
            catch (Exception e)
            {
                isPassed = false;
                return isPassed;
            }
            return isPassed;
        }

        public bool activateSellerAccount(string activationId)
        {
            bool isPassed = false;

            string command = "SELECT Username, IsApproved from Seller where ActivationCode = '" + activationId + "';";

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();

            // read response
            using (var reader = myCommand.ExecuteReader())
            {
                // will only run if the query returns a record
                isPassed = reader.Read() && (Convert.ToBoolean(reader["IsApproved"]) == false);
            }

            if (isPassed)
            {
                string stringCmd = "UPDATE Seller SET IsApproved = 1, ApprovalDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE ActivationCode = '" + activationId + "';";

                MySqlCommand secondCommand = new MySqlCommand(stringCmd);
                secondCommand.Connection = myConnection;
                secondCommand.ExecuteNonQuery(); // ExecuteNonQuery is required to update, insert and delete from the DB 
            }
            myConnection.Close();

            return isPassed;

        }
        public bool verifyUserByEmail(string email, out string sellerName)
        {
            bool isPassed = false;

            string command = "SELECT Username, Email from Seller where Email = '" + email + "';";

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();

            // read response
            using (var reader = myCommand.ExecuteReader())
            {
                // will only run if the query returns a record
                isPassed = reader.Read();
                if (isPassed)
                {
                    sellerName = reader["Username"].ToString();
                }
                else
                {
                    sellerName = null;
                }

            }

            return isPassed;
        }

        public bool updatePassword(string password, string resetPasswordCode)
        {
            bool isPassed = false;

            string command = "UPDATE Seller SET PasswordHash = '" + password + "' WHERE ResetPasswordCode = '" + resetPasswordCode + "';";

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();
            isPassed = Convert.ToBoolean(myCommand.ExecuteNonQuery()); // only 1 row should be affected; if failed then 0 rows were affected

            // remove ResetPasswordCode from DB since its not needed anymore
            // also prevents anyone from resetting the password with the same link again
            if (isPassed)
            {
                myCommand.CommandText = "UPDATE Seller SET ResetPasswordCode = " + "NULL" + " WHERE ResetPasswordCode = '" + resetPasswordCode + "';";
                isPassed = Convert.ToBoolean(myCommand.ExecuteNonQuery()); // only 1 row should be affected; if failed then 0 rows were affected
                myCommand.Connection.Close();
            }
            else
            {
                myCommand.Connection.Close();
            }

            return isPassed;
        }

        public bool updateResetPasswordCode(string resetPasswordCode, string sellerEmail)
        {
            bool isPassed = false;

            string command = "UPDATE Seller SET ResetPasswordCode = '" + resetPasswordCode + "' WHERE Email = '" + sellerEmail + "';";

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();
            isPassed = Convert.ToBoolean(myCommand.ExecuteNonQuery()); // only 1 row should be affected; if failed then 0 rows were affected
            myCommand.Connection.Close();

            return isPassed;
        }

        public string GetUserIdByUserName(string userName)
        {


            string command = "SELECT SellerId from Seller where Username = '" + userName + "';";
            string userId = null;

            // send query to database
            MySqlConnection myConnection = GetConnection();
            MySqlCommand myCommand = new MySqlCommand(command);
            myCommand.Connection = myConnection;
            myConnection.Open();

            // read response
            using (var reader = myCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    userId = reader["SellerId"].ToString();
                }
            }

            return userId;
        }
        public Seller GetSeller(string userId)
        {
            Seller seller = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Seller " +
                    "where Username = '" + userId + "' or Email = '" + userId + "';", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        seller = new Seller()
                        {
                            AccountType = reader["AccountType"].ToString(),
                            Application = reader["Application"].ToString(),
                            ApprovalDate = DateTime.Now,
                            BusinessPhone = reader["BusinessPhone"].ToString(),
                            CompanyLogoUrl = reader["CompanyLogoUrl"].ToString(),
                            CompanyName = reader["CompanyName"].ToString(),
                            Email = reader["Email"].ToString(),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            IsApproved = Convert.ToBoolean(reader["IsApproved"]),
                            UserName = reader["Username"].ToString(),
                            WebsiteUrl = reader["WebsiteUrl"].ToString(),
                            ActivationCode = reader["ActivationCode"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            ApplicationDate = DateTime.Now,
                            SellerId = Convert.ToInt32(reader["SellerId"]),
                            ResetPasswordCode = reader["ResetPasswordCode"].ToString()
                        };

                    }
                }
            }

            return seller;
        }

        public Seller GetSellerByCompanyName(string companyName)
        {
            Seller seller = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Seller " +
                "where CompanyName = '" + companyName + "';", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        seller = new Seller()
                        {
                            AccountType = reader["AccountType"].ToString(),
                            Application = reader["Application"].ToString(),
                            ApprovalDate = DateTime.Now,
                            BusinessPhone = reader["BusinessPhone"].ToString(),
                            CompanyLogoUrl = reader["CompanyLogoUrl"].ToString(),
                            CompanyName = reader["CompanyName"].ToString(),
                            Email = reader["Email"].ToString(),//"hellokitty@yahoo.ca",
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            IsApproved = Convert.ToBoolean(reader["IsApproved"]),//false,
                            UserName = reader["Username"].ToString(),//"123hello",
                            WebsiteUrl = reader["WebsiteUrl"].ToString(),
                            ActivationCode = reader["ActivationCode"].ToString(),//Guid.NewGuid().ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),//"password",
                            ApplicationDate = DateTime.Now,
                            SellerId = Convert.ToInt32(reader["SellerId"]),
                            ResetPasswordCode = reader["ResetPasswordCode"].ToString()
                        };

                    }
                }
            }

            return seller;
        }

        public Seller GetSellerByPhoneNumber(string phoneNumber)
        {
            Seller seller = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Seller " +
                "where BusinessPhone = '" + phoneNumber + "';", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        seller = new Seller()
                        {
                            AccountType = reader["AccountType"].ToString(),
                            Application = reader["Application"].ToString(),
                            ApprovalDate = DateTime.Now,
                            BusinessPhone = reader["BusinessPhone"].ToString(),
                            CompanyLogoUrl = reader["CompanyLogoUrl"].ToString(),
                            CompanyName = reader["CompanyName"].ToString(),
                            Email = reader["Email"].ToString(),//"hellokitty@yahoo.ca",
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            IsApproved = Convert.ToBoolean(reader["IsApproved"]),//false,
                            UserName = reader["Username"].ToString(),//"123hello",
                            WebsiteUrl = reader["WebsiteUrl"].ToString(),
                            ActivationCode = reader["ActivationCode"].ToString(),//Guid.NewGuid().ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),//"password",
                            ApplicationDate = DateTime.Now,
                            SellerId = Convert.ToInt32(reader["SellerId"]),
                            ResetPasswordCode = reader["ResetPasswordCode"].ToString()
                        };

                    }
                }
            }

            return seller;
        }
    }
}
