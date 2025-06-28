using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;


namespace AmerikaKamaraFirin
{
    internal class Plc
    {


        //-----------------------------------------------------------------------------------------------------------------------------------------------

        // Bool Değişkenler (Bit seviyesinde)
        public static bool
            r_solKapiKapali = false,      // 0.0
            r_solKapiAcik = true,         // 0.1
            r_solKapiAssada = true,       // 0.2
            r_solKapiYukarda = false,     // 0.3
            r_sagKapiKapali = false,      // 0.4
            r_sagKapiAcik = true,         // 0.5
            r_sagKapiAssada = true,       // 0.6
            r_sagKapiYukarda = false,     // 0.7

            r_error = false,              // 1.0
            r_emergencyPano = false,      // 1.1
            r_inverterError = false,      // 1.2
            r_Tc1Hata = false,            // 1.3
            r_Tc2Hata = false,            // 1.4

            r_firinDurum = false,          // 54.0
            r_veriGeldi = true,           // 60.0
            r_KapiAcMinTempError = false, // 60.1

            r_MbLoadError = false,        // 86.0
            r_MbFrekansError = false,     // 86.1
            r_MbAkim1_1Error = false,     // 86.2
            r_MbAkim1_2Error = false,     // 86.3
            r_MbAkim1_3Error = false,     // 86.4
            r_MbAkim2_1Error = false,     // 86.5
            r_MbAkim2_2Error = false,     // 86.6
            r_MbAkim2_3Error = false,     // 86.7
            r_rezistansError = false,     // 120.0
            r_receteTamam = false,        // 120.1
            r_kapiacildi = false,        // 120.2
            r_Error_1 = false,        // 120.3
            r_siren = false;        // 120.4


        // DInt (4 byte - int türünde)
        public static int
            r_Tc1 = 0,                    // 2
            r_Tc1_2 = 0,                  // 6
            r_Tc2 = 0,                    // 10
            r_Tc2_2 = 0,                  // 14
            r_Group1Current = 0,          // 18
            r_Group2Current = 0,          // 22
            r_damper1 = 0,                // 26
            r_damper2 = 0,                // 30
            r_damper3 = 0,                // 34
            r_damper4 = 0,                // 38
            r_step = 1,                   // 42
            r_butonTime = 0,              // 46
            r_total_elapsed_time = 0,     // 50
            r_butonBasmaTime = 0,         // 56
            r_akim1Ort = 0,               // 112
            r_akim2Ort = 0,               // 116
            r_elapsedTime = 0,
            r_Tc1recete = 0,              // 126
            r_Tc2recete = 0;              // 130

        // Real (4 byte - float türünde)
        public static float
            r_Akim1_1 = 0.0f,             // 62
            r_Akim1_2 = 0.0f,             // 66
            r_Akim1_3 = 0.0f,             // 70
            r_Akim2_1 = 0.0f,             // 74
            r_Akim2_2 = 0.0f,             // 78
            r_Akim2_3 = 0.0f;             // 82

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        // Bool (bit veriler)
        public static bool
            w_soldoorclose = false,       // 0.0
            w_soldooropen = false,        // 0.1
            w_soldoordown = false,        // 0.2
            w_soldoorup = false,          // 0.3
            w_sagdoorclose = false,       // 0.4
            w_sagdooropen = false,        // 0.5
            w_sagdoordown = false,        // 0.6
            w_sagdoorup = false,          // 0.7
            w_Group1Run = false,          // 42.0
            w_Group2Run = false,          // 42.1
            w_G1veriGeldi = false,        // 42.2
            w_G2veriGeldi = false,        // 42.3
            w_inverterReset = false,      // 42.4
            w_receteTamam = false,        // 42.5
            w_frekansYaz = false,         // 50.0
            w_sirenSustur = false;        // 50.1
        // DInt (4 byte - int türü)
        public static int
            w_setTemp1 = 0,               // 2
            w_setTime = 0,                // 6
            w_setTemp2 = 0,               // 10
            w_Group1Current = 0,          // 14
            w_Group2Current = 0,          // 18
            w_damper1 = 0,                // 22
            w_damper2 = 0,                // 26
            w_damper3 = 0,                // 30
            w_damper4 = 0,                // 34
            w_tcfarkhata = 10,            // 38
            w_butonbasmatime = 0,         // 44
            w_minTemp = 0,                // 52
            w_adimSayisi = 0;             // 64

        // Word (2 byte - ushort türü)
        public static ushort
            w_surucuFrekans = 0x10;       // 48

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        public static byte[] readBuffer = new byte[134];
        public static byte[] writeBuffer = new byte[68];
        public static byte[] writereadBuffer = new byte[68];

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        public static bool plcoku = false;
        public static bool plcokundu = false;
        public static bool plcyazokundu = false;
        public static bool plcyaz = false;
        public static bool plcyazildi = false;
        public static int okudeneme = 0;
        public static int yazdeneme = 0;
        public static int deneme = 0;

