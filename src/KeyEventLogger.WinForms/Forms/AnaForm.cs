using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Errors;
using KeyEventLogger.Core.Models;
using KeyEventLogger.WinForms.Platform;

namespace KeyEventLogger.WinForms.Forms;

// ================================================================================
//  ANA PENCERE (MVP'deki pasif View)
//
//  Tek sorumlulugu: ekrani gostermek ve kullanicinin yaptiklarini (buton, tus, filtre)
//  olay olarak presenter'a (KayitYoneticisi) bildirmek. Hicbir KARAR vermez.
//
//  Klavye olaylari: KeyPreview = true sayesinde hangi kontrol odakta olursa olsun
//  tuslar ONCE formun KeyDown / KeyPress / KeyUp olaylarina gelir.
//  Bunlar sadece BU pencere aktifken gelir. Global hook yok, baska pencereler gorulmez.
//
//  Kontroller designer ile degil kodla olusturulur (her satirin ne yaptigi gorulsun diye).
// ================================================================================
public sealed class AnaForm : Form, IKayitGorunumu
{
    private static readonly Color AcikRenk = Color.ForestGreen;
    private static readonly Color KapaliRenk = Color.Firebrick;

    private readonly ITusAdiCevirici _tusAdiCevirici;
    private readonly IHataBildirici _hataBildirici;

    // ---- Kontroller ----
    private readonly Label _gosterge;                 // "● KAYIT AÇIK / KAPALI"
    private readonly OdakAlmayanButon _baslatButonu;
    private readonly OdakAlmayanButon _durdurButonu;
    private readonly OdakAlmayanButon _temizleButonu;
    private readonly OdakAlmayanButon _kaydetButonu;
    private readonly CheckBox _filtreKutusu;          // filtre acik/kapali
    private readonly DateTimePicker _baslangicSecici;
    private readonly DateTimePicker _bitisSecici;
    private readonly ListBox _liste;
    private readonly Label _istatistik;

    public AnaForm(ITusAdiCevirici tusAdiCevirici, IHataBildirici hataBildirici)
    {
        ArgumentNullException.ThrowIfNull(tusAdiCevirici);
        ArgumentNullException.ThrowIfNull(hataBildirici);
        _tusAdiCevirici = tusAdiCevirici;
        _hataBildirici = hataBildirici;

        // ---- Pencere ----
        Text = "Key Event Logger";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(920, 580);
        MinimumSize = new Size(720, 420);
        Font = new Font("Segoe UI", 9.75f);
        KeyPreview = true;   // tuslar once forma gelsin
        PencereIkonunuYukle();

        // ---- Ust satir: gosterge + butonlar ----
        _gosterge = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            Margin = new Padding(3, 8, 20, 3)
        };
        _baslatButonu = Buton("Başlat");
        _durdurButonu = Buton("Durdur");
        _temizleButonu = Buton("Temizle");
        _kaydetButonu = Buton("Kaydet (.txt)");
        _baslatButonu.Click += (_, _) => OlayiTetikle(BaslatTiklandi);
        _durdurButonu.Click += (_, _) => OlayiTetikle(DurdurTiklandi);
        _temizleButonu.Click += (_, _) => OlayiTetikle(TemizleTiklandi);
        _kaydetButonu.Click += (_, _) => OlayiTetikle(KaydetTiklandi);

