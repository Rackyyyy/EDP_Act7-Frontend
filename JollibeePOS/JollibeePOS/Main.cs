using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JollibeePOS
{
    public partial class Main : Form
    {
        private string connectionString = "server=localhost;database=jollibee_daraga;uid=root;pwd=09264050857;";
        private DataTable menuItems;
        private DataTable orderItems = new DataTable();

        public Main()
        {
            InitializeComponent();
        }

        private void LoadForm(UserControl control)
        {
            panelMain.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panelMain.Controls.Add(control);
            control.Show();
        }

        private void panelNav_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            labelTitle.Text = "Order Management";
            LoadForm(new OrderControl());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelTitle.Text = "Customer Management";
            LoadForm(new CustomerControl());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            labelTitle.Text = "Delivery Tracking";
            LoadForm(new DeliveryTrackingControl());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            labelTitle.Text = "Sales Reports";
            LoadForm(new ReportControl());
        }

        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }
    }
}
