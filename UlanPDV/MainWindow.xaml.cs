using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UlanPDV
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string username;

        public MainWindow(string username = "")
        {
            this.username = username;
            InitializeComponent();
            usernameTextBox.Text = this.username;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Database db = new Database();
            this.username = usernameTextBox.Text;
            bool exists = db.Login(this.username, passwordTextBox.Password);
            if (exists == true)
            {
                this.Hide();
                //Main main = new Main(this.username);
                //main.Closed += (s, args) => this.Close();
                //main.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error");
            }
        }
    }
}
