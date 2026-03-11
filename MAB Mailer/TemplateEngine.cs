using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB_Mailer
{
    public class TemplateEngine
    {
        public string Parse(string template, Recipient recipient)
        {
            if (string.IsNullOrEmpty(template)) return "";
            string result = template;

            // Temel alanlar
            result = result.Replace("{Ad}", recipient.Ad ?? "");
            result = result.Replace("{Soyad}", recipient.Soyad ?? "");
            result = result.Replace("{Email}", recipient.Email ?? "");

            // Dinamik Excel alanları
            if (recipient.DataFields != null)
            {
                foreach (var field in recipient.DataFields)
                {
                    result = result.Replace($"{{{field.Key}}}", field.Value ?? "");
                }
            }
            return result;
        }
    }
}
