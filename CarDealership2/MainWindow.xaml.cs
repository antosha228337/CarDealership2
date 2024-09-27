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

        private DataSet dataSet = new DataSet(); 

        public MainWindow()
        {
            InitializeComponent();

            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            carAdapter = new NpgsqlDataAdapter("SELECT * FROM car;", connectionString);
            modificationAdapter = new NpgsqlDataAdapter("SELECT * FROM modification;", connectionString);

            carAdapter.Fill(dataSet, "car");
            modificationAdapter.Fill(dataSet, "modification");

            dataGridCar.ItemsSource = dataSet.Tables["car"]?.DefaultView;

            dataGridMod.ItemsSource = dataSet.Tables["modification"]?.DefaultView;
        }
    }
}