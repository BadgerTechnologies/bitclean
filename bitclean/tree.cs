using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * bitclean: /bitclean/tree.cs
 * author: Austin Herman
 * 2/11/2019
 * basic tree algorithms retrofitted to work with pixel id's
 */

namespace BitClean
{
    public static class Tree
    {
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