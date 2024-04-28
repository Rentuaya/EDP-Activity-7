using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace RentalApartment
{
    public partial class SignUpForm : Form
    {
        string connectionString = "Server=localhost;Port=3306;Database=apartment;Uid=root;Pwd=rentuaya2022;";
        public SignUpForm()
        {
            InitializeComponent();
        }

        private void SignUpForm_Load(object sender, EventArgs e)
        {

        }

        private void signUp_Click(object sender, EventArgs e)
        {
            loginForm formlogIn = new loginForm();
            formlogIn.Show();
            this.Hide();
        }

        private void signupBtn_Click(object sender, EventArgs e)
        {
            string email = emailBox.Text;
            string username = userBox.Text;
            string firstName = firstnameBox.Text;
            string lastName = lastnameBox.Text;
            string createpass = newpassBox.Text;
            string confirmPass = confirmpassBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(createpass) || string.IsNullOrEmpty(confirmPass))
            {
                MessageBox.Show("Please fill in all fields.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (createpass != confirmPass)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (user_account(email, username, firstName, lastName, createpass))
            {
                MessageBox.Show("Sign up successful!", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                loginForm formlogin = new loginForm();
                formlogin.Show();
            }
            else
            {
                MessageBox.Show("Failed to sign up. Please try again.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool user_account(string email, string username, string firstName, string lastName, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO users (email, username, firstName, lastName, password) VALUES (@email, @username, @firstName, @lastName, @password)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@password", password);
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

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
