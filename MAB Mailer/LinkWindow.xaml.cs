using System.Windows;

namespace MAB_Mailer
{
    public partial class LinkWindow : Window
    {
        public string ResultHtml { get; private set; }

        public LinkWindow()
        {
            InitializeComponent();
            txtDisplay.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text) || string.IsNullOrWhiteSpace(txtDisplay.Text))
            {
                CustomAlert.Show("Lütfen tüm alanları doldur.", "EKSİK");
                return;
            }
            // HTML formatında link oluşturuyoruz
            ResultHtml = $"<a href=\"{txtUrl.Text}\">{txtDisplay.Text}</a>";
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}