using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * bitclean: /bitclean/image.cs
 * author: Austin Herman
 * 2/11/2019
 * deep algorithm for one touch object finding
 */

namespace BitClean
{
    public class Selection
    {
		Selection()
        {
            Console.Write(selection_err + "initialized without pixels\n");
        }

        public Selection(Pixel[] pixels, int width, int total)
        {
            buffer = new List<int>();
            perimeter = new List<int>();
            p = pixels;
            this.width = width;
            this.total = total;
            buffersize = 0;
        }

        public bool Get(int id)
        {
	        if (CheckPixel(ref p[id])) // check the current pixel
	        {
		        Iterate();		// find the rest of the object
				if(buffersize < Constants.MAX_OBJECT_SIZE_ESTIMATE) {
					FillPixels();   // fill in any white pixels inside the selection
					FindEdges();    // find edge pixels in selection
					return true;
				}
			}
	        return false;
        }

        private void Iterate()
        {
            for (int i = 0; i < buffersize; i++)
				NextPixel(buffer[i]); // check neighbors for non-white
        }

        private void NextPixel(int id)
        {
            if ((id - width - 1) > 0)
            {
                CheckPixel(ref p[id - width - 1]);   //top left
				CheckPixel(ref p[id - width]);       //top center
				CheckPixel(ref p[id - width + 1]);   //top right
            }

            if (id % width != 0)
				CheckPixel(ref p[id - 1]);   //center left
            if (id % (width + 1) != 0)
				CheckPixel(ref p[id + 1]);   //center right

            if ((id + width + 1) < total)
            {
				CheckPixel(ref p[id + width - 1]);   //bottom left
				CheckPixel(ref p[id + width]);       //bottom center
				CheckPixel(ref p[id + width + 1]);   //bottom right
            }
        }

        private bool CheckPixel(ref Pixel p)
        {
            if (p.value != Constants.INT_WHITE) 
            {
                if (!p.selected)
                {	// add pixel to buffer, make it selected, insert into buffer tree
                    buffer.Add(p.id); 
                    p.selected = true;
                    buffersize++;
                    Tree.Insert(ref buff, p.id);
                    return true;
				} // if not white and not selected
			}
            return false;
        }

        private void FillPixels()
        {
            Filler fill = new Filler(p, width, total);  // create fill object
			
			// run fill algorithm 
			// add any filled in pixels to the buffer and bump buffer size
			buffersize += fill.Run(buffer, buffersize, buff);

			objbounds = fill.GetBounds();
        }

        private void FindEdges()
        {
            Edge e = new Edge(width, total);
            e.Detect(buffer, buff);
            perimeter = e.GetPerimiter();
            Edges = e.GetEdges();
        }

        public void ClearBuffer()
        {
            buffer.Clear();
            buffersize = 0;
            buff = null;
        }

		public int Edges { get; private set; }

		public ref List<int> Buffer => ref buffer;

        public ref List<int> Perimeter => ref perimeter;

		public BoundingRectangle getBounds()
		{
			return new BoundingRectangle
			{
				top = objbounds.top,
				left = objbounds.left,
				bottom = objbounds.bottom,
				right = objbounds.right,
				width = objbounds.right - objbounds.left,
				height = objbounds.bottom - objbounds.top
			};
		}

        private readonly Pixel[] p;
        private readonly int width, total;
		private ObjectBounds objbounds;
        private List<int> buffer, perimeter;
        private int buffersize;
		private Node buff;
        private readonly string selection_err = "::SELECTION::error : ";
    }
}