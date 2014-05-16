
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    /// <summary>Represents a collection of <see cref="CheckBoxStudio.WinForms.OrgNode"/> objects.</summary>
    public class OrgNodeCollection : IEnumerable<OrgNode> {

        private OrgNode owner = null;
        private List<OrgNode> items = new List<OrgNode>();

        internal OrgNodeCollection (OrgNode owner) {

            this.owner = owner;

        }

        IEnumerator IEnumerable.GetEnumerator () {

            return this.GetEnumerator();

        }

        /// <summary>Returns an enumerator that iterates through the node collection.</summary>
        public IEnumerator<OrgNode> GetEnumerator () {

            return items.GetEnumerator();

        }

        /// <summary>Gets the number of nodes contained in the collection.</summary>
        public int Count {

            get { return items.Count; }

        }

        /// <summary>Creates a node with a new label control using the specified text and adds it to the end of the collection.</summary>
        /// <param name="text">The text for the new label control of the node.</param>
        public void Add (string text) {

            this.Add(new OrgNode(text));

        }

        /// <summary>Creates a node with a new label control using the specified text and style name, and adds it to the end of the collection.</summary>
        /// <param name="text">The text for the new label control of the node.</param>
        /// <param name="styleName">The name of the node style.</param>
        public void Add (string text, string styleName) {

            this.Add(new OrgNode(text, styleName));

        }

        /// <summary>Creates a node with the specified control and adds it to the end of the collection.</summary>
        /// <param name="control">The control of the new node.</param>
        public void Add (Control control) {

            this.Add(new OrgNode(control));

        }

        /// <summary>Creates a node with the specified control and style name, and adds it to the end of the collection.</summary>
        /// <param name="control">The control of the new node.</param>
        /// <param name="styleName">The name of the node style.</param>
        public void Add (Control control, string styleName) {

            this.Add(new OrgNode(control, styleName));

        }

        /// <summary>Adds the specified node to the end of the collection.</summary>
        /// <param name="node">The node to add to the collection.</param>
        public void Add (OrgNode node) {

            this.Insert(this.Count, node);

        }

        /// <summary>Creates a node with a new label control using the specified text and inserts it into the collection at the specified index.</summary>
        /// <param name="index">The location within the collection to insert the node.</param>
        /// <param name="text">The text for the new label control of the node.</param>
        public void Insert (int index, string text) {

            this.Insert(index, new OrgNode(text));

        }

        /// <summary>Creates a node with a new label control using the specified text and style name, and inserts it into the collection at the specified index.</summary>
        /// <param name="index">The location within the collection to insert the node.</param>
        /// <param name="text">The text for the new label control of the node.</param>
        /// <param name="styleName">The name of the node style.</param>
        public void Insert (int index, string text, string styleName) {

            this.Insert(index, new OrgNode(text, styleName));

        }

        /// <summary>Creates a node with the specified control and inserts it into the collection at the specified index.</summary>
        /// <param name="index">The location within the collection to insert the node.</param>
        /// <param name="control">The control of the new node.</param>
        public void Insert (int index, Control control) {

            this.Insert(index, new OrgNode(control));

        }

        /// <summary>Creates a node with the specified control and style name, and inserts it into the collection at the specified index.</summary>
        /// <param name="index">The location within the collection to insert the node.</param>
        /// <param name="control">The control of the new node.</param>
        /// <param name="styleName">The name of the node style.</param>
        public void Insert (int index, Control control, string styleName) {

            this.Insert(index, new OrgNode(control, styleName));

        }

        /// <summary>Inserts the specified node into the collection at the specified index.</summary>
        /// <param name="index">The location within the collection to insert the node.</param>
        /// <param name="node">The node to insert into the collection.</param>
        public void Insert (int index, OrgNode node) {

            OrgNodeCollection.Validate(owner, node);

            node.parent = owner;
            items.Insert(index, node);

            OrgPanel panel = owner.Panel;

            if (panel != null) {

                panel.SuspendLayout();
                this.AddControls(panel, node);
                panel.ResumeLayout(true);

            }

        }

        /// <summary>Returns the index of the specified node in the collection.</summary>
        /// <param name="node">The node to locate in the collection.</param>
        public int IndexOf (OrgNode node) {

            return items.IndexOf(node);

        }

        /// <summary>Gets the node at the specified index in the collection.</summary>
        /// <param name="index">The location of the node in the collection.</param>
        public OrgNode this[int index] {

            get { return items[index]; }

        }

        /// <summary>Determines whether the collection contains the specified node.</summary>
        /// <param name="node">The node to locate in the collection.</param>
        public bool Contains (OrgNode node) {

            return items.Contains(node);

        }

        /// <summary>Finds the node associated with the specified control in the collection or any subnodes.</summary>
        /// <param name="control">The control associated with the node to find.</param>
        public OrgNode Find (Control control) {

            OrgNode node = null;

            foreach (OrgNode item in items)
                if ((node = item.Find(control)) != null)
                    return node;

            return null;

        }

        /// <summary>Finds all nodes associated with the specified controls in the collection or any subnodes.</summary>
        /// <param name="controls">The controls associated with the nodes to find.</param>
        public OrgNode[] FindAll (Control[] controls) {

            List<OrgNode> list = new List<OrgNode>();
            
            foreach (OrgNode item in items)
                list.AddRange(item.FindAll(controls));

            return list.ToArray();

        }

        /// <summary>Finds all nodes associated with the specified node style in the collection or any subnodes.</summary>
        /// <param name="styleName">The node style name associated with the nodes to find.</param>
        public OrgNode[] FindAll (string styleName) {

            List<OrgNode> list = new List<OrgNode>();

            foreach (OrgNode item in items)
                list.AddRange(item.FindAll(styleName));

            return list.ToArray();

        }

        /// <summary>Expands all nodes in the collection.</summary>
        public void ExpandAll () {

            this.ExpandAll(false);

        }

        /// <summary>Expands all nodes in the collection, and optionally all subnodes.</summary>
        /// <param name="allSubNodes">Set to true to expand all subnodes.</param>
        public void ExpandAll (bool allSubNodes) {

            OrgPanel panel = owner.Panel;

            if (panel != null)
                panel.SuspendLayout();

            foreach (OrgNode item in items)
                item.Expand(allSubNodes);

            if (panel != null) {

                panel.ResumeLayout(false);
                panel.PerformLayout(); // must call "PerformLayout" because no other event is called to initiate layout

            }

        }

        /// <summary>Collapses all nodes in the collection.</summary>
        public void CollapseAll () {

            this.CollapseAll(false);

        }

        /// <summary>Collapses all nodes in the collection, and optionally all subnodes.</summary>
        /// <param name="allSubNodes">Set to true to collapse all subnodes.</param>
        public void CollapseAll (bool allSubNodes) {

            OrgPanel panel = owner.Panel;

            if (panel != null)
                panel.SuspendLayout();

            foreach (OrgNode item in items)
                item.Collapse(allSubNodes);

            if (panel != null) {

                panel.ResumeLayout(false);
                panel.PerformLayout(); // must call "PerformLayout" because no other event is called to initiate layout

            }

        }

        /// <summary>Removes a node from the collection at the specified index.</summary>
        /// <param name="index">The location of the node in the collection.</param>
        public void RemoveAt (int index) {

            this.Remove(this[index]);

        }

        /// <summary>Removes the specified node from the collection.</summary>
        /// <param name="node">The node to remove from the collection.</param>
        public bool Remove (OrgNode node) {

            bool value = items.Remove(node);
            node.parent = null;

            OrgPanel panel = owner.Panel;

            if (panel != null) {

                panel.SuspendLayout();
                this.RemoveControls(panel, node);
                panel.ResumeLayout(true);

            }

            return value;

        }

        /// <summary>Removes all nodes from the collection.</summary>
        public void Clear () {

            OrgPanel panel = owner.Panel;

            if (panel != null)
                panel.SuspendLayout();

            while (this.Count > 0)
                this.RemoveAt(0);

            if (panel != null)
                panel.ResumeLayout(true);

        }

        /// <summary>Releases resources for all nodes in the collection.</summary>
        public void DisposeAll () {

            OrgPanel panel = owner.Panel;

            if (panel != null)
                panel.SuspendLayout();

            foreach (OrgNode item in items)
                item.Dispose();

            if (panel != null)
                panel.ResumeLayout(false);

        }

        private void AddControls (OrgPanel panel, OrgNode node) {

            panel.Controls.AddInternal(node.Control);

            foreach (OrgNode child in node.Nodes)
                node.Nodes.AddControls(panel, child);

        }

        private void RemoveControls (OrgPanel panel, OrgNode node) {

            foreach (OrgNode child in node.Nodes)
                node.Nodes.RemoveControls(panel, child);

            panel.Controls.RemoveInternal(node.Control);

        }

        /*private void DisposeNodes (OrgNode node) {
            foreach (OrgNode child in node.Nodes)
                node.Nodes.DisposeNodes(child);
            //node.FrameBrush.Dispose();
            //node.BorderPen.Dispose();
            node.Control.Dispose();
        }*/

        /*/// <summary>Removes a node from the collection at the specified index, and optionally disposes resources.</summary>
        /// <param name="index">The location of the node in the collection.</param>
        /// <param name="dispose">Determines whether to dispose resources at the specified index and subnodes.</param>
        public void RemoveAt (int index, bool dispose) {
            this.Remove(this[index], dispose);
        }*/

        /*/// <summary>Removes the specified node from the collection, and optionally disposes resources.</summary>
        /// <param name="node">The node to remove from the collection.</param>
        /// <param name="dispose">Determines whether to dispose resources in the specified node and subnodes.</param>
        public bool Remove (OrgNode node, bool dispose) {
            bool value = items.Remove(node);
            node.parent = null;
            OrgPanel panel = owner.Panel;
            if (panel != null) {
                panel.SuspendLayout();
                this.RemoveControls(panel, node);
                panel.ResumeLayout(true);
            }
            if (dispose)
                this.DisposeNodes(node);
            return value;
        }*/

        private static void Validate (OrgNode parent, OrgNode node) {

            if (node == null)
                throw new ArgumentException("Specified node cannot be null.");

            if (node.parent != null)
                throw new ArgumentException("Specified node is already a child of a parent.");

            if (node.Control.IsDisposed)
                throw new ArgumentException("Specified node control cannot be disposed.");

            if (parent.Root.Find(node.Control) != null)
                throw new ArgumentException("Specified node control already exists in the structure.");

        }

    }

}
