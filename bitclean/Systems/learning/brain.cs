using System;
using System.Collections.Generic;

/* /Systems/learning/brain.cs
 * The broken, wannabe neural network
 */

namespace bitclean
{
    /// <summary>
    /// Initialise hyper parameters.
    /// </summary>
	public class HyperParameters
	{
		public int attributeCount = 0;
		public int synapseLayerCount = 0;
		public int neuronLayerCount = 0;
	}

    /// <summary>
    /// Create/Delete/Run Brain functions
    /// </summary>
	public class Brain
	{
		private List<Neuron> inputlayer;
		private List<List<Neuron>> neurons;
		private List<List<Synapse>> synapses;
		private List<List<int>> weights;
		public int attributeCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Brain"/> class.
        /// </summary>
        /// <param name="numAtttributes">Number atttributes.</param>
		public Brain(int numAtttributes)
		{
			attributeCount = numAtttributes;
			Generate();			
		}

        /// <summary>
        /// Run through each synapse group and transmit data.
        /// </summary>
        /// <returns>The think.</returns>
        /// <param name="inputdata">Inputdata.</param>
		public double Think(double[] inputdata)
		{
			DumpMemory();

			if (inputdata.Length != attributeCount)
				Regenerate(inputdata.Length);

			for (int i = 0; i < attributeCount; i++)
				inputlayer[i].data.Add(inputdata[i]);

			for (int i = 0; i < synapses.Count; i++) {
				for (int j = 0; j < synapses[i].Count; j++)
					synapses[i][j].Transmit();
			}

			synapses[attributeCount][0].Transmit();

			return neurons[attributeCount][0].axon;
		}

        /// <summary>
        /// Remove all neurons
        /// </summary>
		private void DumpMemory()
		{
			for (int i = 0; i < neurons.Count; i++) {
				for (int j = 0; j < neurons[i].Count; j++)
					neurons[i][j].Fry();
			}
		}

        /// <summary>
        /// Build a net of synapses and neurons
        /// </summary>
        /// <param name="numAttributes">Number attributes.</param>
		private void Regenerate(int numAttributes)
		{
			attributeCount = numAttributes;

			for (int i = 0; i < synapses.Count; i++)
				synapses[i].Clear();
			synapses.Clear();

			for (int i = 0; i < neurons.Count; i++)
				neurons[i].Clear();
			neurons.Clear();

			Generate();
		}

		/// <summary>
        /// Generate the neurons/synapses and hook them together
        /// </summary>
		private void Generate()
		{
			GenerateNeurons();
			GenerateSynapses();
		}

        /// <summary>
        /// Generates the neurons.
        /// </summary>
		private void GenerateNeurons()
		{
			inputlayer = new List<Neuron>();

			for (int i = 0; i < attributeCount; i++)
				inputlayer.Add(new Neuron(new Linear()));

            neurons = new List<List<Neuron>> {
                inputlayer
            };

            for (int i = 1; i < attributeCount + 1; i++) {
				neurons.Add(new List<Neuron>());
				for (int j = attributeCount - i; j >= 0; j--)
					neurons[i].Add(new Neuron(new Linear()));
			}
		}

        /// <summary>
        /// Generates the synapses.
        /// </summary>
		private void GenerateSynapses()
		{
			synapses = new List<List<Synapse>>();
			weights  = new List<List<int>>();

			// transmitter | receiver | weight
			synapses.Add(new List<Synapse>());
			weights.Add(new List<int>());

			for(int n = 0; n < attributeCount; n ++) {
				weights[0].Add(0);
				synapses[0].Add(new Synapse(inputlayer[n], neurons[1][n], weights[0][n]));
			}

			for(int l = 1; l < neurons.Count - 1; l ++)
			{
				synapses.Add(new List<Synapse>());
				weights.Add(new List<int>());
				for (int n = 0; n < neurons[l].Count; n++)
				{
					for(int nn = 0; nn < neurons[l + 1].Count; nn++) {
						weights[l].Add(l);
						synapses[l].Add(new Synapse(neurons[l][n], neurons[l + 1][nn], weights[l][nn]));
					}
				}
			}

			weights.Add(new List<int>());
			synapses.Add(new List<Synapse>());

			weights[attributeCount].Add(1);
			synapses[attributeCount].Add(new Synapse(neurons[attributeCount][0], null, weights[attributeCount][0]));

		}

	}

    /// <summary>
    /// Neuron.
    /// </summary>
	public class Neuron
	{
        // all of the data from incoming synapses
		public List<double> data = new List<double>();
        // the single numerical output for this neuron
		public double axon;

		// dendrite summation
		private double sum;
        // whether or not the sum has been calculated yet
		private bool calculatedSum;

        // the function to put the summed data through for the output
		private ActivationFunction func;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Neuron"/> class.
        /// </summary>
        /// <param name="func">Func.</param>
		public Neuron(ActivationFunction func) { this.func = func; }
		
        /// <summary>
        /// Calculates the axon.
        /// </summary>
		public void CalculateAxon()
		{
			if (calculatedSum) return;
			
			for (int i = 0; i < data.Count; i++) sum += data[i];

			axon = func.Activate(sum);
			calculatedSum = true;
		}
		
        /// <summary>
        /// Fry this neuron.
        /// </summary>
		public void Fry()
		{
			data.Clear();
			axon = 0.0;
			sum = 0.0;
			calculatedSum = false;
		}
	}

    /// <summary>
    /// Synapse.
    /// </summary>
	public class Synapse
	{
        // the incoming neuron
		private Neuron transmitter;
        // the receiving neuron
		private Neuron receiver;
        // the weight to apply for this data transfer
		private readonly int weight;
        // alternative to using a weight - square the data or not
		private readonly bool squared;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Synapse"/> class.
        /// </summary>
        /// <param name="transmitter">Transmitter.</param>
        /// <param name="receiver">Receiver.</param>
        /// <param name="weight">Weight.</param>
		public Synapse(Neuron transmitter, Neuron receiver, int weight)
		{
			this.transmitter = transmitter;
			this.receiver = receiver;
			this.weight = weight;
            squared |= weight == 0; // if weight == 0, squared = true
        }

        /// <summary>
        /// Transmit data from the transmitter to the receiver.
        /// </summary>
        public void Transmit()
		{
			transmitter.CalculateAxon();

			if(receiver != null) {
				if (squared)
					receiver.data.Add(transmitter.axon * transmitter.axon);
				else
					receiver.data.Add(transmitter.axon * weight);
			}
		}

        /// <summary>
        /// Removes receivers' data.
        /// </summary>
		public void ClearReceivers()
		{
			if (receiver != null)
				receiver.data.Clear();
		}
	}

}
