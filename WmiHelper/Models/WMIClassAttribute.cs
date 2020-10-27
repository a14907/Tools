using System;

namespace WmiHelper.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WMIClassAttribute : Attribute
    {
        public string Name
        {
            get;
        }

        public WMIClassAttribute(string name = "")
        {
            Name = name;
        }
    }

}
