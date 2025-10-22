using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace _522_Medvedeva.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPaymentPage.xaml
    /// </summary>
    public partial class AddPaymentPage : Page
    {
        private Payment _currentPayment;
        private bool _isEditMode;

        public AddPaymentPage()
        {
            InitializeComponent();
            _currentPayment = new Payment();
            _currentPayment.Date = DateTime.Today;
            _isEditMode = false;
            DataContext = _currentPayment;

            LoadComboBoxes();
        }

        public AddPaymentPage(Payment paymentToEdit)
        {
            InitializeComponent();
            _currentPayment = paymentToEdit;
            _isEditMode = true;
            DataContext = _currentPayment;

            LoadComboBoxes();
        }
        private void LoadComboBoxes()
        {
            using (var db = new Entities())
            {
                cmbUser.ItemsSource = null;
                cmbCategory.ItemsSource = null;

                cmbUser.ItemsSource = db.User.ToList();
                cmbCategory.ItemsSource = db.Category.ToList();
            }
        }
        private void cmbUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUser.SelectedItem != null)
                _currentPayment.User = (User)cmbUser.SelectedItem;
        }

        private void TBName_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameHintText.Visibility = string.IsNullOrEmpty(TBName.Text) ? Visibility.Visible : Visibility.Collapsed;
            _currentPayment.Name = TBName.Text;
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool noSelection = cmbCategory.SelectedItem == null;
            if (!noSelection)
                _currentPayment.Category = (Category)cmbCategory.SelectedItem;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (_currentPayment.Date == null)
                errors.AppendLine("Укажите дату!");
            if (_currentPayment.User == null)
                errors.AppendLine("Выберите пользователя!");
            if (string.IsNullOrWhiteSpace(_currentPayment.Name))
                errors.AppendLine("Укажите название!");
            if (_currentPayment.Num <= 0)
                errors.AppendLine("Количество должно быть больше 0!");
            if (_currentPayment.Price <= 0)
                errors.AppendLine("Сумма должна быть больше 0!");
            if (_currentPayment.Category == null)
                errors.AppendLine("Выберите категорию!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new Entities())
                {
                    if (!_isEditMode)
                    {
                        var categoryFromDb = db.Category.Find(_currentPayment.Category.ID);
                        if (categoryFromDb != null)
                            _currentPayment.Category = categoryFromDb;

                        var userFromDb = db.User.Find(_currentPayment.User.ID);
                        if (userFromDb != null)
                            _currentPayment.User = userFromDb;

                        db.Payment.Add(_currentPayment);
                    }
                    else
                    {
                        var paymentInDb = db.Payment.Include(p => p.User).Include(p => p.Category).FirstOrDefault(p => p.ID == _currentPayment.ID);
                        if (paymentInDb != null)
                        {
                            paymentInDb.Date = _currentPayment.Date;
                            paymentInDb.UserID = _currentPayment.User.ID;
                            paymentInDb.Name = _currentPayment.Name;
                            paymentInDb.Num = _currentPayment.Num;
                            paymentInDb.Price = _currentPayment.Price;

                            var categoryFromDb = db.Category.Find(_currentPayment.Category.ID);
                            paymentInDb.Category = categoryFromDb;

                            db.Entry(paymentInDb).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }

                MessageBox.Show("Платеж успешно сохранён!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            DPDate.Text = null;
            cmbUser.SelectedItem = null;
            TBName.Text = "";
            TBNum.Text = "";
            TBPrice.Text = "";
            cmbCategory.SelectedItem = null;
        }
    }
}