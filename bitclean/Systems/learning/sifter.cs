using System;

/*
 * bitclean: /system/sifterbitclean.cs
 * author: Austin Herman
 * 5/8/2019
 */

namespace bitclean
{

	class Sifter
	{
		private readonly AttributeStatistics[] dustStats;
        private Linear sizeLinear; // edgeLinear, densityLinear;
        private Logistic sizeLogistic; // edgeLogistic, densityLogistic;

        readonly double tolerance = .001;

		//	[0] size | [1] edges | [2] density
		public Sifter(AttributeStatistics[] dustStats) 
        { 
            this.dustStats = dustStats;
            SetUpFunctions();
        }

		public int Sift()
		{
			int output = 0;

			// calculate size value
			// calculate edge ratio value
			// calculate density value

			// add together

			return output;
		}

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
