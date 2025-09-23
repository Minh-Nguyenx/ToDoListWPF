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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        ToDoListDBEntities db = new ToDoListDBEntities();
        
        public Register()
        {
            InitializeComponent();
        }

        private void BtnRegister(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUserName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string pass1 = txtPassWord.Password.Trim();
                string pass2 = txtPassWordAgain.Password.Trim();

                // 1. Kiểm tra dữ liệu rỗng
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(pass1) || string.IsNullOrEmpty(pass2))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Kiểm tra mật khẩu khớp
                if (pass1 != pass2)
                {
                    MessageBox.Show("Mật khẩu không khớp!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. Kiểm tra username/email đã tồn tại
                var checkUser = db.Users.FirstOrDefault(u => u.username == username);
                if (checkUser != null)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var checkEmail = db.Users.FirstOrDefault(u => u.email == email);
                if (checkEmail != null)
                {
                    MessageBox.Show("Email đã được đăng ký!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 4. Thêm user mới
                User us = new User
                {
                    username = username,
                    email = email,
                    password = pass1   
                };

                db.Users.Add(us);
                db.SaveChanges();

                // 5. Thông báo thành công
                MessageBox.Show("Đăng ký thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // 6. Điều hướng sang Login (ví dụ)
                Login loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
