
namespace CSC.Nodestuff
{
    public sealed class NodeStore
    {
        public readonly NodePositionSorting Positions = new();
        private readonly Dictionary<Node, List<Node>> childs = [];
        private readonly Dictionary<Node, List<Node>> parents = [];

        public void Add(Node node)
        {
            childs.TryAdd(node, []);
            parents.TryAdd(node, []);
        }

        public int Count => childs.Count;

        public List<Node> Nodes => [.. childs.Keys];

        public void Add(Node node, IEnumerable<Node> childs_)
        {
            childs.TryAdd(node, [.. childs_]);
            parents.TryAdd(node, []);
            if (childs_.Any())
            {
                foreach (var child in childs_)
                {
                    parents[child].Add(node);
                }
            }
        }

        public void Add(Node node, IEnumerable<Node> parents_, object _)
        {
            childs.TryAdd(node, []);
            parents.TryAdd(node, [.. parents_]);
            if (parents_.Any())
            {
                foreach (var parent in parents_)
                {
                    childs[parent].Add(node);
                }
            }
        }

        public void Add(Node node, IEnumerable<Node> childs_, IEnumerable<Node> parents_)
        {
            Add(node, [.. childs_]);
            Add(node, [.. parents_], new object());
        }

        public void Remove(Node node)
        {
            childs.Remove(node);
            parents.Remove(node);
            Positions.ClearNode(node);

            foreach (var childs in childs.Values)
            {
                childs.Remove(node);
            }

            foreach (var parents in parents.Values)
            {
                parents.Remove(node);
            }
        }

        public void Clear()
        {
            childs.Clear();
            parents.Clear();
            Positions.Clear();
        }

        public bool Contains(Node node)
        {
            bool contains = ContainsKey(node);
            if (contains)
            {
                return true;
            }
            else
            {
                contains = ContainsValue(node);
            }
            return contains;
        }

        public bool ContainsKey(Node node)
        {

            bool contains = childs.ContainsKey(node);
            return contains;
        }

        public bool ContainsValue(Node node)
        {
            foreach (var child in childs.Values)
            {
                if (child.Contains(node))
                {
                    return true;
                }
            }
            foreach (var parent in parents.Values)
            {
                if (parent.Contains(node))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddParent(Node node, Node parent)
        {
            Add(parent);
            if (parents.TryGetValue(node, out _))
            {
                if (!parents[node].Contains(parent))
                {
                    parents[node].Add(parent);
                }
            }
            else
            {
                parents.Add(node, [parent]);
            }

            if (childs.TryGetValue(parent, out _))
            {
                if (!childs[parent].Contains(node))
                {
                    childs[parent].Add(node);
                }
            }
            else
            {
                childs.Add(parent, [node]);
            }
        }

        public void RemoveParent(Node node, Node parent)
        {
            if (parents.TryGetValue(node, out var list))
            {
                list.Remove(parent);
            }
            else
            {
                parents.Add(node, []);
            }

            if (childs.TryGetValue(parent, out list))
            {
                list.Remove(node);
            }
            else
            {
                childs.Add(parent, []);
            }
        }

        public void ClearParents(Node node)
        {
            if (parents.TryGetValue(node, out var list))
            {
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        RemoveChild(list[i], node);
                    }
                }
                list.Clear();
            }
            else
            {
                parents.Add(node, []);
            }
        }

        public void AddChild(Node node, Node child)
        {
            Add(child);
            if (childs.TryGetValue(node, out _))
            {
                if (!childs[node].Contains(child))
                {
                    childs[node].Add(child);
                }
            }
            else
            {
                childs.Add(node, [child]);
            }

            if (parents.TryGetValue(child, out _))
            {
                if (!parents[child].Contains(node))
                {
                    parents[child].Add(node);
                }
            }
            else
            {
                parents.Add(child, [node]);
            }
        }

        public void RemoveChild(Node node, Node child)
        {
            if (childs.TryGetValue(node, out var list))
            {
                list.Remove(child);
            }
            else
            {
                childs.Add(node, []);
            }

            if (parents.TryGetValue(child, out list))
            {
                list.Remove(node);
            }
            else
            {
                parents.Add(child, []);
            }
        }

        public void ClearChilds(Node node)
        {
            if (childs.TryGetValue(node, out var list))
            {
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        RemoveParent(list[i], node);
                    }
                }
                list.Clear();
            }
            else
            {
                childs.Add(node, []);
            }
        }

        public void AddParents(Node node, IEnumerable<Node> parents_)
        {
            foreach (var item in parents_)
            {
                AddParent(node, item);
            }
        }

        public void AddChilds(Node node, IEnumerable<Node> childs_)
        {
            foreach (var item in childs_)
            {
                AddChild(node, item);
            }
        }

        public List<Node> Childs(Node node)
        {
            if (childs.TryGetValue(node, out var list))
            {
                return list;
            }
            else
            {
                childs.Add(node, []);
                return childs[node];
            }
        }

        public List<Node> Parents(Node node)
        {
            if (parents.TryGetValue(node, out var list))
            {
                return list;
            }
            else
            {
                parents.Add(node, []);
                return parents[node];
            }
        }

        public Dictionary<Node, List<Node>>.KeyCollection KeyNodes()
        {
            return childs.Keys;
        }

        public Family this[Node node]
        {
            get
            {
                return new Family(Childs(node), Parents(node));
            }
        }

        public void Replace(Node node, Node replacement)
        {
            Add(replacement);
            List<Node> childs = [.. Childs(node)];
            List<Node> parents = [.. Parents(node)];

            ClearChilds(node);
            ClearParents(node);

            AddChilds(replacement, childs);
            AddParents(replacement, parents);

            Remove(node);
            Main.ClearNodePos(node, node.FileName);
            Main.ClearNodePos(node, replacement.FileName);
        }

        internal bool AreConnected(Node node1, Node node2)
        {
            if (Childs(node1).Contains(node2))
            {
                return true;
            }
            if (Parents(node1).Contains(node2))
            {
                return true;
            }
            if (Childs(node2).Contains(node1))
            {
                return true;
            }
            if (Parents(node2).Contains(node1))
            {
                return true;
            }
            return false;
        }
    }

    public record struct Family(List<Node> Childs, List<Node> Parents)
    {
        public static implicit operator (List<Node> childs, List<Node> parents)(Family value) => (value.Childs, value.Parents);
        public static implicit operator Family((List<Node> childs, List<Node> parents) value) => new(value.childs, value.parents);
    }
}
