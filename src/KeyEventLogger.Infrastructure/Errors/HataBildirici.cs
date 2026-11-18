using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Errors;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Infrastructure.Errors;

// IHataBildirici'nin gercek uygulamasi (AliBird ile ayni).
// Tek sorumlulugu: hatanin hangi kanala gidecegine karar vermek (S).
//   - HER hata loga yazilir (IGunlukcu)
//   - EkranVeGunluk ise kullaniciya da gosterilir (IKullaniciBildirici)
// Butun catch bloklarinin son duragi burasi; hicbir yolu disari exception sizdirmaz.
public sealed class HataBildirici : IHataBildirici
{
    // Mesaj bos gelirse (programci hatasi) yine de anlamli bir metin gosterilir.
    private const string VarsayilanKullaniciMesaji = HataMesajlari.YakalanmamisHata;

    private readonly IGunlukcu _gunlukcu;
    private readonly IKullaniciBildirici _kullaniciBildirici;

    public HataBildirici(IGunlukcu gunlukcu, IKullaniciBildirici kullaniciBildirici)
    {
        ArgumentNullException.ThrowIfNull(gunlukcu);
        ArgumentNullException.ThrowIfNull(kullaniciBildirici);
        _gunlukcu = gunlukcu;
        _kullaniciBildirici = kullaniciBildirici;
    }

    public void Bildir(Exception hata, Gorunurluk gorunurluk, string kullaniciMesaji)
    {
        // Yanlis kullanimda bile cokmeyelim: hata nesnesi gelmediyse yerine bir tane uret.
        hata ??= new InvalidOperationException("Hata nesnesi verilmeden Bildir cagrildi.");
        bool mesajBos = string.IsNullOrWhiteSpace(kullaniciMesaji);

        // 1) Her durumda loga yaz.
        try
        {
            _gunlukcu.Hata(hata, mesajBos ? hata.Message : kullaniciMesaji);
        }
        catch
        {
            // log yazilamiyorsa sessizce devam.
        }

        // 2) Sadece EkranVeGunluk ise kullaniciya goster.
        if (gorunurluk != Gorunurluk.EkranVeGunluk)
        {
            return;
        }

        try
        {
            _kullaniciBildirici.MesajGoster(mesajBos ? VarsayilanKullaniciMesaji : kullaniciMesaji);
        }
        catch (Exception gostermeHatasi)
        {
            // Mesaj kutusu acilamadiysa bu da loga yazilir, uygulama devam eder.
            try
            {
                _gunlukcu.Hata(gostermeHatasi, HataMesajlari.HataMesajiGosterilemedi);
            }
            catch
            {
                // bilerek bos: uygulama devam etsin
            }
        }
    }
}
