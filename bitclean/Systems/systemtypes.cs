using System.Collections.Generic;
using System.Drawing;

/* /Systems/systemtypes.cs
 * Holds types used by the classes found in the /Systems/ directory
 */

namespace bitclean
{
    /// <summary>
    /// Mix of generic constants.
    /// </summary>
	static class Constants
	{
		public const short VALUE_THRESHOLD = 0;				// a useful threshold for pixel values (white)
		public const int MAX_OBJECT_SIZE_ESTIMATE = 10000;	// if an object is bigger than this ignore it -- optimization thing
		public const int COLOR_CLEAR = 0;	// color to clear selections with
		public const int BRUSH_SIZE = 16;	// brush size for trajectory path
		public static readonly Color FLOOR = Color.FromArgb(255, 0, 255);   // this is magenta - what the floor looks like from cloud compare
		public static readonly Color WHITE = Color.FromArgb(255, 255, 255); // white color - what we want to change the floor to
		public static readonly Color OBJ_TAG = Color.FromArgb(0, 0, 0);
		public const short INT_WHITE = 0;   // white color can be represented as 0 because color values run [1->1021]
											//  - this is helpful for average hue and average density functions
		public const short INT_OBJ_TAG = 1300;
		public const int BOUNDING_RECT_OFFSET = 300;    // offset for the bounding rectangle used to find neighbors
        public const int TOLERANCE = 4;
	}

    /// <summary>
    /// the image file's basic data
    /// </summary>
	public struct Data
    {
        public int width, height;	// counted from 1
        public int totalpixels;		// counted from 1
    };

    /// <summary>
    /// a basic pixel structure
    /// </summary>
    public struct Pixel
    {
        public bool selected;   //used for selection
        public short value;     //converted color integer value
		public byte r, g, b;
        public int id;          //ID [0->totalpixels - 1]
    };

    /// <summary>
    /// Coordinate structure
    /// </summary>
	public struct Coordinate
	{
		public int x, y;
	}

    /// <summary>
    /// Octan is a made up word. It's used for storing the eight neighboring 
    /// pixels surrounding a single pixel.
    /// </summary>
    class Octan
    {
        public int tl = -1, t = -1, tr = -1,
                   l = -1,          r = -1,
                   bl = -1, b = -1, br = -1;

        public Octan()
        {
            tl = -1; t = -1; tr = -1;
            l = -1;          r = -1;
            bl = -1; b = -1; br = -1;
        }
    };

    /// <summary>
    /// Edge and filler use this for navigation around the pixel map.
    /// </summary>
    enum Direction
    {
        none, up, down, left, right
    };

    /// <summary>
    /// Trail, very similar to a basic vector in physics but has a starting 
    /// location instead of a magnitude.
    /// </summary>
    class Trail
    {
        public Direction dir = Direction.none;
        public int id = -1;
        public Trail(Direction d, int i) { dir = d; id = i; }
    };

    /// <summary>
    /// Object bounds.
    /// </summary>
	public struct ObjectBounds
	{
		public int top, left, bottom, right;
	}
	
    /// <summary>
    /// Bounding rectangle.
    /// </summary>
	public struct BoundingRectangle
	{
		public int top, left, bottom, right,
					width, height;
	}

    /// <summary>
    /// Confidence.
    /// </summary>
	public class Confidence
	{
		public double structure, dust,
						s_size, d_size,
                        s_edge, d_edge,
                        s_val, d_val;
        public bool isStructure;
        public string decision = "";
	};

    /// <summary>
    /// Object data.
    /// </summary>
	public class ObjectData
    {
		public double avghue;		// average integer color value of all pixels in selection
		public double density;		// percentage of non-floor colored pixels occupying the selection
		public int size;			// total size in pixels of selection
		public double edgeratio;    // ratio of total size to calculated edges
		public int tag;             // unique id associated with object
		public Coordinate position;     // x,y coordinate of top-left most pixel
		public BoundingRectangle bounds;        // integer coordinate of top,left,bottom,right most pixels
		public BoundingRectangle rect;	// bounding rectangle for checking neighbors (w + 300, h + 300)
		public List<int> neighbors; // list of tags of other objects in the vicinity of this one
        public Confidence objconf;	// confidence property
    }

    /// <summary>
    /// Basic node for a binary tree.
    /// </summary>
	public class Node
    {
        public Node left, right;
        public int id;
        public Node() { left = null; right = null; id = -1; }
        public Node(int i) { left = null; right = null; id = i; }
    };

    /// <summary>
    /// Tup is most certainly a made up word. It simply stores two integers in 
    /// one structure
    /// </summary>
    class Tup
    {
        public int s, e;
        public Tup(int st, int en) { s = st; e = en; }
        public void Change(int st, int en) { s = st; e = en; }
    };

    /// <summary>
    /// Another way to represent the eight neighbors surrounding a pixel.
    /// </summary>
    public enum Field
    {
        tl, t, tr,
        l,      r,
        bl, b, br
    };

    /// <summary>
    /// Field vectors for assigning tolerances.
    /// </summary>
    public static class FieldVector
    {
        // moving up or down
        public static readonly int[] verticalfield =
        {
            -1, 0, 1,
            -2,    2,
            -1, 0, 1
        };

        // moving left or right
        public static readonly int[] horizontalfield =
        {
            -1, -2, -1,
             0,      0,
             1,  2,  1
        };

        // moving up-right or down-left
        public static readonly int[] leftslantfield =
        {
            0, -1, -2,
            1,     -1,
            2,  1,  0
        };

        // moving up-left or down-right
        public static readonly int[] rightslantfield =
        {
            -2, -1, 0,
            -1,     1,
             0,  1, 2
        };
    }

}