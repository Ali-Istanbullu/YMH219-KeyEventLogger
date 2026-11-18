namespace KeyEventLogger.Core.Abstractions;

// Dosya yazma hatasinin SEBEBINI kullanicinin anlayacagi bir cumleye cevirir.
public interface IDosyaHatasiAciklayici
{
    string Acikla(Exception hata);
}
