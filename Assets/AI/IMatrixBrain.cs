using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    interface IMatrixBrain
    {
        List<HiddenLayer> HiddenLayers { get; set; }

        SigmoidNode OutputNode { get; set; }

        double CalculateOutput(double[,] inputs);
    }
}
