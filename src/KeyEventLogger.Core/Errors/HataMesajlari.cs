namespace KeyEventLogger.Core.Errors;

// Kullaniciya gosterilen hata mesajlari tek yerde.
// Sadece BEKLENEN hatalar kullaniciya sebebiyle gosterilir (dosya yazilamadi gibi).
// Kodumuzdaki hatalar (bug) yakalanmaz: Debug'da debugger durur, Release'te GenelHataYakalayici
// loga yazip YakalanmamisHata mesajini gosterir.
public static class HataMesajlari
{
    public const string DosyaYazilamadi = "Kayıtlar dosyaya yazılamadı.";
    public const string UygulamaBaslatilamadi = "Key Event Logger başlatılamadı. Ayrıntılar log dosyasında.";
    public const string PencereIkonuYuklenemedi = "Pencere ikonu yüklenemedi.";
    public const string HataMesajiGosterilemedi = "Kullanıcıya hata mesajı gösterilemedi.";
    public const string YakalanmamisHata = "Beklenmeyen bir hata oluştu. Uygulama çalışmaya devam ediyor.";
}
