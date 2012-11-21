using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using GT = Gadgeteer;

namespace Typhoon.ServerGT
{
    public partial class Program
    {
        private short n = 0;


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


            GT.Timer timer = new GT.Timer(100);
            timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            timer.Start();




            //WiFiRS9110 wifi = new WiFiRS9110(SPI.SPI_module.SPI1, chipSelect, externalInterrupt, resetPin);


            //InitDB();


        }

        void timer_Tick(GT.Timer timer)
        {
            if (n > 6)
            {
                n = 0;
                //Debug.Print("RAM: " + Debug.GC(false));
            }

            ledArray.Clear();
            ledArray[n] = true;

            n++;
        }

        void InitDB()
        {
            RemovableMedia.Insert += new InsertEventHandler(RemovableMedia_Insert);
            bool res = Mainboard.MountStorageDevice("SD1");

            bool q = res;

            int a = 0;
            int b = a;


        }

        void RemovableMedia_Insert(object sender, MediaEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
