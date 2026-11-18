using System.Diagnostics;
using System.Text;
using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Presentation;
using KeyEventLogger.Core.Services;
using KeyEventLogger.Infrastructure.Errors;
using KeyEventLogger.Infrastructure.Files;
using KeyEventLogger.Infrastructure.FileSystem;
using KeyEventLogger.Infrastructure.Logging;
using KeyEventLogger.Infrastructure.Platform;
using KeyEventLogger.WinForms;
using KeyEventLogger.WinForms.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace KeyEventLogger.SmokeTests;

// Smoke: uygulamanin parcalari GERCEK halleriyle bir arada calisiyor mu?
[TestClass]
[TestCategory("Smoke")]
public sealed class UygulamaSmokeTestleri
{
    private string _klasor = null!;

    [TestInitialize]
    public void Hazirla()
    {
        _klasor = Path.Combine(Path.GetTempPath(), "kel-smoke-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_klasor);
    }

    [TestCleanup]
    public void Temizle()
    {
        try { Directory.Delete(_klasor, recursive: true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
    }

    // Program.cs'teki GERCEK kayitlarla container kuruluyor mu (ValidateOnBuild), ana parcalar olusuyor mu?
    [TestMethod]
    public void DI_GercekKayitlarlaKurulurVeAnaParcalarOlusur()
    {
        using ServiceProvider servisler = Program.ServisleriKur(_klasor);

        AnaForm form = servisler.GetRequiredService<AnaForm>();
        KayitYoneticisi yonetici = servisler.GetRequiredService<KayitYoneticisi>();
        yonetici.Hazirla();

        Assert.AreSame(form, servisler.GetRequiredService<IKayitGorunumu>());   // presenter ayni formu kullaniyor
        form.Dispose();
    }

    // Ana akis: Baslat -> tuslar -> Kaydet. Gercek oturum, bicimleyici, StreamWriter ve Serilog; sadece form ve
    // "Farkli Kaydet" penceresi sahte (testte ekrana pencere acilmasin).
    [TestMethod]
    public void UctanUca_BaslatTuslarKaydet_DosyaDiskeYazilir()
    {
        string dosya = Path.Combine(_klasor, "kayit.txt");
        var gorunum = new KucukGorunum();
        using var gunlukcu = new SerilogGunlukcu(new UygulamaYollari(_klasor));
        var bildirici = new KucukBildirici();
        var yonetici = new KayitYoneticisi(gorunum, new KayitOturumu(), new TusIstatistikHesaplayici(),
            new FiltreDogrulayici(), new KayitFiltreleyici(), new SatirBicimleyici(),
            new DosyaIcerigiOlusturucu(new SatirBicimleyici()), new SabitYolDiyalogu(dosya), new MetinDosyasiYazici(),
            new DosyaHatasiAciklayici(), bildirici, new HataBildirici(gunlukcu, bildirici), gunlukcu, new SistemSaati());
        yonetici.Hazirla();

        gorunum.Tetikle(gorunum.Baslat);
        gorunum.Tus(OlayTuru.TusBasildi, "A", null);
        gorunum.Tus(OlayTuru.KarakterYazildi, "a", 'a');
        gorunum.Tus(OlayTuru.TusBirakildi, "A", null);
        gorunum.Tetikle(gorunum.Kaydet);

        string icerik = File.ReadAllText(dosya, Encoding.UTF8);
        Assert.Contains("Kayıt sayısı: 3", icerik);
        Assert.Contains("KeyPress  'a'", icerik);
        Assert.AreEqual(1, gorunum.SonIstatistik?.ToplamTus);
    }

    // Derlenen exe gercekten aciliyor ve duzgun kapaniyor mu?
    [TestMethod]
    public void Exe_AcilirVeDuzgunKapanir()
    {
        string exe = Path.Combine(AppContext.BaseDirectory, "KeyEventLogger.exe");
        if (!File.Exists(exe))
        {
            Assert.Inconclusive($"Exe bulunamadi: {exe}");
        }

        using Process surec = Process.Start(new ProcessStartInfo(exe) { UseShellExecute = false })!;
        try
        {
            // WaitForInputIdle pencere gorunmeden donebilir: ana pencere tutamaci (handle) olusana kadar bekle.
            var sayac = Stopwatch.StartNew();
            while (surec.MainWindowHandle == IntPtr.Zero && !surec.HasExited && sayac.ElapsedMilliseconds < 15000)
            {
                Thread.Sleep(100);
                surec.Refresh();
            }
            Assert.IsFalse(surec.HasExited, "Uygulama acilir acilmaz kapandi.");
            Assert.AreNotEqual(IntPtr.Zero, surec.MainWindowHandle, "Ana pencere 15 sn icinde acilmadi.");
            Thread.Sleep(500);   // pencere ilk cizimini bitirsin

            Assert.IsTrue(surec.CloseMainWindow(), $"Kapat mesaji gonderilemedi. Pencere: '{surec.MainWindowTitle}'");
            Assert.IsTrue(surec.WaitForExit(10000), $"Uygulama 10 sn icinde kapanmadi. Pencere: '{surec.MainWindowTitle}'");
            Assert.AreEqual(0, surec.ExitCode);
        }
        finally
        {
            if (!surec.HasExited)
            {
                surec.Kill();
            }
        }
    }

    // ---- Smoke testine ozel kucuk sahte siniflar ----

    private sealed class KucukGorunum : IKayitGorunumu
    {
        public event EventHandler? BaslatTiklandi;
        public event EventHandler? DurdurTiklandi;
        public event EventHandler? TemizleTiklandi;
        public event EventHandler? KaydetTiklandi;
        public event EventHandler? FiltreDegisti;
        public event EventHandler<HamTusOlayi>? TusOlayiGeldi;

        public bool FiltreAcikMi => false;
        public DateTime FiltreBaslangic => DateTime.MinValue;
        public DateTime FiltreBitis => DateTime.MaxValue;
        public IstatistikOzeti? SonIstatistik { get; private set; }

        public string Baslat => nameof(BaslatTiklandi);
        public string Kaydet => nameof(KaydetTiklandi);

        public void SatirEkle(string satir) { }
        public void ListeyiDoldur(IReadOnlyList<string> satirlar) { }
        public void KayitDurumunuGoster(bool kayitAcik) { }
        public void IstatistigiGoster(IstatistikOzeti ozet) => SonIstatistik = ozet;

        public void Tetikle(string olay)
        {
            EventHandler? h = olay == nameof(BaslatTiklandi) ? BaslatTiklandi : KaydetTiklandi;
            h?.Invoke(this, EventArgs.Empty);
            _ = (DurdurTiklandi, TemizleTiklandi, FiltreDegisti);   // kullanilmayan olay uyarisi olmasin
        }

        public void Tus(OlayTuru tur, string tus, char? karakter) =>
            TusOlayiGeldi?.Invoke(this, new HamTusOlayi(tur, tus, karakter, false, false, false));
    }

    private sealed class KucukBildirici : IKullaniciBildirici
    {
        public void MesajGoster(string mesaj) { }
    }

    private sealed class SabitYolDiyalogu(string yol) : IKaydetmeDiyalogu
    {
        public string? YolSor(string varsayilanDosyaAdi) => yol;
    }
}
