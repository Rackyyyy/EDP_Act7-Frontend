using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JollibeePOS
{
    public partial class DeliveryTrackingControl : UserControl
    {
        private string connectionString = "server=localhost;database=jollibee_daraga;uid=root;pwd=;";
        private int selectedDeliveryId = 0;

        public DeliveryTrackingControl()
        {
            InitializeComponent();
            LoadDeliveries();
            LoadDeliveryStatusOptions();
        }

        private void LoadDeliveries()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT 
                                        d.delivery_id AS Delivery_ID, 
                                        d.order_id AS Order_ID, 
                                        d.delivery_status AS Status, 
                                        c.address AS Address 
                                     FROM delivery d
                                     JOIN `order` o ON d.order_id = o.order_id
                                     JOIN customer c ON o.customer_id = c.customer_id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable deliveryTable = new DataTable();
                    adapter.Fill(deliveryTable);

                    dataGridView1.DataSource = deliveryTable;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.RowHeadersVisible = false;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading deliveries: " + ex.Message);
            }
        }

        private void LoadDeliveryStatusOptions()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Pending");
            comboBox1.Items.Add("Dispatched");
            comboBox1.Items.Add("Delivered");
            comboBox1.Items.Add("Failed");
            comboBox1.SelectedIndex = 0;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedDeliveryId = Convert.ToInt32(row.Cells["Delivery_ID"].Value);
                string currentStatus = row.Cells["Status"].Value.ToString();

                comboBox1.SelectedItem = currentStatus;
            }
        }

        private void btnSaveDeliveryStatus_Click(object sender, EventArgs e)
        {
            if (selectedDeliveryId > 0 && comboBox1.SelectedItem != null)
            {
                string newStatus = comboBox1.SelectedItem.ToString();
                UpdateDeliveryStatus(selectedDeliveryId, newStatus);
            }
            else
            {
                MessageBox.Show("Please select a delivery from the table and choose a status.");
            }
        }

        private void UpdateDeliveryStatus(int deliveryId, string status)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE delivery SET delivery_status = @status WHERE delivery_id = @deliveryId";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@deliveryId", deliveryId);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Delivery status updated successfully!");
                        LoadDeliveries(); 
                    }
                    else
                    {
                        MessageBox.Show("Failed to update delivery status.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating delivery status: " + ex.Message);
            }
        }

        private void DeliveryTrackingControl_Load(object sender, EventArgs e)
        {

        }
    }
}
