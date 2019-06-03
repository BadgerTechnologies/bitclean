using System;
using System.Collections.Generic;

/* /Systems/bitclean/selection.cs
 * Drives the algorithms for finding an object, filling it in, and finding its 
 edges
 */

namespace bitclean
{
    /// <summary>
    /// Selection.
    /// </summary>
    public class Selection
    {
        private readonly Pixel[] p; // pixel array
        // width of the image, total number of pixels in the image
        private readonly int width, total;
        private ObjectBounds objbounds; // the bounds of the object
        // list of all pixels in the buffer and perimeter pixels
        private List<int> buffer, perimeter;
        private int buffersize; // size of the buffer
        private Node buff; // root node of the buffer tree
        private readonly string selection_err = "::SELECTION::error : ";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Selection"/> class.
        /// </summary>
        Selection() { Console.Write(selection_err + "initialized without pixels\n"); }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Selection"/> class.
        /// </summary>
        /// <param name="pixels">Pixels.</param>
        /// <param name="width">Width.</param>
        /// <param name="total">Total.</param>
        public Selection(Pixel[] pixels, int width, int total)
        {
            buffer = new List<int>();
            perimeter = new List<int>();
            p = pixels;
            this.width = width;
            this.total = total;
            buffersize = 0;
        }

        /// <summary>
        /// Get the rest of the pixels from starting point id.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="id">Identifier.</param>
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

        /// <summary>
        /// Iterate through the current buffer of pixels to find more pixels
        /// </summary>
        private void Iterate()
        {
            for (int i = 0; i < buffersize; i++)
				NextPixel(buffer[i]); // check neighbors for non-white
        }

        /// <summary>
        /// Finds the next pixel given the current pixel
        /// </summary>
        /// <param name="id">Identifier.</param>
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

        /// <summary>
        /// Checks that the pixel is non-white and not selected, then adds it to
        /// the buffer.
        /// </summary>
        /// <returns><c>true</c>, if pixel was checked, <c>false</c> otherwise.</returns>
        /// <param name="pixel">Pixel.</param>
        private bool CheckPixel(ref Pixel pixel)
        {
            if (pixel.value != Constants.INT_WHITE) 
            {
                if (!pixel.selected)
                {	// add pixel to buffer, make it selected, insert into buffer tree
                    buffer.Add(pixel.id);
                    pixel.selected = true;
                    buffersize++;
                    Tree.Insert(ref buff, pixel.id);
                    return true;
				} // if not white and not selected
			}
            return false;
        }

        /// <summary>
        /// Fills in the pixels of the found object.
        /// </summary>
        private void FillPixels()
        {
            Filler fill = new Filler(p, width, total);  // create fill object
			
			// run fill algorithm 
			// add any filled in pixels to the buffer and bump buffer size
			buffersize += fill.Run(buffer, buffersize, buff);

			objbounds = fill.GetBounds();
        }

        /// <summary>
        /// Finds the edges of the found object.
        /// </summary>
        private void FindEdges()
        {
            Edge e = new Edge(width, total);
            e.Detect(buffer, buff);
            perimeter = e.GetPerimiter();
            Edges = e.GetEdges();
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void ClearBuffer()
        {
            buffer.Clear();
            buffersize = 0;
            buff = null;
        }

        /// <summary>
        /// Gets/Sets the edges.
        /// </summary>
        /// <value>The edges.</value>
		public int Edges { get; private set; }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>The buffer.</value>
		public ref List<int> Buffer => ref buffer;

        /// <summary>
        /// Gets the perimeter.
        /// </summary>
        /// <value>The perimeter.</value>
        public ref List<int> Perimeter => ref perimeter;

        /// <summary>
        /// Gets the bounding rectangle of the current object.
        /// </summary>
        /// <returns>The bounds.</returns>
        public BoundingRectangle GetBounds()
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
    }
}