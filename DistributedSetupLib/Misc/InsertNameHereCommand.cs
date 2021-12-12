using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedSetupLib.Misc
{
    public enum InsertNameHereCommand
    {
        Fetch,
        Set,
        Delete,
        Cas,
        Invalid = -1
    }

    public static class InsertNameHereCommandMethod
    {
        public static string Convert(this InsertNameHereCommand command)
        {
            return command.ToString().ToUpper();
        }

        public static InsertNameHereCommand Convert(string commandStr)
        {
            return Enum.Parse<InsertNameHereCommand>(commandStr, true);
        }
    }
}
