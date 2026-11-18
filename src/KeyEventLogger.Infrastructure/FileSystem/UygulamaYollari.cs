namespace KeyEventLogger.Infrastructure.FileSystem;

// IUygulamaYollari'nin gercek uygulamasi.
// Loglar program klasorune DEGIL, %LocalAppData%\KeyEventLogger altina yazilir
// (Program Files'a yazilamaz; uygulama kaldirilsa da loglar kalir).
// Klasor silinirse hata vermez: her istekte klasor yoksa yeniden olusturulur.
public sealed class UygulamaYollari : IUygulamaYollari
{
    private const string GunlukKlasorAdi = "logs";
    private const string GunlukDosyaAdi = "keyeventlogger.log";

    private readonly string _kokKlasor;

    // kokKlasor: gercekte VarsayilanKokKlasor(), testte gecici bir klasor.
    public UygulamaYollari(string kokKlasor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kokKlasor);
        _kokKlasor = kokKlasor;
    }

    // Gercek konum: C:\Users\<kullanici>\AppData\Local\KeyEventLogger
    public static string VarsayilanKokKlasor() =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeyEventLogger");

    public string GunlukKlasoru()
    {
        // Klasor yoksa olusturur, varsa hicbir sey yapmaz. Olusturulamazsa exception; cagiran yakalar.
        string klasor = Path.Combine(_kokKlasor, GunlukKlasorAdi);
        Directory.CreateDirectory(klasor);
        return klasor;
    }

    public string GunlukDosyasi() => Path.Combine(GunlukKlasoru(), GunlukDosyaAdi);
}
