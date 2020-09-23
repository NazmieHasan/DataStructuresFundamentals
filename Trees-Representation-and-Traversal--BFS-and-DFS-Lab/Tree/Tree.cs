namespace Tree
{
    using System;
    using System.Collections.Generic;

    public class Tree<T> : IAbstractTree<T>
    {
        private readonly List<Tree<T>> _children;

        public Tree(T value)
        {
            this.Value = value;
            this.Parent = null;
            this._children = new List<Tree<T>>();
        }

        public Tree(T value, params Tree<T>[] children)
            : this(value)
        {
            foreach (var child in children)
            {
                child.Parent = this;
                this._children.Add(child);
            }  
        }

        public T Value { get; private set; }
        public Tree<T> Parent { get; private set; }
        public IReadOnlyCollection<Tree<T>> Children 
            => this._children.AsReadOnly();

        public bool IsRootDeleted { get; private set; }

        public ICollection<T> OrderBfs()
        {
            var result = new List<T>();
            var queue = new Queue<Tree<T>>();

            if (this.IsRootDeleted)
            {
                return result;
            }

            queue.Enqueue(this);
            while(queue.Count > 0)
            {
                var subtree = queue.Dequeue();
                result.Add(subtree.Value);

                foreach (var child in subtree.Children)
                {
                    queue.Enqueue(child);
                }
            }
            
            return result;
        }

        public ICollection<T> OrderDfs()
        {
            var result = new List<T>();

            if (this.IsRootDeleted)
            {
                return result;
            }

            this.Dfs(this, result);
            return result;
        }

        public void AddChild(T parentKey, Tree<T> child)
        {
            var parentSubtree = this.FindBfs(parentKey);
            this.CheckEmptyNode(parentSubtree);
            
            parentSubtree._children.Add(child);
        }

        private void CheckEmptyNode(Tree<T> parentSubtree)
        {
            if (parentSubtree == null)
            {
                throw new ArgumentNullException("Searched node not found!");
            }
        }

        public void RemoveNode(T nodeKey)
        {
            var currentNode = this.FindBfs(nodeKey);
            this.CheckEmptyNode(currentNode);

            foreach (var child in currentNode.Children)
            {
                child.Parent = null;
            }

            currentNode._children.Clear();
            
            var parentNode = currentNode.Parent;

            if ( parentNode == null)
            {
                this.IsRootDeleted = true;
            }
            else
            {
                parentNode._children.Remove(currentNode);
                currentNode.Parent = null;
            }
            
            currentNode.Value = default(T);
        }

        public void Swap(T firstKey, T secondKey)
        {
            var firstNode = this.FindBfs(firstKey);
            var secondNode = this.FindBfs(secondKey);
            this.CheckEmptyNode(firstNode);
            this.CheckEmptyNode(secondNode);

            var firstParent = firstNode.Parent;
            var secondParent = secondNode.Parent;

            if (firstParent == null)
            {
                SwapRoot(secondNode);
                return;
            }

            if (secondParent == null)
            {
                SwapRoot(firstNode);
                return;
            }

            firstNode.Parent = secondParent;
            secondNode.Parent = firstParent;  
            
            int indexOfFirst = firstParent._children.IndexOf(firstNode);
            int indexOfSecond = secondParent._children.IndexOf(secondNode);

            firstParent._children[indexOfFirst] = secondNode;
            secondParent._children[indexOfSecond] = firstNode;
        }

        private void SwapRoot(Tree<T> secondNode)
        {
            this.Value = secondNode.Value;
            this._children.Clear();
            foreach (var child in secondNode.Children)
            {
                this._children.Add(child);
            }
        }

        private void Dfs(Tree<T> subtree, List<T> result)
        {
            foreach (var child in subtree.Children)
            {
                this.Dfs(child, result);
            }

            result.Add(subtree.Value);
        }
         
        private ICollection<T> OrderDfsWithStack()
        {
            var result = new Stack<T>();
            var toTraverse = new Stack<Tree<T>>();

            toTraverse.Push(this);

            while (toTraverse.Count > 0) 
            {
                var subtree = toTraverse.Pop();

                foreach (var child in subtree.Children)
                {
                    toTraverse.Push(child);
                }

                result.Push(subtree.Value);
            }

            return new List<T>(result);
        }

        private Tree<T> FindBfs(T value)
        {
            var queue = new Queue<Tree<T>>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var subtree = queue.Dequeue();

                if (subtree.Value.Equals(value))
                {
                    return subtree;
                }

                foreach (var child in subtree.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        private Tree<T> FindDfs(T value, Tree<T> subtree)
        {
            if (subtree.Value.Equals(value))
            {
                return subtree;
            }

            foreach (var child in subtree.Children)
            {
                Tree<T> current = this.FindDfs(value, child);

                if (current != null && current.Value.Equals(value))
                {
                    return current;
                }  
            }

            return null;
        }

    }
}
