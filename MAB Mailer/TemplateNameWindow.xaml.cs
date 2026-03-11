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
    /// Interaction logic for TemplateNameWindow.xaml
    /// </summary>
    public partial class TemplateNameWindow : Window
    {
        public string EnteredName { get; private set; }

        public TemplateNameWindow()
        {
            InitializeComponent();
            txtTemplateName.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            EnteredName = txtTemplateName.Text;
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
