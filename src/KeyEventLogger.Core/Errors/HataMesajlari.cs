namespace KeyEventLogger.Core.Errors;

// ================================================================================
//  HATA KATALOGU (CK19): uygulamadaki BUTUN hata mesajlari tek yerde, adiyla.
//  Her hata loga MUTLAKA yazilir (hata turu + yigin izi ile). Ekranda gosterilip
//  gosterilmeyecegi yanindaki yorumda yazar:
//    [EKRAN] kullanici bir sey yapti ve sonucu olmadi -> bilmesi gerekir
//    [LOG]   kullanicinin yapabilecegi bir sey yok / her tusta mesaj kutusu acilmamali
//  Tablonun aynisi docs/DECISIONS.md "Hata haritasi" bolumunde.
// ================================================================================
public static class HataMesajlari
{
    // ---- Butonlar [EKRAN] ----
    public const string KayitBaslatilamadi = "Kayıt başlatılamadı.";
    public const string KayitDurdurulamadi = "Kayıt durdurulamadı.";
    public const string KayitlarTemizlenemedi = "Kayıtlar temizlenemedi.";
    public const string KaydetmePenceresiAcilamadi = "Kaydetme penceresi açılamadı.";
    public const string DosyaYazilamadi = "Kayıtlar dosyaya yazılamadı.";
    public const string KaydetmeBasarisiz = "Kaydetme işlemi tamamlanamadı.";

    // ---- Filtre [EKRAN] ----
    public const string FiltreUygulanamadi = "Filtre uygulanamadı.";

    // ---- Acilis [EKRAN] ----
    public const string UygulamaHazirlanamadi = "Uygulama hazırlanamadı.";
    public const string UygulamaBaslatilamadi = "Key Event Logger başlatılamadı. Ayrıntılar log dosyasında.";

    // ---- Arka planda [LOG] ----
    public const string TusOlayiIslenemedi = "Tuş olayı işlenemedi.";              // her tusta mesaj kutusu acilmasin
    public const string TusOlayiIletilemedi = "Tuş olayı formdan iletilemedi.";
    public const string ButonOlayiIletilemedi = "Buton olayı formdan iletilemedi.";
    public const string PencereIkonuYuklenemedi = "Pencere ikonu yüklenemedi.";
    public const string HataMesajiGosterilemedi = "Kullanıcıya hata mesajı gösterilemedi.";

    // ---- Son savunma hatti ----
    public const string YakalanmamisHata = "Beklenmeyen bir hata oluştu. Uygulama çalışmaya devam ediyor.";   // ilk kez [EKRAN], sonra [LOG]
}
