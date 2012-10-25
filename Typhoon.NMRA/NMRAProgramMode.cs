using System.ComponentModel;

namespace Typhoon.NMRA
{
    public enum NMRAProgramMode
    {
        [Description("On The Main (POM)")]
        POM,

        [Description("Service, Direct")]
        ServiceDirect,

        [Description("Service, Register")]
        ServiceRegister,

        [Description("Service, Page")]
        ServicePage
    }
}
