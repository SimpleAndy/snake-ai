using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class SoftmaxNode : INode
    {
        public int NumberOfInputs { get; set; }

        public double[] Weights { get; set; }
        public double BiasWeight { get; set; }
        public IActivationFunction ActivationFunction { get; set; }

        public SoftmaxNode(int numberOfInputs, int numberOfClasses)
        {
            NumberOfInputs = numberOfInputs;
            Weights = new double[numberOfInputs];
            ActivationFunction = new SoftmaxActivationFunction(numberOfClasses);
        }

        public void Backpropagate(double error)
        {
            throw new NotImplementedException();
        }

        public double CalculateOutput(double[] inputs)
        {

            if (inputs.Length != NumberOfInputs)
            {
                throw new ArgumentException("This node must take " + NumberOfInputs + "inputs, but received " + inputs.Length + ".");
            }

            double sum = 0;

            for (int i = 0; i < NumberOfInputs; i++)
            {
                sum += inputs[i] * Weights[i];
            }

            return ActivationFunction.CalculateOutput(sum + BiasWeight);
        }
    }
}
