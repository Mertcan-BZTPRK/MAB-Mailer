using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB_Mailer
{
    public class MessageTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; }   // Dropdown'da görünecek isim
        public string Subject { get; set; } // Kayıtlı Konu
        public string Body { get; set; }    // Kayıtlı İçerik
    }
}
