
using System;
using System.Collections;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    /// <summary>Represents a collection of <see cref="System.Windows.Forms.Control"/> objects.</summary>
    [ListBindable(false)]
    [DesignerSerializer(typeof(OrgPanelControlCollectionSerializer), typeof(CodeDomSerializer))]
    public class OrgPanelControlCollection : Control.ControlCollection {

        private OrgPanel panel = null;

        internal OrgPanelControlCollection (OrgPanel owner) : base(owner) {

            panel = owner;

        }

        internal OrgPanel Panel {

            get { return panel; }

        }

        /// <summary>Creates a node with the specified control and adds it to the end of the root node collection.</summary>
        /// <param name="control">The control of the new node.</param>
        public override void Add (Control control) {

            // use target control/index for designer purposes
            // otherwise, Add is expected to insert at the root node collection

            panel.GetNode(panel.TargetControl).Nodes.Insert(panel.TargetIndex, control); 

        }

        /// <summary>Creates a node with the specified control and adds it to the end of the node collection of the specified parent control.</summary>
        /// <param name="control">The control of the new node.</param>
        /// <param name="parent">The parent control of the new node.</param>
        public virtual void Add (Control control, Control parent) {

            this.Add(control, parent, "", 0);

        }

        /// <summary>Creates a node with the specified control, sets the specified style name, and adds it to the end of the node collection of the specified parent control.</summary>
        /// <param name="control">The control of the new node.</param>
        /// <param name="parent">The parent control of the new node.></param>
        /// <param name="styleName">The name of the node style.</param>
        public virtual void Add (Control control, Control parent, string styleName) {

            this.Add(control, parent, styleName, 0);

        }

        /// <summary>Creates a node with the specified control, sets the specified parent offset, and adds it to the end of the node collection of the specified parent control.</summary>
        /// <param name="control">The control of the new node.</param>
        /// <param name="parent">The parent control of the new node.></param>
        /// <param name="parentOffset">The parent offset of the new node.</param>
        public virtual void Add (Control control, Control parent, int parentOffset) {

            this.Add(control, parent, "", parentOffset);

        }

        /// <summary>Creates a node with the specified control, sets the specified style name and parent offset, and adds it to the end of the node collection of the specified parent control.</summary>
        /// <param name="control">The control of the new node.</param>
        /// <param name="parent">The parent control of the new node.></param>
        /// <param name="styleName">The name of the node style.</param>
        /// <param name="parentOffset">The parent offset of the new node.</param>
        public virtual void Add (Control control, Control parent, string styleName, int parentOffset) {

            OrgNode node = new OrgNode(control);

            node.StyleName = styleName;
            node.ParentOffset = parentOffset;

            panel.GetNode(parent).Nodes.Add(node); // use nodes to add control...

        }

        /// <summary>Removes the node associated with the specified control and ascends all child nodes to the parent.</summary>
        /// <param name="control">The control associated with the node to remove.</param>
        public override void Remove (Control control) {

            panel.GetNode(control).Remove(true); // use nodes to remove controls...

        }

        /// <summary>Removes all nodes from the panel.</summary>
        public override void Clear () {

            panel.Nodes.Clear();

        }

        internal void AddInternal (Control control) {

            base.Add(control); // add internal is called from nodes

        }

        internal void RemoveInternal (Control control) {

            base.Remove(control); // remove internal is called from nodes

        }

    }

    internal class OrgPanelControlCollectionSerializer : CollectionCodeDomSerializer {

        protected override object SerializeCollection (IDesignerSerializationManager manager, CodeExpression targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize) {

            CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(targetExpression, "Add");
            OrgPanelControlCollection controls = (OrgPanelControlCollection)originalCollection;

            return this.SerializeNode(controls.Panel.Root, manager, method);

        }

        private CodeStatementCollection SerializeNode (OrgNode parent, IDesignerSerializationManager manager, CodeMethodReferenceExpression method) {

            CodeStatementCollection statements = new CodeStatementCollection();

            foreach (OrgNode node in parent.Nodes) {

                CodeMethodInvokeExpression expr = new CodeMethodInvokeExpression();

                expr.Method = method;
                expr.Parameters.Add(base.SerializeToExpression(manager, node.Control));
                expr.Parameters.Add(base.SerializeToExpression(manager, parent.Control));

                if (node.StyleName != "")
                    expr.Parameters.Add(base.SerializeToExpression(manager, node.StyleName));

                if (node.ParentOffset > 0)
                    expr.Parameters.Add(base.SerializeToExpression(manager, node.ParentOffset));


                statements.Add(expr);
                statements.AddRange(this.SerializeNode(node, manager, method));

            }

            return statements;

        }

    }

}
