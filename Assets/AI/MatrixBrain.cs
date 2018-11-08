using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI
{
    public class MatrixBrain : IMatrixBrain
    {
        public List<HiddenLayer> HiddenLayers { get; set; }

        public SigmoidNode OutputNode { get; set; }

        public MatrixBrain()
        {
            HiddenLayers = new List<HiddenLayer>();
        }

        public MatrixBrain(int xInputs, int yInputs, List<int> numberOfNodesInEachLayer, IActivationFunction activationFunction)
        {
            HiddenLayers = new List<HiddenLayer>();

            var numberOfInputs = xInputs * yInputs;

            foreach (var number in numberOfNodesInEachLayer)
            {
                HiddenLayers.Add(new HiddenLayer(number));
            }

            HiddenLayers[0].InitialiseHiddenLayer(numberOfInputs, activationFunction);

            for (int i = 1; i < numberOfNodesInEachLayer.Count; i++)
            {
                HiddenLayers[i].InitialiseHiddenLayer(HiddenLayers[i - 1].Nodes.Length, activationFunction);
            }

            OutputNode = new SigmoidNode(HiddenLayers[HiddenLayers.Count - 1].Nodes.Length, activationFunction);
        }

        public void RandomiseWeightsAndBiases()
        {
            var random = new Random();

            foreach (var layer in HiddenLayers)
            {
                for (int i = 0; i < layer.Nodes.Length; i++)
                {
                    layer.Nodes[i].BiasWeight = (random.NextDouble() * 2) - 1;

                    for (int j = 0; j < layer.Nodes[i].Weights.Length; j++)
                    {
                        layer.Nodes[i].Weights[j] = (random.NextDouble() * 2) - 1;
                    }
                }
            }

            for (int i = 0; i < OutputNode.Weights.Length; i++)
            {
                OutputNode.Weights[i] = (random.NextDouble() * 2) - 1;
            }
            OutputNode.BiasWeight = (random.NextDouble() * 2) - 1;
        }

        public double CalculateOutput(double[,] inputs)
        {
            var linearisedInputs = new double[inputs.GetLength(0) * inputs.GetLength(1)];
            var index = 0;

            for (int x = 0; x < inputs.GetLength(0); x++)
            {
                for (int y = 0; y < inputs.GetLength(1); y++)
                {
                    linearisedInputs[index] = inputs[x, y];
                    index++;
                }
            }

            var firstLayerOutputs = new double[HiddenLayers[0].Nodes.Length];

            for (int i = 0; i < HiddenLayers[0].Nodes.Length; i++)
            {
                firstLayerOutputs[i] = HiddenLayers[0].Nodes[i].CalculateOutput(linearisedInputs);
            }

            var nextInputs = firstLayerOutputs;

            for (int i = 1; i < HiddenLayers.Count; i++)
            {
                var outputs = new double[HiddenLayers[i].Nodes.Length];

                for (int j = 0; j < HiddenLayers[i].Nodes.Length; j++)
                {
                    outputs[j] = HiddenLayers[i].Nodes[j].CalculateOutput(nextInputs);
                }

                nextInputs = outputs;
            }

            var output = OutputNode.CalculateOutput(nextInputs);

            return OutputNode.CalculateOutput(nextInputs);
        }
    }
}
