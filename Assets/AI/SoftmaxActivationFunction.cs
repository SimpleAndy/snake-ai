using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class SoftmaxActivationFunction : IActivationFunction
    {
        private int NumberOfClasses;

        public SoftmaxActivationFunction(int numberOfClasses)
        {
            NumberOfClasses = numberOfClasses;
        }

        public double CalculateOutput(double sum)
        {
            throw new NotImplementedException();
        }
    }
}
