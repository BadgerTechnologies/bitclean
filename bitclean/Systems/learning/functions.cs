using System;

/*
 * bitclean: /system/sifterbitclean.cs
 * author: Austin Herman
 * 5/8/2019
 */

namespace bitclean
{
    public struct LogisticParameters
    {
        public double a, b, c;
        public int offset;
    }

    public abstract class ActivationFunction
    {
        public abstract double Activate(int data);
        public abstract double Activate(double data);
    }

    public class Linear : ActivationFunction
    {
        private readonly double slope;
        private readonly double offset;

        public Linear()
        {
            slope = 1;
            offset = 0;
        }
        public Linear(double slope, double offset)
        {
            this.slope = slope;
            this.offset = offset;
        }
        public override double Activate(int data)
        {
            return data * slope + offset;
        }
        public override double Activate(double data)
        {
            return data * slope + offset;
        }
    }

    public class RectifiedLinear : ActivationFunction
    {
        public override double Activate(int data)
        {
            if (data < 0.0)
                return 0.0;
            return data;
        }
        public override double Activate(double data)
        {
            if (data < 0.0)
                return 0.0;
            return data;
        }
    }

    public class Logistic : ActivationFunction
    {
        public double a, b, c;
        public int offset;

        public Logistic(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            offset = 0;
        }

        public Logistic(double a, double b, double c, int offset)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.offset = offset;
        }

        public Logistic(LogisticParameters p)
        {
            a = p.a;
            b = p.b;
            c = p.c;
            offset = p.offset;
        }

        public Logistic()
        {
            a = 0;
            b = 0;
            c = 0;
            offset = 0;
        }

        public override double Activate(int data)
        {
            return c / (1 + a * Math.Exp(-data * b)) + offset;
        }
        public override double Activate(double data)
        {
            return c / (1 + a * Math.Exp(-data * b)) + offset;
        }
    }

    public class Heaviside : ActivationFunction
    {
        public override double Activate(int data)
        {
            if (data < 0.0) return 0.0;
            return 1.0;
        }
        public override double Activate(double data)
        {
            if (data < 0.0) return 0.0;
            return 1.0;
        }
    }
}
