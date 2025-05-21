using Sharp7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            defaultPassword = "20212021",
            HataBasligi = "",
            HataIcerigi = "";
        public static bool
            IsError = false,
            PlcLaminasyonBaglanti = false,
        //plcden gelecekler ------------------
            MakineAktif = false,
            SuAkisAktif = false,
            OrtalayiciAktif = false,
            SungerGirisIleri = false,
            AstarGirisIleri = false,
            KumasGirisIleri = false,
            LamineliIleri = false,
            SungerGirisGeri = false,
            AstarGirisGeri = false,
            KumasGirisGeri = false,
            LamineliGeri = false,
            SungerDogGeri = false,
            AstarDogGeri = false,
            KumasDogGeri = false,
            SungerDogIleri = false,
            AstarDogIleri = false,
            KumasDogIleri = false,
            KumasAlev = false,
            AstarAlev = false;
        //plcden gelecekler ------------------
        public static int
            ConnectTryCount = 1;

            public static List<User> Users = new();
            public static User LoggedInUser;


        public static void UpdateStatus(string message, bool error = false, string title = "Bir Hatayla Karşılaşıldı!")
        {

            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}"; // Tarih, saat ve mesajın birleştirilmesi
            if (error)
            { 
                string filePath = "status_log.txt"; // Log dosyasının adı ve yolu
                IsError = true;
                HataBasligi = title;
                HataIcerigi = Globals.HataIcerigi + "/" + logMessage;
                // Dosya yoksa oluştur, varsa aç ve mesajı ekle
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(logMessage);
                }
                Task.Delay(3000).Wait();
            }
            else
            {
                IsError = false;
                HataBasligi = title;
                HataIcerigi = Globals.HataIcerigi + "/" + logMessage;
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
                    return "Başarılı, işlem sorunsuz tamamlandı.";
                case S7Consts.errTCPSocketCreation:
                    return "TCP soketi oluşturulamadı.";
                case S7Consts.errTCPConnectionTimeout:
                    return "TCP bağlantısı zaman aşımına uğradı.";
                case S7Consts.errTCPConnectionFailed:
                    return "TCP bağlantısı başarısız oldu.";
                case S7Consts.errTCPReceiveTimeout:
                    return "TCP veri alımı zaman aşımına uğradı.";
                case S7Consts.errTCPDataReceive:
                    return "TCP üzerinden veri alınamadı.";
                case S7Consts.errTCPSendTimeout:
                    return "TCP veri gönderimi zaman aşımına uğradı.";
                case S7Consts.errTCPDataSend:
                    return "TCP üzerinden veri gönderilemedi.";
                case S7Consts.errTCPConnectionReset:
                    return "TCP bağlantısı sıfırlandı.";
                case S7Consts.errTCPNotConnected:
                    return "TCP bağlantısı kurulmamış.";
                case S7Consts.errTCPUnreachableHost:
                    return "TCP ana bilgisayar erişilemez.";
                case S7Consts.errIsoConnect:
                    return "ISO bağlantısı başarısız oldu.";
                case S7Consts.errIsoInvalidPDU:
                    return "Geçersiz ISO PDU (Protokol Veri Birimi) algılandı.";
                case S7Consts.errIsoInvalidDataSize:
                    return "ISO veri boyutu geçersiz.";
                case S7Consts.errCliNegotiatingPDU:
                    return "PDU (Protokol Veri Birimi) müzakeresi başarısız oldu.";
                case S7Consts.errCliInvalidParams:
                    return "Geçersiz parametreler gönderildi.";
                case S7Consts.errCliJobPending:
                    return "Önceki işlem hala beklemede.";
                case S7Consts.errCliTooManyItems:
                    return "Çok fazla öğe gönderildi.";
                case S7Consts.errCliInvalidWordLen:
                    return "Geçersiz veri uzunluğu.";
                case S7Consts.errCliPartialDataWritten:
                    return "Yalnızca kısmi veri yazıldı.";
                case S7Consts.errCliSizeOverPDU:
                    return "Veri boyutu PDU sınırını aşıyor.";
                case S7Consts.errCliInvalidPlcAnswer:
                    return "PLC'den alınan yanıt geçersiz.";
                case S7Consts.errCliAddressOutOfRange:
                    return "Adres aralığı dışına çıkıldı.";
                case S7Consts.errCliInvalidTransportSize:
                    return "Geçersiz taşıma boyutu.";
                case S7Consts.errCliWriteDataSizeMismatch:
                    return "Yazılan veri boyutu uyumsuz.";
                case S7Consts.errCliItemNotAvailable:
                    return "İstenen öğe mevcut değil.";
                case S7Consts.errCliInvalidValue:
                    return "Geçersiz değer.";
                case S7Consts.errCliCannotStartPLC:
                    return "PLC başlatılamıyor.";
                case S7Consts.errCliAlreadyRun:
                    return "PLC zaten çalışıyor.";
                case S7Consts.errCliCannotStopPLC:
                    return "PLC durdurulamıyor.";
                case S7Consts.errCliCannotCopyRamToRom:
                    return "RAM'den ROM'a kopyalama başarısız oldu.";
                case S7Consts.errCliCannotCompress:
                    return "Sıkıştırma işlemi başarısız oldu.";
                case S7Consts.errCliAlreadyStop:
                    return "PLC zaten durdurulmuş.";
                case S7Consts.errCliFunNotAvailable:
                    return "Fonksiyon mevcut değil.";
                case S7Consts.errCliUploadSequenceFailed:
                    return "Yükleme sırası başarısız oldu.";
                case S7Consts.errCliInvalidDataSizeRecvd:
                    return "Alınan veri boyutu geçersiz.";
                case S7Consts.errCliInvalidBlockType:
                    return "Geçersiz blok türü.";
                case S7Consts.errCliInvalidBlockNumber:
                    return "Geçersiz blok numarası.";
                case S7Consts.errCliInvalidBlockSize:
                    return "Geçersiz blok boyutu.";
                case S7Consts.errCliNeedPassword:
                    return "Şifre gerekli.";
                case S7Consts.errCliInvalidPassword:
                    return "Geçersiz şifre.";
                case S7Consts.errCliNoPasswordToSetOrClear:
                    return "Ayarlanacak veya temizlenecek şifre yok.";
                case S7Consts.errCliJobTimeout:
                    return "İşlem zaman aşımına uğradı.";
                case S7Consts.errCliPartialDataRead:
                    return "Yalnızca kısmi veri okundu.";
                case S7Consts.errCliBufferTooSmall:
                    return "Tampon boyutu çok küçük.";
                case S7Consts.errCliFunctionRefused:
                    return "Fonksiyon reddedildi.";
                case S7Consts.errCliDestroying:
                    return "Bağlantı kapatılıyor.";
                case S7Consts.errCliInvalidParamNumber:
                    return "Geçersiz parametre numarası.";
                case S7Consts.errCliCannotChangeParam:
                    return "Parametre değiştirilemiyor.";
                case S7Consts.errCliFunctionNotImplemented:
                    return "Fonksiyon uygulanmamış.";
                default:
                    return $"Bilinmeyen hata kodu: {error}";
            }
        }
    }

}
