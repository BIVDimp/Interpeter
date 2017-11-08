using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInterpreter
{
    public sealed class Triplet<TFirst, TSecond, TThird>
    {
        public TFirst First { get; set; }
        public TSecond Second { get; set; }
        public TThird Third { get; set; }

        public Triplet(TFirst first, TSecond second, TThird third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }
    }
}
