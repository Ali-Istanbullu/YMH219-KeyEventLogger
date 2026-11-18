using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Errors;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.WinForms.Platform;

// Uygulamanin "son savunma hatti": kodumuzdaki (bug) ve gozden kacan hatalar buraya duser.
// Neden DI'dan gelmiyor? Bu yakalayicilar DI kurulmadan ONCE devreye girmeli; Program.cs onu
// ilk is olarak olusturur, hata bildirici hazir olunca BildiriciyiAta ile verilir.
public sealed class GenelHataYakalayici
{
    private const string Baslik = "Key Event Logger";

    private IHataBildirici? _hataBildirici;

    // Mesaj kutusu en fazla bir kez acilir; sonrakiler sadece loga gider.
    private bool _ekrandaGosterildi;

    public void Kur()
    {
        // Arka plan is parcaciklarindaki ve her turlu yakalanmamis hata en azindan loga yazilsin.
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Bildir(e.ExceptionObject as Exception ?? new InvalidOperationException("Bilinmeyen hata"));

        if (DerlemeModu.HataAyiklamaMi)
        {
            // Debug: arayuz hatalari YAKALANMAZ. Debugger hatanin oldugu satirda durur (bug'i yazarken goruruz).
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            return;
        }

        // Release: arayuz is parcacigindaki hata uygulamayi kapatmaz; loga yazilir, kullanici bilgilendirilir.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Bildir(e.Exception);

        // Beklenmeyen (await edilmemis) Task hatalari: loga yaz ve "goruldu" isaretle.
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Bildir(e.Exception);
            e.SetObserved();
        };
    }

    public void BildiriciyiAta(IHataBildirici hataBildirici)
    {
        ArgumentNullException.ThrowIfNull(hataBildirici);
        _hataBildirici = hataBildirici;
    }

    private void Bildir(Exception hata)
    {
        try
        {
            if (_hataBildirici is null)
            {
                MessageBox.Show(HataMesajlari.YakalanmamisHata, Baslik, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Gorunurluk gorunurluk = _ekrandaGosterildi ? Gorunurluk.SadeceGunluk : Gorunurluk.EkranVeGunluk;
            _ekrandaGosterildi = true;
            _hataBildirici.Bildir(hata, gorunurluk, HataMesajlari.YakalanmamisHata);
        }
        catch
        {
            // son savunma hattinda da exception disari cikmaz.
        }
    }
}
