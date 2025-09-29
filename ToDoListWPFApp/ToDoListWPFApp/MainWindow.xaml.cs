using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ToDoListWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ToDoListDBEntities db = new ToDoListDBEntities();
        public MainWindow()
        {
            InitializeComponent();
            DeadlinePicker.SelectedDate = DateTime.Today;

            // Khởi tạo danh sách giờ (0-23)
            for (int i = 0; i < 24; i++)
            {
                HourBox.Items.Add(i.ToString("D2"));
            }
            HourBox.SelectedIndex = DateTime.Now.Hour;

            // Khởi tạo danh sách phút (0-59, cách 5 phút)
            for (int i = 0; i < 60; i += 5)
            {
                MinuteBox.Items.Add(i.ToString("D2"));
            }
            MinuteBox.SelectedIndex = DateTime.Now.Minute / 5;

            LoadTasks();
        }

        public void LoadTasks()
        {
            TasksPanel.Children.Clear();

            var tasks = db.Tasks
                          .Where(t => t.iduser == Login.userid)
                          .ToList();

            if (!tasks.Any())
            {
                TasksPanel.Children.Add(new TextBlock
                {
                    Text = "Chưa có công việc nào!",
                    FontSize = 16,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10)
                });
                return;
            }

            foreach (var task in tasks)
            {
                var item = new TodoItemControl();
                item.DataContext = task;
                item.TaskTitle.Text = task.title;
                item.TaskCheckBox.IsChecked = task.iscompleted;
                TasksPanel.Children.Add(item);
            }
        }

        private DateTime? CombineDateTime(DateTime? date, object hourObj, object minuteObj)
        {
            if (!date.HasValue || hourObj == null || minuteObj == null)
                return null;

            int hour = int.Parse(hourObj.ToString());
            int minute = int.Parse(minuteObj.ToString());

            return date.Value.Date.AddHours(hour).AddMinutes(minute);
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = NewTaskBox.Text.Trim();
            int idUser = Login.userid;

            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề công việc");
                return;
            }

            var task = new Task
            {
                iduser = idUser,
                title = title,
                iscompleted = false,
                created_at = DateTime.Now,
                deadline = CombineDateTime(DeadlinePicker.SelectedDate, HourBox.SelectedItem, MinuteBox.SelectedItem)
            };

            db.Tasks.Add(task);
            db.SaveChanges();

            // Thêm thẳng vào UI
            var item = new TodoItemControl();
            item.DataContext = task;
            item.TaskTitle.Text = task.title;
            item.TaskCheckBox.IsChecked = task.iscompleted;
            if (task.deadline.HasValue)
            {
                item.DeadlineText.Text = "Hạn: " + task.deadline.Value.ToString("dd/MM/yyyy");
                if (task.deadline.Value.Date < DateTime.Today)
                    item.DeadlineText.Foreground = Brushes.Red;
            }
            TasksPanel.Children.Add(item);
            NewTaskBox.Clear();
            DeadlinePicker.SelectedDate = DateTime.Today;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Login.userid = 0;
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
