using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace JollibeePOS
{
    public partial class OrderControl : UserControl
    {
        private string connectionString = "server=localhost;database=jollibee_daraga;uid=root;pwd=09264050857;";
        private DataTable menuItems;
        private DataTable orderItems = new DataTable();

        public OrderControl()
        {
            InitializeComponent();

            orderItems.Columns.Add("Item ID", typeof(int));
            orderItems.Columns.Add("Item Name", typeof(string));
            orderItems.Columns.Add("Quantity", typeof(int));
            orderItems.Columns.Add("Price", typeof(decimal));
            orderItems.Columns.Add("Subtotal", typeof(decimal));
        }

        private void OrderControl_Load(object sender, EventArgs e)
        {
            LoadCustomers();
            LoadMenuItems();
            lvOrderItems.Items.Clear();
            UpdateTotalAmount();
        }

        private void LoadCustomers()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT customer_id, CONCAT(first_name, ' ', last_name) AS full_name FROM customer";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable customerTable = new DataTable();
                    adapter.Fill(customerTable);

                    cmbCustomer.DataSource = customerTable;
                    cmbCustomer.DisplayMember = "full_name";
                    cmbCustomer.ValueMember = "customer_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
            }
        }

        private void LoadMenuItems()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT menu_id, item_name, description, price FROM menu";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    menuItems = new DataTable();
                    adapter.Fill(menuItems);

                    dgvMenuItems.DataSource = menuItems;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading menu items: " + ex.Message);
            }
        }

        private void dgvMenuItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMenuItems.Rows[e.RowIndex];
                int itemId = Convert.ToInt32(row.Cells["menu_id"].Value);
                string itemName = row.Cells["item_name"].Value.ToString();
                decimal price = Convert.ToDecimal(row.Cells["price"].Value);

                AddItemToOrder(itemId, itemName, price);
            }
        }

        private void AddItemToOrder(int itemId, string itemName, decimal price)
        {
            foreach (ListViewItem item in lvOrderItems.Items)
            {
                if (item.SubItems[0].Text == itemId.ToString())
                {
                    int quantity = int.Parse(item.SubItems[2].Text) + 1;
                    decimal subtotal = quantity * price;

                    item.SubItems[2].Text = quantity.ToString();
                    item.SubItems[4].Text = subtotal.ToString("0.00");

                    UpdateTotalAmount();
                    return;
                }
            }

            ListViewItem newItem = new ListViewItem(itemId.ToString());
            newItem.SubItems.Add(itemName);
            newItem.SubItems.Add("1");
            newItem.SubItems.Add(price.ToString("0.00"));
            newItem.SubItems.Add(price.ToString("0.00"));

            lvOrderItems.Items.Add(newItem);
            UpdateTotalAmount();
        }

        private void RemoveItemFromOrder()
        {
            if (lvOrderItems.SelectedItems.Count > 0)
            {
                lvOrderItems.Items.Remove(lvOrderItems.SelectedItems[0]);
                UpdateTotalAmount();
            }
            else
            {
                MessageBox.Show("Please select an item to remove.");
            }
        }

        private void UpdateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (ListViewItem item in lvOrderItems.Items)
            {
                totalAmount += decimal.Parse(item.SubItems[4].Text);
            }
            lblTotalAmount.Text = totalAmount.ToString("0.00");
        }

        private void FinalizeOrder()
        {
            if (cmbCustomer.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a customer before finalizing the order.");
                return;
            }

            if (lvOrderItems.Items.Count == 0)
            {
                MessageBox.Show("Cannot finalize an empty order.");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string orderQuery = "INSERT INTO `order` (customer_id, total_amount, order_date, status) VALUES (@customerId, @totalAmount, @orderDate, 'Pending'); SELECT LAST_INSERT_ID();";
                    MySqlCommand orderCommand = new MySqlCommand(orderQuery, connection);
                    orderCommand.Parameters.AddWithValue("@customerId", cmbCustomer.SelectedValue);
                    orderCommand.Parameters.AddWithValue("@totalAmount", decimal.Parse(lblTotalAmount.Text));
                    orderCommand.Parameters.AddWithValue("@orderDate", DateTime.Now);

                    int orderId = Convert.ToInt32(orderCommand.ExecuteScalar());

                    foreach (ListViewItem item in lvOrderItems.Items)
                    {
                        int itemId = int.Parse(item.SubItems[0].Text);
                        int quantity = int.Parse(item.SubItems[2].Text);
                        decimal price = decimal.Parse(item.SubItems[3].Text);
                        decimal subtotal = decimal.Parse(item.SubItems[4].Text);

                        string orderItemQuery = "INSERT INTO orderitem (order_id, menu_id, quantity, price, subtotal) VALUES (@orderId, @menuId, @quantity, @price, @subtotal)";
                        MySqlCommand orderItemCommand = new MySqlCommand(orderItemQuery, connection);
                        orderItemCommand.Parameters.AddWithValue("@orderId", orderId);
                        orderItemCommand.Parameters.AddWithValue("@menuId", itemId);
                        orderItemCommand.Parameters.AddWithValue("@quantity", quantity);
                        orderItemCommand.Parameters.AddWithValue("@price", price);
                        orderItemCommand.Parameters.AddWithValue("@subtotal", subtotal);
                        orderItemCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("Order finalized successfully!");
                    lvOrderItems.Items.Clear();
                    UpdateTotalAmount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error finalizing order: " + ex.Message);
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (dgvMenuItems.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvMenuItems.SelectedRows[0];
                int itemId = Convert.ToInt32(selectedRow.Cells["menu_id"].Value);
                string itemName = selectedRow.Cells["item_name"].Value.ToString();
                decimal price = Convert.ToDecimal(selectedRow.Cells["price"].Value);

                AddItemToOrder(itemId, itemName, price);
            }
            else
            {
                MessageBox.Show("Please select a menu item to add.");
            }
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            RemoveItemFromOrder();
        }

        private void btnFinalizeOrder_Click(object sender, EventArgs e)
        {
            FinalizeOrder();
        }

        private void lvOrderItems_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblTotalAmount_Click(object sender, EventArgs e)
        {

        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
    }
}
