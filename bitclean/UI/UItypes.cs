using System;
using System.Collections.Generic;

/*
 * bitclean: /system/UItypes.cs
 * author: Austin Herman
 * 5/9/2019
 */

namespace bitclean
{
    static class UIConstants
    {
        public const double EPSILON = 0.000001f;
    }

    public class ChartObject
    {
        public string decision;
        public double avghue;       // average integer color value of all pixels in selection
        public double density;      // percentage of non-floor colored pixels occupying the selection
        public int size;            // total size in pixels of selection
        public double edgeratio;    // ratio of total size to calculated edges
        public int tag;             // unique id associated with object
        public List<int> neighbors; // list of tags of other objects in the vicinity of this one
        public Confidence objconf;  // confidence property 
    }

    public class AttributeStatistics
    {
        public double max = -1.0, min = -1.0, avg = 0.0;

        public void Clear()
        {
            max = -1.0;
            min = -1.0;
            avg = 0.0;
        }

    }

    public class ChartOptions
    {
        public string horizontalChoice, verticalChoice;
        public ActivationFunction function;
        public bool squared;
        public bool dust, structures;
    }

    enum AxisChoice { tag, size, avghue, density, edgeratio, neighbors }
}
