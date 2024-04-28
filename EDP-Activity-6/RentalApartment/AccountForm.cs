using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RentalApartment
{
    public partial class AccountForm : Form
    {
        // Connection string to your MySQL database
        private string connectionString = "Server=localhost;Database=apartment;Uid=root;Pwd=rentuaya2022;";
        private DataTable _userData;
        private DataGridView dataGridView1;

        public AccountForm()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadUsersData(); // Call the method to load user data into the DataGridView
        }
        private void InitializeDataGridView()
        {
            // Create a new instance of DataGridView
            dataGridView1 = new DataGridView();

            // Set properties of the DataGridView
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Dock = DockStyle.Fill;

            // Add the DataGridView to the form's Controls collection
            panel3.Controls.Add(dataGridView1);
            dataGridView1.Location = new Point(10, 10);

            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void LoadUsersData()
        {
            _userData = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT username, email, firstname, lastname, is_active FROM users";
                MySqlCommand command = new MySqlCommand(query, connection);
                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(_userData);
                    // Add a new column for checkboxes representing is_active field
                    _userData.Columns.Add("Is Active", typeof(bool));

                    foreach (DataRow row in _userData.Rows)
                    {
                        // Populate the Is Active column with checkbox values
                        row["Is Active"] = Convert.ToBoolean(row["is_active"]);
                    }

                    // Remove the original is_active column from the DataTable
                    _userData.Columns.Remove("is_active");

                    dataGridView1.DataSource = _userData;
                    dataGridView1.ReadOnly = true;
                    /*
                    // Add a DataGridViewCheckBoxColumn to display checkboxes
                    DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                    checkBoxColumn.Name = "Is Active";
                    checkBoxColumn.HeaderText = "Is Active";
                    checkBoxColumn.DataPropertyName = "Is Active";
                    dataGridView1.Columns.Insert(5, checkBoxColumn);
                    */
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm formAbout = new aboutForm();
            this.Hide();
            formAbout.Show();
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

        private void label4_Click(object sender, EventArgs e)
        {
            formRental formRental = new formRental();
            this.Hide();
            formRental.Show();
        }

        private void label22_Click(object sender, EventArgs e)
        {
            aboutForm formAbout = new aboutForm();
            this.Hide();
            formAbout.Show();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                userBox.Text = selectedRow.Cells["username"].Value.ToString();
                emailBox.Text = selectedRow.Cells["email"].Value.ToString();
                firstNBox.Text = selectedRow.Cells["firstname"].Value.ToString();
                lastNBox.Text = selectedRow.Cells["lastname"].Value.ToString();
            }
        }

        private void UpdateChanges()
        {

            string usernameEdit = userBox.Text;
            string emailEdit = emailBox.Text;
            string firstnameEdit = firstNBox.Text;
            string lastnameEdit = lastNBox.Text;


            foreach (DataRow row in _userData.Rows)
            {

                if (row["username"].ToString() == usernameEdit)
                {

                    row["email"] = emailEdit;
                    row["firstname"] = firstnameEdit;
                    row["lastname"] = lastnameEdit;

                    break;
                }
            }
            dataGridView1.DataSource = _userData;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE users SET email = @email, firstname = @firstname, lastname = @lastname WHERE username = @username";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", emailEdit);
                command.Parameters.AddWithValue("@firstname", firstnameEdit);
                command.Parameters.AddWithValue("@lastname", lastnameEdit);
                command.Parameters.AddWithValue("@username", usernameEdit);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User information updated successfully!", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No rows were updated.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating user information: " + ex.Message);
                }
            }
        }

        private void updateBtn_Click(object sender, EventArgs e)
        {
            UpdateChanges();

            LoadUsersData();
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string searchText = searchBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                // Filter the DataTable based on search text
                DataTable filteredTable = _userData.Clone(); // Create a clone of the original DataTable structure
                foreach (DataRow row in _userData.Rows)
                {
                    if (row["username"].ToString().Contains(searchText) ||
                        row["email"].ToString().Contains(searchText) ||
                        row["firstname"].ToString().Contains(searchText) ||
                        row["lastname"].ToString().Contains(searchText))
                    {
                        filteredTable.ImportRow(row);
                    }
                }

                // Update the DataGridView with filtered data
                dataGridView1.DataSource = filteredTable;
            }
            else
            {
                // If search text is empty, reload all data
                LoadUsersData();
            }

        }

        private void label5_Click(object sender, EventArgs e)
        {
            UnitsForm unitsForm = new UnitsForm();
            this.Hide();
            unitsForm.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm();
            this.Hide();
            paymentForm.Show();
        }

        private void sideUser_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You are already on the User's Management screen.");
        }
    }
}
