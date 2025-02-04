using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBAN_Studio.Common.Attributes;

namespace VBAN_Studio.Core
{
    [RegisterStartupMethod(nameof(Start))]
    public class Init
    {
        public void Start()
        {
            Console.WriteLine("Started VBAN_Studio.Core");
        }
    }
}
