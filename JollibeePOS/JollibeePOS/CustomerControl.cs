using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JollibeePOS
{
    public partial class CustomerControl : UserControl
    {
        private string connectionString = "server=localhost;database=jollibee_daraga;uid=root;pwd=09264050857;";

        public CustomerControl()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT customer_id, CONCAT(first_name, ' ', last_name) AS Name, phone, email, address FROM customer";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
            }
        }

        private void ClearFields()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter both First Name and Last Name.");
                return false;
            }
            return true;
        }

        // Add new customer
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO customer (first_name, last_name, phone, email, address) 
                                     VALUES (@firstName, @lastName, @phone, @email, @address)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@firstName", txtFirstName.Text.Trim());
                    command.Parameters.AddWithValue("@lastName", txtLastName.Text.Trim());
                    command.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                    command.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    command.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    command.ExecuteNonQuery();

                    MessageBox.Show("Customer added successfully!");
                    LoadCustomers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
           
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to update.");
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                int customerId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["customer_id"].Value);

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE customer SET first_name = @firstName, last_name = @lastName, phone = @phone, email = @email, address = @address 
                                     WHERE customer_id = @customerId";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.Parameters.AddWithValue("@firstName", txtFirstName.Text.Trim());
                    command.Parameters.AddWithValue("@lastName", txtLastName.Text.Trim());
                    command.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                    command.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    command.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
                    command.ExecuteNonQuery();

                    MessageBox.Show("Customer updated successfully!");
                    LoadCustomers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer: " + ex.Message);
            }
        }

        // Delete selected customer
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to delete.");
                return;
            }

            try
            {
                int customerId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["customer_id"].Value);

                var confirmResult = MessageBox.Show("Are you sure to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM customer WHERE customer_id = @customerId";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@customerId", customerId);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Customer deleted successfully!");
                        LoadCustomers();
                        ClearFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting customer: " + ex.Message);
            }
        }

        // When user clicks a row in DataGridView, populate the textboxes with that customer's data
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string[] nameParts = row.Cells["Name"].Value.ToString().Split(' ');
                txtFirstName.Text = nameParts.Length > 0 ? nameParts[0] : ""; // First name
                txtLastName.Text = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : ""; 
                txtPhone.Text = row.Cells["phone"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["email"].Value?.ToString() ?? "";
                txtAddress.Text = row.Cells["address"].Value?.ToString() ?? "";
            }
        }

        private void CustomerControl_Load(object sender, EventArgs e)
        {

        }
    }
}
