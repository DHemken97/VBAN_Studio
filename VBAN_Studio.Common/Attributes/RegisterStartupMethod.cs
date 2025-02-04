using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBAN_Studio.Common.Attributes
{
    public class RegisterStartupMethod : System.Attribute
    {
        public string MethodName { get; }

        public RegisterStartupMethod(string methodName)
        {
            MethodName = methodName;
        }

        public void Execute(Type classType)
        {
            var method = classType.GetMethod(MethodName);
            var instance = Activator.CreateInstance(classType);
            method?.Invoke(instance, []);
        }
    }
}
