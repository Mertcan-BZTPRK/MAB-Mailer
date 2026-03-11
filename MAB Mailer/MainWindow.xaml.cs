using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MAB_Mailer
{
    // Arayüzle haberleşebilen Grup Sınıfı
    public class SelectableGroup : INotifyPropertyChanged
    {
        private bool _isSelected;
        public string GroupName { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        public RecipientGroup OriginalGroup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class MainWindow : Window
    {
        private DatabaseService _dbService = new DatabaseService();
        private ExcelService _excelService = new ExcelService();
        private MailService _mailService = new MailService();
        private ObservableCollection<MailDraft> _draftQueue = new ObservableCollection<MailDraft>();
        private int _selectedRecipientId = 0;

        public MainWindow()
        {
            InitializeComponent();
            lstQueue.ItemsSource = _draftQueue;
            LoadTemplates();
            RefreshList();
            LoadGroups();
        }

        // --- YENİ EKLENEN ŞABLON/İÇERİ AKTARMA BUTONLARI ---
        private void BtnTemplate_Click(object sender, RoutedEventArgs e)
        {
            int colCount = MyInputBox.Show("Kaç tane ekstra veri sütunu (Veri 1, Veri 2...) olsun?\n(Sadece Ad, Soyad, Email, Grup için 0 yazın)", "Şablon Ayarı", 0);
            if (colCount == -1) return;

            try
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MABMAİLER_Kişi_Listesi.xlsx");
                _excelService.CreateMasterTemplate(path, colCount);
                CustomAlert.Show("Şablon masaüstüne oluşturuldu.", "BAŞARILI");
            }
            catch (Exception ex) { CustomAlert.Show("Hata: " + ex.Message, "HATA"); }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Excel Files|*.xlsx;*.xls" };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var data = _excelService.LoadRecipientsFromExcel(openFileDialog.FileName);
                    if (data.Count == 0)
                    {
                        CustomAlert.Show("Veri bulunamadı.", "UYARI");
                        return;
                    }

                    foreach (var person in data) _dbService.ImportRecipient(person);

                    RefreshList();
                    LoadGroups();
                    CustomAlert.Show($"{data.Count} kişi işlendi.", "İŞLEM TAMAM");
                }
                catch (Exception ex) { CustomAlert.Show("Hata: " + ex.Message, "HATA"); }
            }
        }

        // --- GRUP SEÇİM MANTIĞI (TÜMÜ SEÇME EKLENDİ) ---
        private void GroupCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox?.DataContext is SelectableGroup changedItem)
            {
                if (changedItem.GroupName == "Tümü")
                {
                    foreach (var item in lstGroupSelection.Items.Cast<SelectableGroup>())
                        item.IsSelected = changedItem.IsSelected;
                }
                else if (!changedItem.IsSelected)
                {
                    var allItem = lstGroupSelection.Items.Cast<SelectableGroup>().FirstOrDefault(x => x.GroupName == "Tümü");
                    if (allItem != null) allItem.IsSelected = false;
                }
            }
        }

        private void LoadGroups()
        {
            try
            {
                var groups = _dbService.GetGroups();
                var selectableGroups = new List<SelectableGroup>();

                selectableGroups.Add(new SelectableGroup { GroupName = "Tümü", IsSelected = false, OriginalGroup = null });

                var allUsers = _dbService.GetAllRecipients();
                if (allUsers.Any(u => string.IsNullOrEmpty(u.GroupName)))
                {
                    selectableGroups.Add(new SelectableGroup { GroupName = "Grubu Yok (Boş)", IsSelected = false, OriginalGroup = null });
                }

                foreach (var g in groups)
                {
                    selectableGroups.Add(new SelectableGroup { GroupName = g.GroupName, IsSelected = false, OriginalGroup = g });
                }

                lstGroupSelection.ItemsSource = null;
                lstGroupSelection.ItemsSource = selectableGroups;
            }
            catch (Exception ex) { CustomAlert.Show("Grup hatası: " + ex.Message, "HATA"); }
        }

        private void BtnFilterGroups_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = lstGroupSelection.Items.Cast<SelectableGroup>().Where(g => g.IsSelected).ToList();

            if (selectedItems.Count == 0 || selectedItems.Any(x => x.GroupName == "Tümü"))
            {
                RefreshList();
                dgRecipients.SelectAll();
                CustomAlert.Show("Tüm liste getirildi.", "TAM LİSTE");
                btnGroupMenu.IsChecked = false;
                return;
            }

            try
            {
                HashSet<string> targetGroupNames = new HashSet<string>();
                bool includeNoGroup = false;

                foreach (var item in selectedItems)
                {
                    if (item.GroupName == "Grubu Yok (Boş)") includeNoGroup = true;
                    else targetGroupNames.Add(item.GroupName);
                }

                var allRecipients = _dbService.GetAllRecipients();
                var filteredList = allRecipients.Where(r =>
                    (targetGroupNames.Contains(r.GroupName)) ||
                    (includeNoGroup && string.IsNullOrEmpty(r.GroupName))
                ).ToList();

                dgRecipients.ItemsSource = null;
                dgRecipients.ItemsSource = filteredList;
                dgRecipients.SelectAll();

                CustomAlert.Show($"{filteredList.Count} kişi listelendi.", "FİLTRELENDİ");
                btnGroupMenu.IsChecked = false;
            }
            catch (Exception ex)
            {
                CustomAlert.Show("Filtreleme hatası: " + ex.Message, "HATA");
            }
        }
        private void BtnDeletePerson_Click(object sender, RoutedEventArgs e)
        {
            var selectedPeople = dgRecipients.SelectedItems.Cast<Recipient>().ToList();

            if (selectedPeople.Count == 0)
            {
                CustomAlert.Show("Silinecek kişi seçmedin.", "UYARI");
                return;
            }

            if (CustomAlert.Show($"{selectedPeople.Count} kişiyi silmek istiyor musun?", "SİL", true))
            {
                try
                {
                    foreach (var person in selectedPeople)
                    {
                        _dbService.DeleteRecipient(person.Id);
                    }

                    RefreshList();
                    LoadGroups();

                    ResetForm();

                    CustomAlert.Show("Silme işlemi tamam.", "BAŞARILI");
                }
                catch (Exception ex)
                {
                    CustomAlert.Show("Hata: " + ex.Message, "HATA");
                }
            }
        }
        private void BtnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewMail.Text))
            {
                CustomAlert.Show("Mail adresi yazman lazım patron.", "EKSİK");
                return;
            }
            Recipient person = new Recipient
            {
                Id = _selectedRecipientId, 
                Ad = txtNewName.Text.Trim(),
                Soyad = txtNewSurname.Text.Trim(),
                Email = txtNewMail.Text.Trim(),
                GroupName = txtNewGroup.Text.Trim()
            };

            try
            {
                if (_selectedRecipientId == 0)
                {
                    _dbService.AddRecipient(person); 
                    CustomAlert.Show("Kişi başarıyla eklendi.", "EKLEME");
                }
                else
                {
                    _dbService.UpdateRecipientDetails(person);
                    CustomAlert.Show("Kişi bilgileri güncellendi.", "GÜNCELLEME");
                }

                ResetForm();
                RefreshList();
                LoadGroups();
            }
            catch (Exception ex)
            {
                CustomAlert.Show("Hata oluştu: " + ex.Message, "HATA");
            }
        }
        private void DgRecipients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRecipients.SelectedItem is Recipient person)
            {
                _selectedRecipientId = person.Id;

                txtNewName.Text = person.Ad;
                txtNewSurname.Text = person.Soyad;
                txtNewMail.Text = person.Email;
                txtNewGroup.Text = person.GroupName;
                btnAddOrUpdate.Content = "GÜNCELLE";
                btnAddOrUpdate.Style = (Style)FindResource("OrangeGlassButton");
                btnCancelEdit.Visibility = Visibility.Visible;
            }
        }

        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        // Formu Sıfırlama Metodu
        private void ResetForm()
        {
            // 1. Kutuları boşalt
            txtNewName.Clear();
            txtNewSurname.Clear();
            txtNewMail.Clear();
            txtNewGroup.Clear();

            // 2. ID'yi sıfırla (Burası çok önemli!)
            _selectedRecipientId = 0;

            // 3. Butonu tekrar "EKLE" moduna (Yeşil) çevir
            btnAddOrUpdate.Content = "EKLE";
            btnAddOrUpdate.Style = (Style)FindResource("GreenGlassButton");

            // 4. İptal butonunu gizle ve listedeki seçimi kaldır
            btnCancelEdit.Visibility = Visibility.Collapsed;
            dgRecipients.SelectedIndex = -1;
        }
        private void RefreshList()
        {
            var recipients = _dbService.GetAllRecipients();
            dgRecipients.ItemsSource = recipients;
            if (recipients.Count > 0 && recipients[0].DataFields != null)
                UpdateVariableComboBox(recipients[0].DataFields.Keys);
        }

        private void UpdateVariableComboBox(IEnumerable<string> dynamicHeaders)
        {
            cmbVariables.Items.Clear();
            var constants = new List<string> { "Ad", "Soyad", "Email" };
            foreach (var c in constants) cmbVariables.Items.Add(c);
            foreach (var header in dynamicHeaders) if (!constants.Contains(header)) cmbVariables.Items.Add(header);
            cmbVariables.SelectedIndex = 0;
        }

        private void BtnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroups = lstGroupSelection.Items.Cast<SelectableGroup>().Where(g => g.IsSelected).ToList();
            if (selectedGroups.Count == 0) return;
            if (CustomAlert.Show("Seçili grupları silmek istiyor musun?", "SİL", true))
            {
                foreach (var g in selectedGroups) if (g.OriginalGroup != null) _dbService.DeleteGroup(g.GroupName);
                LoadGroups();
            }
        }

        private void BtnClearDb_Click(object sender, RoutedEventArgs e)
        {
            if (CustomAlert.Show("Tüm listeyi silmek istiyor musun?", "DİKKAT", true))
            {
                _dbService.ClearAll();
                RefreshList();
                LoadGroups();
            }
        }

        private void BtnAddTo_Click(object sender, RoutedEventArgs e) { AddToBox(txtTo); }
        private void BtnAddCc_Click(object sender, RoutedEventArgs e) { AddToBox(txtCc); }
        private void AddToBox(TextBox box)
        {
            var items = dgRecipients.SelectedItems;
            if (items.Count == 0) return;
            string text = box.Text.Trim();
            if (!string.IsNullOrEmpty(text) && !text.EndsWith(";")) text += "; ";
            List<string> mails = new List<string>();
            foreach (var item in items) if (item is Recipient r) mails.Add(r.Email);
            box.Text = text + string.Join("; ", mails);
        }

        private void BtnInsertVariable_Click(object sender, RoutedEventArgs e)
        {
            if (cmbVariables.SelectedItem == null) return;
            string tag = $"{{{cmbVariables.SelectedItem}}}";
            int idx = txtBody.CaretIndex;
            txtBody.Text = txtBody.Text.Insert(idx, tag);
            txtBody.CaretIndex = idx + tag.Length;
            txtBody.Focus();
        }

        private void BtnAddToQueue_Click(object sender, RoutedEventArgs e)
        {
            TemplateEngine engine = new TemplateEngine();
            string signature = "";
            if (chkUseSignature.IsChecked == true)
            {
                string sig = _dbService.GetSignature();
                if (!string.IsNullOrEmpty(sig)) signature = "<br><br>" + sig;
            }

            if (dgRecipients.SelectedItems.Count > 0)
            {
                foreach (var item in dgRecipients.SelectedItems)
                {
                    if (item is Recipient p)
                    {
                        _draftQueue.Add(new MailDraft
                        {
                            To = p.Email,
                            Cc = txtCc.Text,
                            Subject = engine.Parse(txtSubject.Text, p),
                            Body = engine.Parse(txtBody.Text, p) + signature,
                            Status = "Bekliyor",
                            SenderEmail = txtSpoofMail.Text.Trim()
                        });
                    }
                }
                lblQueueCount.Text = $"Bekleyen: {_draftQueue.Count}";
                dgRecipients.SelectedItems.Clear();
            }
            else CustomAlert.Show("Kişi seçmedin.", "UYARI");
        }

        // 1. FİLTRELEME METODU (ComboBox değişince çalışır)
        private void CmbQueueFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbQueueFilter.SelectedItem is ComboBoxItem item)
            {
                string filter = item.Content.ToString();

                // Listenin "Görünüm" katmanını alıyoruz
                ICollectionView view = CollectionViewSource.GetDefaultView(_draftQueue);

                if (view != null)
                {
                    if (filter == "Tümü")
                    {
                        view.Filter = null; // Filtreyi iptal et
                    }
                    else
                    {
                        // Sadece durumu eşleşenleri göster
                        view.Filter = (obj) =>
                        {
                            if (obj is MailDraft draft) return draft.Status == filter;
                            return false;
                        };
                    }
                }
            }
        }

        // 2. HATAYA TIKLAYINCA ÇALIŞAN METOD
        private void TxtStatus_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as TextBlock)?.DataContext is MailDraft draft)
            {
                if (draft.Status == "HATA")
                {
                    CustomAlert.Show($"HATA DETAYI:\n\n{draft.ErrorMessage}", "HATA RAPORU");
                }
            }
        }

        // 3. GÖNDERİM METODU (HATALARI YAKALAR VE KAYDEDER)
        private async void BtnSendQueue_Click(object sender, RoutedEventArgs e)
        {
            if (_draftQueue.Count == 0) return;

            // Filtreyi temizle ki gönderim sırasını görelim
            cmbQueueFilter.SelectedIndex = 0;

            int successCount = 0;
            int errorCount = 0;

            for (int i = 0; i < _draftQueue.Count; i++)
            {
                var draft = _draftQueue[i];
                if (draft.Status == "Gönderildi") continue;

                draft.Status = "Gönderiliyor...";

                try
                {
                    bool result = await _mailService.SendEmailAsync(draft.To, draft.Subject, draft.Body, draft.Cc, draft.SenderEmail);

                    if (result)
                    {
                        draft.Status = "Gönderildi";
                        successCount++;
                    }
                    else
                    {
                        throw new Exception("Sunucu bağlantısı reddedildi veya kimlik doğrulama hatası.");
                    }
                }
                catch (Exception ex)
                {
                    draft.Status = "HATA";
                    draft.ErrorMessage = ex.Message; 
                    errorCount++;
                }

      
                await Task.Delay(1000);
            }

            CustomAlert.Show($"İşlem Tamamlandı.\nBaşarılı: {successCount}\nHatalı: {errorCount}", "SONUÇ");
        }

        private void BtnRemoveSingleItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is MailDraft d) _draftQueue.Remove(d);
            lblQueueCount.Text = $"Bekleyen: {_draftQueue.Count}";
        }

        private void BtnClearQueue_Click(object sender, RoutedEventArgs e)
        {
            _draftQueue.Clear();
            lblQueueCount.Text = "Bekleyen: 0";
        }

        private void LstQueue_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) { /* Önizleme kodu */ }

        // Sürükle Bırak ve Pencere Kontrolleri
        private void TextBox_PreviewDragOver(object s, DragEventArgs e) { e.Effects = DragDropEffects.Copy; e.Handled = true; }
        private void TextBox_Drop(object s, DragEventArgs e) { /* Resim ekleme kodu */ }
        private void TitleBar_MouseDown(object s, System.Windows.Input.MouseButtonEventArgs e) { if (e.ChangedButton == System.Windows.Input.MouseButton.Left) DragMove(); }
        private void BtnClose_Click(object s, RoutedEventArgs e) { Application.Current.Shutdown(); }
        private void BtnMinimize_Click(object s, RoutedEventArgs e) { WindowState = WindowState.Minimized; }
        private void BtnMaximize_Click(object s, RoutedEventArgs e) { WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal; }

        // Şablon, Link, İmza butonları...
        private void LoadTemplates() { cmbTemplates.ItemsSource = _dbService.GetTemplates(); }
        private void BtnSaveTemplate_Click(object s, RoutedEventArgs e)
        {
            TemplateNameWindow win = new TemplateNameWindow();
            if (win.ShowDialog() == true) { _dbService.SaveTemplate(win.EnteredName, txtSubject.Text, txtBody.Text); LoadTemplates(); }
        }
        private void BtnDeleteTemplate_Click(object s, RoutedEventArgs e)
        {
            if (cmbTemplates.SelectedItem is MessageTemplate t && CustomAlert.Show("Silinsin mi?", "SİL", true))
            { _dbService.DeleteTemplate(t.Id); LoadTemplates(); }
        }
        private void CmbTemplates_SelectionChanged(object s, SelectionChangedEventArgs e)
        {
            if (cmbTemplates.SelectedItem is MessageTemplate t) { txtSubject.Text = t.Subject; txtBody.Text = t.Body; }
        }
        private void Logo_Click(object s, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com",
                    UseShellExecute = true
                });
            }
            catch { }
        }
        private void BtnAddLink_Click(object s, RoutedEventArgs e)
        {
            LinkWindow win = new LinkWindow();
            if (win.ShowDialog() == true) { int i = txtBody.CaretIndex; txtBody.Text = txtBody.Text.Insert(i, win.ResultHtml); }
        }
        private void BtnSetSignature_Click(object s, RoutedEventArgs e) { /* İmza penceresi */ }
    }

}