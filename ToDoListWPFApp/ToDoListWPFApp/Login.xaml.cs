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
using System.Windows.Shapes;

namespace ToDoListWPFApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        ToDoListDBEntities db = new ToDoListDBEntities();

        public static int userid;
        public Login()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String username = UsernameBox.Text.Trim();
            String password = PasswordBox.Password.Trim();

            var user = db.Users.FirstOrDefault(u => u.username == username && u.password == password);
            if(user != null)
            {
                userid = user.iduser;
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Dang nhap that bai");
            }
        }
    }
}
