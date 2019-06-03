using System.Collections.Generic;

/* /Systems/bitclean/selection.cs
 * Basic static tree class used to manage binary trees.
 */

namespace bitclean
{
    /// <summary>
    /// Tree.
    /// </summary>
    public static class Tree
    {
        /// <summary>
        /// Insert the specified id into root node n.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="n">N.</param>
        /// <param name="id">Identifier.</param>
        public static bool Insert(ref Node n, int id)
        {
            if (n == null) {
                n = new Node(id);
                return true;
            }
			Node r = n;
            while (n != null)
            {
                if (id < n.id)
                {
                    if (n.left != null)
                        n = n.left;
                    else {
                        n.left = new Node(id);
                        n = r;
                        return true;
                    }
                }
                else if (id > n.id)
                {
                    if (n.right != null)
                        n = n.right;
                    else {
                        n.right = new Node(id);
                        n = r;
                        return true;
                    }
                }
                else
                    break;
            }
            n = r;
            return false;
        }

        /// <summary>
        /// Finds the id in the given root node.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="n">N.</param>
        /// <param name="id">Identifier.</param>
        public static int FindNode(Node n, int id)
        {
            if (n == null) return -1;
			Node r = n;
            while (n != null)
            {
                if (id == n.id) {
                    n = r;
                    return id;
                }
                if (id < n.id) n = n.left;
                else if (id > n.id) n = n.right;
            }
            n = r;
            return -1;
        }

        /// <summary>
        /// Builds the tree as a balanced binary tree from the list.
        /// </summary>
        /// <param name="v">V.</param>
        /// <param name="r">The red component.</param>
        public static void BuildTree(List<int> v, Node r)
        {
            List<Tup> stack = new List<Tup>();
            int m, s, e;
            m = (0 + v.Count - 1) / 2;
            Insert(ref r, v[m]);
			Tup right = new Tup(m + 1, v.Count);
			Tup left = new Tup(0, m);
            stack.Add(right);
            stack.Add(left);
            while (stack.Count > 0)
            {
                s = stack[stack.Count - 1].s;
                e = stack[stack.Count - 1].e;
                stack.RemoveAt(stack.Count - 1);
                if (s < e)
                {
                    m = (s + e) / 2;
                    Insert(ref r, v[m]);
                    right.Change(m + 1, v.Count);
                    left.Change(0, m);
                    stack.Add(right);
                    stack.Add(left);
                }
            }
        }

        /// <summary>
        /// Gets the tree nodes in order and puts them into the list.
        /// </summary>
        /// <param name="n">N.</param>
        /// <param name="v">V.</param>
        public static void GetInOrder(Node n, List<int> v)
        {
			Node r = n;
            List<Node> s = new List<Node>();
            while (n != null || s.Count > 0)
            {
                while (n != null) {
                    s.Add(n);
                    n = n.left;
                }
                n = s[s.Count - 1];
                s.RemoveAt(s.Count - 1);
                v.Add(n.id);
                n = n.right;
            }
            n = r;
        }
    }
}