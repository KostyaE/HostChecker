using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.Common
{    public interface ITimerHostCheck
    {
        void CheckDB();
        bool WebRequest(string webAddress);
        DateTime AddTimeNextOfChecking(int minute, int hours);
    }
}
