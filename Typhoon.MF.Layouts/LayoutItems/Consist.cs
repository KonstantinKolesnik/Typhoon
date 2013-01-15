using System;

namespace Typhoon.MF.Layouts.LayoutItems
{
    public class Consist : LayoutItem
    {
        #region Constructors
        public Consist()
            : this(Guid.NewGuid())
        {
        }
        public Consist(Guid id)
            : base(id)
        {
        }
        #endregion
    }
}
