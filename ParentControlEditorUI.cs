
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Globalization;

namespace CheckBoxStudio.WinForms {

    [ToolboxItem(false)]
    internal partial class ParentControlEditorUI : TreeView {

        private IWindowsFormsEditorService service = null;
        private OrgPanel panel = null;
        private List<Control> selection = null;
        private Control preselected = null;
        private Control selected = null;

        public ParentControlEditorUI (IWindowsFormsEditorService service, List<Control> selection) {

            this.service = service;
            this.selection = selection;

            InitializeComponent();

            if (!this.SelectionInContainer(selection))
                return;

            panel = (OrgPanel)selection[0].Parent;
            preselected = panel.GetNodeParentControl(selection[0]);

            this.BeginUpdate();
            this.CreateNodes(this.Nodes, panel.Root);
            this.EndUpdate();

        }

        public Control ParentControl {

            get { return selected; }

        }

        protected override bool ProcessDialogKey (Keys keyData) {

            if (keyData != Keys.Return)
                return base.ProcessDialogKey(keyData);

            selected = (Control)this.SelectedNode.Tag;
            service.CloseDropDown();

            return true;

        }

        protected override void OnNodeMouseClick (TreeNodeMouseClickEventArgs e) {

            base.OnNodeMouseClick(e);

            selected = (Control)e.Node.Tag;
            service.CloseDropDown();

        }

        protected override void OnBeforeCollapse (TreeViewCancelEventArgs e) {

            e.Cancel = true;

            base.OnBeforeCollapse(e);

        }

        private void CreateNodes (TreeNodeCollection treeNodes, OrgNode orgNode) {

            if (selection.Contains(orgNode.Control))
                return; // ignore selected controls and subnodes

            TreeNode node = new TreeNode();
            Type type = orgNode.Control.GetType();

            this.AddImage(type);

            node.Tag = orgNode.Control;
            node.ImageKey = type.Name;
            node.SelectedImageKey = type.Name;
            node.Text = orgNode.parent == null ? "(root)" : orgNode.Control.Name;
            node.Expand();

            treeNodes.Add(node);

            if (orgNode.Control == preselected)
                this.SelectedNode = node;

            foreach (OrgNode item in orgNode.Nodes)
                this.CreateNodes(node.Nodes, item);

        }

        private bool SelectionInContainer (List<Control> selection) {

            Control container = selection[0].Parent;

            foreach (Control ctrl in selection) {

                if (ctrl.Parent != container)
                    return false;

                container = ctrl.Parent;

            }

            return true;

        }

        private void AddImage (Type type) {

            if (this.ImageList == null)
                this.ImageList = new ImageList();

            if (this.ImageList.Images.ContainsKey(type.Name))
                return;

            ToolboxItem toolboxItem = new ToolboxItem(type);

            this.ImageList.Images.Add(type.Name, toolboxItem.Bitmap);

        }

    }

    internal class ParentControlEditor : UITypeEditor {

        public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context) {

            return UITypeEditorEditStyle.DropDown;

        }

        public override bool IsDropDownResizable {

            get { return true; }

        }

        public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value) {

            IWindowsFormsEditorService service =
                (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (service == null)
                return value;

            ParentControlEditorUI editor = new ParentControlEditorUI(service, ParentControlConverter.GetSelectedControls(context));
            service.DropDownControl(editor);

            Control ctrl = editor.ParentControl;
            editor.Dispose();

            if (ctrl != null)
                return ctrl;

            return base.EditValue(context, provider, value);

        }

    }

}
