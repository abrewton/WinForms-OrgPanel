
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CheckBoxStudio.WinForms {

    internal class OrgPanelDesigner : ScrollableControlDesigner {

        private OrgPanel panel = null;
        private OrgPanelDesignerControlCollection controls = null;
        private ISelectionService selection = null;
        private IDesignerHost host = null;
        private DesignerActionListCollection actions = null;
        private DesignerVerbCollection verbs = null;

        private bool isDragging = false;
        private bool isMoving = false;
        private Control[] selectedControls = new Control[0];

        public override void Initialize (IComponent component) {

            base.Initialize(component);

            panel = (OrgPanel)component;
            controls = new OrgPanelDesignerControlCollection((OrgPanel)this.Control);
            selection = (ISelectionService)this.GetService(typeof(ISelectionService));
            host = (IDesignerHost)this.GetService(typeof(IDesignerHost));

            actions = new DesignerActionListCollection();
            actions.Add(new OrgPanelDesignerActionList(this));

            verbs = new DesignerVerbCollection();
            verbs.Add(new DesignerVerb("Edit Node Styles...", new EventHandler(this.EditNodeStyles)));
            verbs.Add(new DesignerVerb("About", new EventHandler(this.ShowAbout)));

            //if (panel.license == null)
            //    verbs.Add(new DesignerVerb("Register...", new EventHandler(this.ShowRegistration)));

            selection.SelectionChanging += new EventHandler(Selection_SelectionChanging);
            host.TransactionOpened += new EventHandler(Host_TransactionOpening);

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public OrgPanelDesignerControlCollection Controls {

            get { return controls; }

        }

        public override bool ParticipatesWithSnapLines {

            get { return false; }

        }

        public override SelectionRules SelectionRules {

            get {

                if (panel.AutoSize && panel.AutoSizeMode == AutoSizeMode.GrowAndShrink)
                    return SelectionRules.Moveable;

                return base.SelectionRules;

            }
        }

        public override DesignerActionListCollection ActionLists {

            get { return actions; }

        }

        public override DesignerVerbCollection Verbs {

            get { return verbs; }

        }

        protected override void PreFilterProperties (IDictionary properties) {

            base.PreFilterProperties(properties);

            PropertyDescriptor controlsProp = (PropertyDescriptor)properties["Controls"];

            if (controlsProp == null)
                return;

            Attribute[] attrs = new Attribute[controlsProp.Attributes.Count];
            controlsProp.Attributes.CopyTo(attrs, 0);
            properties["Controls"] = TypeDescriptor.CreateProperty(typeof(OrgPanelDesigner), "Controls", typeof(OrgPanelDesignerControlCollection), attrs);

        }

        protected override void OnPaintAdornments (PaintEventArgs e) {

            base.OnPaintAdornments(e);

            if (panel.Controls.Count == 0)
                panel.DrawMessage(e.Graphics, "Drag and drop controls\nto create nodes");
            else if (isDragging)
                this.DrawTargetAdornment(e.Graphics);

        }

        protected override void OnDragEnter (DragEventArgs e) {

            base.OnDragEnter(e);

            isDragging = true;
            isMoving = this.GetPrimarySelection() != null;
            selectedControls = this.GetSelectedControls(true);

            panel.TargetControl = null;
            panel.TargetIndex = -1;
            panel.Invalidate();

        }

        protected override void OnDragOver (DragEventArgs e) {

            base.OnDragOver(e);

            Point pt = panel.PointToClient(new Point(e.X, e.Y));

            Control ctrl = panel.GetTargetControl(pt, 200, selectedControls);
            int index = panel.GetTargetIndex(ctrl, pt, selectedControls);

            if (ctrl == panel.TargetControl && index == panel.TargetIndex)
                return;

            panel.TargetControl = ctrl;
            panel.TargetIndex = index;
            panel.Invalidate();

        }

        protected override void OnDragLeave (EventArgs e) {

            base.OnDragLeave(e);

            isDragging = false;
            isMoving = false;
            selectedControls = new Control[0];

            panel.TargetControl = null;
            panel.TargetIndex = -1;
            panel.Invalidate();

        }

        protected override void OnDragDrop (DragEventArgs e) {

            base.OnDragDrop(e);
            this.OnDragLeave(e);

        }

        protected override void Dispose (bool disposing) {

            if (disposing) {

                selection.SelectionChanging -= new EventHandler(Selection_SelectionChanging);
                host.TransactionOpened -= new EventHandler(Host_TransactionOpening);

            }

            base.Dispose(disposing);

        }

        private void Selection_SelectionChanging (object sender, EventArgs e) {

            panel.TargetControl = this.GetPrimarySelection();

        }

        private void Host_TransactionOpening (object sender, EventArgs e) {

            if (!host.TransactionDescription.StartsWith("Move")) // tap into "Move" transaction...
                return;

            if (!isMoving)
                return;
            
            // for internal drag moves...

            IComponentChangeService changes = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
            PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(panel)["Controls"]; // add Controls property to the transaction...

            changes.OnComponentChanging(panel, controlsProp);
            this.MoveControls();
            changes.OnComponentChanged(panel, controlsProp, null, null);

        }

        private Control GetPrimarySelection () {

            if (!(selection.PrimarySelection is Control))
                return null;

            Control ctrl = (Control)selection.PrimarySelection;

            if (!panel.Contains(ctrl))
                return null;

            return ctrl;

        }

        private Control[] GetSelectedControls (bool subnodes) {

            List<Control> list = new List<Control>();

            foreach (Component comp in selection.GetSelectedComponents()) {

                if (!(comp is Control))
                    continue;

                Control ctrl = (Control)comp;

                if (list.Contains(ctrl) || !panel.Contains(ctrl))
                    continue;

                OrgNode node = panel.Nodes.Find(ctrl);

                if (node == null)
                    continue;

                if (subnodes)
                    list.AddRange(node.GetAllControls());
                else
                    list.Add(ctrl);

            }

            return list.ToArray();

        }

        private void DrawTargetAdornment (Graphics g) {

            Control ctrl = panel.TargetControl;

            if (ctrl == panel)
                return;

            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush);
            Point[] pts = new Point[3];

            pts[0] = new Point(ctrl.Left - 1, ctrl.Top + ctrl.Height / 2);
            pts[1] = new Point(pts[0].X - 9, pts[0].Y + 4);
            pts[2] = new Point(pts[0].X - 9, pts[0].Y - 4);

            g.FillPolygon(brush, pts);
            g.DrawPolygon(pen, pts);

            pts[0] = new Point(ctrl.Right + 1, ctrl.Top + ctrl.Height / 2);
            pts[1] = new Point(pts[0].X + 9, pts[0].Y + 4);
            pts[2] = new Point(pts[0].X + 9, pts[0].Y - 4);

            g.FillPolygon(brush, pts);
            g.DrawPolygon(pen, pts);

            brush.Dispose();
            pen.Dispose();

        }

        private void MoveControls () {

            OrgNode parent = panel.GetNode(panel.TargetControl);
            int index = panel.TargetIndex;

            Control[] controls = this.GetSelectedControls(false);
            OrgNode[] nodes = panel.Nodes.FindAll(controls);

            panel.SuspendLayout();

            foreach (OrgNode node in nodes)
                node.Remove(false);

            foreach (OrgNode node in nodes)
                parent.Nodes.Insert(index++, node);

            panel.ResumeLayout(true);

        }

        private void EditNodeStyles (object sender, EventArgs e) {

            EditorServiceContext.EditValue(this, panel, "NodeStyles");

        }

        private void ShowAbout (object sender, EventArgs e) {

            About form = new About();

            form.ShowDialog();
            form.Dispose();

        }

        private void ShowRegistration (object sender, EventArgs e) {

            Register form = new Register();

            form.ShowDialog();
            form.Dispose();

        }

        //undo = (UndoEngine)this.GetService(typeof(UndoEngine));
        //if (undo != null)
        //    undo.Undone += new EventHandler(Undo_Undone);
        //isDragging = false;
        //if (isDragInternal) { 
        //DesignerTransaction trans = host.CreateTransaction("Reorganize Control(s)");
        //PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(panel)["Controls"];
        //changes.OnComponentChanging(panel, controlsProp);
        //changes.OnComponentChanged(panel, controlsProp, null, null);
        //trans.Commit();
        //BehaviorService.SyncSelection();
        //} // otherwise, allow control collection "Add" to handle this
        //panel.TargetControl = null;
        //panel.TargetIndex = -1;
        //panel.Invalidate();

    }

}