using System;
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
           
        }

        private void LoadTasks()
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
                item.TaskTitle.Text = task.title;
                item.TaskCheckBox.IsChecked = task.iscompleted;
                TasksPanel.Children.Add(item);
            }
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

            var task = new ToDoListWPFApp.Task
            {
                iduser = idUser,
                title = title,
                iscompleted = false
            };

            db.Tasks.Add(task);
            db.SaveChanges();

            LoadTasks();
            NewTaskBox.Clear();
        }
    }
}
