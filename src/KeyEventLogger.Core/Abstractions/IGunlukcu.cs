namespace KeyEventLogger.Core.Abstractions;

// Log yazma sozlesmesi. Core, Serilog'u bilmez (DIP). Uygulamasi: Infrastructure/SerilogGunlukcu.
public interface IGunlukcu
{
    void Bilgi(string mesaj);
    void Uyari(string mesaj);
    void Hata(Exception hata, string mesaj);
}
