using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Errors;
using KeyEventLogger.Core.Presentation;
using KeyEventLogger.Core.Services;
using KeyEventLogger.Infrastructure.Errors;
using KeyEventLogger.Infrastructure.Files;
using KeyEventLogger.Infrastructure.FileSystem;
using KeyEventLogger.Infrastructure.Logging;
using KeyEventLogger.Infrastructure.Platform;
using KeyEventLogger.WinForms.Forms;
using KeyEventLogger.WinForms.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace KeyEventLogger.WinForms;

// ================================================================================
//  UYGULAMANIN BASLANGIC NOKTASI ve "KOMPOZISYON KOKU" (Composition Root)
//
//  Tek sorumlulugu: uygulamayi ayaga kaldirmak.
//   1) Genel hata yakalayiciyi devreye almak
//   2) Siniflari birbirine baglamak: DI container SADECE burada kurulur (R2)
//   3) Pencereyi acmak, acilis/kapanisi loglamak
// ================================================================================
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        // 1) Genel hata yakalayici: HER SEYDEN ONCE (R6).
        var genelHataYakalayici = new GenelHataYakalayici();
        genelHataYakalayici.Kur();

        ApplicationConfiguration.Initialize();

        // 2) Siniflari bagla. Hata olursa uygulama acilamaz; kullaniciya soylenir.
        ServiceProvider servisler;
        try
        {
            servisler = ServisleriKur();
            genelHataYakalayici.BildiriciyiAta(servisler.GetRequiredService<IHataBildirici>());
        }
        catch (Exception hata)
        {
            BaslangicHatasiniBildir(hata, "Uygulama başlatılamadı: servisler (DI) kurulamadı.");
            return;
        }

        // 3) Calistir. "using": cikista butun servisler (log dahil) duzgun kapanir.
        using (servisler)
        {
            IGunlukcu gunlukcu = servisler.GetRequiredService<IGunlukcu>();
            try
            {
                gunlukcu.Bilgi($"KeyEventLogger {Application.ProductVersion} açıldı.");

                AnaForm form = servisler.GetRequiredService<AnaForm>();
                servisler.GetRequiredService<KayitYoneticisi>().Hazirla();   // form olaylarina abone olur

                Application.Run(form);
            }
            catch (Exception hata)
            {
                BaslangicHatasiniBildir(hata, "Uygulama başlatılamadı: ana pencere açılamadı.");
            }
            finally
            {
                gunlukcu.Bilgi("KeyEventLogger kapandı.");
            }
        }
    }

    // ================= DI KURULUMU (hepsi Singleton: tek pencere, tek oturum) =================
    private static ServiceProvider ServisleriKur()
    {
        var servisler = new ServiceCollection();

        // ---- Core ----
        servisler.AddSingleton<IKayitOturumu, KayitOturumu>();
        servisler.AddSingleton<ITusIstatistikHesaplayici, TusIstatistikHesaplayici>();
        servisler.AddSingleton<IFiltreDogrulayici, FiltreDogrulayici>();
        servisler.AddSingleton<IKayitFiltreleyici, KayitFiltreleyici>();
        servisler.AddSingleton<ISatirBicimleyici, SatirBicimleyici>();
        servisler.AddSingleton<IDosyaIcerigiOlusturucu, DosyaIcerigiOlusturucu>();
        servisler.AddSingleton<IDosyaHatasiAciklayici, DosyaHatasiAciklayici>();
        servisler.AddSingleton<KayitYoneticisi>();   // presenter

        // ---- Infrastructure ----
        servisler.AddSingleton<IUygulamaYollari>(_ => new UygulamaYollari(UygulamaYollari.VarsayilanKokKlasor()));
        servisler.AddSingleton<IGunlukcu, SerilogGunlukcu>();
        servisler.AddSingleton<IHataBildirici, HataBildirici>();
        servisler.AddSingleton<IDosyaYazici, MetinDosyasiYazici>();
        servisler.AddSingleton<ISaat, SistemSaati>();

        // ---- WinForms ----
        servisler.AddSingleton<ITusAdiCevirici, TusAdiCevirici>();
        servisler.AddSingleton<IKullaniciBildirici, MesajKutusuBildirici>();
        servisler.AddSingleton<IKaydetmeDiyalogu, KaydetmeDiyalogu>();
        servisler.AddSingleton<AnaForm>();
        // Presenter IKayitGorunumu ister; ona AYNI AnaForm ornegi verilir.
        servisler.AddSingleton<IKayitGorunumu>(sp => sp.GetRequiredService<AnaForm>());

        // ValidateOnBuild: eksik ya da hatali bir kayit varsa uygulama acilirken hemen anlasilir.
        return servisler.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }

    // Uygulama hic acilamadi: kullaniciya soyle ve mumkunse loga yaz.
    // logMesaji: hatanin NEREDE oldugu (DI mi, pencere mi) loga yazilsin diye.
    private static void BaslangicHatasiniBildir(Exception hata, string logMesaji)
    {
        try
        {
            MessageBox.Show(HataMesajlari.UygulamaBaslatilamadi, "Key Event Logger",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch
        {
            // R6
        }

        try
        {
            // DI kurulamamis olabilir: log icin gecici bir gunlukcu (kompozisyon kokunun yedek yolu).
            using var gunlukcu = new SerilogGunlukcu(new UygulamaYollari(UygulamaYollari.VarsayilanKokKlasor()));
            gunlukcu.Hata(hata, logMesaji);
        }
        catch
        {
            // R6
        }
    }
}
