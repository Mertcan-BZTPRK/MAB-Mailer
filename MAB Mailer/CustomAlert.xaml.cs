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

namespace MAB_Mailer
{
    /// <summary>
    /// Interaction logic for CustomAlert.xaml
    /// </summary>
    public partial class CustomAlert : Window
    {
        public CustomAlert(string message, string title, bool isQuestion)
        {
            InitializeComponent();

            txtTitle.Text = title;
            txtMessage.Text = message;

            if (isQuestion)
            {
                // Soru ise Evet/Hayır butonlarını aç
                pnlYesNo.Visibility = Visibility.Visible;
                btnOk.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Sadece bilgi ise Tamam butonunu aç
                pnlYesNo.Visibility = Visibility.Collapsed;
                btnOk.Visibility = Visibility.Visible;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Evet/Tamam dendi
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Hayır/Vazgeç dendi
            this.Close();
        }

        // --- SİHİRLİ METOD (Kullanımı Kolaylaştıran Kısım) ---
        public static bool Show(string message, string title = "Bilgi", bool isQuestion = false)
        {
            // Yeni pencereyi oluştur
            CustomAlert alert = new CustomAlert(message, title, isQuestion);

            // Göster ve cevabı bekle
            var result = alert.ShowDialog();

            return result == true;
        }
    }
}
