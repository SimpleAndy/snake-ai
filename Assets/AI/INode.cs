using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public interface INode
    {
        int NumberOfInputs { get; set; }

        double[] Weights { get; set; }

        double BiasWeight { get; set; }

        IActivationFunction ActivationFunction { get; set; }

        double CalculateOutput(double[] inputs);

        void Backpropagate(double error);
    }
}
