
namespace MFE.Net
{
    class WebSocketOpCode
    {
        public const sbyte Continuation = 0;
        //public const string ContinuationTag = "0";

        public const sbyte Text = 1;
        //public const string TextTag = "1";

        public const sbyte Binary = 2;
        //public const string BinaryTag = "2";

        public const sbyte Close = 8;
        //public const string CloseTag = "8";

        public const sbyte Ping = 9;
        //public const string PingTag = "9";

        public const sbyte Pong = 10;
        //public const string PongTag = "10";
    }
}