        //-----------------------------------------------------------------------------------------------------------------------------------------------

        public static void PlcConnect()
        {
            int tryConnect = 0;
            Config.PlcStatu = 1;
            while (Config.PlcStatu != 0 && tryConnect < Globals.ConnectTryCount)
            {
                Config.PlcStatu = Config.Plc.ConnectTo(Config.PlcIP, 0, 1);
                Task.Delay(500).Wait();
                tryConnect++;
            }
            if (Config.PlcStatu != 0) Globals.UpdateStatus($"{AmerikaKamaraFirin.Resources.fırın_PLC_Baglanamadi}: {Globals.PlcError(Config.PlcStatu)}", true, AmerikaKamaraFirin.Resources.program_acilirken_bazi_hatalar_olustu);
            if (Config.PlcStatu == 0) Globals.plcConnected = true;
            tryConnect = 0;
        }





        public static int PlcCycle()
        {
            int result = 0;
            plcoku = true;

            if (plcoku && !plcyaz)
            {
                plcoku = false;
                plcokundu = false;
                plcyazokundu = false;

                result = PlcRead();
                if (result != 0)
                {
                    deneme++;
                    if (IsConnectionError(result))
                        Globals.plcConnected = false;
                    if (deneme >= 5) { deneme = 0; return result; }
                }
                if (result == 0)
                {
                    deneme = 0;
                    plcokundu = true;
                    if (Globals.seciliRecete != null && Globals.seciliRecete.Adimlar.Count > 0)
                    {
                        double toplamSureSn = Globals.seciliRecete.Adimlar.Sum(a => a.SureDakika) * 60;
                        UpdateLiveTempData(toplamSureSn);
                    }
                }

                result = PlcWriteRead();
                if (result != 0)
                {
                    okudeneme++;
                    if (IsConnectionError(result))
                        Globals.plcConnected = false;
                    if (okudeneme >= 5) { okudeneme = 0; return result; }
                }
                if (result == 0)
                {
                    okudeneme = 0;
                    plcyazokundu = true;
                }

                if (plcokundu && plcyazokundu)
                {
                    plcoku = true;
                }
            }

            if (plcyaz)
            {
                int farkf = S7.GetDIntAt(writeBuffer, 38);
                if (farkf > 1)
                {
                    plcoku = false;
                    plcyazildi = false;

                    result = PlcWrite();
                    if (result != 0)
                    {
                        yazdeneme++;
                        if (IsConnectionError(result))
                            Globals.plcConnected = false;
                        if (yazdeneme >= 5) { yazdeneme = 0; return result; }
                    }
                    if (result == 0)
                    {
                        yazdeneme = 0;
                        plcyazildi = true;
                        plcyaz = false;
                        plcoku = true;
                    }
                }
            }

            return result;
        }





        private static bool IsConnectionError(int code)
        {
            return code == 1 || code == 5 || code == 2064 || code == 33072;
        }





