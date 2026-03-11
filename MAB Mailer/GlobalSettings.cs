using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB_Mailer
{
    public static class GlobalSettings
    {
        public static string UserMail { get; set; }
        public static string UserPass { get; set; }
        public static string SmtpHost { get; set; }
        public static string UserDisplayName { get; set; }
    }
}
