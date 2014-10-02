using GHI.Premium.SQLite;
using MFE.Core;
using Microsoft.SPOT;
using System;
using System.Collections;
using System.IO;
using Typhoon.MF.Layouts;
using Typhoon.MF.Layouts.LayoutItems;

namespace Typhoon.Server
{
    public class DBManager
    {
        private string fileName;
        private Database db = new Database();

        public long Size
        {
            get
            {
                long res = 0;
                if (!Utils.StringIsNullOrEmpty(fileName) && File.Exists(fileName))
                    using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        res = fs.Length;
                return res;
            }
        }

        public bool Open(string path)
        {
            fileName = path;

            bool res = false;
            
            try
            {
                bool dbExists = File.Exists(fileName);
                
                // for test!!!
                if (dbExists)
                    File.Delete(fileName);

                db.Open(fileName);
                //if (!dbExists)
                    CreateDB();

                //db.Close();

                res = true;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                Debug.Print(Database.GetLastError());
            }
            return res;
        }
        public void Add(LayoutItem item)
        {
            if (item is Locomotive)
            {
                Locomotive entity = (Locomotive)item;
                db.ExecuteNonQuery(
                      "INSERT INTO Locomotives (ID, Name, Description, Protocol, ConsistID, ConsistForward) " +
                      "VALUES (" +
                      Utils.StringQuotate(Utils.ToBase64String(entity.ID)) + ", " +
                      Utils.StringQuotate(entity.Name) + ", " +
                      Utils.StringQuotate(entity.Description) + ", " +
                      entity.Protocol + ", " +
                      Utils.StringQuotate(Utils.ToBase64String(entity.ConsistID)) + ", " +
                      (entity.ConsistForward ? 1 : 0) + ")");
            }
            else if (item is Consist)
                db.ExecuteNonQuery(
                      "INSERT INTO Consists (ID, Name, Description) " +
                      "VALUES (" + Utils.StringQuotate(Utils.ToBase64String(item.ID)) + ", " + Utils.StringQuotate(item.Name) + ", " + Utils.StringQuotate(item.Description) + ")");



        //    SQLiteDataTable table = db.ExecuteQuery("SELECT * FROM Locomotives");

        //    String[] origin_names = table.ColumnOriginNames;
        //    ArrayList[] tabledata = table.ColumnData;


        //    String temp = "Fields: ";
        //    for (int i = 0; i < table.Columns; i++)
        //        temp += table.ColumnOriginNames[i] + "|";
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
        //            temp += "|";
        //        }
        //        Debug.Print(temp);
        //    }
        }

        public ArrayList GetLocomotives()
        {
            ArrayList res = new ArrayList();
            SQLiteDataTable table = db.ExecuteQuery("SELECT * FROM Locomotives");
            for (int row = 0; row < table.Rows; row++)
            {
                Locomotive loco = new Locomotive(Utils.FromBase64StringToGuid(table.ReadRecord(0, row).ToString()))
                {
                    Name = table.ReadRecord(1, row).ToString(),
                    Description = table.ReadRecord(2, row).ToString(),
                    Protocol = (ProtocolType)int.Parse(table.ReadRecord(3, row).ToString()),
                    ConsistID = Utils.FromBase64StringToGuid(table.ReadRecord(4, row).ToString()),
                    ConsistForward = int.Parse(table.ReadRecord(5, row).ToString()) == 1,
                };
                res.Add(loco);
            }
            return res;
        }
        public ArrayList GetLocomotives(Guid consistId)
        {
            ArrayList res = new ArrayList();
            SQLiteDataTable table = db.ExecuteQuery("SELECT * FROM Locomotives WHERE ConsistID=" + Utils.StringQuotate(Utils.ToBase64String(consistId)));
            for (int row = 0; row < table.Rows; row++)
            {
                Locomotive loco = new Locomotive(Utils.FromBase64StringToGuid(table.ReadRecord(0, row).ToString()))
                {
                    Name = table.ReadRecord(1, row).ToString(),
                    Description = table.ReadRecord(2, row).ToString(),
                    Protocol = (ProtocolType)int.Parse(table.ReadRecord(3, row).ToString()),
                    ConsistID = Utils.FromBase64StringToGuid(table.ReadRecord(4, row).ToString()),
                    ConsistForward = int.Parse(table.ReadRecord(5, row).ToString()) == 1,
                };
                res.Add(loco);
            }
            return res;
        }
        public Locomotive GetLocomotive(Guid id)
        {
            ArrayList res = new ArrayList();
            SQLiteDataTable table = db.ExecuteQuery("SELECT * FROM Locomotives WHERE ID=" + Utils.StringQuotate(Utils.ToBase64String(id)));
            for (int row = 0; row < table.Rows; row++)
            {
                Locomotive loco = new Locomotive(Utils.FromBase64StringToGuid(table.ReadRecord(0, row).ToString()))
                {
                    Name = table.ReadRecord(1, row).ToString(),
                    Description = table.ReadRecord(2, row).ToString(),
                    Protocol = (ProtocolType)int.Parse(table.ReadRecord(3, row).ToString()),
                    ConsistID = Utils.FromBase64StringToGuid(table.ReadRecord(4, row).ToString()),
                    ConsistForward = int.Parse(table.ReadRecord(5, row).ToString()) == 1,
                };
                res.Add(loco);
            }
            return res.Count > 0 ? (Locomotive)res[0] : null;
        }

        public ArrayList GetConsists()
        {
            ArrayList res = new ArrayList();
            SQLiteDataTable table = db.ExecuteQuery("SELECT * FROM Consists");
            for (int row = 0; row < table.Rows; row++)
            {
                Consist consist = new Consist(Utils.FromBase64StringToGuid(table.ReadRecord(0, row).ToString()))
                {
                    Name = table.ReadRecord(1, row).ToString(),
                    Description = table.ReadRecord(2, row).ToString(),
                };
                res.Add(consist);
            }
            return res;
        }
        public Consist GetConsist(Guid id)
        {
            ArrayList res = new ArrayList();
            SQLiteDataTable table = db.ExecuteQuery("SELECT * FROM Consists WHERE ID=" + Utils.StringQuotate(Utils.ToBase64String(id)));
            for (int row = 0; row < table.Rows; row++)
            {
                Consist consist = new Consist(Utils.FromBase64StringToGuid(table.ReadRecord(0, row).ToString()))
                {
                    Name = table.ReadRecord(1, row).ToString(),
                    Description = table.ReadRecord(2, row).ToString(),
                };
                res.Add(consist);
            }
            return res.Count > 0 ? (Consist)res[0] : null;
        }








        
        private void CreateDB()
        {
            db.ExecuteNonQuery("CREATE Table Locomotives(ID TEXT, Name TEXT, Description TEXT, Protocol INTEGER, ConsistID TEXT, ConsistForward INTEGER)");
            db.ExecuteNonQuery("CREATE Table Consists(ID TEXT, Name TEXT, Description TEXT)");
            
            
            
        }
    }
}
