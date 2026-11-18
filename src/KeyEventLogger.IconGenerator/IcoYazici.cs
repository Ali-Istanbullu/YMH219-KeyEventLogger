namespace KeyEventLogger.IconGenerator;

// Tek sorumlulugu: PNG resimlerden .ico dosyasi yazmak (S).
// ICO bicimi (Windows Vista+ PNG sikistirmali girdileri destekler):
//   ICONDIR      6 bayt : 0 (ayrilmis), 1 (tur = ikon), girdi sayisi
//   ICONDIRENTRY 16 bayt: genislik, yukseklik (256 icin 0), renk sayisi 0, ayrilmis 0,
//                         renk duzlemi 1, bit/piksel 32, veri boyutu, veri konumu
//   ardindan her girdinin PNG verisi
public sealed class IcoYazici
{
    public void Yaz(string yol, IReadOnlyList<(int Boyut, byte[] Png)> resimler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(yol);
        ArgumentNullException.ThrowIfNull(resimler);
        if (resimler.Count == 0)
        {
            throw new ArgumentException("En az bir resim gerekli.", nameof(resimler));
        }

        using var akis = new FileStream(yol, FileMode.Create, FileAccess.Write);
        using var yazici = new BinaryWriter(akis);

        yazici.Write((ushort)0);
        yazici.Write((ushort)1);
        yazici.Write((ushort)resimler.Count);

        int konum = 6 + 16 * resimler.Count;
        foreach ((int boyut, byte[] png) in resimler)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(boyut, 256);
            byte kenar = boyut == 256 ? (byte)0 : (byte)boyut;
            yazici.Write(kenar);
            yazici.Write(kenar);
            yazici.Write((byte)0);
            yazici.Write((byte)0);
            yazici.Write((ushort)1);
            yazici.Write((ushort)32);
            yazici.Write(png.Length);
            yazici.Write(konum);
            konum += png.Length;
        }

        foreach ((_, byte[] png) in resimler)
        {
            yazici.Write(png);
        }
    }
}
