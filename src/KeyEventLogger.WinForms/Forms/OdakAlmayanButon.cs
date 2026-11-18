namespace KeyEventLogger.WinForms.Forms;

// Klavye odagini HIC almayan buton (AliBird ile ayni).
// Neden? Odaktaki bir butona Space/Enter basilirsa buton tiklanmis sayilir. Kayit sirasinda
// Space'e basmak yanlislikla "Durdur" ya da "Temizle"ye basmasin diye butonlar sadece fareyle calisir.
public sealed class OdakAlmayanButon : Button
{
    public OdakAlmayanButon()
    {
        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;
    }
}
