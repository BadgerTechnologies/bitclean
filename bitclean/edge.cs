using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * bitclean: /bitclean/edge.cs
 * author: Austin Herman
 * 2/11/2019
 * deep algorithm for calculating how many edges are on the object (roughly)
 */

namespace BitClean
{
    class Edge
    {
		Edge()
        {
            Console.WriteLine(edge_warn + "edge initialized with no pixels\n");
        }

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

        private void IterateEdges()
        {
            while (stack.Count != 0)
            {
                int id = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
				CheckNeighbors(GetOctan(id), id);
            }
        }

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

        private void AddPerimeterPixel(int p)
        {
            stack.Add(p);
            perimeter.Add(p);
            perimSize++;
        }

        private void CheckField(Field dir)
        {
            if (!fieldSet) {
				SetField(dir);
                numEdges++;
            }
            else
            {
                tolerance += curfield[Convert.ToInt32(dir)];
                if (tolerance < -4 || tolerance > 4)
                {
                    tolerance = 0;
                    numEdges++;
					SetField(dir);
                }
            }
        }

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

        public List<int> GetPerimiter()
        {
            return perimeter;
        }

        public int GetSizeofPerimeter()
        {
            return perimSize;
        }

        public int GetEdges()
        {
            return numEdges;
        }

        private Node sel = new Node();
        private Node per = new Node();
        private int[] curfield = new int[8];
        private bool fieldSet;
        private List<int> perimeter = new List<int>();
        private List<int> stack = new List<int>();
        private readonly int width, total;
        private int numEdges, perimSize, tolerance;
        private readonly string edge_warn = "::EDGE::warning : ";
    }
}