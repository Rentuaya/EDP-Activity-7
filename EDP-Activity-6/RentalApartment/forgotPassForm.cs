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
    public partial class forgotPassForm : Form
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=apartment;Uid=root;Pwd=rentuaya2022;";
        public forgotPassForm()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string email = emailPRBox.Text;
            string newPassword = newPRBox.Text;
            string confirmPassword = confirmPRBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.");
                return;
            }

            if (ResetPassword(email, newPassword))
            {
                MessageBox.Show("Your password has been reset.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide(); // Hide the current ForgotPass form
                loginForm loginForm = new loginForm();
                loginForm.Show(); // Show the LogIn form again
            }
            else
            {
                MessageBox.Show("Failed to reset password. Please try again later.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool ResetPassword(string email, string newPassword)
        {
            string query = "UPDATE users SET password = @newPassword WHERE email = @email";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                try
                {
                    command.Parameters.AddWithValue("@newPassword", newPassword);
                    command.Parameters.AddWithValue("@email", email);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            loginForm formlogin = new loginForm();
            formlogin.Show();
        }
    }
}
