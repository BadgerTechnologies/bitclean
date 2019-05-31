using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * bitclean: /bitclean/filler.cs
 * author: Austin Herman
 * 2/11/2019
 * deep algorithm for filling in the object's untouched pixels
 */

namespace BitClean
{
    class Filler
    {
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
                }	// end big if
            }

            return count;
        }

        public void FeedBounds(int id)
        {
            if ((id % width) < bounds.left) bounds.left = id % width;

            if ((id % width) > bounds.right) bounds.right = id % width;

            if ((id / width) < bounds.top) bounds.top = id / width;

            if ((id / width) > bounds.bottom) bounds.bottom = id / width;
        }

        private bool Inbounds(int id)
        {
            if (id % width < bounds.left
            || id % width > bounds.right
            || id / width < bounds.top
            || id / width > bounds.bottom)
                return false;
            return true;
        }

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

        private void Check(Trail pd)
        {
            switch (pd.dir) //0 none, 1 up, 2 down, 3 left, 4 right
            {
                case Direction.up:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
                            AddToPath(Direction.up, pd.id - width); //up
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   //left
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
                            AddToPath(Direction.right, pd.id + 1);  //right
                    }
                    break;
                }
                case Direction.down:
                {
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
                            AddToPath(Direction.down, pd.id + width);   //down
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   //left
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
                            AddToPath(Direction.right, pd.id + 1);  //right
                    }
                    break;
                }
                case Direction.left:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
                            AddToPath(Direction.up, pd.id - width); //up
                    }
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
                            AddToPath(Direction.down, pd.id + width);   //down
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   //left
                    }
                    break;
                }
                case Direction.right:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
                            AddToPath(Direction.up, pd.id - width); //up
                    }
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
							AddToPath(Direction.down, pd.id + width);   //down
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
							AddToPath(Direction.right, pd.id + 1);  //right
                    }
                    break;
                }
                case Direction.none:
                {
                    if (pd.id - width >= 0) {
                        if (!p[pd.id - width].selected)
							AddToPath(Direction.up, pd.id - width); //up
                    }
                    if (pd.id + width < total) {
                        if (!p[pd.id + width].selected)
							AddToPath(Direction.down, pd.id + width);   //down
                    }
                    if (pd.id % width != 0) {
                        if (!p[pd.id - 1].selected)
                            AddToPath(Direction.left, pd.id - 1);   //left
                    }
                    if ((pd.id + 1) % width != 0) {
                        if (!p[pd.id + 1].selected)
                            AddToPath(Direction.right, pd.id + 1);  //right
                    }
                    break;
                }
            }
        }

        private void ClearPath()
        {
			for(int i = 0; i < pathSize; i++)
				p[curpath[i].id].selected = false;

            curpath.Clear();
            pathSize = 0;
        }

        public List<Trail> GetPath()
        {
            return curpath;
        }

		public ObjectBounds GetBounds()
		{
			return bounds;
		}

        private Pixel[] p;
        private readonly int width, total;
		private ObjectBounds bounds;
        private List<Trail> curpath = new List<Trail>();
        private int pathSize;
        private bool boundsError;
        private readonly string filler_warn = "::FILLER::warning : ";
    }
}