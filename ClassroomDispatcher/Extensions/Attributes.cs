using System;

namespace ClassroomDispatcher.Extensions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Label : Attribute
    {
        private string _label;

        public Label(string label)
        {
            this._label = label;
        }
    }
}
