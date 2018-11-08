using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public interface IHiddenLayer
    {
        INode[] Nodes { get; set; }

        void InitialiseHiddenLayer(int numberOfINputs, IActivationFunction activationFunction);
    }
}