        public static int PlcRead()
        {
            int result = Config.Plc.DBRead(2, 0, readBuffer.Length, readBuffer);

            // Bool alanlar
            r_solKapiKapali = S7.GetBitAt(readBuffer, 0, 0);
            r_solKapiAcik = S7.GetBitAt(readBuffer, 0, 1);
            r_solKapiAssada = S7.GetBitAt(readBuffer, 0, 2);
            r_solKapiYukarda = S7.GetBitAt(readBuffer, 0, 3);
            r_sagKapiKapali = S7.GetBitAt(readBuffer, 0, 4);
            r_sagKapiAcik = S7.GetBitAt(readBuffer, 0, 5);
            r_sagKapiAssada = S7.GetBitAt(readBuffer, 0, 6);
            r_sagKapiYukarda = S7.GetBitAt(readBuffer, 0, 7);

            r_firinDurum = S7.GetBitAt(readBuffer, 54, 0);
            r_veriGeldi = S7.GetBitAt(readBuffer, 60, 0);

            r_error = S7.GetBitAt(readBuffer, 1, 0);
            r_emergencyPano = S7.GetBitAt(readBuffer, 1, 1);
            r_inverterError = S7.GetBitAt(readBuffer, 1, 2);
            r_Tc1Hata = S7.GetBitAt(readBuffer, 1, 3);
            r_Tc2Hata = S7.GetBitAt(readBuffer, 1, 4);

            r_KapiAcMinTempError = S7.GetBitAt(readBuffer, 60, 1);

            r_MbLoadError = S7.GetBitAt(readBuffer, 86, 0);
            r_MbFrekansError = S7.GetBitAt(readBuffer, 86, 1);
            r_MbAkim1_1Error = S7.GetBitAt(readBuffer, 86, 2);
            r_MbAkim1_2Error = S7.GetBitAt(readBuffer, 86, 3);
            r_MbAkim1_3Error = S7.GetBitAt(readBuffer, 86, 4);
            r_MbAkim2_1Error = S7.GetBitAt(readBuffer, 86, 5);
            r_MbAkim2_2Error = S7.GetBitAt(readBuffer, 86, 6);
            r_MbAkim2_3Error = S7.GetBitAt(readBuffer, 86, 7);

            r_rezistansError = S7.GetBitAt(readBuffer, 120, 0);
            r_receteTamam = S7.GetBitAt(readBuffer, 120, 1);
            r_kapiacildi = S7.GetBitAt(readBuffer, 120, 2);
            r_Error_1 = S7.GetBitAt(readBuffer, 120, 3);
            r_siren = S7.GetBitAt(readBuffer, 120, 4);



            // DInt (4 byte)
            r_Tc1 = S7.GetDIntAt(readBuffer, 2);
            r_Tc1_2 = S7.GetDIntAt(readBuffer, 6);
            r_Tc2 = S7.GetDIntAt(readBuffer, 10);
            r_Tc2_2 = S7.GetDIntAt(readBuffer, 14);
            r_Group1Current = S7.GetDIntAt(readBuffer, 18);
            r_Group2Current = S7.GetDIntAt(readBuffer, 22);
            r_damper1 = S7.GetDIntAt(readBuffer, 26);
            r_damper2 = S7.GetDIntAt(readBuffer, 30);
            r_damper3 = S7.GetDIntAt(readBuffer, 34);
            r_damper4 = S7.GetDIntAt(readBuffer, 38);
            r_step = S7.GetDIntAt(readBuffer, 42);
            r_butonTime = S7.GetDIntAt(readBuffer, 46);
            r_total_elapsed_time = S7.GetDIntAt(readBuffer, 50);
            r_elapsedTime = S7.GetDIntAt(readBuffer, 122);
            r_butonBasmaTime = S7.GetDIntAt(readBuffer, 56);
            r_akim1Ort = S7.GetDIntAt(readBuffer, 112);
            r_akim2Ort = S7.GetDIntAt(readBuffer, 116);
            r_Tc1recete = S7.GetDIntAt(readBuffer, 126);
            r_Tc2recete = S7.GetDIntAt(readBuffer, 130);

            // Real (4 byte)
            r_Akim1_1 = S7.GetRealAt(readBuffer, 62);
            r_Akim1_2 = S7.GetRealAt(readBuffer, 66);
            r_Akim1_3 = S7.GetRealAt(readBuffer, 70);
            r_Akim2_1 = S7.GetRealAt(readBuffer, 74);
            r_Akim2_2 = S7.GetRealAt(readBuffer, 78);
            r_Akim2_3 = S7.GetRealAt(readBuffer, 82);

            if (result == 0) CheckPlcErrors();
            return result;
        }
        public static int PlcWrite()
        {

            int result = Config.Plc.DBWrite(3, 0, writeBuffer.Length, writeBuffer);

            return result;

        }
        public static int PlcWriteRead()
        {
            int result = Config.Plc.DBRead(3, 0, writereadBuffer.Length, writereadBuffer);

            // Bool değişkenler
            w_soldoorclose = S7.GetBitAt(writereadBuffer, 0, 0);   // 0.0
            w_soldooropen = S7.GetBitAt(writereadBuffer, 0, 1);    // 0.1
            w_soldoordown = S7.GetBitAt(writereadBuffer, 0, 2);    // 0.2
            w_soldoorup = S7.GetBitAt(writereadBuffer, 0, 3);      // 0.3
            w_sagdoorclose = S7.GetBitAt(writereadBuffer, 0, 4);   // 0.4
            w_sagdooropen = S7.GetBitAt(writereadBuffer, 0, 5);    // 0.5
            w_sagdoordown = S7.GetBitAt(writereadBuffer, 0, 6);    // 0.6
            w_sagdoorup = S7.GetBitAt(writereadBuffer, 0, 7);      // 0.7

            w_Group1Run = S7.GetBitAt(writereadBuffer, 42, 0);     // 42.0
            w_Group2Run = S7.GetBitAt(writereadBuffer, 42, 1);     // 42.1
            w_G1veriGeldi = S7.GetBitAt(writereadBuffer, 42, 2);   // 42.2
            w_G2veriGeldi = S7.GetBitAt(writereadBuffer, 42, 3);   // 42.3
            w_inverterReset = S7.GetBitAt(writereadBuffer, 42, 4); // 42.4
            w_receteTamam = S7.GetBitAt(writereadBuffer, 42, 5);   // 42.5
            w_frekansYaz = S7.GetBitAt(writereadBuffer, 50, 0);    // 50.0
            w_sirenSustur = S7.GetBitAt(writereadBuffer, 50, 1);   // 50.1

            // DInt değişkenler (4 byte)
            w_setTemp1 = S7.GetDIntAt(writereadBuffer, 2);         // 2
            w_setTime = S7.GetDIntAt(writereadBuffer, 6);          // 6
            w_setTemp2 = S7.GetDIntAt(writereadBuffer, 10);        // 10
            w_Group1Current = S7.GetDIntAt(writereadBuffer, 14);   // 14
            w_Group2Current = S7.GetDIntAt(writereadBuffer, 18);   // 18
            w_damper1 = S7.GetDIntAt(writereadBuffer, 22);         // 22
            w_damper2 = S7.GetDIntAt(writereadBuffer, 26);         // 26
            w_damper3 = S7.GetDIntAt(writereadBuffer, 30);         // 30
            w_damper4 = S7.GetDIntAt(writereadBuffer, 34);         // 34
            w_tcfarkhata = S7.GetDIntAt(writereadBuffer, 38);      // 38
            w_butonbasmatime = S7.GetDIntAt(writereadBuffer, 44);  // 44
            w_minTemp = S7.GetDIntAt(writereadBuffer, 52);         // 52
            w_adimSayisi = S7.GetDIntAt(writereadBuffer, 64);      // 64

            // Word (2 byte)
            w_surucuFrekans = S7.GetWordAt(writereadBuffer, 48);   // 48

            return result;
        }