        var ustSatir = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 8, 8, 0) };
        ustSatir.Controls.AddRange([_gosterge, _baslatButonu, _durdurButonu, _temizleButonu, _kaydetButonu]);

        // ---- Ikinci satir: tarih/saat filtresi ----
        _filtreKutusu = new CheckBox { Text = "Filtrele:", AutoSize = true, TabStop = false, Margin = new Padding(3, 7, 3, 3) };
        _baslangicSecici = TarihSecici(DateTime.Today);
        _bitisSecici = TarihSecici(DateTime.Today.AddDays(1).AddSeconds(-1));   // bugun 23:59:59
        _filtreKutusu.CheckedChanged += (_, _) =>
        {
            _baslangicSecici.Enabled = _bitisSecici.Enabled = _filtreKutusu.Checked;
            OlayiTetikle(FiltreDegisti);
            _liste?.Focus();   // tuslar yine listeye gitsin
        };
        _baslangicSecici.ValueChanged += (_, _) => OlayiTetikle(FiltreDegisti);
        _bitisSecici.ValueChanged += (_, _) => OlayiTetikle(FiltreDegisti);

        var filtreSatiri = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 4, 8, 4) };
        filtreSatiri.Controls.AddRange(
        [
            _filtreKutusu, _baslangicSecici,
            new Label { Text = "—", AutoSize = true, Margin = new Padding(3, 7, 3, 3) },
            _bitisSecici
        ]);

        // ---- Sag: istatistik ----
        _istatistik = new Label { Dock = DockStyle.Fill, Font = new Font("Consolas", 10f), Padding = new Padding(6) };
        var istatistikKutusu = new GroupBox { Text = "İstatistik", Dock = DockStyle.Right, Width = 230 };
        istatistikKutusu.Controls.Add(_istatistik);

        // ---- Alt: bilgi satiri (kullaniciya ne yapildigini acikca soyler) ----
        var bilgi = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 24,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 0, 0, 0),
            ForeColor = Color.DimGray,
            Text = "Tuşlar sadece bu pencere aktifken ve kayıt açıkken yakalanır. Tuşlar log dosyasına yazılmaz."
        };

        // ---- Orta: liste (ListBox) ----
        _liste = new ListBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10f),   // esit genislikli: sutunlar hizali
            HorizontalScrollbar = true,
            IntegralHeight = false
        };
        // Tab ve ok tuslari normalde odagi tasir / KeyDown uretmez; liste odaktayken onlar da kaydedilsin.
        _liste.PreviewKeyDown += (_, e) => e.IsInputKey = true;

        Controls.AddRange([_liste, istatistikKutusu, bilgi, filtreSatiri, ustSatir]);
        _liste.BringToFront();   // Dock = Fill olan kontrol en son yerlesir: kalan alani doldurur

        // ---- Klavye olaylari (PDF: KeyDown, KeyPress, KeyUp) ----
        KeyDown += (_, e) => TusOlayiBildir(OlayTuru.TusBasildi, e);
        KeyUp += (_, e) => TusOlayiBildir(OlayTuru.TusBirakildi, e);
        KeyPress += (_, e) => KarakterOlayiBildir(e);

        Shown += (_, _) => _liste.Focus();
    }

    // ================= IKayitGorunumu: olaylar =================
    public event EventHandler? BaslatTiklandi;
    public event EventHandler? DurdurTiklandi;
    public event EventHandler? TemizleTiklandi;
    public event EventHandler? KaydetTiklandi;
    public event EventHandler? FiltreDegisti;
    public event EventHandler<HamTusOlayi>? TusOlayiGeldi;

    // ================= IKayitGorunumu: filtre degerleri =================
    public bool FiltreAcikMi => _filtreKutusu.Checked;
    public DateTime FiltreBaslangic => _baslangicSecici.Value;
    public DateTime FiltreBitis => _bitisSecici.Value;

    // ================= IKayitGorunumu: gosterim =================
    public void SatirEkle(string satir)
    {
        _liste.Items.Add(satir);
        _liste.TopIndex = _liste.Items.Count - 1;   // son satir gorunsun (otomatik kaydirma)
    }

    public void ListeyiDoldur(IReadOnlyList<string> satirlar)
    {
        // BeginUpdate/EndUpdate: binlerce satirda her ekleme icin yeniden cizim yapilmasin.
        _liste.BeginUpdate();
        try
        {
            _liste.Items.Clear();
            _liste.Items.AddRange(satirlar.ToArray<object>());
        }
        finally
        {
            _liste.EndUpdate();
        }
        if (_liste.Items.Count > 0)
        {
            _liste.TopIndex = _liste.Items.Count - 1;
        }
    }

    public void KayitDurumunuGoster(bool kayitAcik)
    {
        _gosterge.Text = kayitAcik ? "● KAYIT AÇIK" : "● KAYIT KAPALI";
        _gosterge.ForeColor = kayitAcik ? AcikRenk : KapaliRenk;
        _baslatButonu.Enabled = !kayitAcik;
        _durdurButonu.Enabled = kayitAcik;
        Text = kayitAcik ? "Key Event Logger — kayıt açık" : "Key Event Logger";
    }

    public void IstatistigiGoster(IstatistikOzeti ozet)
    {
        var satirlar = new List<string> { $"Toplam tuş: {ozet.ToplamTus}", "", "En sık tuşlar:" };
        if (ozet.EnSikTuslar.Count == 0)
        {
            satirlar.Add("  -");
        }
        for (int i = 0; i < ozet.EnSikTuslar.Count; i++)
        {
            TusSayisi tus = ozet.EnSikTuslar[i];
            satirlar.Add($"{i + 1}. {tus.Tus,-10} {tus.Adet,5}");
        }
        _istatistik.Text = string.Join(Environment.NewLine, satirlar);
    }

    // Tek basina Alt (ya da F10) birakilinca Windows pencerenin sistem menusune gecer ve sonraki
    // tuslari menu yutar; kayit kopuk gorunur. Bu tek durum engellenir (Alt+Space menusu calismaya devam eder).
    protected override void WndProc(ref Message m)
    {
        const int WmSysCommand = 0x0112;
        const int ScKeyMenu = 0xF100;
        if (m.Msg == WmSysCommand && ((int)m.WParam & 0xFFF0) == ScKeyMenu && m.LParam == IntPtr.Zero)
        {
            return;
        }
        base.WndProc(ref m);
    }

    // ================= Yardimcilar =================

    private void TusOlayiBildir(OlayTuru tur, KeyEventArgs e) =>
        TusOlayiGeldi?.Invoke(this, new HamTusOlayi(tur, _tusAdiCevirici.Ad(e.KeyCode), null, e.Control, e.Shift, e.Alt));

    private void KarakterOlayiBildir(KeyPressEventArgs e)
    {
        // KeyPress'te Ctrl/Shift/Alt bilgisi gelmez; o anki durumu Windows'tan okuruz.
        Keys ek = ModifierKeys;
        TusOlayiGeldi?.Invoke(this, new HamTusOlayi(OlayTuru.KarakterYazildi, e.KeyChar.ToString(), e.KeyChar,
            ek.HasFlag(Keys.Control), ek.HasFlag(Keys.Shift), ek.HasFlag(Keys.Alt)));
    }

    private void OlayiTetikle(EventHandler? olay) => olay?.Invoke(this, EventArgs.Empty);

    private static OdakAlmayanButon Buton(string metin) =>
        new() { Text = metin, AutoSize = true, Padding = new Padding(8, 2, 8, 2), Margin = new Padding(3, 3, 6, 3) };

    private static DateTimePicker TarihSecici(DateTime deger) => new()
    {
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "dd.MM.yyyy HH:mm:ss",
        ShowUpDown = true,   // saat/dakika/saniye de secilebilsin
        Width = 170,
        Enabled = false,     // filtre kutusu isaretlenince acilir
        Value = deger
    };

    // Pencere ikonu = exe'ye gomulu ikon. Bulunamazsa varsayilan ikon kalir.
    private void PencereIkonunuYukle()
    {
        try
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? Icon;
        }
        catch (Exception hata) when (hata is IOException or ArgumentException or UnauthorizedAccessException)
        {
            // Beklenen: exe dosyasi okunamadi. Ikon olmadan da uygulama calisir: sadece log.
            _hataBildirici.Bildir(hata, Gorunurluk.SadeceGunluk, HataMesajlari.PencereIkonuYuklenemedi);
        }
    }
}
