using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class SigmoidActivationFunction : IActivationFunction
    {
        public double CalculateOutput(double input)
        {
            return (1 / (1 + (Math.Exp(-(input)))));
        }
    }
}
