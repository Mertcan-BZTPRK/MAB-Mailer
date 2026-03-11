using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAB_Mailer
{
    public class RecipientGroup
    {
        public int Id { get; set; }
        public string GroupName { get; set; } 
        public List<string> MemberEmails { get; set; } = new List<string>();
    }
}
