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

namespace ToDoListWPFApp
{
    /// <summary>
    /// Interaction logic for TodoItemControl.xaml
    /// </summary>
    public partial class TodoItemControl : UserControl
    {
        public Task CurrentTask => DataContext as Task;
        public TodoItemControl()
        {
            InitializeComponent();

            // Đăng ký sự kiện thay đổi CheckBox
            TaskCheckBox.Checked += TaskCheckBox_Changed;
            TaskCheckBox.Unchecked += TaskCheckBox_Changed;
        }

        // ====== Edit ======
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            TaskTitle.Visibility = Visibility.Collapsed;
            TaskEditor.Visibility = Visibility.Visible;
            TaskEditor.Focus();
            TaskEditor.SelectAll();
        }

        private void TaskEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SaveEdit();
            else if (e.Key == Key.Escape) CancelEdit();
        }

        private void TaskEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveEdit();
        }

        private void SaveEdit()
        {
            if (CurrentTask == null) return;

            string newTitle = TaskEditor.Text.Trim();
            if (string.IsNullOrEmpty(newTitle))
            {
                CancelEdit();
                return;
            }

            using (var db = new ToDoListDBEntities())
            {
                var task = db.Tasks.FirstOrDefault(t => t.idtask == CurrentTask.idtask);
                if (task != null)
                {
                    task.title = newTitle;
                    task.updated_at = DateTime.Now;
                    db.SaveChanges();
                }
            }

            TaskTitle.Text = newTitle;
            TaskEditor.Visibility = Visibility.Collapsed;
            TaskTitle.Visibility = Visibility.Visible;
        }

        private void CancelEdit()
        {
            TaskEditor.Visibility = Visibility.Collapsed;
            TaskTitle.Visibility = Visibility.Visible;
        }

        // ====== Delete ======
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTask == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa công việc '{CurrentTask.title}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var db = new ToDoListDBEntities())
                {
                    var task = db.Tasks.FirstOrDefault(t => t.idtask == CurrentTask.idtask);
                    if (task != null)
                    {
                        db.Tasks.Remove(task);
                        db.SaveChanges();
                    }
                }

                // Xóa control khỏi UI ngay lập tức
                var parent = this.Parent as Panel;
                parent?.Children.Remove(this);
            }
        }

        // ====== Check/Uncheck ======
        private void TaskCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (CurrentTask == null) return;

            using (var db = new ToDoListDBEntities())
            {
                var task = db.Tasks.FirstOrDefault(t => t.idtask == CurrentTask.idtask);
                if (task != null)
                {
                    task.iscompleted = TaskCheckBox.IsChecked == true;
                    task.updated_at = DateTime.Now;
                    db.SaveChanges();
                }
            }
        }
    }
}
