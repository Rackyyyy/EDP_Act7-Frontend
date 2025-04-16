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
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace JollibeePOS
{
    public partial class ReportControl : UserControl
    {
        private string connectionString = "server=localhost;database=jollibee_daraga;uid=root;pwd=09264050857;";

        public ReportControl()
        {
            InitializeComponent();

            dtStart.Format = DateTimePickerFormat.Custom;
            dtStart.CustomFormat = "MMM yyyy";
            dtStart.Value = DateTime.Now.AddMonths(-11); 

            dtEnd.Format = DateTimePickerFormat.Custom;
            dtEnd.CustomFormat = "MMM yyyy";
            dtEnd.Value = DateTime.Now;

            InitializeChart();
            GenerateReport();
        }

        private void InitializeChart()
        {
            salesChart.Series.Clear();
            Series salesSeries = new Series("Sales");
            salesSeries.ChartType = SeriesChartType.Column;
            salesSeries.Color = Color.SteelBlue;
            salesChart.Series.Add(salesSeries);

            salesChart.Titles.Add("Sales Report");
            salesChart.ChartAreas[0].AxisX.Title = "Month";
            salesChart.ChartAreas[0].AxisY.Title = "Sales Amount";

            salesChart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            salesChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            salesChart.ChartAreas[0].BackColor = Color.White;
            salesChart.BorderlineColor = Color.LightGray;
            salesChart.BorderlineDashStyle = ChartDashStyle.Solid;
            salesChart.BorderlineWidth = 1;
        }

        private void GenerateReport()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    DateTime startDate = new DateTime(dtStart.Value.Year, dtStart.Value.Month, 1);
                    DateTime endDate = new DateTime(dtEnd.Value.Year, dtEnd.Value.Month, 1).AddMonths(1).AddDays(-1);

                    string query = @"
                        SELECT 
                            DATE_FORMAT(o.order_date, '%Y-%m-01') AS month_start,
                            DATE_FORMAT(o.order_date, '%b') AS month_name,
                            SUM(o.total_amount) AS total_sales 
                        FROM `order` o 
                        WHERE o.order_date BETWEEN @startDate AND @endDate 
                        GROUP BY DATE_FORMAT(o.order_date, '%Y-%m-01'), DATE_FORMAT(o.order_date, '%b')
                        ORDER BY month_start";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@startDate", startDate);
                    command.Parameters.AddWithValue("@endDate", endDate);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable salesData = new DataTable();
                    adapter.Fill(salesData);

                    dataGridReport.DataSource = salesData;

                    salesChart.Series[0].Points.Clear();

                    List<string> allMonths = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    Dictionary<string, decimal> monthlySales = new Dictionary<string, decimal>();
                    foreach (string month in allMonths)
                    {
                        monthlySales[month] = 0;
                    }

                    foreach (DataRow row in salesData.Rows)
                    {
                        string monthName = row["month_name"].ToString();
                        decimal totalSales = Convert.ToDecimal(row["total_sales"]);
                        monthlySales[monthName] = totalSales;
                    }

                    int startMonth = startDate.Month - 1;
                    int endMonth = endDate.Month - 1;
                    int startYear = startDate.Year;
                    int endYear = endDate.Year;

                    if (startYear != endYear)
                    {
                        for (int i = 0; i < allMonths.Count; i++)
                        {
                            string month = allMonths[i];
                            salesChart.Series[0].Points.AddXY(month, monthlySales[month]);
                        }
                    }
                    else
                    {
                        for (int i = startMonth; i <= endMonth; i++)
                        {
                            if (i >= 0 && i < allMonths.Count)
                            {
                                string month = allMonths[i];
                                salesChart.Series[0].Points.AddXY(month, monthlySales[month]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message);
            }
        }

        private void dtStart_ValueChanged(object sender, EventArgs e)
        {
            if (dtStart.Value > dtEnd.Value)
            {
                MessageBox.Show("Start date cannot be after end date!");
                dtStart.Value = dtEnd.Value;
                return;
            }

            GenerateReport();
        }

        private void dtEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dtEnd.Value < dtStart.Value)
            {
                MessageBox.Show("End date cannot be before start date!");
                dtEnd.Value = dtStart.Value;
                return;
            }

            GenerateReport();
        }
        private void salesChart_Click(object sender, EventArgs e)
        {

        }

        private void dataGridReport_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ReportControl_Load(object sender, EventArgs e)
        {

        }
    }
}
