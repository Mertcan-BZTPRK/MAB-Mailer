using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MAB_Mailer
{
    public static class MyInputBox
    {
        public static int Show(string prompt, string title, int defaultValue = 5)
        {
            // 1. PENCERE AYARLARI (Şeffaf zemin, kenarlıksız)
            Window window = new Window()
            {
                Width = 350,
                Height = 210,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                Background = Brushes.Transparent, // Arkası şeffaf olsun ki Border'ın köşeleri görünsün
                AllowsTransparency = true,
                Topmost = true
            };

            // 2. ANA KASA (Temadan renk alıyor)
            Border border = new Border()
            {
                BorderThickness = new Thickness(1),
                // BorderBrush rengini App.xaml'den çekiyoruz
                BorderBrush = (Brush)Application.Current.Resources["BorderBrush"],
                CornerRadius = new CornerRadius(15),
                // Arka plan rengini App.xaml'den çekiyoruz
                Background = (Brush)Application.Current.Resources["CardBackground"],
                Padding = new Thickness(20)
            };

            // Derinlik hissi için gölge
            border.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 20,
                ShadowDepth = 5,
                Opacity = 0.5
            };

            // 3. İÇERİK DÜZENİ
            StackPanel stack = new StackPanel();

            // Soru Metni
            TextBlock text = new TextBlock()
            {
                Text = prompt,
                Margin = new Thickness(0, 0, 0, 15),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                // Yazı rengini App.xaml'den çekiyoruz
                Foreground = (Brush)Application.Current.Resources["TextPrimary"],
                TextWrapping = TextWrapping.Wrap
            };

            // Sayı Giriş Kutusu
            TextBox textBox = new TextBox()
            {
                Text = defaultValue.ToString(),
                Height = 35,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5),
                // Input renklerini App.xaml'den çekiyoruz
                Background = (Brush)Application.Current.Resources["InputBackground"],
                Foreground = (Brush)Application.Current.Resources["InputForeground"],
                BorderBrush = (Brush)Application.Current.Resources["InputBorder"],
                FontSize = 14
            };

            // Buton Paneli (Sağa Yaslı)
            StackPanel btnPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            // İptal Butonu
            Button btnCancel = new Button()
            {
                Content = "İptal",
                Width = 80,
                Height = 35,
                Margin = new Thickness(0, 0, 10, 0),
                Style = (Style)Application.Current.Resources["RedGlassButton"] // Kırmızı stil
            };
            btnCancel.Click += (s, e) => { window.DialogResult = false; window.Close(); };

            // Onay Butonu
            Button btnOk = new Button()
            {
                Content = "OLUŞTUR",
                Width = 100,
                Height = 35,
                Style = (Style)Application.Current.Resources["BaseGlassButton"], // Mavi stil
                IsDefault = true // Enter'a basınca çalışsın
            };
            btnOk.Click += (sender, e) => { window.DialogResult = true; window.Close(); };

            // Elemanları birleştir
            btnPanel.Children.Add(btnCancel);
            btnPanel.Children.Add(btnOk);

            stack.Children.Add(text);
            stack.Children.Add(textBox);
            stack.Children.Add(btnPanel);

            border.Child = stack;
            window.Content = border;

            // Pencereyi sürükleyebilmek için
            border.MouseLeftButtonDown += (s, e) => window.DragMove();

            // Göster ve sonucu bekle
            if (window.ShowDialog() == true)
            {
                int.TryParse(textBox.Text, out int result);
                return result > 0 ? result : defaultValue;
            }
            return -1; // İptal edilirse -1 döner
        }
    }
}