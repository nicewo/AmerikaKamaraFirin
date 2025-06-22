using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmerikaKamaraFirin
{
    internal class Plc
    {
        public static bool plcoku = false;

        public static bool
            r_solKapiAssada = true,     // 0.2
            r_solKapiYukarda = false,   // 0.3
            r_solKapiAcik = true,       // 0.1
            r_solKapiKapali = false,    // 0.0
            r_sagKapiAssada = true,     // 0.2
            r_sagKapiYukarda = false,   // 0.7
            r_sagKapiAcik = true,       // 0.5
            r_sagKapiKapali = false,    // 0.4
            r_error = false,            // 1.0
            r_emergencyPano = false,    // 1.1
            r_firinDurum = true,        // 54.0
            r_veriGeldi = true,        // 60.0
            r_KapiMinTempError = false;    // 60.1, 

        public static int
            r_Tc1 = 0,                  // 2
            r_Tc1_2 = 0,                // 6
            r_Tc2 = 0,                  // 10
            r_Tc2_2 = 0,                // 14
            r_Group1Current = 0,        // 18
            r_Group2Current = 0,        // 22
            r_damper1 = 0,              // 26
            r_damper2 = 0,              // 30
            r_damper3 = 0,              // 34
            r_damper4 = 0,              // 38
            r_step = 1,                 // 42
            r_butonTime = 0,            // 46
            r_total_elapsed_time = 0,   // 50
            r_butonBasmaTime = 0;      // 56


        public static byte[] readBuffer = new byte[62];
        public static byte[] writeBuffer = new byte[52];
        public static byte[] writereadBuffer = new byte[52];
        public static bool plcisWrite = false;


        //plcden gelecekler ------------------

        public static bool
            w_soldoorclose = false, // 0.0
            w_soldooropen = false,  // 0.1
            w_soldoordown = false,  // 0.2
            w_soldoorup = false,    // 0.3
            w_sagdoorclose = false, // 0.4
            w_sagdooropen = false,  // 0.5
            w_sagdoordown = false,  // 0.6
            w_sagdoorup = false,    // 0.7
            w_G1veriGeldi = false,  // 42.2
            w_G2veriGeldi = false;  // 42.3
        public static int
            w_setTemp1 = 0,        // 2
            w_setTime = 0,         // 6
            w_setTemp2 = 0,        // 10
            w_damper1 = 0,         // 22
            w_damper2 = 0,         // 26
            w_damper3 = 0,         // 30
            w_damper4 = 0,         // 34
            w_tcfarkhata = 0,      // 38
            w_butonbasmatime = 0;  // 44


        public static int PlcRead()
        {
            int result = 0;
            if (!plcoku)
            {
                result = Config.Plc.DBRead(2, 0, readBuffer.Length, readBuffer);

                r_solKapiKapali = S7.GetBitAt(readBuffer, 0, 0);
                r_solKapiAcik = S7.GetBitAt(readBuffer, 0, 1);
                r_solKapiAssada = S7.GetBitAt(readBuffer, 0, 2);
                r_solKapiYukarda = S7.GetBitAt(readBuffer, 0, 3);
                r_sagKapiKapali = S7.GetBitAt(readBuffer, 0, 4);
                r_sagKapiAcik = S7.GetBitAt(readBuffer, 0, 5);
                r_sagKapiAssada = S7.GetBitAt(readBuffer, 0, 6);
                r_sagKapiYukarda = S7.GetBitAt(readBuffer, 0, 7);
                r_firinDurum = S7.GetBitAt(readBuffer, 54, 0);
                r_error = S7.GetBitAt(readBuffer, 1, 0);
                r_emergencyPano = S7.GetBitAt(readBuffer, 1, 1);
                r_veriGeldi = S7.GetBitAt(readBuffer, 60, 0);
                r_KapiMinTempError = S7.GetBitAt(readBuffer, 60, 1);

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
                r_butonBasmaTime = S7.GetDIntAt(readBuffer, 56);

                if (result != 0) return result;

                result = Config.Plc.DBRead(3, 0, writereadBuffer.Length, writereadBuffer);

                w_soldoorclose = S7.GetBitAt(writereadBuffer, 0, 0);
                w_soldooropen = S7.GetBitAt(writereadBuffer, 0, 1);
                w_soldoordown = S7.GetBitAt(writereadBuffer, 0, 2);
                w_soldoorup = S7.GetBitAt(writereadBuffer, 0, 3);
                w_sagdoorclose = S7.GetBitAt(writereadBuffer, 0, 4);
                w_sagdooropen = S7.GetBitAt(writereadBuffer, 0, 5);
                w_sagdoordown = S7.GetBitAt(writereadBuffer, 0, 6);
                w_sagdoorup = S7.GetBitAt(writereadBuffer, 0, 7);




            }
            return result;

        }


        public static int PlcWrite()
        {


            int result = Config.Plc.DBWrite(3, 0, writereadBuffer.Length, writereadBuffer);


            return result;

        }

        public static int PlcWriteRead()
        {


            int result = Config.Plc.DBRead(3, 0, writeBuffer.Length, writeBuffer);

            w_soldoorclose = S7.GetBitAt(writeBuffer, 0, 0);   // 0.0
            w_soldooropen = S7.GetBitAt(writeBuffer, 0, 1);    // 0.1
            w_soldoordown = S7.GetBitAt(writeBuffer, 0, 2);    // 0.2
            w_soldoorup = S7.GetBitAt(writeBuffer, 0, 3);      // 0.3
            w_sagdoorclose = S7.GetBitAt(writeBuffer, 0, 4);   // 0.4
            w_sagdooropen = S7.GetBitAt(writeBuffer, 0, 5);    // 0.5
            w_sagdoordown = S7.GetBitAt(writeBuffer, 0, 6);    // 0.6
            w_sagdoorup = S7.GetBitAt(writeBuffer, 0, 7);      // 0.7
            w_G1veriGeldi = S7.GetBitAt(writeBuffer, 42, 2);   // 42.2
            w_G2veriGeldi = S7.GetBitAt(writeBuffer, 42, 3);   // 42.3

            w_setTemp1 = S7.GetDIntAt(writeBuffer, 2);         // 2
            w_setTime = S7.GetDIntAt(writeBuffer, 6);          // 6
            w_setTemp2 = S7.GetDIntAt(writeBuffer, 10);        // 10
            w_damper1 = S7.GetDIntAt(writeBuffer, 22);         // 22
            w_damper2 = S7.GetDIntAt(writeBuffer, 26);         // 26
            w_damper3 = S7.GetDIntAt(writeBuffer, 30);         // 30
            w_damper4 = S7.GetDIntAt(writeBuffer, 34);         // 34
            w_tcfarkhata = S7.GetDIntAt(writeBuffer, 38);      // 38
            w_butonbasmatime = S7.GetDIntAt(writeBuffer, 44);  // 44



            writereadBuffer = writeBuffer;


            return result;

        }

    }
}
