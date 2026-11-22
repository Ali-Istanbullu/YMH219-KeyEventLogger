; ================================================================================
;  KeyEventLogger kurulum betigi (Inno Setup)
;
;  Once uygulamayi yayinla (repo kokunden):
;    dotnet publish src/KeyEventLogger.WinForms -c Release -r win-x64 --self-contained false -o publish/KeyEventLogger
;  Sonra bu dosyayi Inno Setup ile derle (ISCC.exe installer\KeyEventLogger.iss).
;  Cikti: installer\Output\KeyEventLogger-Kurulum-1.0.0.exe  (repoya girmez, GitHub Releases'e yuklenir)
;
;  Requirement: .NET 10 Desktop Runtime (x64) pakete GOMULMEZ (kurulum kucuk kalir).
;  Kurulum runtime'i kontrol eder; yoksa Microsoft'tan indirir ve sessizce kurar.
;  Indirme ya da kurulum basarisiz olursa hata gosterilir ve kurulum ilerlemez.
; ================================================================================

#define UygulamaAdi "KeyEventLogger"
#define GorunenAd "Key Event Logger"
#define UygulamaSurumu "1.0.0"
#define Yayinci "Ali İstanbullu"
#define ExeAdi "KeyEventLogger.exe"
#define YayinKlasoru "..\publish\KeyEventLogger"

; Microsoft'un "her zaman son .NET 10 Desktop Runtime x64" kalici baglantisi (aka.ms/dotnet/<kanal>/...)
#define RuntimeUrl "https://aka.ms/dotnet/10.0/windowsdesktop-runtime-win-x64.exe"
#define RuntimeDosyasi "windowsdesktop-runtime-win-x64.exe"

[Setup]
AppId={{2CFF1361-7AE4-4D12-B713-14A8468DAF93}
AppName={#GorunenAd}
AppVersion={#UygulamaSurumu}
AppPublisher={#Yayinci}
; Program Files\KeyEventLogger, yonetici yetkisi gerekir
DefaultDirName={autopf}\{#UygulamaAdi}
DefaultGroupName={#GorunenAd}
PrivilegesRequired=admin
; Sadece Windows x64
; x64 Windows 10/11. Arm64 Windows reddedilir: orada x64 .NET "dotnet\x64" klasorune kurulur, test edilmedi.
ArchitecturesAllowed=x64compatible and not arm64
; .NET 10, Windows 7/8.1 desteklemez: kurulum orada hic baslamaz.
MinVersion=10.0
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=Output
OutputBaseFilename={#UygulamaAdi}-Kurulum-{#UygulamaSurumu}
SetupIconFile=..\assets\keyeventlogger.ico
UninstallDisplayIcon={app}\{#ExeAdi}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Kaldirma %LocalAppData%\KeyEventLogger (loglar) klasorune DOKUNMAZ (AliBird ile ayni kural).
; Kullanicinin kaydettigi .txt dosyalari kendi sectigi yerdedir, kurulum onlari bilmez.

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
; Kisayollar istege bagli
Name: "masaustu"; Description: "Masaüstüne kısayol oluştur"; GroupDescription: "Kısayollar:"
Name: "baslatmenusu"; Description: "Başlat menüsüne kısayol oluştur"; GroupDescription: "Kısayollar:"

[Files]
; .pdb (hata ayiklama sembolleri) kullaniciya gerekmez, kurulum kucuk kalsin
Source: "{#YayinKlasoru}\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autodesktop}\{#GorunenAd}"; Filename: "{app}\{#ExeAdi}"; Tasks: masaustu
Name: "{group}\{#GorunenAd}"; Filename: "{app}\{#ExeAdi}"; Tasks: baslatmenusu
Name: "{group}\{#GorunenAd} Kaldır"; Filename: "{uninstallexe}"; Tasks: baslatmenusu

[UninstallDelete]
; Kaldirmada program klasoru TAMAMEN silinir (sonradan olusmus dosyalar dahil).
; %LocalAppData%\KeyEventLogger (loglar) bilerek SILINMEZ.
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{app}\{#ExeAdi}"; Description: "Key Event Logger'ı başlat"; Flags: nowait postinstall skipifsilent

[Code]
var
  IndirmeSayfasi: TDownloadWizardPage;   // indirilen dosya {tmp}'ye gelir, kurulum bitince silinir

// .NET 10 Desktop Runtime (x64) kurulu mu? Microsoft'un kurdugu klasorde 10.x surumu aranir.
function RuntimeKuruluMu: Boolean;
var
  Bulunan: TFindRec;
begin
  Result := FindFirst(ExpandConstant('{commonpf64}\dotnet\shared\Microsoft.WindowsDesktop.App\10.*'), Bulunan);
  if Result then
    FindClose(Bulunan);
end;

procedure InitializeWizard;
begin
  IndirmeSayfasi := CreateDownloadPage('.NET 10 Desktop Runtime indiriliyor',
    'Key Event Logger için gerekli .NET 10 Desktop Runtime indiriliyor, lütfen bekleyin.', nil);
end;

// Runtime'i sessizce kurar. Basarisizsa False ve hata mesaji.
function RuntimeKur(var Hata: String): Boolean;
var
  Sayfa: TOutputMarqueeProgressWizardPage;
  CikisKodu: Integer;
begin
  Result := False;
  Sayfa := CreateOutputMarqueeProgressPage('.NET 10 Desktop Runtime kuruluyor',
    'Bu birkaç dakika sürebilir, lütfen bekleyin.');
  Sayfa.Show;
  try
    Sayfa.Animate;
    if not Exec(ExpandConstant('{tmp}\{#RuntimeDosyasi}'), '/install /quiet /norestart', '',
                SW_HIDE, ewWaitUntilTerminated, CikisKodu) then
    begin
      Hata := 'Runtime kurulum programı çalıştırılamadı: ' + SysErrorMessage(CikisKodu);
      Exit;
    end;
    // 0 = basarili, 3010/1641 = basarili ama yeniden baslatma gerekli
    if (CikisKodu <> 0) and (CikisKodu <> 3010) and (CikisKodu <> 1641) then
    begin
      Hata := 'Runtime kurulumu başarısız oldu (çıkış kodu ' + IntToStr(CikisKodu) + ').';
      Exit;
    end;
    if not RuntimeKuruluMu then
    begin
      Hata := 'Runtime kuruldu görünüyor ama bulunamadı.';
      Exit;
    end;
    Result := True;
  finally
    Sayfa.Hide;
  end;
end;

// "Kur" dugmesine basilinca: runtime yoksa indir + kur. Hata olursa kurulum ilerlemez.
function NextButtonClick(CurPageID: Integer): Boolean;
var
  Hata: String;
begin
  Result := True;
  if (CurPageID <> wpReady) or RuntimeKuruluMu then
    Exit;

  IndirmeSayfasi.Clear;
  IndirmeSayfasi.Add('{#RuntimeUrl}', '{#RuntimeDosyasi}', '');
  IndirmeSayfasi.Show;
  try
    try
      IndirmeSayfasi.Download;
    except
      if not IndirmeSayfasi.AbortedByUser then
        SuppressibleMsgBox('.NET 10 Desktop Runtime indirilemedi. İnternet bağlantınızı kontrol edip tekrar deneyin.' + #13#10#13#10 +
          AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
      Result := False;
      Exit;
    end;
  finally
    IndirmeSayfasi.Hide;
  end;

  if not RuntimeKur(Hata) then
  begin
    SuppressibleMsgBox(Hata + #13#10#13#10 + 'Key Event Logger kurulamadı.', mbCriticalError, MB_OK, IDOK);
    Result := False;
  end;
end;