        public static void UpdateLiveTempData(double toplamSureSn, int hedefVeriSayisi = 1000)
        {
            double orneklemeAraligi = toplamSureSn / hedefVeriSayisi;
            double now = Plc.r_total_elapsed_time;

            // Reçete sıfırlandıysa (zaman geri gittiyse)
            if (now < Globals.lastRecordedTime)
            {
                Globals.LiveDataList.Clear();
                Globals.lastRecordedTime = 0;
                try { File.Delete(Globals.LiveDataJsonPath); } catch { }
            }

            if (now - Globals.lastRecordedTime >= orneklemeAraligi)
            {
                Globals.LiveDataList.Add(new LiveDataPoint
                {
                    Time = now,
                    Tc1 = Plc.r_Tc1,
                    Tc2 = Plc.r_Tc2
                });

                Globals.lastRecordedTime = now;

                try
                {
                    var json = JsonSerializer.Serialize(Globals.LiveDataList);
                    File.WriteAllText(Globals.LiveDataJsonPath, json);
                }
                catch { }
            }
        }



        public static void CheckPlcErrors()
        {
            if (Plc.r_error)
                Globals.UpdateStatus(Resources.genel_sistem_hatasi, true);

            if (Plc.r_emergencyPano)
                Globals.UpdateStatus(Resources.acil_stop_aktif, true);

            if (Plc.r_rezistansError)
                Globals.UpdateStatus(Resources.rezistansError, true);

            if (Plc.r_inverterError)
                Globals.UpdateStatus(Resources.inverterError, true);

            if (Plc.r_Tc1Hata)
                Globals.UpdateStatus(Resources.tc1_sicaklik_sensoru_hatali, true);

            if (Plc.r_Tc2Hata)
                Globals.UpdateStatus(Resources.tc2_sicaklik_sensoru_hatali, true);

            if (Plc.r_KapiAcMinTempError)
                Globals.UpdateStatus(Resources.kapi_acikken_min_sicaklik_asildi, true);

            if (Plc.r_MbLoadError)
                Globals.UpdateStatus(Resources.modbus_veri_yukleme_hatasi, true);

            if (Plc.r_MbFrekansError)
                Globals.UpdateStatus(Resources.modbus_frekans_okuma_hatasi, true);

            if (Plc.r_MbAkim1_1Error)
                Globals.UpdateStatus(Resources.grup1_faz1_akim_hatasi, true);

            if (Plc.r_MbAkim1_2Error)
                Globals.UpdateStatus(Resources.grup1_faz2_akim_hatasi, true);

            if (Plc.r_MbAkim1_3Error)
                Globals.UpdateStatus(Resources.grup1_faz3_akim_hatasi, true);

            if (Plc.r_MbAkim2_1Error)
                Globals.UpdateStatus(Resources.grup2_faz1_akim_hatasi, true);

            if (Plc.r_MbAkim2_2Error)
                Globals.UpdateStatus(Resources.grup2_faz2_akim_hatasi, true);

            if (Plc.r_MbAkim2_3Error)
                Globals.UpdateStatus(Resources.grup2_faz3_akim_hatasi, true);
        }
    }
}
