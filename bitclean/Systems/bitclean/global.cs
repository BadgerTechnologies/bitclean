using System.Collections.Generic;

/* /Systems/bitclean/global.cs
 * Contains the system for calculating the number of neighbors near a given 
 object. 
 */

namespace bitclean
{
    /// <summary>
    /// Global systems.
    /// </summary>
	class GlobalSystems
	{
        // list of objects sorted by their x coordinate
        private List<ObjectData> objectsByX;
        // list of objects sorted by their y coordinate
        private List<ObjectData> objectsByY = new List<ObjectData>();

        /// <summary>
        /// Gets the neighbors.
        /// </summary>
        /// <param name="objectList">Object list.</param>
        public void GetNeighbors(List<ObjectData> objectList)
		{
			objectsByX = objectList;

			// quicksort list by x
			QuickSortByX(0, objectsByX.Count - 1);

			// iterate through list
			for(int i = 0; i < objectsByX.Count; i ++)
			{
				// check object -> left
				for (int j = i - 1; j >= 0; j--) {
					// add any intersections to y List
					if (XIntersection(objectsByX[i].rect, objectsByX[j].bounds))
						objectsByY.Add(objectsByX[j]);
					else
						break;
				}
				// check object -> right
				for (int j = i + 1; j < objectsByX.Count; j++) {
					// add any intersections to y List
					if (XIntersection(objectsByX[i].rect, objectsByX[j].bounds))
						objectsByY.Add(objectsByX[j]);
					else
						break;
				}

				// sort Y list
				QuickSortByY(0, objectsByY.Count - 1);

				// iterate through y list
				for(int k = 0; k < objectsByY.Count; k ++) {
					// check y interstections, add tag to neighbors of i
					if (YIntersection(objectsByX[i].rect, objectsByY[k].bounds))
						objectsByX[i].neighbors.Add(objectsByY[k].tag);
					else
						break;
				}
				objectsByY.Clear();
			}

		}

		#region X axis functions

        /// <summary>
        /// Swaps the two integers in the X sorted list.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
		private void SwapX(int a, int b)
		{
			ObjectData hold = objectsByX[a];
			objectsByX[a] = objectsByX[b];
			objectsByX[b] = hold;
		}

        /// <summary>
        /// Partitions the X sorted list.
        /// </summary>
        /// <returns>The by x.</returns>
        /// <param name="low">Low.</param>
        /// <param name="high">High.</param>
		private int PartitionByX(int low, int high)
		{
			int pivot = objectsByX[high].position.x;    // pivot 
			int i = low - 1;  // Index of smaller element 

            for (int j = low; j <= high - 1; j++)
			{
				// If current element is smaller than or 
				// equal to pivot 
				if (objectsByX[j].position.x <= pivot)
				{
					i++;    // increment index of smaller element 
					SwapX(i, j);
				}
			}

			SwapX(i + 1, high);

			return i + 1;
		}

        /// <summary>
        /// Quicksort in the X direction.
        /// </summary>
        /// <param name="low">Low.</param>
        /// <param name="high">High.</param>
		private void QuickSortByX(int low, int high)
		{
			if (low < high)
			{
				/* pi is partitioning index, arr[p] is now 
				   at right place */
				int pi = PartitionByX(low, high);

				// Separately sort elements before 
				// partition and after partition 
				QuickSortByX(low, pi - 1);
				QuickSortByX(pi + 1, high);
			}
		}

        /// <summary>
        /// Checks if the two bounding rectangles intersect in the x direction
        /// </summary>
        /// <returns><c>true</c>, if ntersection was XIed, <c>false</c> otherwise.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
		private bool XIntersection(BoundingRectangle a, BoundingRectangle b)
		{
			if (a.left < b.right && a.right > b.left)
				return true;
			if (b.left < a.right && b.right > a.left)
				return true;

			return false;
		}

        #endregion

        #region Y axis functions

        /// <summary>
        /// Swaps the two integers in the Y sorted list.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        private void SwapY(int a, int b)
		{
			ObjectData hold = objectsByY[a];
			objectsByY[a] = objectsByY[b];
			objectsByY[b] = hold;
		}

        /// <summary>
        /// Partitions the Y sorted list.
        /// </summary>
        /// <returns>The by y.</returns>
        /// <param name="low">Low.</param>
        /// <param name="high">High.</param>
        private int PartitionByY(int low, int high)
		{
			int pivot = objectsByY[high].position.y;    // pivot 
			int i = low - 1;  // Index of smaller element 

            for (int j = low; j <= high - 1; j++)
			{
				// If current element is smaller than or 
				// equal to pivot 
				if (objectsByY[j].position.y <= pivot)
				{
					i++;    // increment index of smaller element 
					SwapY(i, j);
				}
			}

			SwapY(i + 1, high);

			return i + 1;
		}

        /// <summary>
        /// Quicksort in the Y direction.
        /// </summary>
        /// <param name="low">Low.</param>
        /// <param name="high">High.</param>
		private void QuickSortByY(int low, int high)
		{
			if (low < high)
			{
				/* pi is partitioning index, arr[p] is now 
				   at right place */
				int pi = PartitionByY(low, high);

				// Separately sort elements before 
				// partition and after partition 
				QuickSortByY(low, pi - 1);
				QuickSortByY(pi + 1, high);
			}
		}

        /// <summary>
        /// Checks if the two bounding rectangles intersect in the Y direction
        /// </summary>
        /// <returns><c>true</c>, if ntersection was YIed, <c>false</c> otherwise.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
		private bool YIntersection(BoundingRectangle a, BoundingRectangle b)
		{
			if (a.top < b.bottom && a.bottom > b.top)
				return true;
			if (b.top < a.bottom && b.bottom > a.top)
				return true;

			return false;
		}

		#endregion
    }
}
