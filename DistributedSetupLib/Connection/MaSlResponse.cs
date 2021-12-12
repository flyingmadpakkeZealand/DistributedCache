using DistributedSetupLib.Misc;

namespace DistributedSetupLib.Connection
{
    public class MaSlResponse
    {
        public static MaSlResponse EmptyResponse { get; } = new MaSlResponse(){StatusCode = StatusCode.Empty, Body = null};

        public static MaSlResponse NotFoundResponse { get; } = new MaSlResponse(){StatusCode = StatusCode.NotFound, Body = null};

        public StatusCode StatusCode { get; set; }

        public string Body { get; set; }

        public override string ToString()
        {
            return $"STATUS_CODE: {StatusCode}\n" +
                   $"BODY:\n" +
                   $"{Body}";
        }
    }
}
