namespace CSC
{
    internal sealed class NodeStore
    {
        private readonly Dictionary<Node, List<Node>> childs = [];
        private readonly Dictionary<Node, List<Node>> parents = [];

        public void Add(Node node)
        {
            childs.TryAdd(node, []);
            parents.TryAdd(node, []);
        }

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
        }

        public void Clear()
        {
            childs.Clear();
            parents.Clear();
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
            return false;
        }

        public void AddParent(Node node, Node parent)
        {
            if (parents.TryGetValue(node, out var list))
            {
                list.Add(parent);
            }
            else
            {
                parents.Add(node, [parent]);
            }

            if (childs.TryGetValue(parent, out list))
            {
                list.Add(node);
            }
            else
            {
                childs.Add(parent, [node]);
            }
        }

        public void RemoveParent(Node node, Node parent)
        {
            if (parents.Remove(node, out var list))
            {
                list.Remove(parent);
            }
            else
            {
                parents.Add(node, []);
            }

            if (childs.Remove(node, out list))
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
                    foreach (var parent in list)
                    {
                        RemoveChild(parent, node);
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
            if (childs.TryGetValue(node, out var list))
            {
                list.Add(child);
            }
            else
            {
                childs.Add(node, [child]);
            }

            if (parents.TryGetValue(node, out list))
            {
                list.Add(node);
            }
            else
            {
                parents.Add(child, [node]);
            }
        }

        public void RemoveChild(Node node, Node child)
        {
            if (childs.Remove(node, out var list))
            {
                list.Remove(child);
            }
            else
            {
                childs.Add(node, []);
            }

            if (parents.Remove(node, out list))
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
                    foreach (var child in list)
                    {
                        RemoveParent(child, node);
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
            if (parents.TryGetValue(node, out var list))
            {
                list.AddRange(parents_);
            }
            else
            {
                parents.Add(node, [.. parents_]);
            }
            if (parents_.Any())
            {
                foreach (var parent in parents_)
                {
                    childs[parent].Add(node);
                }
            }
        }

        public void AddChilds(Node node, IEnumerable<Node> childs_)
        {
            if (childs.TryGetValue(node, out var list))
            {
                list.AddRange(childs_);
            }
            else
            {
                childs.Add(node, [.. childs_]);
            }
            if (childs_.Any())
            {
                foreach (var child in childs_)
                {
                    parents[child].Add(node);
                }
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

        public Dictionary<Node, List<Node>>.KeyCollection Keys()
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
    }

    internal record struct Family(List<Node> Childs, List<Node> Parents)
    {
        public static implicit operator (List<Node> childs, List<Node> parents)(Family value) => (value.Childs, value.Parents);
        public static implicit operator Family((List<Node> childs, List<Node> parents) value) => new(value.childs, value.parents);
    }
}
