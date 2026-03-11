using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MAB_Mailer
{
    public partial class LoginWindow : Window
    {
        private DatabaseService _dbService = new DatabaseService();

        public LoginWindow()
        {
            InitializeComponent();
            LoadAccounts();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void LoadAccounts()
        {
            var accounts = _dbService.GetAccounts();
            cmbAccounts.ItemsSource = accounts;
            if (accounts.Count > 0) cmbAccounts.SelectedIndex = 0;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAccounts.SelectedItem is EmailAccount selectedAccount)
            {
                GlobalSettings.UserMail = selectedAccount.Email;
                GlobalSettings.UserPass = selectedAccount.Password;
                GlobalSettings.SmtpHost = selectedAccount.SmtpHost;
                GlobalSettings.UserDisplayName = selectedAccount.DisplayName;

                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                lblStatus.Text = "Lütfen bir hesap seçin.";
            }
        }

        private void BtnSaveAccount_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewEmail.Text) || string.IsNullOrWhiteSpace(txtNewPass.Password))
            {
                CustomAlert.Show("Mail ve şifre boş olamaz!", "HATA");
                return;
            }

            string dName = txtDisplayName.Text.Trim();
            if (string.IsNullOrEmpty(dName)) dName = txtNewEmail.Text.Split('@')[0];

            var newAccount = new EmailAccount
            {
                DisplayName = dName,
                Email = txtNewEmail.Text,
                Password = txtNewPass.Password,
                SmtpHost = txtNewHost.Text,
                SmtpPort = int.Parse(txtNewPort.Text),
                EnableSsl = chkSsl.IsChecked == true
            };

            _dbService.SaveAccount(newAccount);

            CustomAlert.Show("Hesap başarıyla eklendi.", "İşlem Başarılı");
            txtNewEmail.Clear();
            txtNewPass.Clear();
            txtDisplayName.Clear();
            LoadAccounts();
        }

        private void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (cmbAccounts.SelectedItem is EmailAccount account)
            {
                if (CustomAlert.Show("Silmek istediğine emin misin patron?", "Silme Onayı", true))
                {
                    _dbService.DeleteAccount(account.Id);
                    LoadAccounts();
                }
            }
        }

        private void CmbProviders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtNewHost == null) return;

            int index = cmbProviders.SelectedIndex;
            switch (index)
            {
                case 0:
                    txtNewHost.Text = "smtp.office365.com";
                    txtNewPort.Text = "587";
                    chkSsl.IsChecked = true;
                    break;
                case 1:
                    txtNewHost.Text = "smtp.gmail.com";
                    txtNewPort.Text = "587";
                    chkSsl.IsChecked = true;
                    break;
                default:
                    txtNewHost.Text = "";
                    txtNewPort.Text = "587";
                    break;
            }
        }
        private void Logo_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com",
                    UseShellExecute = true
                });
            }
            catch { }
        }
    }
}