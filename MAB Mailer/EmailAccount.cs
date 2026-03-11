namespace MAB_Mailer
{
    public class EmailAccount
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; } = 587;
        public string DisplayName { get; set; }
        public bool EnableSsl { get; set; } = true;

        public override string ToString()
        {
            return Email;
        }
    }
}