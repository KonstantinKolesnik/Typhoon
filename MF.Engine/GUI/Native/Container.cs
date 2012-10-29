using System;
using System.Collections;

namespace MF.Engine.GUI
{
    [Serializable]
    public abstract class Container : Control
    {
        #region Fields
        private ArrayList children = new ArrayList();
        #endregion

        #region Properties
        public ArrayList Children
        {
            get { return children; }
        }
        #endregion

        #region Public Methods
        public virtual void AddChild(Control child)
        {
            child.Parent = this;
            children.Add(child);
        }
        public virtual void ClearChildren()
        {
            children.Clear();
        }
        public virtual Control GetChildAt(int index)
        {
            return (Control)children[index];
        }
        public virtual void RemoveChild(Control child)
        {
            children.Remove(child);
        }
        public virtual void RemoveChildAt(int index)
        {
            children.RemoveAt(index);
        }
        #endregion
    }
}
