
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    /// <summary>Specifies the line display style between parent and child nodes in a <see cref="CheckBoxStudio.WinForms.OrgPanel"/>.</summary>
    public enum LinkLineStyle {

        /// <summary>Lines are drawn vertically and horizontally from each parent node to child nodes.</summary>
        Default = -1,

        /// <summary>Lines are drawn diagonally from each parent node directly to child nodes.</summary>
        Direct = 0

        /*/// <summary>Direct-spread pattern.</summary>
        DirectSpread = 1,

        /// <summary>Broom-spread pattern.</summary>
        BroomSpread = 2,

        /// <summary>Comb-spread pattern.</summary>
        CombSpread = 3,

        /// <summary>Fork-spread pattern.</summary>
        ForkSpread = 4,

        /// <summary>Claw-spread pattern.</summary>
        ClawSpread = 5,

        /// <summary>Comb-bracket pattern.</summary>
        CombBracket = 6,

        /// <summary>Fork-bracket pattern.</summary>
        ForkBracket = 7,

        /// <summary>Claw-bracket pattern.</summary>
        ClawBracket = 8*/

    }

    /// <summary>Represents a panel that lays out content in an organized hierarchy with each control representing a node.</summary>
    //// [LicenseProvider(typeof(OrgPanelLicenseProvider))]
    [ToolboxItem(true), ToolboxBitmap(typeof(OrgPanel), "OrgPanel.png")]
    [Description("Lays out content in an organized hierarchy with each control representing a node.")]
    [Designer(typeof(OrgPanelDesigner))]
    [ProvideProperty("NodeParentControl", typeof(Control))]
    [ProvideProperty("NodeIndex", typeof(Control))]
    [ProvideProperty("NodeStyleName", typeof(Control))]
    [ProvideProperty("NodeParentOffset", typeof(Control))]
    [DefaultProperty("Orientation")]
    [DefaultEvent("OrientationChanged")]
    public partial class OrgPanel : Panel, IExtenderProvider {

        internal License license = null;

        private EventHandler onOrientationChanged;
        private OrgPanelCancelEventHandler onBeforeExpand;
        private OrgPanelCancelEventHandler onBeforeCollapse;
        private OrgPanelEventHandler onAfterExpand;
        private OrgPanelEventHandler onAfterCollapse;

        private OrgNode root = null;
        private OrgNodeStyleCollection nodeStyles = null;
        private Control targetControl = null;
        private int targetIndex = -1;

        private Orientation orientation = Orientation.Horizontal;
        private int parentSpacing = 15;
        private int siblingSpacing = 9;
        private bool showRootLines = true;
        private bool showPlusMinus = true;
        private LinkLineStyle linkLineStyle = LinkLineStyle.Default;
        private Pen linkBackPen = new Pen(Color.LightGray);
        private Pen linkForePen = new Pen(Color.Black);
        private SolidBrush shadowBrush = new SolidBrush(Color.DarkGray);
        private Point shadowOffset = new Point(3, 3);
        private Color backColor = SystemColors.Window;
        private Bitmap minus = Properties.Resources.Expanded;
        private Bitmap plus = Properties.Resources.Collapsed;
        private IComponentChangeService compChanged = null;

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgPanel"/> class.</summary>
        public OrgPanel () {

            //LicenseManager.IsValid(typeof(OrgPanel), this, out license);

            root = new OrgNode(this);
            nodeStyles = new OrgNodeStyleCollection(this);
            
            InitializeComponent();

        }

        /// <summary>Occurs when the orientation property changes.</summary>
        [Description("Occurs when the orientation property changes.")]
        [Category("Behavior")]
        public event EventHandler OrientationChanged {

            add { onOrientationChanged += value; }
            remove { onOrientationChanged -= value; }

        }

        /// <summary>Occurs before a node is expanded.</summary>
        [Description("Occurs before a node is expanded.")]
        [Category("Behavior")]
        public event OrgPanelCancelEventHandler BeforeExpand {

            add { onBeforeExpand += value; }
            remove { onBeforeExpand -= value; }

        }

        /// <summary>Occurs before a node is collapsed.</summary>
        [Description("Occurs before a node is collapsed.")]
        [Category("Behavior")]
        public event OrgPanelCancelEventHandler BeforeCollapse {

            add { onBeforeCollapse += value; }
            remove { onBeforeCollapse -= value; }

        }

        /// <summary>Occurs after a node is expanded.</summary>
        [Description("Occurs after a node is expanded.")]
        [Category("Behavior")]
        public event OrgPanelEventHandler AfterExpand {

            add { onAfterExpand += value; }
            remove { onAfterExpand -= value; }

        }

        /// <summary>Occurs after a node is collapsed.</summary>
        [Description("Occurs after a node is collapsed.")]
        [Category("Behavior")]
        public event OrgPanelEventHandler AfterCollapse {

            add { onAfterCollapse += value; }
            remove { onAfterCollapse -= value; }

        }

        /// <summary></summary>
        [Obsolete(null, true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler BackgroundImageChanged {

            add { }
            remove { }

        }

        /// <summary></summary>
        [Obsolete(null, true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler BackgroundImageLayoutChanged {

            add { }
            remove { }

        }

        /// <summary></summary>
        [Obsolete(null, true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Image BackgroundImage {

            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }

        }

        /// <summary></summary>
        [Obsolete(null, true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageLayout BackgroundImageLayout {

            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }

        }

        /// <summary>Gets the collection of controls contained within the panel.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new OrgPanelControlCollection Controls {

            get { return (OrgPanelControlCollection)base.Controls; }

        }

        /// <summary>Gets the collection of root nodes associated with the panel.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OrgNodeCollection Nodes {

            get { return root.Nodes; }

        }

        /// <summary>Gets the collection of node styles associated with the panel.</summary>
        [Description("The collection of node styles associated with the panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public OrgNodeStyleCollection NodeStyles {

            get { return nodeStyles; }

        }

        /// <summary>Gets or sets the background color of the panel control.</summary>
        [Description("The background color of the panel control.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "Window")]
        public new Color BackColor {

            get { return backColor; }
            set {

                if (value == backColor)
                    return;

                backColor = value == Color.Empty ? SystemColors.Window : value;
                this.Invalidate();
            
            }

        }

        /// <summary>Gets or sets the border style of the panel control.</summary>
        [Description("The border style of the panel control.")]
        [Category("Appearance"), DefaultValue(typeof(BorderStyle), "FixedSingle")]
        public new BorderStyle BorderStyle {

            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }

        }

        /// <summary>Gets or sets the internal spacing of the panel control.</summary>
        [Description("The internal spacing of the panel control.")]
        [Category("Layout"), DefaultValue(typeof(Padding), "12, 12, 12, 12")]
        public new Padding Padding {

            get { return base.Padding; }
            set { base.Padding = value; }

        }

        /// <summary>Gets or sets the size of the auto-scroll margin.</summary>
        [Description("The size of the auto-scroll margin.")]
        [Category("Layout"), DefaultValue(typeof(Size), "16, 16")]
        public new Size AutoScrollMargin {

            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }

        }

        /// <summary>Gets or sets the orientation of nodes organized in the panel.</summary>
        [Description("The orientation of nodes organized in the panel.")]
        [Category("Layout"), DefaultValue(typeof(Orientation), "Horizontal")]
        public Orientation Orientation {

            get { return orientation; }
            set {

                if (value == orientation)
                    return;

                orientation = value;

                this.OnOrientationChanged(new EventArgs());
                this.PerformLayout();

            }

        }

        /// <summary>Gets or sets the spacing between parent and child nodes in the panel.</summary>
        [Description("The spacing between parent and child nodes in the panel.")]
        [Category("Layout"), DefaultValue(15)]
        public int ParentSpacing {

            get { return parentSpacing; }
            set {

                if (value == parentSpacing)
                    return;

                parentSpacing = value < 3 ? 3 : value;
                this.PerformLayout();

            }

        }

        /// <summary>Gets or sets the spacing between sibling nodes in the panel.</summary>
        [Description("The spacing between sibling nodes in the panel.")]
        [Category("Layout"), DefaultValue(9)]
        public int SiblingSpacing {

            get { return siblingSpacing; }
            set {

                if (value == siblingSpacing)
                    return;

                siblingSpacing = value < 1 ? 1 : value;
                this.PerformLayout();

            }

        }

        /// <summary>Gets or sets a value indicating whether lines are displayed for root nodes in the panel.</summary>
        [Description("Indicates whether lines are displayed for root nodes in the panel.")]
        [Category("Behavior"), DefaultValue(true)]
        public bool ShowRootLines {

            get { return showRootLines; }
            set {

                if (value == showRootLines)
                    return;

                showRootLines = value;
                this.PerformLayout();

            }

        }

        /// <summary>Gets or sets a value indicating whether plus/minus buttons are displayed with parent nodes in the panel.</summary>
        [Description("Indicates whether plus/minus buttons are displayed with parent nodes in the panel.")]
        [Category("Behavior"), DefaultValue(true)]
        public bool ShowPlusMinus {

            get { return showPlusMinus; }
            set {

                if (value == showPlusMinus)
                    return;

                showPlusMinus = value;
                this.PerformLayout();

            }

        }

        /// <summary>Gets or sets the color for child controls that use ambient background colors.</summary>
        [Description("The color for child controls that use ambient background colors.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "Control")]
        public Color ControlBackColor {

            get { return base.BackColor; }
            set { base.BackColor = value; }

        }

        /// <summary>Gets or sets the line style of all links between parent and child nodes in the panel.</summary>
        [Description("The line style of all links between parent and child nodes in the panel.")]
        [Category("Appearance"), DefaultValue(typeof(LinkLineStyle), "Default")]
        public LinkLineStyle LinkLineStyle {

            get { return linkLineStyle; }
            set {

                if (value == linkLineStyle)
                    return;

                linkLineStyle = value;
                this.PerformLayout();

            }

        }

        /// <summary>Gets or sets the color for all background links in the panel.</summary>
        [Description("The color for all background links in the panel.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "LightGray")]
        public Color LinkBackColor {

            get { return linkBackPen.Color; }
            set {

                if (value == linkBackPen.Color)
                    return;

                linkBackPen.Color = value == Color.Empty ? Color.LightGray : value;
                this.Invalidate();

            }

        }

        /// <summary>Gets or sets the color for all foreground links in the panel.</summary>
        [Description("The color for all foreground links in the panel.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "Black")]
        public Color LinkForeColor {

            get { return linkForePen.Color; }
            set {

                if (value == linkForePen.Color)
                    return;

                linkForePen.Color = value == Color.Empty ? Color.Black : value;
                this.Invalidate();

            }

        }

        /// <summary>Gets or sets the shadow color for each node in the panel.</summary>
        [Description("The shadow color for each node in the panel.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "DarkGray")]
        public Color ShadowColor {

            get { return shadowBrush.Color; }
            set {

                if (value == shadowBrush.Color)
                    return;

                shadowBrush.Color = value == Color.Empty ? Color.DarkGray : value;
                this.Invalidate();

            }

        }

        /// <summary>Gets or sets the shadow offset for each node in the panel.</summary>
        [Description("The shadow offset for each node in the panel.")]
        [Category("Appearance"), DefaultValue(typeof(Point), "3, 3")]
        public Point ShadowOffset {

            get { return shadowOffset; }
            set {

                if (value == shadowOffset)
                    return;

                shadowOffset = value;
                this.Invalidate();

            }

        }

        /// <summary>Gets the parent control of the node.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The parent control of the node.")]
        [DisplayName("Node.ParentControl"), Category("Layout")]
        [TypeConverter(typeof(ParentControlConverter)), Editor(typeof(ParentControlEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control GetNodeParentControl (Control control) {

            return this.GetNode(control).parent.Control; // use internal parent because public Parent will return null for virtual root

        }

        /// <summary>Sets the parent control of the node.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The parent control to set.</param>
        public void SetNodeParentControl (Control control, Control value) {

            OrgNode node = this.GetNode(control);
            
            if (node.parent == null)
                return;

            OrgNode parent = this.GetNode(value);

            if (node.parent == parent)
                return;

            if (node.Find(value) != null)
                throw new ArgumentException("Specified parent is already contained within the current node.");

            this.SuspendLayout();
            this.ControlsChanging();

            node.Remove(false);
            parent.Nodes.Add(node);

            this.ControlsChanged();
            this.ResumeLayout(true);

        }

        /// <summary>Gets the index of the node relative to other sibling nodes.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The index of the node relative to other sibling nodes.")]
        [DisplayName("Node.Index"), Category("Layout"), MergableProperty(false)]
        [TypeConverter(typeof(NodeIndexConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int GetNodeIndex (Control control) {

            return this.GetNode(control).Index;

        }

        /// <summary>Sets the index of the node relative to other sibling nodes.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The sibling index of the node.</param>
        public void SetNodeIndex (Control control, int value) {

            OrgNode node = this.GetNode(control);

            if (node.parent == null || node.Index == value)
                return;

            OrgNodeCollection nodes = node.parent.Nodes;

            if (value < 0 || value >= nodes.Count)
                throw new ArgumentOutOfRangeException();

            this.SuspendLayout();
            this.ControlsChanging();

            nodes.Remove(node);
            nodes.Insert(value, node);

            this.ControlsChanged();
            this.ResumeLayout(true);

        }

        /// <summary>Gets the style name of the node.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The style name of the node.")]
        [DisplayName("Node.StyleName"), Category("Appearance"), DefaultValue("")]
        [TypeConverter(typeof(NodeStyleNameConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GetNodeStyleName (Control control) {

            return this.GetNode(control).StyleName;

        }

        /// <summary>Sets the style name of the node.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The style name to set.</param>
        public void SetNodeStyleName (Control control, string value) {

            OrgNode node = this.GetNode(control);

            if (value == node.StyleName)
                return;

            this.ControlsChanging();
            node.StyleName = value;
            this.ControlsChanged();

        }

        /// <summary>Gets the parent offset position of the node.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The parent offset position of the node.")]
        [DisplayName("Node.ParentOffset"), Category("Layout"), DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int GetNodeParentOffset (Control control) {

            return this.GetNode(control).ParentOffset;

        }

        /// <summary>Sets the parent offset position of the node.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The offset position to set.</param>
        public void SetNodeParentOffset (Control control, int value) {

            OrgNode node = this.GetNode(control);

            if (value == node.ParentOffset)
                return;

            this.ControlsChanging();
            node.ParentOffset = value;
            this.ControlsChanged();

        }

        /// <summary>Retrieves the distance between the nearest edge of the specified control and coordinates.</summary>
        /// <param name="control">The control used to determine the distance.</param>
        /// <param name="pt">The coordinate point used to determine the distance.</param>
        public int GetDistance (Control control, Point pt) {

            if (!this.Controls.Contains(control))
                return -1;

            double x = 0;
            double y = 0;

            if (pt.X < control.Left)
                x = control.Left - pt.X;
            else if (pt.X > control.Right)
                x = pt.X - control.Right;

            if (pt.Y < control.Top)
                y = control.Top - pt.Y;
            else if (pt.Y > control.Bottom)
                y = pt.Y - control.Bottom;

            return (int)Math.Sqrt(x * x + y * y);

        }

        /// <summary>Retrieves the control located nearest to the specified coordinates.</summary>
        /// <param name="pt">The coordinate point used to determine the nearest control.</param>
        public Control GetNearestControl (Point pt) {

            return this.GetNearestControl(pt, int.MaxValue);

        }

        /// <summary>Retrieves the control located nearest to the specified coordinates within the specified maximum distance.</summary>
        /// <param name="pt">The coordinate point used to determine the nearest control.</param>
        /// <param name="maxDistance">The maximum distance to determine the nearest control.</param>
        public Control GetNearestControl (Point pt, int maxDistance) {

            return this.GetNearestControl(pt, maxDistance, new Control[0]);

        }

        /// <summary>Retrieves the control located nearest to the specified coordinates within the specified maximum distance, optionally ignoring the specified controls.</summary>
        /// <param name="pt">The coordinate point used to determine the nearest control.</param>
        /// <param name="maxDistance">The maximum distance to determine the nearest control.</param>
        /// <param name="ignore">The controls to ignore while determining the nearest control.</param>
        public Control GetNearestControl (Point pt, int maxDistance, Control[] ignore) {

            Control control = null;
            List<Control> list = new List<Control>(ignore);

            foreach (Control ctrl in this.Controls) {

                if (!ctrl.Visible || list.Contains(ctrl))
                    continue;

                int distance = this.GetDistance(ctrl, pt);

                if (distance > maxDistance)
                    continue;

                control = ctrl;
                maxDistance = distance;

            }

            return control;

        }

        /// <summary>Retrieves the control that is the parent target of the specified coordinates.</summary>
        /// <param name="pt">The coordinate point used to determine the target.</param>
        public Control GetTargetControl (Point pt) {

            return this.GetTargetControl(pt, int.MaxValue);

        }

        /// <summary>Retrieves the control that is the parent target of the specified coordinates within the specified maximum distance.</summary>
        /// <param name="pt">The coordinate point used to determine the target.</param>
        /// <param name="maxDistance">The maximum distance to determine the target.</param>
        public Control GetTargetControl (Point pt, int maxDistance) {

            return this.GetTargetControl(pt, maxDistance, new Control[0]);

        }

        /// <summary>Retrieves the control that is the parent target of the specified coordinates, optionally ignoring the specified controls.</summary>
        /// <param name="pt">The coordinate point used to determine the target.</param>
        /// <param name="ignore">The controls to ignore while determining the target.</param>
        public Control GetTargetControl (Point pt, Control[] ignore) {

            return this.GetTargetControl(pt, int.MaxValue, new Control[0]);

        }

        /// <summary>Retrieves the control that is the parent target of the specified coordinates within the specified maximum distance, optionally ignoring the specified controls.</summary>
        /// <param name="pt">The coordinate point used to determine the target.</param>
        /// <param name="maxDistance">The maximum distance to determine the target.</param>
        /// <param name="ignore">The controls to ignore while determining the target.</param>
        public Control GetTargetControl (Point pt, int maxDistance, Control[] ignore) {

            List<Control> list = new List<Control>(ignore);

            foreach (Control ctrl in this.Controls)
                if (orientation == Orientation.Vertical && ctrl.Left > pt.X ||
                    orientation == Orientation.Horizontal && ctrl.Top > pt.Y)
                    list.Add(ctrl);

            Control control = this.GetNearestControl(pt, maxDistance, list.ToArray());

            if (control == null)
                return this;

            if (orientation == Orientation.Vertical && control.Right > pt.X ||
                orientation == Orientation.Horizontal && control.Bottom > pt.Y)
                control = this.GetNodeParentControl(control);

            while (list.Contains(control))
                control = this.GetNodeParentControl(control);

            return control;

        }

        /// <summary>Retrieves the target index position of the specified coordinates.</summary>
        /// <param name="target">The target control used to determine the index.</param>
        /// <param name="pt">The coordinate point used to determine the index.</param>
        public int GetTargetIndex (Control target, Point pt) {

            return this.GetTargetIndex(target, pt, new Control[0]);

        }

        /// <summary>Retrieves the target index position of the specified coordinates, optionally ignoring the specified controls.</summary>
        /// <param name="target">The target control used to determine the index.</param>
        /// <param name="pt">The coordinate point used to determine the index.</param>
        /// <param name="ignore">The controls to ignore while determining the index.</param>
        public int GetTargetIndex (Control target, Point pt, Control[] ignore) {

            int index = 0;
            OrgNode node = this.GetNode(target);
            List<Control> list = new List<Control>(ignore);

            foreach (OrgNode child in node.Nodes) {

                if (list.Contains(child.Control))
                    continue;

                if (orientation == Orientation.Horizontal && pt.X <= child.Control.Left + child.Control.Width / 2)
                    break;

                if (orientation == Orientation.Vertical && pt.Y <= child.Control.Top + child.Control.Height / 2)
                    break;

                index++;

            }

            return index;

        }

        /// <summary>Raises the OnOrientationChanged event.</summary>
        /// <param name="e">A <see cref="System.EventArgs"/> that contains the event data.</param>
        protected internal virtual void OnOrientationChanged (EventArgs e) {

            if (onOrientationChanged != null)
                onOrientationChanged(this, e);

        }

        /// <summary>Raises the BeforeExpand event.</summary>
        /// <param name="e">A <see cref="CheckBoxStudio.WinForms.OrgPanelCancelEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnBeforeExpand (OrgPanelCancelEventArgs e) {

            if (onBeforeExpand != null)
                onBeforeExpand(this, e);

        }

        /// <summary>Raises the BeforeCollapse event.</summary>
        /// <param name="e">A <see cref="CheckBoxStudio.WinForms.OrgPanelCancelEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnBeforeCollapse (OrgPanelCancelEventArgs e) {

            if (onBeforeCollapse != null)
                onBeforeCollapse(this, e);

        }

        /// <summary>Raises the AfterExpand event.</summary>
        /// <param name="e">A <see cref="CheckBoxStudio.WinForms.OrgPanelEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnAfterExpand (OrgPanelEventArgs e) {

            if (onAfterExpand != null)
                onAfterExpand(this, e);

        }

        /// <summary>Raises the AfterCollapse event.</summary>
        /// <param name="e">A <see cref="CheckBoxStudio.WinForms.OrgPanelEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnAfterCollapse (OrgPanelEventArgs e) {

            if (onAfterCollapse != null)
                onAfterCollapse(this, e);

        }

        /// <summary>Creates a new instance of the control collection for the control.</summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance () {

            return new OrgPanelControlCollection(this);

        }

        /// <summary>Raises the <see cref="System.Windows.Forms.Control.Layout"/> event.</summary>
        /// <param name="e">A <see cref="System.Windows.Forms.LayoutEventArgs"/> that contains the event data.</param>
        protected override void OnLayout (LayoutEventArgs e) {

            base.OnLayout(e);

            this.DoLayout();
            base.OnLayout(e); // call again, to adjust scrollbars after layout

            this.Invalidate();

        }

        /// <summary>Paints the background of the control.</summary>
        /// <param name="e">A <see cref="System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaintBackground (PaintEventArgs e) {

            base.OnPaintBackground(e);

            e.Graphics.Clear(backColor);

        }

        /// <summary>Raises the <see cref="System.Windows.Forms.Control.Paint"/> event.</summary>
        /// <param name="e">A <see cref="System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint (PaintEventArgs e) {

            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            this.RenderShadows(e.Graphics, root);

            if (!this.DesignMode) // && license == null)
                this.DrawMessage(e.Graphics, "WinForms OrgPanel by CheckBox Studio\nwww.checkboxstudio.com");

            this.RenderRootLines(e.Graphics, root);
            this.RenderBackLinks(e.Graphics, root);
            this.RenderForeLinks(e.Graphics, root);
            this.RenderFrames(e.Graphics, root);

        }

        /// <summary>Raises the <see cref="System.Windows.Forms.Control.MouseClick"/> event.</summary>
        /// <param name="e">A <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseClick (MouseEventArgs e) {

            base.OnMouseClick(e);

            if (this.DesignMode || !this.ShowPlusMinus)
                return;

            foreach (Control ctrl in this.Controls) {

                Rectangle rect = OrgPanel.GetFrameRectangle(ctrl);
                Point pt = this.GetExpandPoint(ctrl, rect);
                Rectangle expand = new Rectangle(pt, new Size(11, 11));

                if (expand.Contains(e.Location)) {

                    this.GetNode(ctrl).Toggle();
                    break;

                }

            }

            if (!this.ContainsFocus)
                this.Focus();

        }

        internal OrgNode Root {

            get { return root; }

        }

        internal OrgNode GetNode (Control control) {

            OrgNode node = root.Find(control);

            if (node == null)
                throw new ArgumentException("Specified control does not exist.");

            return node;

        }

        internal Control TargetControl {

            get {

                if (this.Controls.Contains(targetControl))
                    return targetControl;

                return this;

            }

            set { targetControl = value; }

        }

        internal int TargetIndex {

            get {

                if (targetIndex >= 0)
                    return targetIndex;

                return this.GetNode(this.TargetControl).Nodes.Count;

            }

            set { targetIndex = value; }

        }

        internal void DrawMessage (Graphics g, string message) {

            SolidBrush brush = new SolidBrush(this.BackColor.GetBrightness() < 0.5 ? Color.White : Color.DarkSlateGray);
            StringFormat format = new StringFormat();
            
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            g.DrawString(message, SystemFonts.MenuFont, brush, this.DisplayRectangle, format);

            brush.Dispose();
            format.Dispose();

        }

        bool IExtenderProvider.CanExtend (object comp) {

            if (comp is Control)
                return ((Control)comp).Parent == this;

            return false;

        }

        private bool ShouldSerializeNodeStyles () {

            return nodeStyles.Count > 0;

        }

        private bool ShouldSerializeNodeParentControl (Control control) {

            return this.GetNode(control).parent.Control != this;

        }

        private void ControlsChanging () {

            if (!this.DesignMode)
                return;

            if (compChanged == null)
                compChanged = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));

            PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["Controls"];
            compChanged.OnComponentChanging(this, prop);

        }

        private void ControlsChanged () {

            if (!this.DesignMode)
                return;

            if (compChanged == null)
                compChanged = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));

            PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["Controls"];
            compChanged.OnComponentChanged(this, prop, null, null);

        }

        private void DoLayout () {

            int left = this.Padding.Left + this.AutoScrollPosition.X;
            int top = this.Padding.Top + this.AutoScrollPosition.Y;
            int initSpacing = this.ShowRootLines ? parentSpacing : 0;
            int tabIndex = 0;

            this.Initialize(root, ref tabIndex);

            if (orientation == Orientation.Vertical)
                this.LayoutVertically(this.Root, left + initSpacing, top);
            else
                this.LayoutHorizontally(this.Root, left, top + initSpacing);

        }

        private void LayoutNode (OrgNode node, int x, int y) {

            Control ctrl = node.Control;
            OrgNodeStyle style = node.Style;

            x += ctrl.Margin.Left;
            y += ctrl.Margin.Top;

            Point pt = new Point(x, y);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(ctrl);

            if (ctrl.Location != pt)
                props["Location"].SetValue(ctrl, pt);

        }

        private int GetPlusMinusSpacing (Control control) {

            if (!showPlusMinus)
                return 0;

            if (orientation == Orientation.Vertical && control.Margin.Right < 7)
                return 12 - control.Margin.Right;

            if (orientation == Orientation.Horizontal && control.Margin.Bottom < 7)
                return 12 - control.Margin.Bottom;

            return 5;

        }

        private void Initialize (OrgNode node, ref int tab) {

            int tabIndex = tab;

            if (node != root)
                tab++;

            foreach (OrgNode item in node.Nodes)
                this.Initialize(item, ref tab);

            if (node == root)
                return;

            bool visible = !node.IsCollapsed;
            Control ctrl = node.Control;
            Padding margin = node.Style.ControlMargin;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(ctrl);

            if (ctrl.Dock != DockStyle.None)
                props["Dock"].ResetValue(ctrl);

            if (ctrl.Anchor != (AnchorStyles.Top | AnchorStyles.Left))
                props["Anchor"].ResetValue(ctrl);

            if (ctrl.Margin != margin)
                props["Margin"].SetValue(ctrl, margin);

            if (ctrl.Visible != visible)
                props["Visible"].SetValue(ctrl, visible);

            if (ctrl.TabIndex != tabIndex)
                props["TabIndex"].SetValue(ctrl, tabIndex);

            if (!ctrl.Visible)
                ctrl.SendToBack();

        }

        private int LayoutHorizontally (OrgNode node, int left, int top) {

            int spanDiff = this.GetSpanDiff(node);
            int setLeft = left;
            int nextTop = top;

            if (spanDiff > 0)
                left += spanDiff / 2;

            if (node != root)
                nextTop += node.ParentOffset + this.GetNodeHeight(node) + this.GetPlusMinusSpacing(node.Control) + parentSpacing;

            if (node.Expanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    left = this.LayoutHorizontally(node.Nodes[i], left + (i > 0 ? siblingSpacing : 0), nextTop);

            if (node == root)
                return 0;

            if (spanDiff < 0)
                setLeft += spanDiff / -2;

            this.LayoutNode(node, setLeft, top + node.ParentOffset);

            if (spanDiff >= 0)
                left = node.Control.Right + node.Control.Margin.Right;

            return left;

        }

        private int LayoutVertically (OrgNode node, int left, int top) {

            int spanDiff = this.GetSpanDiff(node);
            int setTop = top;
            int nextLeft = left;

            if (spanDiff > 0)
                top += spanDiff / 2;

            if (node != root)
                nextLeft += node.ParentOffset + this.GetNodeWidth(node) + this.GetPlusMinusSpacing(node.Control) + parentSpacing;

            if (node.Expanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    top = this.LayoutVertically(node.Nodes[i], nextLeft, top + (i > 0 ? siblingSpacing : 0));

            if (node == root)
                return 0;

            if (spanDiff < 0)
                setTop += spanDiff / -2;

            this.LayoutNode(node, left + node.ParentOffset, setTop);

            if (spanDiff >= 0)
                top = node.Control.Bottom + node.Control.Margin.Bottom;

            return top;

        }

        private int GetSpanDiff (OrgNode node) {

            if (node == this.Root && this.AutoSize && this.AutoSizeMode == AutoSizeMode.GrowAndShrink)
                return 0;

            int nodeSpan = this.GetNodeSpan(node);

            if (!node.Expanded)
                return nodeSpan;

            return nodeSpan - this.GetOrgSpan(node.Nodes);

        }

        private int GetNodeSpan (OrgNode node) {

            if (orientation == Orientation.Vertical)
                return this.GetNodeHeight(node);
            else
                return this.GetNodeWidth(node);

        }

        private int GetOrgSpan (OrgNode node) {

            if (!node.Control.Visible)
                return 0;

            int nodeSpan = this.GetNodeSpan(node);
            int orgSpan = node.Expanded ? this.GetOrgSpan(node.Nodes) : 0;

            return nodeSpan < orgSpan ? orgSpan : nodeSpan;

        }

        private int GetOrgSpan (OrgNodeCollection nodes) {

            int total = 0;

            foreach (OrgNode node in nodes) {

                int span = this.GetOrgSpan(node);

                if (span == 0)
                    continue;

                if (total > 0)
                    total += siblingSpacing;

                total += span;

            }

            return total;

        }

        private int GetNodeWidth (OrgNode node) {

            int width = 0;

            if (node == this.Root)
                width = this.ClientRectangle.Width - this.Padding.Horizontal;
            else if (!node.Control.Visible || !this.Controls.Contains(node.Control))
                return 0;
            else
                width = node.Control.Width + node.Control.Margin.Horizontal;

            if (orientation == Orientation.Horizontal && width % 2 == 0)
                width++;

            return width;

        }

        private int GetNodeHeight (OrgNode node) {

            int height = 0;

            if (node == this.Root)
                height = this.ClientRectangle.Height - this.Padding.Vertical;
            else if (!node.Control.Visible || !this.Controls.Contains(node.Control))
                return 0;
            else
                height = node.Control.Height + node.Control.Margin.Vertical;

            if (orientation == Orientation.Vertical && height % 2 == 0)
                height++;

            return height;

        }

        private void RenderShadows (Graphics graphics, OrgNode parent) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                if (child.Style.ShadowVisible)
                    OrgPanel.DrawShadow(graphics, child.Control, child.Style.Corners, shadowOffset, shadowBrush);

                this.RenderShadows(graphics, child);

            }

        }

        private void RenderRootLines (Graphics graphics, OrgNode root) {

            if (!showRootLines)
                return;

            foreach (OrgNode node in root.Nodes) { // draw individual parent lines...

                if (node.Style.LinkColorStyle == LinkColorStyle.None)
                    continue;

                Point p2 = OrgPanel.GetFinalPoint(node.Control, orientation);

                Point p1 = orientation == Orientation.Vertical
                    ? new Point(p2.X - parentSpacing - node.ParentOffset, p2.Y)
                    : new Point(p2.X, p2.Y - parentSpacing - node.ParentOffset);

                Color color = node.Style.LinkColorStyle == LinkColorStyle.Background ? linkBackPen.Color : linkForePen.Color;
                Pen pen = new Pen(new LinearGradientBrush(p1, p2, this.BackColor, color));

                if (orientation == Orientation.Vertical)
                    p1.Offset(1, 0);
                else
                    p1.Offset(0, 1);

                graphics.DrawLine(pen, p1, p2);
                pen.Dispose();

            }

        }

        private void RenderBackLinks (Graphics graphics, OrgNode parent) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                if (parent != root && child.Style.LinkColorStyle == LinkColorStyle.Background) {

                    if (linkLineStyle == LinkLineStyle.Direct)
                        OrgPanel.DrawDirectLink(graphics, parent.Control, child.Control, orientation, linkBackPen);
                    else
                        OrgPanel.DrawDefaultLink(graphics, parent.Control, child.Control, orientation, parentSpacing / 2 + 1 + child.ParentOffset, linkBackPen);

                }

                this.RenderBackLinks(graphics, child);

            }

        }

        private void RenderForeLinks (Graphics graphics, OrgNode parent) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                if (parent != root && child.Style.LinkColorStyle == LinkColorStyle.Foreground) {

                    if (linkLineStyle == LinkLineStyle.Direct)
                        OrgPanel.DrawDirectLink(graphics, parent.Control, child.Control, orientation, linkForePen);
                    else
                        OrgPanel.DrawDefaultLink(graphics, parent.Control, child.Control, orientation, parentSpacing / 2 + 1 + child.ParentOffset, linkForePen);

                }

                this.RenderForeLinks(graphics, child);

            }

        }

        private void RenderFrames (Graphics graphics, OrgNode parent) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                Rectangle rect = OrgPanel.GetFrameRectangle(child.Control);
                OrgNodeStyle style = child.Style;
                LinearGradientBrush brush = new LinearGradientBrush(rect, style.FrameStartColor, style.FrameEndColor, style.GradientMode);

                OrgPanel.DrawFrame(graphics, child.Control, style.Corners, brush, style.BorderPen);

                brush.Dispose();

                if (child.Nodes.Count > 0 && showPlusMinus) {

                    Point pt = this.GetExpandPoint(child.Control, rect);
                    graphics.DrawImageUnscaled(child.Expanded ? minus : plus, pt);

                }
                
                this.RenderFrames(graphics, child);

            }

        }

        private Point GetExpandPoint (Control control, Rectangle rect) {

            if (orientation == Orientation.Vertical) {

                int y = rect.Top + rect.Height / 2 - 5 + rect.Height % 2;

                if (control.Margin.Right < 7)
                    return new Point(control.Right + 1, y);
                else
                    return new Point(rect.Right - 5, y);

            } else {

                int x = rect.Left + rect.Width / 2 - 5 + rect.Width % 2;

                if (control.Margin.Bottom < 7)
                    return new Point(x, control.Bottom + 1);
                else
                    return new Point(x, rect.Bottom - 5);

            }

        }

        private static Point GetInitialPoint (Control control, Orientation orientation) {

            Rectangle rect = OrgPanel.GetFrameRectangle(control);
            Point center = new Point(rect.Left + rect.Width / 2 + rect.Width % 2, rect.Top + rect.Height / 2 + rect.Height % 2);

            if (orientation == Orientation.Horizontal && rect.Height > rect.Width)
                return new Point(center.X, rect.Bottom - rect.Width / 2);

            if (orientation == Orientation.Vertical && rect.Width > rect.Height)
                return new Point(rect.Right - rect.Height / 2, center.Y);

            return center;

        }

        private static Point GetFinalPoint (Control control, Orientation orientation) {

            Rectangle rect = OrgPanel.GetFrameRectangle(control);

            if (orientation == Orientation.Vertical)
                return new Point(rect.Left, rect.Top + rect.Height / 2 + rect.Height % 2);
            else
                return new Point(rect.Left + rect.Width / 2 + rect.Width % 2, rect.Top);

        }

        private static void DrawShadow (Graphics graphics, Control control, Corners corners, Point offset, SolidBrush brush) {

            Rectangle rect = OrgPanel.GetFrameRectangle(control);
            rect.Offset(offset);

            GraphicsPath path = OrgPanel.GetFramePath(rect, OrgPanel.GetCornerRadius(control.Margin), corners);
            graphics.FillPath(brush, path);

            path.Dispose();

        }

        private static void DrawDefaultLink (Graphics graphics, Control parent, Control child, Orientation orientation, int spacing, Pen pen) {

            Point[] pts = new Point[4];

            pts[0] = OrgPanel.GetInitialPoint(parent, orientation);
            pts[3] = OrgPanel.GetFinalPoint(child, orientation);

            if (orientation == Orientation.Vertical) {

                pts[1] = new Point(pts[3].X - spacing, pts[0].Y);
                pts[2] = new Point(pts[1].X, pts[3].Y);

            } else {

                pts[1] = new Point(pts[0].X, pts[3].Y - spacing);
                pts[2] = new Point(pts[3].X, pts[1].Y);

            }

            graphics.DrawLines(pen, pts);

        }

        private static void DrawDirectLink (Graphics graphics, Control parent, Control child, Orientation orientation, Pen pen) {

            Point p1 = OrgPanel.GetInitialPoint(parent, orientation);
            Point p2 = OrgPanel.GetFinalPoint(child, orientation);

            graphics.DrawLine(pen, p1, p2);

        }

        private static void DrawFrame (Graphics graphics, Control control, Corners corners, LinearGradientBrush frameBrush, Pen borderPen) {

            Rectangle rect = OrgPanel.GetFrameRectangle(control);
            GraphicsPath path = OrgPanel.GetFramePath(rect, OrgPanel.GetCornerRadius(control.Margin), corners);

            graphics.FillPath(frameBrush, path);
            graphics.DrawPath(borderPen, path);

            path.Dispose();

        }

        private static GraphicsPath GetFramePath (Control control, Corners corners) {

            return OrgPanel.GetFramePath(OrgPanel.GetFrameRectangle(control), OrgPanel.GetCornerRadius(control.Margin), corners);

        }

        private static GraphicsPath GetFramePath (Rectangle rect, int cornerRadius, Corners corners) {

            GraphicsPath path = new GraphicsPath();

            if (corners.TopLeft == CornerStyle.None || cornerRadius < 1)
                path.AddLine(rect.Left, rect.Top, rect.Left, rect.Top);
            else if (corners.TopLeft == CornerStyle.Beveled)
                path.AddLine(rect.Left, rect.Top + cornerRadius / 2, rect.Left + cornerRadius / 2, rect.Top);
            else
                path.AddArc(rect.Left, rect.Top, cornerRadius, cornerRadius, 180, 90);

            if (corners.TopRight == CornerStyle.None || cornerRadius < 1)
                path.AddLine(rect.Right, rect.Top, rect.Right, rect.Top);
            else if (corners.TopRight == CornerStyle.Beveled)
                path.AddLine(rect.Right - cornerRadius / 2, rect.Top, rect.Right, rect.Top + cornerRadius / 2);
            else
                path.AddArc(rect.Right - cornerRadius, rect.Top, cornerRadius, cornerRadius, 270, 90);

            if (corners.BottomRight == CornerStyle.None || cornerRadius < 1)
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            else if (corners.BottomRight == CornerStyle.Beveled)
                path.AddLine(rect.Right, rect.Bottom - cornerRadius / 2, rect.Right - cornerRadius / 2, rect.Bottom);
            else
                path.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);

            if (corners.BottomLeft == CornerStyle.None || cornerRadius < 1)
                path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Bottom);
            else if (corners.BottomLeft == CornerStyle.Beveled)
                path.AddLine(rect.Left + cornerRadius / 2, rect.Bottom, rect.Left, rect.Bottom - cornerRadius / 2);
            else
                path.AddArc(rect.Left, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);

            path.CloseAllFigures();

            return path;

            /*if (cornerStyle == CornerStyle.None || cornerRadius < 1) {
                path.AddLine(rect.Left, rect.Top, rect.Left, rect.Top);
                path.AddLine(rect.Right, rect.Top, rect.Right, rect.Top);
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
                path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Bottom);
            } else if (cornerStyle == CornerStyle.Beveled) {
                path.AddLine(rect.Left, rect.Top + cornerRadius / 2, rect.Left + cornerRadius / 2, rect.Top);
                path.AddLine(rect.Right - cornerRadius / 2, rect.Top, rect.Right, rect.Top + cornerRadius / 2);
                path.AddLine(rect.Right, rect.Bottom - cornerRadius / 2, rect.Right - cornerRadius / 2, rect.Bottom);
                path.AddLine(rect.Left + cornerRadius / 2, rect.Bottom, rect.Left, rect.Bottom - cornerRadius / 2);
            } else { // rounded
                path.AddArc(rect.Left, rect.Top, cornerRadius, cornerRadius, 180, 90);
                path.AddArc(rect.Right - cornerRadius, rect.Top, cornerRadius, cornerRadius, 270, 90);
                path.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            }*/

        }

        internal static Rectangle GetFrameRectangle (Control control) {

            return new Rectangle(
                control.Left - control.Margin.Left,
                control.Top - control.Margin.Top,
                control.Width + control.Margin.Horizontal - 1,
                control.Height + control.Margin.Vertical - 1
            );

        }

        private static int GetCornerRadius (Padding margin) {

            int[] allMargins = new int[4] { margin.Left, margin.Top, margin.Right, margin.Bottom };
            int minMargin = allMargins[0];

            foreach (int value in allMargins)
                if (value < minMargin)
                    minMargin = value;

            return minMargin * 2;

        }

        /*/// <summary>Gets the color style of the link to the parent node.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The color style of the link to the parent node.")]
        [DisplayName("Node.LinkColorStyle"), Category("Appearance"), DefaultValue(typeof(LinkColorStyle), "Foreground")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LinkColorStyle GetNodeLinkColorStyle (Control control) {
            return this.GetNode(control).LinkColorStyle;
        }
        /// <summary>Sets the color style of the link to the parent node.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The color style of the link.</param>
        public void SetNodeLinkColorStyle (Control control, LinkColorStyle value) {
            OrgNode node = this.GetNode(control);
            if (value == node.LinkColorStyle)
                return;
            this.ControlsChanging();
            node.LinkColorStyle = value;
            this.ControlsChanged();
        }
        /// <summary>Gets the frame corner style of the margin around the node control.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The color style of the link to the parent node.")]
        [DisplayName("Node.CornerStyle"), Category("Appearance"), DefaultValue(typeof(CornerStyle), "Rounded")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CornerStyle GetNodeCornerStyle (Control control) {
            return this.GetNode(control).CornerStyle;
        }
        /// <summary>Sets the color style of the link to the parent node.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The corner styles of the node.</param>
        public void SetNodeCornerStyle (Control control, CornerStyle value) {
            OrgNode node = this.GetNode(control);
            if (value == node.CornerStyle)
                return;
            this.ControlsChanging();
            node.CornerStyle = value;
            this.ControlsChanged();
        }
        /// <summary>Gets the border color of the margin around the node control.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The border color of the margin around the node control.")]
        [DisplayName("Node.BorderColor"), Category("Appearance"), DefaultValue(typeof(Color), "ControlDarkDark")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color GetNodeBorderColor (Control control) {
            return this.GetNode(control).BorderColor;
        }
        /// <summary>Sets the border color of the margin around the node control.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The border color of the node.</param>
        public void SetNodeBorderColor (Control control, Color value) {
            OrgNode node = this.GetNode(control);
            if (value == node.BorderColor)
                return;
            this.ControlsChanging();
            node.BorderColor = value;
            this.ControlsChanged();
        }
        /// <summary>Gets the frame color of the margin around the node control.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("The frame color of the margin around the node control.")]
        [DisplayName("Node.FrameColor"), Category("Appearance"), DefaultValue(typeof(Color), "Control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color GetNodeFrameColor (Control control) {
            return this.GetNode(control).FrameColor;
        }
        /// <summary>Sets the frame color of the margin around the node control.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">The background color of the node.</param>
        public void SetNodeFrameColor (Control control, Color value) {
            OrgNode node = this.GetNode(control);
            if (value == node.FrameColor)
                return;
            this.ControlsChanging();
            node.FrameColor = value;
            this.ControlsChanged();
        }
        /// <summary>Gets a value indicating whether the node shadow is visible.</summary>
        /// <param name="control">A child control of the panel.</param>
        [Description("Indicates whether the node shadow is visible.")]
        [DisplayName("Node.ShadowVisible"), Category("Appearance"), DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool GetNodeShadowVisible (Control control) {
            return this.GetNode(control).ShadowVisible;
        }
        /// <summary>Sets a value indicating whether the node shadow is visible.</summary>
        /// <param name="control">A child control of the panel.</param>
        /// <param name="value">Determines whether the shadow is visible.</param>
        public void SetNodeShadowVisible (Control control, bool value) {
            OrgNode node = this.GetNode(control);
            if (value == node.ShadowVisible)
                return;
            this.ControlsChanging();
            node.ShadowVisible = value;
            this.ControlsChanged();
        }*/

        /*private void RenderLinks (Graphics graphics, OrgNode parent, LinkColorStyle style, Pen pen) {
            if (parent == null || !parent.Expanded)
                return;
            foreach (OrgNode child in parent.Nodes) {
                if (linkLineStyle == LinkLineStyle.Direct)
                    OrgPanel.DrawDirectLink(graphics, parent.Control, child.Control, orientation, pen);
                else
                    OrgPanel.DrawDefaultLink(graphics, parent.Control, child.Control, orientation, pen);
                //this.DrawLink(parent, child, style, graphics, pen);
                this.RenderLinks(graphics, child, style, pen);
            }
        }*/

        //private void DrawShadow (OrgNode node, Graphics graphics, Brush brush) {
        //    if (node == null || !node.ShadowVisible)
        //        return;
        //    Rectangle rect = OrgPanel.GetFrameRectangle(node.Control);
        //    int margin = OrgPanel.GetMinimumMargin(node.Control.Margin);
        //    Point[] pts = new Point[8];
        //    rect.Offset(this.ShadowOffset);
        //    pts[0] = new Point(rect.Left, rect.Top + margin);
        //    pts[1] = new Point(rect.Left + margin, rect.Top);
        //    pts[2] = new Point(rect.Right - margin, rect.Top);
        //    pts[3] = new Point(rect.Right, rect.Top + margin);
        //    pts[4] = new Point(rect.Right, rect.Bottom - margin);
        //    pts[5] = new Point(rect.Right - margin, rect.Bottom);
        //    pts[6] = new Point(rect.Left + margin, rect.Bottom);
        //    pts[7] = new Point(rect.Left, rect.Bottom - margin);
        //    graphics.FillPolygon(brush, pts);
        //    //graphics.FillRectangle(brush, rect);
        //    /*Point[] pts = new Point[4] {
        //        new Point(rect.Left, rect.Top),
        //        new Point(rect.Left, rect.Bottom),
        //        new Point(rect.Right, rect.Bottom),
        //        new Point(rect.Right, rect.Top)
        //    };*/
        //    //GraphicsPath path = new GraphicsPath();
        //    //PathGradientBrush b = new PathGradientBrush(path);
        //    //b.CenterPoint = new PointF(0, 0);
        //    //b.CenterColor = Color.Black;
        //    //b.SurroundColors = new Color[] { Color.Transparent };
        //    //graphics.FillPath(b, path);            
        //    //b.WrapMode = WrapMode.Clamp
        //    //LinearGradientBrush b = new LinearGradientBrush(rect, Color.White, Color.Black, 45);
        //    //graphics.FillRectangle(b, rect);
        //}

        /*private void DrawLink (OrgNode parent, OrgNode child, LinkColorStyle style, Graphics graphics, Pen pen) {
            if (parent == null || child == null)
                return;
            if (parent == this.Root || child.LinkColorStyle != style)
                return;
            Point[] pts;
            if (linkLineStyle == LinkLineStyle.Direct) {
                pts = new Point[2];
                pts[0] = this.GetInitialPoint(parent.Control);
                pts[1] = this.GetFinalPoint(child.Control);
            } else {
                pts = new Point[4];
                pts[0] = this.GetInitialPoint(parent.Control);
                pts[3] = this.GetFinalPoint(child.Control);
                int spacing = parentSpacing / 2 + 1;
                if (orientation == Orientation.Vertical) {
                    pts[1] = new Point(pts[3].X - spacing, pts[0].Y);
                    pts[2] = new Point(pts[1].X, pts[3].Y);
                } else {
                    pts[1] = new Point(pts[0].X, pts[3].Y - spacing);
                    pts[2] = new Point(pts[3].X, pts[1].Y);
                }
            }
            graphics.DrawLines(pen, pts);
        }*/

        /*private void DrawFrames (OrgNode node, Graphics graphics) {
            if (node == null)
                return;
            Rectangle rect = OrgPanel.GetFrameRectangle(node.Control);
            int margin = OrgPanel.GetMinimumMargin(node.Control.Margin);
            Point[] pts = new Point[8];
            pts[0] = new Point(rect.Left, rect.Top + margin);
            pts[1] = new Point(rect.Left + margin, rect.Top);
            pts[2] = new Point(rect.Right - margin, rect.Top);
            pts[3] = new Point(rect.Right, rect.Top + margin);
            pts[4] = new Point(rect.Right, rect.Bottom - margin);
            pts[5] = new Point(rect.Right - margin, rect.Bottom);
            pts[6] = new Point(rect.Left + margin, rect.Bottom);
            pts[7] = new Point(rect.Left, rect.Bottom - margin);
            //GraphicsPath path = new GraphicsPath();
            //path.StartFigure();
            //path.AddLine(
            //path.CloseFigure();
            //LinearGradientBrush brush = new LinearGradientBrush(pts[1], pts[6], Color.White, node.FrameBrush.Color);
            //graphics.FillPolygon(brush, pts);
            graphics.FillPolygon(node.FrameBrush, pts);
            graphics.DrawPolygon(node.BorderPen, pts);
            //brush.Dispose();
            //graphics.FillRectangle(node.FrameBrush, rect);
            //graphics.DrawRectangle(node.BorderPen, rect);
            if (node.Nodes.Count == 0 || !this.ShowPlusMinus)
                return;
            Point pt = this.GetExpandPoint(node.Control, rect);
            graphics.DrawImageUnscaled(node.Expanded ? minus : plus, pt);
        }*/

        /*/// <summary>Raises the <see cref="System.Windows.Forms.Control.MouseClick"/> event.</summary>
        /// <param name="e">A <see cref="System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseClick (MouseEventArgs e) {

            base.OnMouseClick(e);

            if (this.DesignMode || !showPlusMinus)
                return;
            
            foreach (Control ctrl in this.Controls) {

                Rectangle frame = this.GetFrameRectangle(ctrl);
                Point pt = this.GetExpandPoint(ctrl, frame);
                Rectangle expand = new Rectangle(pt, new Size(11, 11));

                if (expand.Contains(e.Location)) {

                    this.GetNode(ctrl).Toggle();
                    break;

                }

            }

            if (!this.ContainsFocus)
                this.Focus();

        }*/

        /*private int GetSpanDiff (OrgNode node) {

            if (node == root && this.AutoSize && this.AutoSizeMode == AutoSizeMode.GrowAndShrink)
                return 0;

            int nodeSpan = this.GetNodeSpan(node);

            if (!node.Expanded)
                return nodeSpan;

            return nodeSpan - this.GetOrgSpan(node.Nodes);

        }

        private int GetNodeWidth (OrgNode node) {

            int width = 0;

            if (node == root)
                width = this.ClientRectangle.Width - this.Padding.Horizontal;
            else if (!node.Control.Visible || !this.Controls.Contains(node.Control))
                return 0;
            else
                width = node.Control.Width + node.Control.Margin.Horizontal;

            if (orientation == Orientation.Horizontal && width % 2 == 0)
                width++;

            return width;

        }

        private int GetNodeHeight (OrgNode node) {

            int height = 0;

            if (node == root)
                height = this.ClientRectangle.Height - this.Padding.Vertical;
            else if (!node.Control.Visible || !this.Controls.Contains(node.Control))
                return 0;
            else
                height = node.Control.Height + node.Control.Margin.Vertical;

            if (orientation == Orientation.Vertical && height % 2 == 0)
                height++;

            return height;

        }

        private int GetNodeSpan (OrgNode node) {

            if (orientation == Orientation.Vertical)
                return this.GetNodeHeight(node);
            else
                return this.GetNodeWidth(node);

        }

        private int GetOrgSpan (OrgNode node) {

            if (!node.Control.Visible)
                return 0;

            int nodeSpan = this.GetNodeSpan(node);
            int orgSpan = node.Expanded ? this.GetOrgSpan(node.Nodes) : 0;

            return nodeSpan < orgSpan ? orgSpan : nodeSpan;

        }

        private int GetOrgSpan (OrgNodeCollection nodes) {

            int total = 0;

            foreach (OrgNode node in nodes) {

                int span = this.GetOrgSpan(node);

                if (span == 0)
                    continue;

                if (total > 0)
                    total += siblingSpacing;

                total += span;

            }

            return total;

        }*/

        /*private void Organize () {

            int left = this.Padding.Left + this.AutoScrollPosition.X;
            int top = this.Padding.Top + this.AutoScrollPosition.Y;
            int initSpacing = showRootLines ? parentSpacing : 0;

            int tabIndex = 0;
            this.Initialize(root, ref tabIndex);

            if (orientation == Orientation.Vertical)
                this.OrganizeVertically(root, left + initSpacing, top);
            else
                this.OrganizeHorizontally(root, left, top + initSpacing);

        }

        private void Organize (Control control, int x, int y) {

            x += control.Margin.Left;
            y += control.Margin.Top;

            Point pt = new Point(x, y);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(control);

            if (control.Location != pt)
                props["Location"].SetValue(control, pt);

        }

        private int OrganizeHorizontally (OrgNode node, int left, int top) {

            int spanDiff = this.GetSpanDiff(node);
            int setLeft = left;
            int nextTop = top;

            if (spanDiff > 0)
                left += spanDiff / 2;

            if (node != root)
                nextTop += this.GetNodeHeight(node) + parentSpacing + (showPlusMinus ? 8 : 0);

            if (node.Expanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    left = this.OrganizeHorizontally(node.Nodes[i], left + (i > 0 ? siblingSpacing : 0), nextTop);

            if (node == root)
                return 0;

            if (spanDiff < 0)
                setLeft += spanDiff / -2;

            this.Organize(node.Control, setLeft, top);

            if (spanDiff >= 0)
                left = node.Control.Right + node.Control.Margin.Right;

            return left;

        }

        private int OrganizeVertically (OrgNode node, int left, int top) {

            int spanDiff = this.GetSpanDiff(node);
            int setTop = top;
            int nextLeft = left;

            if (spanDiff > 0)
                top += spanDiff / 2;

            if (node != root)
                nextLeft += this.GetNodeWidth(node) + parentSpacing + (showPlusMinus ? 8 : 0);

            if (node.Expanded)
                for (int i = 0; i < node.Nodes.Count; i++)
                    top = this.OrganizeVertically(node.Nodes[i], nextLeft, top + (i > 0 ? siblingSpacing : 0));

            if (node == root)
                return 0;

            if (spanDiff < 0)
                setTop += spanDiff / -2;

            this.Organize(node.Control, left, setTop);

            if (spanDiff >= 0)
                top = node.Control.Bottom + node.Control.Margin.Bottom;

            return top;

        }

        private void RenderShadows (OrgNode parent, Graphics graphics, Brush brush) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                this.DrawShadow(child, graphics, brush);
                this.RenderShadows(child, graphics, brush);

            }

        }

        private void RenderRootLines (OrgNode root, Graphics graphics) {

            if (!showRootLines)
                return;

            foreach (OrgNode node in root.Nodes) { // draw individual parent lines...

                if (node.LinkColorStyle == OrgLinkColorStyle.None)
                    continue;

                Point p1;
                Point p2 = this.GetFinalPoint(node.Control);

                if (orientation == Orientation.Vertical)
                    p1 = new Point(p2.X - parentSpacing, p2.Y);
                else
                    p1 = new Point(p2.X, p2.Y - parentSpacing);

                Color color = node.LinkColorStyle == OrgLinkColorStyle.Background ? linkBackPen.Color : linkForePen.Color;
                Pen pen = new Pen(new LinearGradientBrush(p1, p2, this.BackColor, color));

                if (orientation == Orientation.Vertical)
                    p1.Offset(1, 0);
                else
                    p1.Offset(0, 1);

                graphics.DrawLine(pen, p1, p2);
                pen.Dispose();

            }

        }

        private void RenderLinks (OrgNode parent, OrgLinkColorStyle style, Graphics graphics, Pen pen) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                this.DrawLink(parent, child, style, graphics, pen);
                this.RenderLinks(child, style, graphics, pen);

            }

        }

        private void RenderFrames (OrgNode parent, Graphics graphics) {

            if (parent == null || !parent.Expanded)
                return;

            foreach (OrgNode child in parent.Nodes) {

                this.DrawFrame(child, graphics);
                this.RenderFrames(child, graphics);

            }

        }

        private Point GetInitialPoint (Control control) {

            Rectangle rect = this.GetFrameRectangle(control);
            Point center = new Point(rect.Left + rect.Width / 2 + rect.Width % 2, rect.Top + rect.Height / 2 + rect.Height % 2);

            if (orientation == Orientation.Horizontal && rect.Height > rect.Width)
                return new Point(center.X, rect.Bottom - rect.Width / 2);

            if (orientation == Orientation.Vertical && rect.Width > rect.Height)
                return new Point(rect.Right - rect.Height / 2, center.Y);

            return center;

        }

        private Point GetFinalPoint (Control control) {

            Rectangle rect = this.GetFrameRectangle(control);

            if (orientation == Orientation.Vertical)
                return new Point(rect.Left, rect.Top + rect.Height / 2 + rect.Height % 2);
            else
                return new Point(rect.Left + rect.Width / 2 + rect.Width % 2, rect.Top);

        }

        private void DrawShadow (OrgNode node, Graphics graphics, Brush brush) {

            if (node == null || !node.ShadowVisible)
                return;

            Rectangle rect = this.GetFrameRectangle(node.Control);

            rect.Offset(shadowOffset);
            graphics.FillRectangle(brush, rect);

        }

        private void DrawLink (OrgNode parent, OrgNode child, OrgLinkColorStyle style, Graphics graphics, Pen pen) {

            if (parent == null || child == null)
                return;

            if (parent == root || child.LinkColorStyle != style)
                return;

            Point[] pts;

            if (linkLineStyle == OrgLinkLineStyle.Direct) {

                pts = new Point[2];

                pts[0] = this.GetInitialPoint(parent.Control);
                pts[1] = this.GetFinalPoint(child.Control);

            } else {

                pts = new Point[4];

                pts[0] = this.GetInitialPoint(parent.Control);
                pts[3] = this.GetFinalPoint(child.Control);

                int spacing = parentSpacing / 2 + 1;

                if (orientation == Orientation.Vertical) {

                    pts[1] = new Point(pts[3].X - spacing, pts[0].Y);
                    pts[2] = new Point(pts[1].X, pts[3].Y);

                } else {

                    pts[1] = new Point(pts[0].X, pts[3].Y - spacing);
                    pts[2] = new Point(pts[3].X, pts[1].Y);

                }

            }

            graphics.DrawLines(pen, pts);

        }

        private void DrawFrame (OrgNode node, Graphics graphics) {

            if (node == null)
                return;

            Rectangle rect = this.GetFrameRectangle(node.Control);

            graphics.FillRectangle(node.BackBrush, rect);
            graphics.DrawRectangle(node.BorderPen, rect);

            if (node.Nodes.Count == 0 || !showPlusMinus)
                return;

            Point pt = this.GetExpandPoint(node.Control, rect);
            graphics.DrawImageUnscaled(node.Expanded ? minus : plus, pt);

        }

        private Point GetExpandPoint (Control control, Rectangle rect) {

            if (orientation == Orientation.Vertical)
                return new Point(control.Right + 1, rect.Top + rect.Height / 2 - 5 + rect.Height % 2);
            else
                return new Point(rect.Left + rect.Width / 2 - 5 + rect.Width % 2, control.Bottom + 1);

        }

        private Rectangle GetFrameRectangle (Control control) {

            Rectangle rect = new Rectangle(
                control.Left - control.Margin.Left,
                control.Top - control.Margin.Top,
                control.Width + control.Margin.Horizontal - 1,
                control.Height + control.Margin.Vertical - 1
            );

            return rect;

        }*/

    }

}
