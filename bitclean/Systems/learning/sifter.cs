using System;


/* /Systems/learning/sifter.cs
 * The sifter class used to sift data based on a given function
 */

namespace bitclean
{
    /// <summary>
    /// Sifter.
    /// </summary>
	class Sifter
	{
        // array of statistics on just the dust data
        private readonly AttributeStatistics[] dustStats;
        private Linear sizeLinear; // edgeLinear, densityLinear;
        private Logistic sizeLogistic; // edgeLogistic, densityLogistic;
        // tolerance for how far off we can be from our target percentages
        readonly double tolerance = .001;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Sifter"/> class.
        /// [0] size | [1] edges | [2] density
        /// </summary>
        /// <param name="dustStats">Dust stats.</param>
        public Sifter(AttributeStatistics[] dustStats) 
        { 
            this.dustStats = dustStats;
            SetUpFunctions();
        }

        /// <summary>
        /// Sift the data.
        /// </summary>
        /// <returns>The sift.</returns>
		public int Sift()
		{
        		int output = 0;

        		// calculate size value
        		// calculate edge ratio value
        		// calculate density value

        		// add together

        		return output;
		}

        /// <summary>
        /// Sets up size/edge ratio/density piecewise functions.
        /// </summary>
        private void SetUpFunctions()
        {
            // set up size piecewise functions
            sizeLinear = new Linear(1.0 / dustStats[0].avg, -1);
            sizeLogistic = new Logistic();
            GenerateLogisticParameters(sizeLogistic, dustStats[0]);

            Console.WriteLine("a:{0}\tb:{1}\tc:{2}", sizeLogistic.a, sizeLogistic.b, sizeLogistic.c);
            // set up edge ratio piecewise functions

            // set up density inverted piecewise functions
        }

        /// <summary>
        /// Automatically generates the logistic parameters.
        /// </summary>
        /// <param name="func">Func.</param>
        /// <param name="stats">Stats.</param>
        public void GenerateLogisticParameters(Logistic func, AttributeStatistics stats)
        {
            // we want sifter to use the statistics from the object data to generate 
            // a logistic curve such that the 90% mark is roughly equal to the max
            // value for this parameter (size, density, edge ratio, hue, etc) and
            // the 50% mark is roughly equal to zero. This will also force the 10%
            // mark to equal -(90%)


            // set initial generic parameter values
            func.b = .001;  // beta generic
            func.c = 2;     // c generic
            func.a = Math.Exp(stats.avg * func.b);  // alpha generic
            func.offset = -1;

            double bump = .001;
            double calculated = 0.0;

            int attempts = 1000;

            while(func.Activate(stats.max) > .9)
                func.b -= bump;

            do
            {
                attempts--;
                calculated = func.Activate(stats.max);

                if (calculated < .9)
                    func.b += bump;
                else if (calculated > .9)
                {
                    func.b -= bump;
                    bump = bump / 10.0;
                    func.b += bump;
                }

                if (attempts < 0) break;

                Console.WriteLine(func.b);

            } while (!(calculated > (.9 - tolerance) && calculated < (.9 + tolerance)));
        }
    }
}
