using Sharp7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmerikaKamaraFirin.Resources;

namespace AmerikaKamaraFirin
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } 
        public string Role { get; set; }
    }

    public class Globals
    {
        public static string
            defaultUser = "Servis",
            defaultPassword = "8889",
            HataBasligi = "",
            HataIcerigi = "";
        public static bool
            IsError = false,
            plcConnected = false;


        public static int
            ConnectTryCount = 3;

            public static List<User> Users = new();
            public static User LoggedInUser;


        private static readonly object _lock = new object();



        public static List<LiveDataPoint> LiveDataList = new List<LiveDataPoint>();
        public static double lastRecordedTime = 0;

        public static string LiveDataJsonPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiveTempData.json");

        public static Recete seciliRecete = null; // varsa burada tutabilirsin



        public static void UpdateStatus(string message, bool error = false, string title = "Bir Hatayla Karşılaşıldı!")
        {
            title = AmerikaKamaraFirin.Resources.bir_hatayla_karsilasildi;

            if (error)
            {
                // Aynı hata daha önce HataIcerigi içinde varsa tekrar ekleme
                if (!Globals.HataIcerigi.Contains(message))
                {
                    Globals.IsError = true;
                    Globals.HataBasligi = title;
                    Globals.HataIcerigi += "/" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

                    // Log dosyasına da yaz
                    lock (_lock)
                    {
                        File.AppendAllText("status_log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\r\n");
                    }
                }
            }
            else
            {
                Globals.IsError = false;
                Globals.HataBasligi = title;
                Globals.HataIcerigi += "/" + $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            }
        }
        public static string RemoveTurkishAndSpecialChars(string input)
        {
            string[] turkish = { "İ", "I", "Ş", "Ğ", "Ü", "Ö", "Ç", "ı", "ş", "ğ", "ü", "ö", "ç" };
            string[] english = { "I", "I", "S", "G", "U", "O", "C", "i", "s", "g", "u", "o", "c" };

            for (int i = 0; i < turkish.Length; i++)
            {
                input = input.Replace(turkish[i], english[i]);
            }

            // Boşlukları _ yap
            input = input.Replace(" ", "_");

            // Geçerli karakterleri al (harf, rakam, alt çizgi)
            input = new string(input.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());

            return input;
        }
        public static string PlcError(int error)
        {
            switch (error)
            {
                case S7Consts.ResultOK:
                    return basarili_islem_sorunsuz_tamamlandi;
                case S7Consts.errTCPSocketCreation:
                    return tcp_soketi_olusturulamadi;
                case S7Consts.errTCPConnectionTimeout:
                    return tcp_baglantisi_zaman_asimina_ugradi;
                case S7Consts.errTCPConnectionFailed:
                    return tcp_baglantisi_basarisiz_oldu;
                case S7Consts.errTCPReceiveTimeout:
                    return tcp_veri_alimi_zaman_asimina_ugradi;
                case S7Consts.errTCPDataReceive:
                    return tcp_uzerinden_veri_alinamadi;
                case S7Consts.errTCPSendTimeout:
                    return tcp_veri_gonderimi_zaman_asimina_ugradi;
                case S7Consts.errTCPDataSend:
                    return tcp_uzerinden_veri_gonderilemedi;
                case S7Consts.errTCPConnectionReset:
                    return tcp_baglantisi_sifirlandi;
                case S7Consts.errTCPNotConnected:
                    return tcp_baglantisi_kurulmamis;
                case S7Consts.errTCPUnreachableHost:
                    return tcp_ana_bilgisayar_erisilemez;
                case S7Consts.errIsoConnect:
                    return iso_baglantisi_basarisiz_oldu;
                case S7Consts.errIsoInvalidPDU:
                    return gecersiz_iso_pdu_protokol_veri_birimi_algilandi;
                case S7Consts.errIsoInvalidDataSize:
                    return iso_veri_boyutu_gecersiz;
                case S7Consts.errCliNegotiatingPDU:
                    return pdu_protokol_veri_birimi_muzakeresi_basarisiz_oldu;
                case S7Consts.errCliInvalidParams:
                    return gecersiz_parametreler_gonderildi;
                case S7Consts.errCliJobPending:
                    return onceki_islem_hala_beklemede;
                case S7Consts.errCliTooManyItems:
                    return cok_fazla_oge_gonderildi;
                case S7Consts.errCliInvalidWordLen:
                    return gecersiz_veri_uzunlugu;
                case S7Consts.errCliPartialDataWritten:
                    return yalnizca_kismi_veri_yazildi;
                case S7Consts.errCliSizeOverPDU:
                    return veri_boyutu_pdu_sinirini_asiyor;
                case S7Consts.errCliInvalidPlcAnswer:
                    return plcden_alinan_yanit_gecersiz;
                case S7Consts.errCliAddressOutOfRange:
                    return adres_araligi_disina_cikildi;
                case S7Consts.errCliInvalidTransportSize:
                    return gecersiz_tasima_boyutu;
                case S7Consts.errCliWriteDataSizeMismatch:
                    return yazilan_veri_boyutu_uyumsuz;
                case S7Consts.errCliItemNotAvailable:
                    return istenen_oge_mevcut_degil;
                case S7Consts.errCliInvalidValue:
                    return gecersiz_deger;
                case S7Consts.errCliCannotStartPLC:
                    return plc_baslatilamiyor;
                case S7Consts.errCliAlreadyRun:
                    return plc_zaten_calisiyor;
                case S7Consts.errCliCannotStopPLC:
                    return plc_durdurulamiyor;
                case S7Consts.errCliCannotCopyRamToRom:
                    return ramden_roma_kopyalama_basarisiz_oldu;
                case S7Consts.errCliCannotCompress:
                    return sikistirma_islemi_basarisiz_oldu;
                case S7Consts.errCliAlreadyStop:
                    return plc_zaten_durdurulmus;
                case S7Consts.errCliFunNotAvailable:
                    return fonksiyon_mevcut_degil;
                case S7Consts.errCliUploadSequenceFailed:
                    return yukleme_sirasi_basarisiz_oldu;
                case S7Consts.errCliInvalidDataSizeRecvd:
                    return alinan_veri_boyutu_gecersiz;
                case S7Consts.errCliInvalidBlockType:
                    return gecersiz_blok_turu;
                case S7Consts.errCliInvalidBlockNumber:
                    return gecersiz_blok_numarasi;
                case S7Consts.errCliInvalidBlockSize:
                    return gecersiz_blok_boyutu;
                case S7Consts.errCliNeedPassword:
                    return sifre_gerekli;
                case S7Consts.errCliInvalidPassword:
                    return gecersiz_sifre;
                case S7Consts.errCliNoPasswordToSetOrClear:
                    return ayarlanacak_veya_temizlenecek_sifre_yok;
                case S7Consts.errCliJobTimeout:
                    return islem_zaman_asimina_ugradi;
                case S7Consts.errCliPartialDataRead:
                    return yalnizca_kismi_veri_okundu;
                case S7Consts.errCliBufferTooSmall:
                    return tampon_boyutu_cok_kucuk;
                case S7Consts.errCliFunctionRefused:
                    return fonksiyon_reddedildi;
                case S7Consts.errCliDestroying:
                    return baglanti_kapatiliyor;
                case S7Consts.errCliInvalidParamNumber:
                    return gecersiz_parametre_numarasi;
                case S7Consts.errCliCannotChangeParam:
                    return parametre_degistirilemiyor;
                case S7Consts.errCliFunctionNotImplemented:
                    return fonksiyon_uygulanmamis;
                default:
                    return $"{bir_hatayla_karsilasildi}: {error}";
            }
        }
    }

}
