
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    /// <summary>Represents a node of a <see cref="CheckBoxStudio.WinForms.OrgPanel"/>.</summary>
    public class OrgNode : IDisposable {

        internal OrgNode parent = null;

        private OrgPanel panel = null;
        private Control control = null;
        private OrgNodeCollection nodes = null;
        private static OrgNodeStyle defaultStyle = new OrgNodeStyle();
        private string styleName = "";
        private int parentOffset = 0;
        private LinearGradientBrush frameBrush = null;
        private bool expanded = true;

        internal OrgNode (OrgPanel panel) : this((Control)panel) {

            this.panel = panel;

        }

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgNode"/> class with a new label control using the specified text.</summary>
        /// <param name="text">The text for the new label control of the node.</param>
        public OrgNode (string text) : this (text, "") {



        }

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgNode"/> class with a new label control using the specified text and style name.</summary>
        /// <param name="text">The text for the new label control of the node.</param>
        /// <param name="styleName">The name of the node style.</param>
        public OrgNode (string text, string styleName) : this (new Label(), styleName) {

            control.AutoSize = true;
            control.Text = text;

        }

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgNode"/> class with the specified control.</summary>
        /// <param name="control">The control of the new node.</param>
        public OrgNode (Control control) : this (control, "") {
 


        }

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgNode"/> class with the specified control and style name.</summary>
        /// <param name="control">The control of the new node.</param>
        /// <param name="styleName">The name of the node style.</param>
        public OrgNode (Control control, string styleName) {

            OrgNode.Validate(control);

            this.control = control;
            this.StyleName = styleName;

            nodes = new OrgNodeCollection(this);

        }

        internal OrgNode Root {

            get {

                if (parent == null)
                    return this;

                return parent.Root;

            }

        }

        /// <summary>Gets the parent node of the current node.</summary>
        public OrgNode Parent {

            get {

                OrgPanel panel = this.Panel;

                if (panel != null && parent == panel.Root) // do not return virtual root in panel
                    return null;

                return parent;

            }

        }

        /// <summary>Gets the panel that the current node is assigned to.</summary>
        public OrgPanel Panel {

            get { return this.Root.panel; }

        }

        /// <summary>Gets the control of the current node.</summary>
        public Control Control {

            get { return control; }

        }

        /// <summary>Gets the collection of child nodes of the current node.</summary>
        public OrgNodeCollection Nodes {

            get { return nodes; }

        }

        /// <summary>Gets the previous sibling node of the current node.</summary>
        public OrgNode PrevSibling {

            get {

                if (parent == null || this.Index == 0)
                    return null;

                return parent.Nodes[this.Index - 1];

            }

        }

        /// <summary>Gets the next sibling node of the current node.</summary>
        public OrgNode NextSibling {

            get {

                if (parent == null || this.Index == parent.Nodes.Count - 1)
                    return null;

                return parent.Nodes[this.Index + 1];

            }

        }

        /// <summary>Gets the first child node of the current node.</summary>
        public OrgNode FirstChild {

            get {

                if (nodes.Count == 0)
                    return null;

                return nodes[0];

            }

        }

        /// <summary>Gets the last child node of the current node.</summary>
        public OrgNode LastChild {

            get {

                if (nodes.Count == 0)
                    return null;

                return nodes[nodes.Count - 1];

            }

        }

        /// <summary>Gets the level of the current node relative to the root.</summary>
        public int Level {

            get {

                if (parent == null)
                    return 0;

                return 1 + parent.Level;

            }

        }

        /// <summary>Gets the zero-based index of the current node relative to other sibling nodes.</summary>
        public int Index {

            get {

                if (parent == null)
                    return 0;

                return parent.Nodes.IndexOf(this);

            }

        }

        /// <summary>Gets a value indicating whether the current node is collapsed by any parent node.</summary>
        public bool IsCollapsed {

            get {

                if (parent == null)
                    return false;

                if (!parent.Expanded)
                    return true;

                return parent.IsCollapsed;

            }

        }

        /// <summary>Gets the style of the current node.</summary>
        public OrgNodeStyle Style {

            get {

                OrgPanel panel = this.Panel;

                if (panel == null)
                    return defaultStyle;

                OrgNodeStyle style = panel.NodeStyles[styleName];

                if (style == null)
                    return defaultStyle;

                return style;

            }

        }

        /// <summary>Gets or sets the style name of the current node.</summary>
        public string StyleName {

            get { return styleName; }
            set {

                value = value == null ? "" : value.Trim();

                if (value == styleName)
                    return;

                styleName = value;

                OrgPanel panel = this.Panel;

                if (panel != null)
                    panel.PerformLayout();

            }

        }

        /// <summary>Gets or sets the offset position of the current node from the parent node.</summary>
        public int ParentOffset {

            get { return parentOffset; }
            set {

                if (value < 0)
                    value = 0;

                if (value == parentOffset)
                    return;

                parentOffset = value;

                OrgPanel panel = this.Panel;

                if (panel != null)
                    panel.PerformLayout();

            }

        }

        /// <summary>Gets or sets a value indicating whether the current node is expanded.</summary>
        public bool Expanded {

            get { return expanded; }
            set {

                if (value)
                    this.Expand();
                else
                    this.Collapse();

            }

        }

        /// <summary>Expands the current node.</summary>
        public void Expand () {

            this.Expand(false);

        }

        /// <summary>Expands the current node, and optionally all subnodes.</summary>
        /// <param name="allSubNodes">Set to true to expand all subnodes.</param>
        public void Expand (bool allSubNodes) {

            if (expanded && !allSubNodes)
                return;

            OrgPanel panel = this.Panel;

            if (panel != null) {

                OrgPanelCancelEventArgs e = new OrgPanelCancelEventArgs(this, false);

                panel.OnBeforeExpand(e);

                if (e.Cancel)
                    return;

                panel.SuspendLayout();

            }

            expanded = true;

            if (allSubNodes)
                nodes.ExpandAll(true);

            if (panel != null) {

                panel.OnAfterExpand(new OrgPanelEventArgs(this));

                panel.ResumeLayout(false);
                panel.PerformLayout(); // must call "PerformLayout" because no other event is called to initiate layout

            }

        }

        /// <summary>Collapses the current node.</summary>
        public void Collapse () {

            this.Collapse(false);

        }

        /// <summary>Collapses the current node, and optionally all subnodes.</summary>
        /// <param name="allSubNodes">Set to true to collapse all subnodes.</param>
        public void Collapse (bool allSubNodes) {

            if (!expanded && !allSubNodes)
                return;

            OrgPanel panel = this.Panel;

            if (panel != null) {

                OrgPanelCancelEventArgs e = new OrgPanelCancelEventArgs(this, false);

                panel.OnBeforeCollapse(e);

                if (e.Cancel)
                    return;

                panel.SuspendLayout();

            }

            expanded = false;

            if (allSubNodes)
                nodes.CollapseAll(true);

            if (panel != null) {

                panel.OnAfterCollapse(new OrgPanelEventArgs(this));

                panel.ResumeLayout(false);
                panel.PerformLayout(); // must call "PerformLayout" because no other event is called to initiate layout

            }

        }

        /// <summary>Toggles the current node by expanding or collapsing.</summary>
        public void Toggle () {

            this.Expanded = !this.Expanded;

        }

        /// <summary>Returns the current node, or subnode, associated with the specified control.</summary>
        /// <param name="control">The specified control to find.</param>
        public OrgNode Find (Control control) {

            if (this.control == control)
                return this;

            return nodes.Find(control);

        }

        /// <summary>Returns the current node, and any subnodes, associated with the specified controls.</summary>
        /// <param name="controls">The specified controls to find.</param>
        public OrgNode[] FindAll (Control[] controls) {

            List<OrgNode> list = new List<OrgNode>();

            list.AddRange(nodes.FindAll(controls));
            
            foreach (Control ctrl in controls)
                if (this.control == ctrl)
                    list.Add(this);

            return list.ToArray();

        }

        /// <summary>Returns the current node, and any subnodes, associated with the specified node style.</summary>
        /// <param name="styleName">The specified node style name to find.</param>
        public OrgNode[] FindAll (string styleName) {

            List<OrgNode> list = new List<OrgNode>();

            list.AddRange(nodes.FindAll(styleName));

            if (this.styleName == styleName)
                list.Add(this);

            return list.ToArray();

        }

        /// <summary>Returns all controls for the current node and subnodes.</summary>
        public Control[] GetAllControls () {
            
            List<Control> list = new List<Control>();

            foreach (OrgNode node in nodes)
                list.AddRange(node.GetAllControls());

            list.Add(control);

            return list.ToArray();

        }

        /// <summary>Moves all child nodes up to the parent node of the current node.</summary>
        public void AscendNodes () {

            if (parent == null)
                return;

            OrgPanel panel = this.Panel;

            if (panel != null)
                panel.SuspendLayout();

            OrgNode node = null;

            while ((node = this.LastChild) != null) {

                nodes.Remove(node);
                parent.Nodes.Insert(this.Index + 1, node);

            }

            if (panel != null)
                panel.ResumeLayout(true);

        }

        /// <summary>Removes the current node, and all subnodes, from the panel.</summary>
        public void Remove () {

            this.Remove(false);

        }

        /// <summary>Removes the current node from the panel, and optionally ascends child nodes.</summary>
        /// <param name="ascendNodes">Determines whether to ascend the child nodes.</param>
        public void Remove (bool ascendNodes) {

            if (parent == null)
                return;

            OrgPanel panel = this.Panel;

            if (panel != null)
                panel.SuspendLayout();

            if (ascendNodes)
                this.AscendNodes();

            parent.Nodes.Remove(this);

            if (panel != null)
                panel.ResumeLayout(true);

        }

        /// <summary>Releases all resources used by the current node and all subnodes.</summary>
        public void Dispose () {

            this.Dispose(true);

        }

        /// <summary>Releases all resources used by the current node and all subnodes.</summary>
        /// <param name="disposing">Determines whether to release resources.</param>
        protected virtual void Dispose (bool disposing) {

            if (!disposing)
                return;

            if (nodes != null)
                nodes.DisposeAll();

            OrgPanel panel = this.Panel;

            if (panel != null)
                panel.SuspendLayout();

            this.Remove(); // remove if not already removed

            if (control != null)
                control.Dispose();

            if (frameBrush != null)
                frameBrush.Dispose();

            if (panel != null)
                panel.ResumeLayout(true);

        }

        /// <summary>Returns a <see cref="System.String"/> representing the associated control of the node.</summary>
        public override string ToString () {

            return string.Format("Control = \"{0}\"", this.Control);

        }

        private static void Validate (Control control) {

            if (control == null)
                throw new ArgumentException("Specified control cannot be null.");

            if (control.IsDisposed)
                throw new ArgumentException("Specified control cannot be disposed.");

            if (control is Form)
                throw new ArgumentException("Form control cannot be associated with a node.");

            if (control.Dock != DockStyle.None)
                throw new ArgumentException("Docked control cannot be associated with a node.");

            //if (ctrl.TopLevelControl == ctrl)
            //    throw new ArgumentException("Top-level control cannot be added as a node.");

        }

        /*/// <summary>Gets the frame brush of the current node.</summary>
        public LinearGradientBrush FrameBrush {
            get {
                if (frameBrush == null) {
                    Rectangle rect = OrgPanel.GetFrameRectangle(control);
                    OrgNodeStyle style = this.Style;
                    frameBrush = new LinearGradientBrush(rect, style.FrameStartColor, style.FrameEndColor, style.GradientMode);
                }
                return frameBrush;
            }
            internal set { frameBrush = value; } // this should be set only during layout
        }*/

        /*/// <summary>Removes the current node from the panel, and optionally ascends child nodes and disposes resources.</summary>
        /// <param name="ascendNodes">Determines whether to ascend the child nodes.</param>
        /// <param name="dispose">Determines whether to dispose resources in the current node and subnodes.</param>
        public void Remove (bool ascendNodes, bool dispose) {
            if (parent == null)
                return;
            OrgPanel panel = this.Panel;
            if (panel != null)
                panel.SuspendLayout();
            if (ascendNodes)
                this.AscendNodes();
            parent.Nodes.Remove(this, dispose);
            if (panel != null)
                panel.ResumeLayout(true);
        }*/

        /*internal LinearGradientBrush FrameBrush {
            get { return frameBrush; }
        }*/

        /*internal Pen BorderPen {
            get { return borderPen; }
        }*/

        /*internal bool HasDefaultStyles {
            get {
                return
                    linkColorStyle == LinkColorStyle.Foreground &&
                    cornerStyle == CornerStyle.Rounded &&
                    frameBrush.Color == SystemColors.Control &&
                    borderPen.Color == SystemColors.ControlDarkDark &&
                    shadowVisible == true;
            }
        }*/

        /*private void Invalidate () {
            OrgPanel panel = this.Panel;
            if (panel == null)
                return;
            panel.Invalidate();
        }*/

        /*/// <summary>Gets or sets the color style of the link from the current node to the parent node.</summary>
        internal LinkColorStyle LinkColorStyle {
            get { return style == null ? LinkColorStyle.Foreground : style.LinkStyle; }
            /*set {
                if (value == linkColorStyle)
                    return;
                linkColorStyle = value;
                this.Invalidate();
            }*/

        //}*/
        /*/// <summary>Gets or sets the frame corner styles of the margin around the node control.</summary>
        public CornerStyle CornerStyle {
            get {
                if (style == null)
                    return CornerStyle.Rounded;
                return style.CornerStyle;
            }*/
        /*set {
            if (value == cornerStyle)
                return;
            cornerStyle = value;
            this.Invalidate();
        }*/
        //}
        //// <summary>Gets or sets the border color of the margin around the node control.</summary>
        //public Color BorderColor {
        //    get { return borderPen.Color; }
        /*set {
            if (value == borderPen.Color)
                return;
            borderPen.Color = value == Color.Empty ? SystemColors.ControlDarkDark : value;
            this.Invalidate();
        }*/
        //}
        /*/// <summary>Gets or sets the frame color of the margin around the node control.</summary>
        public Color FrameColor {
            get { return frameBrush.Color; }
            set {
                if (value == frameBrush.Color || value == Color.Transparent)
                    return;
                frameBrush.Color = value == Color.Empty ? SystemColors.Control : value;
                this.Invalidate();
            }
        }
        /// <summary>Gets or sets a value indicating whether the shadow is displayed for the current node.</summary>
        public bool ShadowVisible {
            get { return shadowVisible; }
            set {
                if (value == shadowVisible)
                    return;
                shadowVisible = value;
                this.Invalidate();
            }
        }*/

        /*private void PerformLayout () {
            OrgPanel panel = this.Panel;
            if (panel == null)
                return;
            panel.PerformLayout();
        }*/

        /*internal static void ValidateMove (OrgNode node, OrgNodeCollection nodes) {
            if (node == null)
                throw new ArgumentException("Specified node cannot be null.");
            if (nodes == null)
                throw new ArgumentException("Specified node collection cannot be null.");
            if (node.Find(nodes.Owner.Control) != null)
                throw new ArgumentException("Specified node collection is already contained within the current node.");
        }
        internal static void ValidateMove (OrgNodeCollection nodes, int index) {
            if (nodes == null)
                throw new ArgumentException("Specified node collection cannot be null.");
            if (index < 0 || index >= nodes.Count)
                throw new ArgumentOutOfRangeException();
        }*/
        /*/// <summary>Moves the current node to the end of the specified node collection.</summary>
        /// <param name="nodes">The specified node collection to move to.</param>
        public void MoveTo (OrgNodeCollection nodes) {
            if (parent == null || parent.Nodes == nodes)
                return;
            OrgNode.ValidateMove(this, nodes);
            OrgPanel panel = this.Panel;
            if (panel != null)
                panel.SuspendLayout();
            parent.Nodes.Remove(this, false);
            nodes.Add(this);
            if (panel != null)
                panel.ResumeLayout(true);
        }
        /// <summary>Moves the current node to the specified index.</summary>
        /// <param name="index">The specified index to move to.</param>
        public void MoveTo (int index) {
            if (parent == null || this.Index == index)
                return;
            OrgNodeCollection nodes = parent.Nodes;
            OrgNode.ValidateMove(nodes, index);
            OrgPanel panel = this.Panel;
            if (panel != null)
                panel.SuspendLayout();
            nodes.Remove(this, false);
            nodes.Insert(index, this);
            if (panel != null)
                panel.ResumeLayout(true);
        }
        /// <summary>Moves the current node to the specified node collection at the specified index.</summary>
        /// <param name="nodes">The specified node collection to move to.</param>
        /// <param name="index">The specified index to move to.</param>
        public void MoveTo (OrgNodeCollection nodes, int index) {
            OrgPanel panel = this.Panel;
            MessageBox.Show(index.ToString());
            if (panel != null)
                panel.SuspendLayout();
            this.MoveTo(nodes);
            this.MoveTo(index);
            if (panel != null)
                panel.ResumeLayout(true);
        }*/

        /*internal static void ValidateMove (OrgNode node, OrgNodeCollection nodes, int index) {
            if (node == null || nodes == null)
                throw new ArgumentNullException();
            if (node.Parent == null)
                throw new ArgumentException("Specified node has no parent.");
            if (index < 0)
                throw new ArgumentOutOfRangeException();
            if (node.Parent.Nodes == nodes && index >= nodes.Count)
                throw new ArgumentOutOfRangeException();
            if (index > nodes.Count)
                throw new ArgumentOutOfRangeException();
            if (node.Find(nodes.Owner.Control) != null)
                throw new ArgumentException("Specified node collection is already contained within the current node.");
        }*/

    }

}
