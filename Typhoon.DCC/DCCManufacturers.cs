using System.Collections;

namespace Typhoon.DCC
{
    public static class DCCManufacturers
    {
        #region Fields
        private static Hashtable ht = new Hashtable(256);
        #endregion

        #region Constructor
        static DCCManufacturers()
        {
            ht[0] = "NMRA";
            ht[1] = "CML Electronics Limited";
            ht[2] = "Train Technology";
            ht[11] = "NCE Corporation (formerly North Coast Engineering)";
            ht[12] = "Wangrow Electronics";
            ht[13] = "Public Domain & Do-It-Yourself Decoders";
            ht[14] = "PSI –Dynatrol";
            ht[15] = "Ramfixx Technologies (Wangrow)";
            ht[17] = "Advance IC Engineering";
            ht[18] = "JMRI";
            ht[19] = "AMW";
            ht[20] = "T4T – Technology for Trains GmbH";
            ht[21] = "Kreischer Datentechnik";
            ht[22] = "KAM Industries";
            ht[23] = "S Helper Service";
            ht[24] = "MoBaTron.de";
            ht[25] = "Team Digital, LLC";
            ht[26] = "MBTronik – PiN GITmBH";
            ht[27] = "MTH Electric Trains, Inc.";
            ht[28] = "Heljan A/S";
            ht[29] = "Mistral Train Models";
            ht[30] = "Digsight";
            ht[31] = "Brelec";
            ht[32] = "Regal Way Co. Ltd";
            ht[33] = "Praecipuus";
            ht[34] = "Aristo-Craft Trains";
            ht[35] = "Electronik & Model Produktion";
            ht[36] = "DCCconcepts";
            ht[37] = "NAC Services, Inc";
            ht[38] = "Broadway Limited Imports, LLC";
            ht[39] = "Educational Computer, Inc.";
            ht[40] = "KATO Precision Models";
            ht[41] = "Passmann";
            ht[42] = "Digirails";
            ht[43] = "Ngineering";
            ht[44] = "SPROG-DCC";
            ht[45] = "ANE Model Co, Ltd";
            ht[46] = "GFB Designs";
            ht[47] = "Capecom";
            ht[48] = "Hornby Hobbies Ltd";
            ht[49] = "Joka Electronic";
            ht[50] = "N&Q Electronics";
            ht[51] = "DCC Supplies, Ltd";
            ht[52] = "Krois-Modell";
            ht[53] = "Rautenhaus Digital Vertrieb";
            ht[54] = "TCH Technology";
            ht[55] = "QElectronics GmbH";
            ht[56] = "LDH";
            ht[57] = "Rampino Elektronik";
            ht[58] = "KRES GmbH";
            ht[59] = "Tam Valley Depot";
            ht[60] = "Bluecher-Electronic";
            ht[61] = "TrainModules";
            ht[62] = "Tams Elektronik GmbH";
            ht[63] = "Noarail";
            ht[64] = "Digital Bahn";
            ht[66] = "Railnet Solutions, LLC";
            ht[68] = "MAWE Elektronik";
            ht[71] = "New Youk Byano Limited";
            ht[73] = "The Electric Railroad Company";
            ht[85] = "Uhlenbrock GmbH";
            ht[87] = "RR-Cirkits";
            ht[95] = "Sanda Kan Industrial, Ltd.";
            ht[97] = "Doehler & Haas";
            ht[99] = "Lenz Elektronik GmbH";
            ht[101] = "Bachmann Trains";
            ht[103] = "Nagasue System Design Office";
            ht[105] = "Computer Dialysis France";
            ht[109] = "Viessmann Modellspielwaren GmbH";
            ht[111] = "Haber & Koenig Electronics GmbH (HKE)";
            ht[113] = "QS Industries (QSI)";
            ht[115] = "Dietz Modellbahntechnik";
            ht[117] = "cT Elektronik";
            ht[119] = "W. S. Ataras Engineering";
            ht[123] = "Massoth Elektronik, GmbH";
            ht[125] = "ProfiLok Modellbahntechnik GmbH";
            ht[127] = "Atlas Model Railroad Products";
            ht[129] = "Digitrax";
            ht[131] = "Trix Modelleisenbahn";
            ht[132] = "ZTC";
            ht[133] = "Intelligent Command Control";
            ht[135] = "CVP Products";
            ht[139] = "RealRail Effects";
            ht[141] = "Throttle-Up (Soundtraxx)";
            ht[143] = "Model Rectifier Corp.";
            ht[145] = "Zimo Elektronik";
            ht[147] = "Umelec Ing. Buero";
            ht[149] = "Rock Junction Controls";
            ht[151] = "Electronic Solutions Ulm GmbH";
            ht[153] = "Train Control Systems";
            ht[155] = "Gebr. Fleischmann GmbH & Co.";
            ht[157] = "Kuehn Ing.";
            ht[159] = "LGB (Ernst Paul Lehmann Patentwerk)";
            ht[161] = "Modelleisenbahn GmbH (formerly Roco)";
            ht[163] = "WP Railshops";
            ht[165] = "Model Electronic Railway Group";
            ht[170] = "AuroTrains";
            ht[173] = "Arnold – Rivarossi";
            ht[186] = "BRAWA Modellspielwaren GmbH & Co.";
            ht[204] = "Con-Com GmbH";
            ht[225] = "Elproma Electronics Poland";
            ht[238] = "NMRA Reserved";
        }
        #endregion

        #region Public methods
        public static string Get(byte id)
        {
            return (string)ht[id];
        }
        #endregion
    }
}
