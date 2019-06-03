using System.Collections.Generic;

/* /Systems/bitclean/Filler.cs
 * Takes a selection of only non-white pixels and adds to the buffer any 
 unselected pixels that may be contained inside the selection.
 */

namespace bitclean
{
    /// <summary>
    /// Filler.
    /// </summary>
    class Filler
    {
        private readonly Pixel[] p; // array of every pixel in the image
        private readonly int width, total; // width of image and total pixels
        private ObjectBounds bounds; // bounding coordinates of the current object
        private List<Trail> curpath = new List<Trail>(); // list of trails
        private int pathSize; // size of the path
        private bool boundsError; // if there was a bounds error or not
        private readonly string filler_warn = "::FILLER::warning : ";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Filler"/> class.
        /// </summary>
        Filler()
        {
            bounds.top		= -1;
            bounds.bottom	= -1;
            bounds.left		= -1;
            bounds.right	= -1;
            pathSize		= 0;
            boundsError		= false;
            System.Console.WriteLine(filler_warn + "object created with out passing pixels and information.\n");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:bitclean.Filler"/> class.
        /// </summary>
        /// <param name="pixels">Pixels.</param>
        /// <param name="imageWidth">Image width.</param>
        /// <param name="totalPixels">Total pixels.</param>
        public Filler(Pixel[] pixels, int imageWidth, int totalPixels)
        {
			bounds.top		= totalPixels;
			bounds.bottom	= -1;
			bounds.left		= imageWidth;
			bounds.right	= -1;
            pathSize		= 0;
            boundsError		= false;
            p		= pixels;
            width	= imageWidth;
            total	= totalPixels;
        }

        /// <summary>
        /// Run the filler to fill in unselected pixels in the specified buffer 
        /// pixels
        /// </summary>
        /// <returns>The run.</returns>
        /// <param name="buffer">Buffer.</param>
        /// <param name="buffersize">Buffersize.</param>
        /// <param name="buff">Buff.</param>
        public int Run(List<int> buffer, int buffersize, Node buff)
        {
            for (int i = 0; i < buffersize; i++)	// *** add this to Check pixel for speed
                FeedBounds(buffer[i]);	// feed in all pixels to find the object's boundaries

            int count = 0;
            for (int i = 0; i < buffersize; i++)
            {
				// Check that we can look at the pixel underneath the current pixel
				// Check that the pixel underneath the current is not selected
				// Check that the pixel underneath the current is white
                if (buffer[i] + width < total 
					&& !p[buffer[i] + width].selected 
					&& p[buffer[i] + width].value == Constants.INT_WHITE)
                {
                    Start(buffer[i] + width);
                    List<Trail> whitePixels = GetPath();
                    if (whitePixels.Count != 0)
                    {
                        for (int j = 0; j < whitePixels.Count; j++)
                        {
                            buffer.Add(whitePixels[j].id);
                            Tree.Insert(ref buff, whitePixels[j].id);
                            count++;
                        }
                    }
                } // end big if
            }

            return count;
        }

        /// <summary>
        /// Takes a pixel and checks if it is the furthest up, down, left, or 
        /// right pixel.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void FeedBounds(int id)
        {
            if ((id % width) < bounds.left) bounds.left = id % width;

            if ((id % width) > bounds.right) bounds.right = id % width;

            if ((id / width) < bounds.top) bounds.top = id / width;

            if ((id / width) > bounds.bottom) bounds.bottom = id / width;
        }

        /// <summary>
        /// Takes a pixel id and tests if it's within the bounds of the object.
        /// </summary>
        /// <returns>The inbounds.</returns>
        /// <param name="id">Identifier.</param>
        private bool Inbounds(int id)
        {
            if (id % width < bounds.left
            || id % width > bounds.right
            || id / width < bounds.top
            || id / width > bounds.bottom)
                return false;
            return true;
        }

        /// <summary>
        /// Start the iteration process.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public void Start(int id)
        {	// reset the path and bounds error flag
            curpath.Clear();
            pathSize = 0;
            boundsError = false;

            AddToPath(Direction.none, id);
            if (boundsError) {
                ClearPath();
                return;
            }

            IteratePath();
        }

        /// <summary>
        /// Adds the current direction and pixel id to the path list.
        /// </summary>
        /// <param name="dir">Dir.</param>
        /// <param name="id">Identifier.</param>
        private void AddToPath(Direction dir, int id)
        {
            if (!Inbounds(id))
                boundsError = true;
            else if(p[id].value == Constants.INT_WHITE)
            {
				Trail pd = new Trail(dir, id);
                curpath.Add(pd);
                pathSize++;
                p[pd.id].selected = true;
            }
        }

        /// <summary>
        /// Iterates through the current path.
        /// </summary>
        private void IteratePath()
        {
            for (int i = 0; i < pathSize; i++)
            {
                Check(curpath[i]);
                if (boundsError) {
                    ClearPath();
                    return;
                }
            }
        }

        /// <summary>
        /// Check the specified trail to see if there are any other unselected
        /// pixels.
        /// </summary>
        /// <param name="pd">Pd.</param>
        private void Check(Trail pd)
        {
            switch (pd.dir) // 0 none, 1 up, 2 down, 3 left, 4 right
            {
                case Direction.up:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
                            AddToPath(Direction.up, pd.id - width); // up
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   // left
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
                            AddToPath(Direction.right, pd.id + 1);  // right
                    }
                    break;
                }
                case Direction.down:
                {
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
                            AddToPath(Direction.down, pd.id + width);   // down
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   // left
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
                            AddToPath(Direction.right, pd.id + 1);  // right
                    }
                    break;
                }
                case Direction.left:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
                            AddToPath(Direction.up, pd.id - width); // up
                    }
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
                            AddToPath(Direction.down, pd.id + width);   // down
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   // left
                    }
                    break;
                }
                case Direction.right:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
                            AddToPath(Direction.up, pd.id - width); // up
                    }
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
							AddToPath(Direction.down, pd.id + width);   // down
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
							AddToPath(Direction.right, pd.id + 1);  // right
                    }
                    break;
                }
                case Direction.none:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
							AddToPath(Direction.up, pd.id - width); // up
                    }
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
							AddToPath(Direction.down, pd.id + width);   // down
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   // left
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
                            AddToPath(Direction.right, pd.id + 1);  // right
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Clears the path.
        /// </summary>
        private void ClearPath()
        {
			for(int i = 0; i < pathSize; i++)
				p[curpath[i].id].selected = false;

            curpath.Clear();
            pathSize = 0;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <returns>The path.</returns>
        public List<Trail> GetPath() { return curpath; }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <returns>The bounds.</returns>
		public ObjectBounds GetBounds() { return bounds; }
    }
}