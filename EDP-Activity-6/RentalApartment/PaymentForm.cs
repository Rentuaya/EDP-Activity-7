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
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Application = System.Windows.Forms.Application;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using DocumentFormat.OpenXml.Wordprocessing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.Office.Core;

namespace RentalApartment
{
    public partial class PaymentForm : Form
    {
        private string connectionString = "Server=localhost;Database=apartment;Uid=root;Pwd=rentuaya2022;";
        private DBConnection mydbConnection;
        private System.Data.DataTable _unitsData;
        public PaymentForm()
        {
            InitializeComponent();
            mydbConnection = new DBConnection("localhost", "apartment", "root", "rentuaya2022");
            unitsdataGrid.CellClick += unitsdataGrid_CellClick;
        }
        private void unitsdataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.RowIndex < unitsdataGrid.Rows.Count)
            {
                statusBox.Enabled = true;
                rentBox.Enabled = true;
                duedateBox.Enabled = true;

                DataGridViewRow selectedRow = unitsdataGrid.Rows[e.RowIndex];

                unitBox.Text = selectedRow.Cells["UNIT NAME"].Value.ToString();
                string option = selectedRow.Cells["STATUS"].Value.ToString();
                if (option == "rented")
                {
                    rentStatus.Checked = true;
                }
                else if (option == "available")
                {
                    availStatus.Checked = true;
                }

                rentBox.Text = selectedRow.Cells["RENT AMOUNT"].Value.ToString();

                object dateTimeValue = selectedRow.Cells["DUE DATE"].Value;
                if (dateTimeValue != null && dateTimeValue != DBNull.Value)
                {
                    duedateBox.Value = Convert.ToDateTime(selectedRow.Cells["DUE DATE"].Value);
                }
                else
                {
                    DateTime defaultDateTime = new DateTime(2024, 01, 01);
                    duedateBox.Value = defaultDateTime;
                }
            }
        }

        private void UpdateChanges()
        {
            // Update DataGridView based on user input
            string unitEdit = unitBox.Text;
            string option = rentStatus.Checked ? "rented" : "available";
            string rentEdit = rentBox.Text;
            string duedatetextEdit = duedateBox.Value.ToString("yyyy-MM-dd");

            foreach (DataRow row in _unitsData.Rows)
            {
                if (row["UNIT NAME"].ToString() == unitEdit)
                {
                    row["STATUS"] = option;
                    row["RENT AMOUNT"] = rentEdit;
                    row["DUE DATE"] = duedatetextEdit;

                    break;
                }
            }

            // Update DataGridView with the changes
            unitsdataGrid.DataSource = _unitsData;

            // Update database
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "UPDATE units SET status = @statusValue, rent_amount = @rentValue, due_date = @duedateValue WHERE unit_name = @unitValue";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@statusValue", option);
                command.Parameters.AddWithValue("@rentValue", rentEdit);
                command.Parameters.AddWithValue("@duedateValue", duedatetextEdit);
                command.Parameters.AddWithValue("@unitValue", unitEdit);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Units Data updated successfully!", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void editBtn_Click(object sender, EventArgs e)
        {
            UpdateChanges();
            LoadData();
        }

        private void LoadData()
        {
            string unitsquery = "SELECT unit_name as 'UNIT NAME', status as 'STATUS', rent_amount as 'RENT AMOUNT', due_date as 'DUE DATE' FROM units";
            string expensequery = "SELECT name as 'EXPENSE NAME', expense_total as 'EXPENSE TOTAL' FROM expenses";
            _unitsData = mydbConnection.ExecuteQuery(unitsquery);
            System.Data.DataTable dt2 = mydbConnection.ExecuteQuery(expensequery);
            if (_unitsData != null && dt2 != null)
            {
                unitsdataGrid.DataSource = _unitsData;
                expensedataGrid.DataSource = dt2;
            }
            else
            {
                MessageBox.Show("Unable to load Data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void addBtn_Click(object sender, EventArgs e)
        {
            paymentPanel.Hide();
            addunitPanel.Show();
            //Active default rented option on Status
            rentAddStatus.Checked = true;
        }
        private void backunitBtn_Click(object sender, EventArgs e)
        {
            paymentPanel.Show();
            addunitPanel.Hide();
        }

        private void addunitBtn_Click(object sender, EventArgs e)
        {
            string unitAdd = unitaddBox.Text;
            string rentAdd = rentaddBox.Text;
            string optionAdd = rentAddStatus.Checked ? "rented" : "available";
            string duedateAdd = duedateaddBox.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(unitAdd) || string.IsNullOrEmpty(rentAdd))
            {
                MessageBox.Show("Please fill in all fields.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (addUnits(unitAdd, rentAdd, optionAdd, duedateAdd))
            {
                MessageBox.Show("Adding Units Successful!", "Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                paymentPanel.Show();
                LoadData();
                addunitPanel.Hide();
            }
            else
            {
                MessageBox.Show("Failed to add Units. Please try again.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool addUnits(string unittoAdd, string renttoAdd, string optiontoAdd, string duedatetoAdd)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO units (unit_name, rent_amount, status, due_date) VALUES (@unitaddValue, @rentaddValue, @statusaddValue, @duedateaddValue)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@unitaddValue", unittoAdd);
                    command.Parameters.AddWithValue("@rentaddValue", renttoAdd);
                    command.Parameters.AddWithValue("@statusaddValue", optiontoAdd);
                    command.Parameters.AddWithValue("@duedateaddValue", duedatetoAdd);
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

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            // Check if any cell is selected
            if (unitsdataGrid.SelectedCells.Count > 0)
            {
                // Get the index of the selected cell
                int selectedRowIndex = unitsdataGrid.SelectedCells[0].RowIndex;

                // Get the unit name from the selected row
                string unitToDelete = unitsdataGrid.Rows[selectedRowIndex].Cells["UNIT NAME"].Value.ToString();

                // Delete the row from the database
                if (DeleteUnitFromDatabase(unitToDelete))
                {
                    // Remove the row from the DataGridView
                    unitsdataGrid.Rows.RemoveAt(selectedRowIndex);
                    MessageBox.Show("Unit deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    unitBox.Text = "";
                    rentBox.Text = "";
                    rentBox.Enabled = false;
                    statusBox.Enabled = false;
                    DateTime defaultDateTime = new DateTime(2024, 01, 01);
                    duedateBox.Value = defaultDateTime;
                    duedateBox.Enabled = false;

                }
                else
                {
                    MessageBox.Show("Failed to delete unit from database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a cell to delete the corresponding row.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool DeleteUnitFromDatabase(string unitName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM units WHERE unit_name = @unitName";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@unitName", unitName);
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

        private void unitsearchBtn_Click(object sender, EventArgs e)
        {
            string unitsearchText = unitsearchBox.Text.Trim();

            if (!string.IsNullOrEmpty(unitsearchText))
            {
                // Filter the DataTable based on search text
                System.Data.DataTable filteredTable = _unitsData.Clone(); // Create a clone of the original DataTable structure
                foreach (DataRow row in _unitsData.Rows)
                {
                    if (row["UNIT NAME"].ToString().Contains(unitsearchText) ||
                        row["STATUS"].ToString().Contains(unitsearchText) ||
                        row["RENT AMOUNT"].ToString().Contains(unitsearchText) ||
                        row["DUE DATE"].ToString().Contains(unitsearchText))
                    {
                        filteredTable.ImportRow(row);
                    }
                }

                // Update the DataGridView with filtered data
                unitsdataGrid.DataSource = filteredTable;
            }
            else
            {
                // If search text is empty, reload all data
                LoadData();
            }
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            ExportDataGridViewToExcel(unitsdataGrid, expensedataGrid);
        }

        private void ExportDataGridViewToExcel(DataGridView dgvUnits, DataGridView dgvExpenses)
        {
            // Create an Excel Application Instance
            Excel.Application excelApp = new Excel.Application();
            if (excelApp == null)
            {
                MessageBox.Show("Excel is not properly Installed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;
            workSheet.Name = "Units and Expenses Data";

            // Export units data
            ExportDataGridToExcelWorksheet(dgvUnits, workSheet, 1);

            // Export expenses data
            ExportDataGridToExcelWorksheet(dgvExpenses, workSheet, dgvUnits.Rows.Count + 3);

            // Adjust column width for better visibility
            workSheet.Columns.AutoFit();

            // Add company logo/picture
            string logoPath = "C:\\Users\\admin\\Desktop\\EDP ME\\Bham\\RentalApartment\\apartment_Logo.png"; 
            if (System.IO.File.Exists(logoPath))
            {
                workSheet.Shapes.AddPicture(logoPath, MsoTriState.msoFalse, MsoTriState.msoCTrue, 450, 40, 150, 150); // Adjust the position and size of the logo as needed
            }
            else
            {
                MessageBox.Show("Company logo not found at the specified path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Add signature placeholder
            Excel.Range signatureRange = workSheet.Range["G" + (dgvUnits.Rows.Count + 6)]; // Adjust the row number as needed
            signatureRange.Value = "Signature: ___________________________";
            signatureRange.Font.Bold = true;

            // Create a new worksheet for charts
            Excel._Worksheet chartSheet = (Excel.Worksheet)excelApp.Worksheets.Add();
            chartSheet.Name = "Charts";

            // Create chart for units
            Excel.ChartObjects chartObjects = (Excel.ChartObjects)chartSheet.ChartObjects();
            Excel.ChartObject chartObject = chartObjects.Add(50, 20, 350, 350);
            Excel.Chart chart = chartObject.Chart;
            Excel.Range chartRange = workSheet.Range["A1", $"C{dgvUnits.Rows.Count + 1}"];
            chart.SetSourceData(chartRange);
            chart.ChartType = Excel.XlChartType.xlColumnClustered;

            // Set chart title and axes labels
            chart.HasTitle = true;
            chart.ChartTitle.Text = "Units Data";
            chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary).HasTitle = true;
            chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary).AxisTitle.Text = "Status";
            chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary).HasTitle = true;
            chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary).AxisTitle.Text = "Rent Amount";

            // Create chart for expenses
            chartObject = chartObjects.Add(450, 20, 350, 350);
            chart = chartObject.Chart;
            chartRange = workSheet.Range["A13", $"B{dgvExpenses.Rows.Count + 13}"];
            chart.SetSourceData(chartRange);
            chart.ChartType = Excel.XlChartType.xlColumnClustered;

            // Set chart title and axes labels
            chart.HasTitle = true;
            chart.ChartTitle.Text = "Expenses Data";
            chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary).HasTitle = true;
            chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary).AxisTitle.Text = "Expense Name";
            chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary).HasTitle = true;
            chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary).AxisTitle.Text = "Expense Total";

            // Make Excel visible to the user
            excelApp.Visible = true;

            // Clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(chartRange);
            Marshal.ReleaseComObject(chart);
            Marshal.ReleaseComObject(chartObject);
            Marshal.ReleaseComObject(chartObjects);
            Marshal.ReleaseComObject(chartSheet);
            Marshal.ReleaseComObject(workSheet);
            Marshal.ReleaseComObject(excelApp.Workbooks);
            Marshal.ReleaseComObject(excelApp);
        }

        private void ExportDataGridToExcelWorksheet(DataGridView dgv, Excel._Worksheet workSheet, int startRow)
        {
            // Column Headers
            for (int i = 1; i < dgv.Columns.Count + 1; i++)
            {
                workSheet.Cells[startRow, i] = dgv.Columns[i - 1].HeaderText;
            }

            // For each row and column, Write the data in the Excel Sheet
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    var value = dgv.Rows[i].Cells[j].Value;
                    var cell = (Excel.Range)workSheet.Cells[startRow + i + 1, j + 1];
                    cell.Value2 = (value != null) ? value.ToString() : "";

                    // Set alignment to center
                    cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    cell.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    Marshal.ReleaseComObject(cell);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void sideDashboard_Click(object sender, EventArgs e)
        {
            formRental formRental = new formRental();
            this.Hide();
            formRental.Show();
        }

        private void unitInfo_Click(object sender, EventArgs e)
        {
            UnitsForm unitsForm = new UnitsForm();
            this.Hide();
            unitsForm.Show();
        }

        private void label22_Click(object sender, EventArgs e)
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

        private void sideUser_Click(object sender, EventArgs e)
        {
            AccountForm accountForm = new AccountForm();
            this.Hide();
            accountForm.Show();
        }

        
    }
}