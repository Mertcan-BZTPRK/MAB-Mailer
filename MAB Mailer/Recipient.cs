using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB_Mailer
{
    public class Recipient
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string GroupName { get; set; }
        public Dictionary<string, string> DataFields { get; set; } = new Dictionary<string, string>();

        public string ExtraDataJson { get; set; }
    }
}
