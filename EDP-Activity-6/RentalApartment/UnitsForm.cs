using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentalApartment
{
    public partial class UnitsForm : Form
    {
        public UnitsForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            // Display a confirmation message
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check the user's response
            if (result == DialogResult.Yes)
            {
                string connectionString = "Server=localhost;Port=3306;Database=apartment;Uid=root;Pwd=rentuaya2022;";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string selectQuery = "SELECT username FROM users WHERE is_active = 1";
                        MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);
                        string loggedInUsername = selectCommand.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(loggedInUsername))
                        {
                            // Update the is_active field to false upon logout
                            string updateQuery = "UPDATE users SET is_active = 0 WHERE username = @username";
                            MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@username", loggedInUsername);
                            updateCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            MessageBox.Show("Error: Unable to retrieve logged-in user information.");
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }

                // Create an instance of the login form
                loginForm backloginForm = new loginForm();

                // Show the login form
                backloginForm.Show();

                // Close the current form (About)
                this.Close();
            }
            // If the user chooses No, do nothing
        }

        private void unitInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You are already on the Unit Information screen.");
        }

        private void label22_Click(object sender, EventArgs e)
        {
            aboutForm formAbout = new aboutForm();
            this.Hide();
            formAbout.Show();
        }

        private void sideUser_Click(object sender, EventArgs e)
        {
            AccountForm formUser = new AccountForm();
            this.Hide();
            formUser.Show();
        }

        private void sideDashboard_Click(object sender, EventArgs e)
        {
            formRental formRental = new formRental();
            this.Hide();
            formRental.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm();
            this.Hide();
            paymentForm.Show();
        }
    }
}
