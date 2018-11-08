using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class HiddenLayer : IHiddenLayer
    {
        public INode[] Nodes { get; set; }

        public HiddenLayer(int numberOfNodes)
        {
            Nodes = new SigmoidNode[numberOfNodes];
        }

        public void InitialiseHiddenLayer(int numberOfInputs, IActivationFunction activationFunction)
        {
            for (int i = 0; i < Nodes.Length; i++)
            { 
                Nodes[i] = new SigmoidNode(numberOfInputs, activationFunction);
            }
        }
    }
}
