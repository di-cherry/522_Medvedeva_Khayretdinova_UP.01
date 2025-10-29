using _522_Medvedeva.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
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
using System.Windows.Threading;

namespace _522_Medvedeva
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string currentUserRole = null;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new AuthPage());
            };
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (o, t) =>
            {
                DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            };
            timer.Start();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
            else
            {
                MainFrame.Visibility = Visibility.Visible;

                if (currentUserRole == "admin")
                {
                    MainFrame.Navigate(new AdminPage());
                }
                else if (currentUserRole == "user")
                {
                    MainFrame.Navigate(new UserPage());
                }
                else
                {
                    // Если роль не определена, возвращаем на страницу авторизации
                    MainFrame.Navigate(new AuthPage());
                }

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string themeFile = selectedItem.Tag.ToString();
                ChangeTheme(themeFile);
            }
        }

        private void ChangeTheme(string themeFile)
        {
            try
            {
                var uri = new Uri(themeFile, UriKind.Relative);
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить тему: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
