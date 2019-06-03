using System;

/* /Systems/learning/functions.cs
 * Contains the functions used by various learning systems
 */

namespace bitclean
{
    /// <summary>
    /// Logistic parameters.
    /// </summary>
    public struct LogisticParameters
    {
        public double a, b, c;
        public int offset;
    }

    /// <summary>
    /// Activation function base class.
    /// </summary>
    public abstract class ActivationFunction
    {
        public abstract double Activate(int data);
        public abstract double Activate(double data);
    }

    /// <summary>
    /// Linear function.
    /// </summary>
    public class Linear : ActivationFunction
    {
        private readonly double slope;
        private readonly double offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Linear"/> class.
        /// </summary>
        public Linear()
        {
            slope = 1;
            offset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Linear"/> class.
        /// </summary>
        /// <param name="slope">Slope.</param>
        /// <param name="offset">Offset.</param>
        public Linear(double slope, double offset)
        {
            this.slope = slope;
            this.offset = offset;
        }

        /// <summary>
        /// Activate the specified int type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(int data)
        {
            return data * slope + offset;
        }

        /// <summary>
        /// Activate the specified double type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(double data)
        {
            return data * slope + offset;
        }
    }

    /// <summary>
    /// Rectified linear function. Linear with slope of 1 for x > 0, else y = 0
    /// </summary>
    public class RectifiedLinear : ActivationFunction
    {
        /// <summary>
        /// Activate the specified int type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(int data)
        {
            if (data < 0.0)
                return 0.0;
            return data;
        }
        /// <summary>
        /// Activate the specified double type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(double data)
        {
            if (data < 0.0)
                return 0.0;
            return data;
        }
    }

    /// <summary>
    /// Logistic function - google "1/(1 + exp(-x)" to see what these look like.
    /// </summary>
    public class Logistic : ActivationFunction
    {
        // parameters a/b/c/offset used as "c/(1 + a*exp(-x*b)) - offset"
        public double a, b, c;
        public int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Logistic"/> class.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        public Logistic(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            offset = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Logistic"/> class.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="c">C.</param>
        /// <param name="offset">Offset.</param>
        public Logistic(double a, double b, double c, int offset)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Logistic"/> class.
        /// </summary>
        /// <param name="p">P.</param>
        public Logistic(LogisticParameters p)
        {
            a = p.a;
            b = p.b;
            c = p.c;
            offset = p.offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Logistic"/> class.
        /// </summary>
        public Logistic()
        {
            a = 0;
            b = 0;
            c = 0;
            offset = 0;
        }

        /// <summary>
        /// Activate the specified int type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(int data)
        {
            return c / (1 + a * Math.Exp(-data * b)) + offset;
        }

        /// <summary>
        /// Activate the specified double type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(double data)
        {
            return c / (1 + a * Math.Exp(-data * b)) + offset;
        }
    }

    /// <summary>
    /// Heaviside.
    /// </summary>
    public class Heaviside : ActivationFunction
    {
        /// <summary>
        /// Activate the specified int type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(int data)
        {
            if (data < 0.0) return 0.0;
            return 1.0;
        }

        /// <summary>
        /// Activate the specified double type data.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="data">Data.</param>
        public override double Activate(double data)
        {
            if (data < 0.0) return 0.0;
            return 1.0;
        }
    }
}
