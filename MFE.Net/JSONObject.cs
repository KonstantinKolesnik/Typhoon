using System;

namespace MFE.Net
{
    public class JSONObject
    {
        public JSONObjectType ObjectType
        {
            get; set;
        }
        public Object Object
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        
        public JSONObject(Object obj, JSONObjectType type, string name = null)
        {
            ObjectType = type;
            Object = obj;
            if (name != null)
                Name = name;
        }
    }
}
