using System;

namespace DistributedSetupLib.Util
{
    internal static class Util
    {
        public static (string, string[]) ParseIdentifierAndArgs(string command)
        {
            string[] split = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return (split[0].ToUpper(), split[Range.StartAt(1)]);
        }
    }
}
