using System;
using System.Collections;
using Gadgeteer.Modules.LoveElectronics;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;

namespace Typhoon.ServerGT
{
    public partial class Program
    {
        private short n = 0;
        private long c = 0;

        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            Debug.Print("Program Started");


            //GT.Timer timer = new GT.Timer(300);
            //timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            //timer.Start();


            //GT.Timer moistureTimer = new GT.Timer(500);
            //moistureTimer.Tick += new GT.Timer.TickEventHandler(moistureTimer_Tick);
            //moistureTimer.Start();

            //WiFiRS9110 wifi = new WiFiRS9110(SPI.SPI_module.SPI1, chipSelect, externalInterrupt, resetPin);

            //DbTest();



            //Microsoft.SPOT.Hardware.Cpu.PWMChannel pwm = GT.Socket.GetSocket(3, true, null, null).PWM7;
            //PWM led = new PWM(pwm, 1, 0.5, false); // blink LED with 1 Hz
            //led.Start();

            //motorControllerL298.MoveMotor(MotorControllerL298.Motor.Motor1, 5);
            //motorControllerL298.MoveMotor(MotorControllerL298.Motor.Motor2, 300);


            //SignalGenerator portGenerator = new SignalGenerator((Cpu.Pin)GT.Socket.GetSocket(3, true, null, null).CpuPins[0], false, 200);
            //uint[] buffer = new uint[100];
            //for (int i = 0; i < buffer.Length; i++)
            //    buffer[i] = (i % 2) == 1 ? (uint)1000000 : (uint)100000;
            //portGenerator.SetBlocking(true, buffer, 0, buffer.Length, 0, false);
        }

        private void DbTest()
        {
            try
            {
                // Mount NAND Flash File Media
                //PersistentStorage ps = new PersistentStorage("SD");
                //ps.MountFileSystem();

                //// Format the media if it was not.
                //VolumeInfo nand = new VolumeInfo("SD");
                //if (!nand.IsFormatted)
                //{
                //    nand.Format("FAT", 0);
                //}





            //    // Create new database file
            //    Database myDatabase = new Database();
            //    // Open a new Database in NAND Flash
            //    //myDatabase.Open(root + "\\Content\\Layout.dat");

            //    myDatabase.Open(":memory:");
                
            //    //add a table
            //    myDatabase.ExecuteNonQuery("CREATE Table Temperature(Room TEXT, Time INTEGER, Value DOUBLE)");

            //    //add rows to table
            //    myDatabase.ExecuteNonQuery(
            //      "INSERT INTO Temperature (Room, Time,Value) " +
            //      "VALUES ('Kitchen',010000,4423)");

            //    myDatabase.ExecuteNonQuery(
            //     "INSERT INTO Temperature (Room, Time,Value) " +
            //     "VALUES ('living room',053000,9300)");

            //    myDatabase.ExecuteNonQuery(
            //     "INSERT INTO Temperature (Room, Time,Value) " +
            //     "VALUES ('bed room',060701,7200)");

            //    // Process SQL query and save returned records in SQLiteDataTable
            //    SQLiteDataTable table = myDatabase.ExecuteQuery("SELECT * FROM Temperature");

            //    // Get a copy of columns origin names example
            //    string[] origin_names = table.ColumnOriginNames;

            //    // Get a copy of table data example
            //    ArrayList[] tabledata = table.ColumnData;


            //    string temp = "Fields: ";
            //    for (int i = 0; i < table.Columns; i++)
            //        temp += table.ColumnOriginNames[i] + " |";
            //    Debug.Print(temp);

            //    object obj;
            //    for (int j = 0; j < table.Rows; j++)
            //    {
            //        temp = j.ToString() + " ";
            //        for (int i = 0; i < table.Columns; i++)
            //        {
            //            obj = table.ReadRecord(i, j);
            //            if (obj == null)
            //                temp += "N/A";
            //            else
            //                temp += obj.ToString();
            //            temp += " |";
            //        }
            //        Debug.Print(temp);

            //    }
            //    myDatabase.Close();
            //    //ps.UnmountFileSystem();
            //    //ps.Dispose();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                //Debug.Print(Database.GetLastError());
            }
        }

        bool st = false;
        //33950 ticks
        void timer_Tick(GT.Timer timer)
        {
            relay.SetRelay(Relay.AvailableRelays.Relay1, st);
            relay.SetRelay(Relay.AvailableRelays.Relay2, st);
            relay.SetRelay(Relay.AvailableRelays.Relay3, st);
            relay.SetRelay(Relay.AvailableRelays.Relay4, st);
            st = !st;


            return;
            
            c++;
            Debug.Print("c = " + c);

            if (n > 6)
            {
                n = 0;
                //Debug.Print("RAM: " + Debug.GC(false));
            }

            //ledArray.Clear();
            //ledArray[n] = true;

            n++;
        }
        void moistureTimer_Tick(GT.Timer timer)
        {
            int v = moistureSensor.GetMoistureReading();
            Debug.Print("Moisture: " + v);
            Mainboard.SetDebugLED(v > 500);

            relay[Relay.AvailableRelays.Relay2] = relay[Relay.AvailableRelays.Relay2];
        }


        //void InitDB()
        //{
        //    RemovableMedia.Insert += new InsertEventHandler(RemovableMedia_Insert);
        //    bool res = Mainboard.MountStorageDevice("SD1");

        //    bool q = res;

        //    int a = 0;
        //    int b = a;


        //}
        void RemovableMedia_Insert(object sender, MediaEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
