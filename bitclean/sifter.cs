using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * bitclean: /bitclean/sifter.cs
 * author: Austin Herman
 * 4/24/2019
 * similar to confidence system, uses equations to assign decision values - IN PROGRESS
 */

namespace BitClean
{

	class Sifter
	{
		private AttributeStatistics[] dustStats;
        private Linear sizeLinear; //edgeLinear, densityLinear;
        private Logistic sizeLogistic; //edgeLogistic, densityLogistic;

		double tolerance = .001;

		//	[0] size | [1] edges | [2] density
		public Sifter(AttributeStatistics[] dustStats)
		{
			this.dustStats = dustStats;
		}

		public int Sift(double[] data)
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
			// set up edge ratio piecewise functions

			// set up density inverted piecewise functions
		}

		public void GenerateLogisticParameters(Logistic func, AttributeStatistics stats)
		{
			// set initial generic parameter values
			func.b = .001;
			func.c = 2;
			func.a = Math.Exp(stats.avg * func.b);

			double ninetyPercentVal;
			double zeroPercentVal;

			do
			{
				double calculated = 0.0;
				do
				{   // approximate b parameter
					calculated = func.Activate(stats.max);
					if (calculated < .9)
					{
						// increase func.b
						func.b += Math.Abs(.9 - calculated) / 2;
					}
					else if (calculated > .9)
					{
						// decrease func.b
						func.b -= Math.Abs(.9 - calculated) / 2;
					}
				} while (!(calculated > .9 - tolerance && calculated < .9 + tolerance));

				double k = 0.0;
				do
				{   // approximate k parameter to calculate a parameter
					calculated = func.Activate(stats.avg);
					if (calculated < 0)
					{
						// increase k
						k += Math.Abs(0 - calculated) / 2;
					}
					else if (calculated > 0)
					{
						// decrease k
						k -= Math.Abs(0 - calculated) / 2;
					}
					func.a = Math.Exp(stats.avg * func.b - k);
				} while (!(calculated > 0 - tolerance && calculated < 0 + tolerance));

				// recalculate 90% and 0% values
				ninetyPercentVal = func.Activate(stats.max);
				zeroPercentVal = func.Activate(stats.avg);

				// check each, repeat approximations if either are off
			} while (!(ninetyPercentVal > .9 - tolerance && ninetyPercentVal < .9 + tolerance) 
				&& !(zeroPercentVal > 0 - tolerance && zeroPercentVal < 0 + tolerance));
		}

	}
}
