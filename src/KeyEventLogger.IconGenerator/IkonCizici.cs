using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace KeyEventLogger.IconGenerator;

// Tek sorumlulugu: verilen boyutta ikon resmini cizmek (S). CK23.
// Gorunus: koyu lacivert yuvarlak kare + ortada acik renkli klavye tusu ("K" harfi)
//          + sag ustte kirmizi "kayit" noktasi.
public sealed class IkonCizici
{
    private static readonly Color Arkaplan = Color.FromArgb(30, 41, 59);
    private static readonly Color TusYuzu = Color.FromArgb(241, 245, 249);
    private static readonly Color TusGolgesi = Color.FromArgb(148, 163, 184);
    private static readonly Color Harf = Color.FromArgb(30, 41, 59);
    private static readonly Color KayitNoktasi = Color.FromArgb(239, 68, 68);

    // boyut x boyut piksellik, saydam arka planli resim. Cagiran Dispose eder.
    public Bitmap Ciz(int boyut)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(boyut, 16);

        var resim = new Bitmap(boyut, boyut, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(resim);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.Clear(Color.Transparent);

        float s = boyut;

        // 1) Arka plan
        using (GraphicsPath kare = YuvarlakDikdortgen(new RectangleF(0, 0, s - 1, s - 1), s * 0.22f))
        using (var firca = new SolidBrush(Arkaplan))
        {
            g.FillPath(firca, kare);
        }

        // 2) Klavye tusu: once golgesi (alttan kalin kenar), sonra yuzu
        var tus = new RectangleF(s * 0.18f, s * 0.20f, s * 0.60f, s * 0.56f);
        using (GraphicsPath golge = YuvarlakDikdortgen(tus with { Y = tus.Y + s * 0.06f }, s * 0.10f))
        using (var firca = new SolidBrush(TusGolgesi))
        {
            g.FillPath(firca, golge);
        }
        using (GraphicsPath yuz = YuvarlakDikdortgen(tus, s * 0.10f))
        using (var firca = new SolidBrush(TusYuzu))
        {
            g.FillPath(firca, yuz);
        }

        // 3) "K" harfi (16 px'te okunmaz, sadece 24 px ve ustunde)
        if (boyut >= 24)
        {
            using var yazi = new Font("Segoe UI", s * 0.34f, FontStyle.Bold, GraphicsUnit.Pixel);
            using var firca = new SolidBrush(Harf);
            using var ortala = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("K", yazi, firca, tus, ortala);
        }

        // 4) Kayit noktasi (sag ust)
        float cap = s * 0.26f;
        var nokta = new RectangleF(s - cap - s * 0.06f, s * 0.06f, cap, cap);
        using (var firca = new SolidBrush(KayitNoktasi))
        {
            g.FillEllipse(firca, nokta);
        }
        using (var kalem = new Pen(Arkaplan, Math.Max(1f, s * 0.035f)))
        {
            g.DrawEllipse(kalem, nokta);
        }

        return resim;
    }

    private static GraphicsPath YuvarlakDikdortgen(RectangleF r, float yaricap)
    {
        float cap = yaricap * 2f;
        var yol = new GraphicsPath();
        yol.AddArc(r.X, r.Y, cap, cap, 180f, 90f);
        yol.AddArc(r.Right - cap, r.Y, cap, cap, 270f, 90f);
        yol.AddArc(r.Right - cap, r.Bottom - cap, cap, cap, 0f, 90f);
        yol.AddArc(r.X, r.Bottom - cap, cap, cap, 90f, 90f);
        yol.CloseFigure();
        return yol;
    }
}
