using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MAB_Mailer
{
    public class MailDraft : INotifyPropertyChanged
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SenderEmail { get; set; }

        // --- DEĞİŞİKLİK 1: HATA MESAJI İÇİN YENİ ALAN ---
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        // --- DEĞİŞİKLİK 2: ARAYÜZÜ GÜNCELLEYEN DURUM ALANI ---
        private string _status = "Bekliyor";
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }

        // --- DEĞİŞİKLİK 3: HABERLEŞME ALTYAPISI (INotifyPropertyChanged) ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}