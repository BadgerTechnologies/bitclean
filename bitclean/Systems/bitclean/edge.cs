using System;
using System.Collections.Generic;

/* /Systems/bitclean/edge.cs
 * Contains the Edge class that has various algorithms for finding and 
 compressing edges of a given pixel buffer.
 */

namespace bitclean
{
    /// <summary>
    /// Edge.
    /// </summary>
    class Edge
    {
        // root node for the selection pixels (entire object)
        private Node sel = new Node();
        // root node for the perimeter pixels
        private Node per = new Node();
        private int[] curfield = new int[8]; // the current vector field
        private bool fieldSet; // whether or not the field has been set yet
        // list of perimeter pixels that were found
        private List<int> perimeter = new List<int>();
        // the current stack of pixels to be searched
        private List<int> stack = new List<int>();
        // width of image and total number of pixels
        private readonly int width, total;
        // number of edges calculated, number of perimeter pixels found, current field tolerance
        private int numEdges, perimSize, tolerance;
        private readonly string edge_warn = "::EDGE::warning : ";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Edge"/> 
        /// class.
        /// </summary>
        Edge() { Console.WriteLine(edge_warn + "edge initialized with no pixels\n"); }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Edge"/> 
        /// class.
        /// </summary>
        /// <param name="w">The width.</param>
        /// <param name="t">T.</param>
        public Edge(int w, int t)
        {
            sel = null;
            per = null;
            numEdges = 0;
            tolerance = 0;
            perimSize = 0;
            fieldSet = false;
            width = w;
            total = t;
        }

        /// <summary>
        /// Begin detecting the perimiter pixels in the mselection and buffer 
        /// data structures.
        /// </summary>
        /// <param name="mselection">Mselection.</param>
        /// <param name="buff">Buff.</param>
        public void Detect(List<int> mselection, Node buff)
        {
            sel = buff;
            stack.Add(mselection[0]);
            perimeter.Add(mselection[0]);
            Tree.Insert(ref per, mselection[0]);
            perimSize++;
            numEdges++;
			IterateEdges();

            per = null;
            sel = null;
        }


        /// <summary>
        /// Iterates the current stack of found perimeter pixels to find more.
        /// </summary>
        private void IterateEdges()
        {
            while (stack.Count != 0)
            {
                int id = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
				CheckNeighbors(GetOctan(id), id);
            }
        }

        /// <summary>
        /// Gets the eight neighboring pixels surrounding pixel at id.
        /// </summary>
        /// <returns>The octan.</returns>
        /// <param name="id">Identifier.</param>
        private Octan GetOctan(int id)
        {
			Octan oct = new Octan();
            if ((id - width) > 0)
            {
                oct.tl = Tree.FindNode(sel, id - width - 1);
                oct.t = Tree.FindNode(sel, id - width);
                oct.tr = Tree.FindNode(sel, id - width + 1);
            }

            if (id % width != 0)
                oct.l = Tree.FindNode(sel, id - 1);

            if ((id + 1) % width != 0)
                oct.r = Tree.FindNode(sel, id + 1);

            if ((id + width) < total)
            {
                oct.bl = Tree.FindNode(sel, id + width - 1);
                oct.b = Tree.FindNode(sel, id + width);
                oct.br = Tree.FindNode(sel, id + width + 1);
            }

            return oct;
        }

        /// <summary>
        /// Checks the neighboring pixels in the octan for perimeter pixels.
        /// </summary>
        /// <param name="oct">Oct.</param>
        /// <param name="id">Identifier.</param>
        private void CheckNeighbors(Octan oct, int id)
        {
            if ((id - width) > 0)
            { //read: "check if top left is an edge"
				Check(oct.tl, oct.t, oct.l, Field.tl);
				Check(oct.t, oct.tl, oct.tr, Field.t);
				Check(oct.tr, oct.t, oct.r, Field.tr);
            }

            if (id % width != 0)
				Check(oct.l, oct.tl, oct.bl, Field.l);

            if ((id + 1) % width != 0)
				Check(oct.r, oct.tr, oct.br, Field.r);

            if ((id + width) < total)
            {
				Check(oct.bl, oct.l, oct.b, Field.bl);
				Check(oct.b, oct.bl, oct.br, Field.b);
				Check(oct.br, oct.b, oct.r, Field.br);
            }
        }

        /// <summary>
        /// Check the specified centerpixel given neighbors neigh1, neigh2 and 
        /// direction dir.
        /// </summary>
        /// <param name="centerpixel">Centerpixel.</param>
        /// <param name="neigh1">Neigh1.</param>
        /// <param name="neigh2">Neigh2.</param>
        /// <param name="dir">Dir.</param>
        private void Check(int centerpixel, int neigh1, int neigh2, Field dir)
        {
            if (centerpixel != -1 && !(neigh1 != -1 && neigh2 != -1))
            {
                if (Tree.Insert(ref per, centerpixel))
                {
					AddPerimeterPixel(centerpixel);
					CheckField(dir);
                }
            }
        }

        /// <summary>
        /// Adds the perimeter pixel to the stack and perimeter buffer.
        /// </summary>
        /// <param name="p">P.</param>
        private void AddPerimeterPixel(int p)
        {
            stack.Add(p);
            perimeter.Add(p);
            perimSize++;
        }

        /// <summary>
        /// Checks that the field is correctly set and that the direction is
        /// still within the preset tolerance.
        /// </summary>
        /// <param name="dir">Dir.</param>
        private void CheckField(Field dir)
        {
            if (!fieldSet) {
				SetField(dir);
                numEdges++;
            }
            else
            {
                tolerance += curfield[Convert.ToInt32(dir)];
                if (tolerance < -Constants.TOLERANCE || tolerance > Constants.TOLERANCE)
                {
                    tolerance = 0;
                    numEdges++;
					SetField(dir);
                }
            }
        }

        /// <summary>
        /// Sets the field vector.
        /// </summary>
        /// <param name="dir">Dir.</param>
        private void SetField(Field dir)
        {
            if (dir == Field.t || dir == Field.b) {   //vertical
                curfield = FieldVector.verticalfield;
                fieldSet = true;
            }

            else if (dir == Field.l || dir == Field.r) {   //horizontal
                curfield = FieldVector.horizontalfield;
                fieldSet = true;
            }
            else if (dir == Field.tl || dir == Field.br) {   //leftslant
                curfield = FieldVector.leftslantfield;
                fieldSet = true;
            }
            else {   //rightslant
                curfield = FieldVector.rightslantfield;
                fieldSet = true;
            }
        }

        /// <summary>
        /// Gets the perimiter list.
        /// </summary>
        /// <returns>The perimiter.</returns>
        public List<int> GetPerimiter() { return perimeter; }

        /// <summary>
        /// Gets the size of the perimeter.
        /// </summary>
        /// <returns>The sizeof perimeter.</returns>
        public int GetSizeofPerimeter() { return perimSize; }

        /// <summary>
        /// Gets the number of edges.
        /// </summary>
        /// <returns>The edges.</returns>
        public int GetEdges() { return numEdges; }
    }
}