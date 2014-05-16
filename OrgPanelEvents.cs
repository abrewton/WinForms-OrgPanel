
using System;
using System.Collections.Generic;
using System.Text;

namespace CheckBoxStudio.WinForms {

    /// <summary>Represents the method that will handle the AfterExpand and AfterCollapse events of a <see cref="CheckBoxStudio.WinForms.OrgPanel"/>.</summary>
    /// <param name="sender">The object raising the event.</param>
    /// <param name="e">The data for the event.</param>
    public delegate void OrgPanelEventHandler (object sender, OrgPanelEventArgs e);

    /// <summary>Represents the method that will handle the BeforeExpand and BeforeCollapse events of a <see cref="CheckBoxStudio.WinForms.OrgPanel"/>.</summary>
    /// <param name="sender">The object raising the event.</param>
    /// <param name="e">The data for the event.</param>
    public delegate void OrgPanelCancelEventHandler (object sender, OrgPanelCancelEventArgs e);

    /// <summary>Provides data for the AfterExpand and AfterCollapse events of a <see cref="CheckBoxStudio.WinForms.OrgPanel"/>.</summary>
    public class OrgPanelEventArgs : EventArgs {

        private OrgNode node = null;

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgPanelEventArgs"/> class with the specified node.</summary>
        /// <param name="node">The node that has been expanded or collapsed.</param>
        public OrgPanelEventArgs (OrgNode node) {

            this.node = node;

        }

        /// <summary>Gets the node that has been expanded or collapsed.</summary>
        public OrgNode Node {

            get { return node; }

        }

    }

    /// <summary>Provides data for the BeforeExpand and BeforeCollapse events of a <see cref="CheckBoxStudio.WinForms.OrgPanel"/>.</summary>
    public class OrgPanelCancelEventArgs : OrgPanelEventArgs {

        private bool cancel = false;

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgPanelCancelEventArgs"/> class with the specified node
        /// and a value specifying whether the event is to be cancelled.</summary>
        /// <param name="node">The node that has been expanded or collapsed.</param>
        /// <param name="cancel">A value specifying whether the event is to be cancelled.></param>
        public OrgPanelCancelEventArgs (OrgNode node, bool cancel) : base(node) {

            this.cancel = cancel;

        }

        /// <summary>Gets or sets a value specifying whether the event is to be cancelled.</summary>
        public bool Cancel {

            get { return cancel; }
            set { cancel = value; }

        }

    }

}
