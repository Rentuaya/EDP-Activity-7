using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RentalApartment
{
    public partial class usersForm : Form
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=apartment;Uid=root;Pwd=rentuaya2022;";
        private DataTable _userData;
        private DataGridView _dataGridView;

        public usersForm()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadUserData();
            AddDataGridViewToPanel();
        }
        private void AddDataGridViewToPanel()
        {
            panel1.Controls.Add(_dataGridView);
        }

        private void LoadUserData()
        {
            _userData = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string query = "SELECT username, email, firstname, lastname, is_active FROM users";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {

                    // Fill the DataTable with data from the database
                    adapter.Fill(_userData);

                    // Bind the DataTable to the DataGridView
                    _dataGridView.DataSource = _userData;
                }

                /*
                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(_userData);
                    dataGridView1.DataSource = _userData;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                */
            }
        }
        private void InitializeDataGridView()
        {
            _dataGridView = new DataGridView();
            _dataGridView.Dock = DockStyle.Fill;
            _dataGridView.AutoGenerateColumns = true;
            _dataGridView.AllowUserToAddRows = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _dataGridView.Rows.Count)
            {
                DataGridViewRow selectedRow = _dataGridView.Rows[e.RowIndex];

                userBox.Text = selectedRow.Cells["username"].Value.ToString();
                emailBox.Text = selectedRow.Cells["email"].Value.ToString();
                firstNBox.Text = selectedRow.Cells["firstName"].Value.ToString();
                lastNBox.Text = selectedRow.Cells["lastName"].Value.ToString();
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            UpdateChanges();
            LoadUserData();
        }

        private void UpdateChanges() {
        
            string email = emailBox.Text;
            string username = userBox.Text;
            string firstName = firstNBox.Text;
            string lastName = lastNBox.Text;


            foreach (DataRow row in _userData.Rows)
            {

                if (row["email"].ToString() == email)
                {

                    row["UserName"] = username;
                    row["FirstName"] = firstName;
                    row["LastName"] = lastName;


                    break;
                }
            }
            _dataGridView.DataSource = _userData;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                string query = "UPDATE users SET username = @username, firstname = @firstName, lastName = @lastName WHERE email = @email";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@email", email);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User information updated successfully!");
                    }
                    else
                    {
                        MessageBox.Show("No rows were updated.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating user information: " + ex.Message);
                }
            }
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string searchCriteria = searchBox.Text;


            string query = "SELECT * FROM users WHERE email LIKE @search OR username LIKE @search OR firstname LIKE @search OR lastname LIKE @search";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@search", "%" + searchCriteria + "%");

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable searchResults = new DataTable();
                    adapter.Fill(searchResults);
                    _dataGridView.DataSource = searchResults;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            // Display a confirmation message
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check the user's response
            if (result == DialogResult.Yes)
            {
                // Create an instance of the login form
                loginForm backloginForm = new loginForm();

                // Show the login form
                backloginForm.Show();

                // Close the current form (About)
                this.Close();
            }
            // If the user chooses No, do nothing
        }
    }
}
