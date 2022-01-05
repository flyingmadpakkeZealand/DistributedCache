using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheTesting
{
    public enum Subscription
    {
        Normal,
        Premium
    }

    public class PremiumAccessData : BasicAccessData
    {
        public Subscription Subscription { get; set; }
    }
}
