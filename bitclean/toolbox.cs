using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

/*
 * bitclean: /bitclean/toolbox.cs
 * author: Austin Herman
 * 2/11/2019
 * drives functions for finding an object, calculating it's properties, and coloring the pixels
 */

namespace BitClean
{
    public class Toolbox
    {
		private Pixel[] pixels = null; // all pixels

		private readonly Data imgdata; // basic image data

		private List<int> buffer = new List<int>();		// buffer of pixel id's
		private List<int> perimeter = new List<int>();	// buffer of perimeter id's from the object
		private List<ObjectData> objdat = new List<ObjectData>();	// list of object data scraped from each object found

		public Toolbox(Pixel[] p, Data dat)
        {
            pixels = p;
			imgdata = dat;
        }

        // the big boy, iterates through the pixels and drives algorithms
        public void Run(ToolStripProgressBar progress, StatusStrip statusStrip, ToolStripStatusLabel toolStripText)
        {
            if (pixels == null) {
                MessageBox.Show("No Pixels Loaded", "no pixels", 0);
                return;
            }

			// create a selection object
            Selection s = new Selection(pixels, imgdata.width, imgdata.totalpixels);
			int tagCount = 0;

			// set progress bar
			progress.Minimum = 0;
			progress.Maximum = imgdata.totalpixels;

			// for each pixel in the image
            for (int i = 0; i < imgdata.totalpixels; i++)
            {
				// update progress
				progress.Value = i;
				if (s.Get(i)) // get the next object
                {
					// set the buffer/perimeter lists
                    buffer = s.Buffer;
                    perimeter = s.Perimeter;

					// using the buffer, get the object's properties
					ObjectData objectdata = GenerateObjectData(tagCount, s.Edges, s.getBounds());

					// increment tag number
					tagCount++;

					// initialize neighbor and confidence members
					objectdata.neighbors = new List<int>();
					Confidence c = new Confidence();

					// ::FOR TESTING::
					if (FindObjectTag()) {
						c.decision = "object";
						ColorBuffer(Color.Green);
					}
					else {
						c.decision = "dust";
						ColorBuffer(Color.Purple);
					}
						
					// give confidence to object's data
					objectdata.objconf = c;

					// add data to all object data vector
					objdat.Add(objectdata);
                }

				// clear buffers for next object
                s.ClearBuffer();
                buffer.Clear();
            }

			// update status
			toolStripText.Text = ToolStripMessages.CONFIDENCE_LOADING;
			statusStrip.Refresh();

			// start global property analysis
			GlobalSystems global = new GlobalSystems();

			// find neighbors for each object
			global.GetNeighbors(objdat, progress);

			// calculate confidence
			// not finished may delete later idk
        }

		//sets the buffer to be colored with color 'c'
		private void ColorBuffer(Color c)
		{
			for (int i = 0; i < buffer.Count; i++)
			{
				pixels[buffer[i]].value = ImageOperations.ColToInt(c);
				pixels[buffer[i]].r = c.R;
				pixels[buffer[i]].g = c.G;
				pixels[buffer[i]].b = c.B;
			}
		}

		//sets the edges to be colored with color 'c'
		private void ColorEdges(Color c)
        {
			for (int i = 0; i < perimeter.Count; i++)
			{
				pixels[perimeter[i]].r = c.R;
				pixels[perimeter[i]].g = c.G;
				pixels[perimeter[i]].b = c.B;
			}
		}

		// calculates average hue or color of the buffer
		private double GetAverageHue()
		{
			double avg = 0;
			for (int i = 0; i < buffer.Count; i++) {
			if (pixels[buffer[i]].value != Constants.INT_WHITE && pixels[buffer[i]].value != Constants.INT_OBJ_TAG)
				avg += pixels[buffer[i]].value;
			}
			return avg / buffer.Count;
        }

		// returns the percentage of non-white pixels in the buffer
		private double GetValueDensity()
		{
			double avg = 0;
			for (int i = 0; i < buffer.Count; i++) {
				if (pixels[buffer[i]].value != Constants.INT_WHITE)
					avg++;
			}
			return avg / buffer.Count * 100;
		}

		private ObjectData GenerateObjectData(int tagCount, int edgeCount, BoundingRectangle objBounds)
		{
			// create object data with buffer's properties
			ObjectData objdata = new ObjectData
			{
				tag		= tagCount,
				avghue	= GetAverageHue(),
				density = GetValueDensity(),
				size	= buffer.Count,
				edgeratio = buffer.Count / edgeCount,
				bounds	= objBounds,
				position = new Coordinate {
					x = buffer[0] % imgdata.width,
					y = buffer[0] / imgdata.width
				}
			};

			// make a bounding rectangle
			BoundingRectangle boundingRect = new BoundingRectangle();

			// give bounds -- do not let bounding rect go beyonds image bounds
			boundingRect.top	= ((objBounds.top - Constants.BOUNDING_RECT_OFFSET)	 < 0)	? 0 : objBounds.top		- Constants.BOUNDING_RECT_OFFSET;
			boundingRect.left	= ((objBounds.left - Constants.BOUNDING_RECT_OFFSET) < 0)	? 0 : objBounds.left	- Constants.BOUNDING_RECT_OFFSET;
			boundingRect.bottom = ((objBounds.bottom + Constants.BOUNDING_RECT_OFFSET)	> imgdata.height)	? imgdata.height : objBounds.bottom	+ Constants.BOUNDING_RECT_OFFSET;
			boundingRect.right	= ((objBounds.right + Constants.BOUNDING_RECT_OFFSET)	> imgdata.width)	? imgdata.width	 : objBounds.right	+ Constants.BOUNDING_RECT_OFFSET;

			boundingRect.width	= boundingRect.right - boundingRect.left;
			boundingRect.height = boundingRect.bottom - boundingRect.top;

			objdata.rect = boundingRect;

			return objdata;
		}

		bool FindObjectTag()
		{
			for(int i = 0; i < buffer.Count; i++) {
				if (pixels[buffer[i]].value == Constants.INT_OBJ_TAG)
					return true;
			}

			return false;
		}

		public List<ObjectData> GetObjectData()
        {
            return objdat;
        }
    }
}
