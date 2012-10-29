using System;
using Typhoon.API;
using Typhoon.Core;
using Typhoon.Layouts;
using Typhoon.Net;
using Typhoon.NMRA;

namespace Typhoon.CommandStation
{
    public static class MessageSender
    {
        #region Events
        public static event EventHandler SentAllBrake;
        public static event EventHandler SentAllStop;
        public static event EventHandler SentAllReset;
        #endregion

        #region Public methods
        public static void GetPower()
        {
            Send(new NetworkMessage(NetworkMessageID.Power));
        }
        public static void SetPower(bool on)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Power);
            msg["Power"] = on.ToString();
            Send(msg);
        }

        public static void GetLayout()
        {
            Send(new NetworkMessage(NetworkMessageID.Layout));
        }
        public static void SetLayout(Layout layout)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Layout);
            msg["Layout"] = Helpers.ToBase64String(layout.SaveToXml());
            Send(msg);
        }

        public static void BroadcastBrake()
        {
            Send(new NetworkMessage(NetworkMessageID.BroadcastBrake));

            if (SentAllBrake != null)
                SentAllBrake(null, EventArgs.Empty);
        }
        public static void BroadcastStop()
        {
            Send(new NetworkMessage(NetworkMessageID.BroadcastStop));

            if (SentAllStop != null)
                SentAllStop(null, EventArgs.Empty);
        }
        public static void BroadcastReset()
        {
            Send(new NetworkMessage(NetworkMessageID.BroadcastReset));

            if (SentAllReset != null)
                SentAllReset(null, EventArgs.Empty);
        }

        public static void SetLocoSpeed14(LocomotiveAddress address, int speed, bool forward, bool light)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoSpeed14);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["Speed"] = speed.ToString();
            msg["Forward"] = forward ? bool.TrueString : bool.FalseString;
            msg["Light"] = light ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        public static void SetLocoSpeed28(LocomotiveAddress address, int speed, bool forward)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoSpeed28);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["Speed"] = speed.ToString();
            msg["Forward"] = forward ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        public static void SetLocoSpeed128(LocomotiveAddress address, int speed, bool forward)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoSpeed128);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["Speed"] = speed.ToString();
            msg["Forward"] = forward ? bool.TrueString : bool.FalseString;

            Send(msg);
        }

        public static void SetLocoFunctionGroup1(LocomotiveAddress address, bool F0, bool F1, bool F2, bool F3, bool F4)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup1);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["F0"] = F0 ? bool.TrueString : bool.FalseString;
            msg["F1"] = F1 ? bool.TrueString : bool.FalseString;
            msg["F2"] = F2 ? bool.TrueString : bool.FalseString;
            msg["F3"] = F3 ? bool.TrueString : bool.FalseString;
            msg["F4"] = F4 ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        public static void SetLocoFunctionGroup2(LocomotiveAddress address, bool F5, bool F6, bool F7, bool F8)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup2);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["F5"] = F5 ? bool.TrueString : bool.FalseString;
            msg["F6"] = F6 ? bool.TrueString : bool.FalseString;
            msg["F7"] = F7 ? bool.TrueString : bool.FalseString;
            msg["F8"] = F8 ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        public static void SetLocoFunctionGroup3(LocomotiveAddress address, bool F9, bool F10, bool F11, bool F12)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup3);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["F9"] = F9 ? bool.TrueString : bool.FalseString;
            msg["F10"] = F10 ? bool.TrueString : bool.FalseString;
            msg["F11"] = F11 ? bool.TrueString : bool.FalseString;
            msg["F12"] = F12 ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        public static void SetLocoFunctionGroup4(LocomotiveAddress address, bool F13, bool F14, bool F15, bool F16, bool F17, bool F18, bool F19, bool F20)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup4);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["F13"] = F13 ? bool.TrueString : bool.FalseString;
            msg["F14"] = F14 ? bool.TrueString : bool.FalseString;
            msg["F15"] = F15 ? bool.TrueString : bool.FalseString;
            msg["F16"] = F16 ? bool.TrueString : bool.FalseString;
            msg["F17"] = F17 ? bool.TrueString : bool.FalseString;
            msg["F18"] = F18 ? bool.TrueString : bool.FalseString;
            msg["F19"] = F19 ? bool.TrueString : bool.FalseString;
            msg["F20"] = F20 ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        public static void SetLocoFunctionGroup5(LocomotiveAddress address, bool F21, bool F22, bool F23, bool F24, bool F25, bool F26, bool F27, bool F28)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.LocoFunctionGroup5);
            msg["Address"] = address.Address.ToString();
            msg["Long"] = address.Long ? bool.TrueString : bool.FalseString;
            msg["F21"] = F21 ? bool.TrueString : bool.FalseString;
            msg["F22"] = F22 ? bool.TrueString : bool.FalseString;
            msg["F23"] = F23 ? bool.TrueString : bool.FalseString;
            msg["F24"] = F24 ? bool.TrueString : bool.FalseString;
            msg["F25"] = F25 ? bool.TrueString : bool.FalseString;
            msg["F26"] = F26 ? bool.TrueString : bool.FalseString;
            msg["F27"] = F27 ? bool.TrueString : bool.FalseString;
            msg["F28"] = F28 ? bool.TrueString : bool.FalseString;

            Send(msg);
        }
        #endregion

        #region Private methods
        private static void Send(NetworkMessage msg)
        {
            //App.Model.TCPClient.SendXml(msg);
            App.Model.TCPClient.SendText(msg);
        }
        #endregion

    }
}
