namespace KeyEventLogger.Core.Models;

// Bir hatanin kullaniciya gosterilip gosterilmeyecegi.
// Iki durumda da hata MUTLAKA log dosyasina yazilir.
public enum Gorunurluk
{
    // Hata hem ekranda kullaniciya gosterilir hem de loga yazilir.
    EkranVeGunluk,

    // Hata sadece loga yazilir, kullanici hicbir sey gormez.
    SadeceGunluk
}
