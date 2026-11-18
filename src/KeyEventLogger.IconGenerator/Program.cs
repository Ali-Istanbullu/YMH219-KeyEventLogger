using System.Drawing.Imaging;
using KeyEventLogger.IconGenerator;

// ================================================================================
//  KEY EVENT LOGGER IKON URETICI
//  Kullanim (repo kokunden):  dotnet run --project src/KeyEventLogger.IconGenerator -- assets
//  Uretir: assets/keyeventlogger.png (256x256) ve assets/keyeventlogger.ico (16..256, 7 boyut)
//  Bu dosya aracin kompozisyon kokudur; 2 nesne burada olusturulur (DI kutuphanesi gereksiz).
// ================================================================================
try
{
    string klasor = Path.GetFullPath(args.Length > 0 ? args[0] : "assets");
    Directory.CreateDirectory(klasor);

    int[] boyutlar = [16, 24, 32, 48, 64, 128, 256];
    var cizici = new IkonCizici();

    var resimler = new List<(int Boyut, byte[] Png)>();
    foreach (int boyut in boyutlar)
    {
        using Bitmap resim = cizici.Ciz(boyut);
        using var bellek = new MemoryStream();
        resim.Save(bellek, ImageFormat.Png);
        resimler.Add((boyut, bellek.ToArray()));
    }

    string pngYolu = Path.Combine(klasor, "keyeventlogger.png");
    File.WriteAllBytes(pngYolu, resimler[^1].Png);

    string icoYolu = Path.Combine(klasor, "keyeventlogger.ico");
    new IcoYazici().Yaz(icoYolu, resimler);

    Console.WriteLine($"Yazildi: {pngYolu}");
    Console.WriteLine($"Yazildi: {icoYolu} ({string.Join(", ", boyutlar)})");
    return 0;
}
catch (Exception hata)
{
    // Arac hata verirse nedeni yazilir, cikis kodu 1 olur.
    Console.Error.WriteLine($"Ikon uretilemedi: {hata}");
    return 1;
}
