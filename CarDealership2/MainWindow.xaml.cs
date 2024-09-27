using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace CarDealership2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string connectionString;

        private NpgsqlDataAdapter carAdapter;
        private NpgsqlDataAdapter modificationAdapter;

        private NpgsqlCommandBuilder carBuilder;

        private DataSet dataSet = new DataSet(); 

        public MainWindow()
        {
            InitializeComponent();

            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            carAdapter = new NpgsqlDataAdapter("SELECT * FROM car;", connectionString);
            modificationAdapter = new NpgsqlDataAdapter("SELECT * FROM modification;", connectionString);

            carAdapter.Fill(dataSet, "car");
            modificationAdapter.Fill(dataSet, "modification");

            carBuilder = new(carAdapter);

            dataGridCar.ItemsSource = dataSet.Tables["car"]?.DefaultView;

            dataGridMod.ItemsSource = dataSet.Tables["modification"]?.DefaultView;

            SetupComboBox();
        }

        private void SetupComboBox()
        {
            using (var sqlConnection = new NpgsqlConnection(connectionString))
            {
                NpgsqlDataAdapter sqlAdapter = new NpgsqlDataAdapter("SELECT * FROM car_brand", sqlConnection);

                DataSet dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "car_brand");

                comboBoxCarBrand.ItemsSource = dataSet.Tables["car_brand"]?.DefaultView;
                comboBoxCarBrand.DisplayMemberPath = "name";

            }
        }

        private void Button_FindModels_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                sqlConnection.Open();
                NpgsqlCommand sqlCommand =
                    new NpgsqlCommand("SELECT c.model, c.body_type FROM car c join car_brand cb on cb.Id = c.car_brand_id WHERE cb.Id = " + comboBoxCarBrand.SelectedIndex + 1
                                  , sqlConnection);
                NpgsqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                DataTable dataTable = new DataTable("report1");
                dataTable.Columns.Add("model");
                dataTable.Columns.Add("body_type");
                while (sqlDataReader.Read())
                {
                    DataRow row = dataTable.NewRow();
                    row["model"] = sqlDataReader["model"];
                    row["body_type"] = sqlDataReader["body_type"];
                    dataTable.Rows.Add(row);
                }
                sqlDataReader.Close();
                dataGridCarByBrand.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Button_FindSales_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                //Trace.WriteLine(comboBoxCarBrand.SelectedItem.ToString());
                NpgsqlCommand sqlCommand = new NpgsqlCommand("SELECT * FROM get_sales_by_brand($1);", sqlConnection)
                {
                    Parameters =
                    {
                        new NpgsqlParameter(){Value = textBoxCarBrand.Text },
                    }
                };
                sqlConnection.Open();
                sqlCommand.Prepare();
                DataTable dataTable = new DataTable("report2");
                var sqlAdapter = new NpgsqlDataAdapter(sqlCommand);
                sqlAdapter.Fill(dataTable);

                dataGridSales.ItemsSource = dataTable.DefaultView;
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            carAdapter.Update(dataSet, "car");
        }
    }
}