using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class SoftmaxLayer
    {
        public INode[] Nodes { get; set; }

        public double[] Outputs { get; set; }

        public SoftmaxLayer(int numberOfInputs, int numberOfClasses)
        {
            Nodes = new SoftmaxNode[numberOfClasses];
            Outputs = new double[numberOfClasses];

            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i] = new SoftmaxNode(numberOfInputs, numberOfClasses);
            }
        }

        public int SelectBestOutputClass()
        {
            var maxOutput = Outputs.Max();
            return Array.IndexOf(Outputs, maxOutput);
        }
    }
}
