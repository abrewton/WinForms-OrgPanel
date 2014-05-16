
using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CheckBoxStudio.WinForms {

    internal class OrgPanelDesignerActionList : DesignerActionList {

        private ComponentDesigner designer = null;
        private OrgPanel panel = null;

        public OrgPanelDesignerActionList (ComponentDesigner designer) : base(designer.Component) {

            this.designer = designer;
            this.panel = (OrgPanel)this.Component;

        }

        public Orientation Orientation {

            get { return panel.Orientation; }
            set { panel.Orientation = value; }

        }

        public LinkLineStyle LinkLineStyle {

            get { return panel.LinkLineStyle; }
            set { panel.LinkLineStyle = value; }

        }

        public bool ShowRootLines {

            get { return panel.ShowRootLines; }
            set { panel.ShowRootLines = value; }

        }

        public bool ShowPlusMinus {

            get { return panel.ShowPlusMinus; }
            set { panel.ShowPlusMinus = value; }

        }

        public void EditNodeStyles () {

            EditorServiceContext.EditValue(designer, panel, "NodeStyles");
            
        }

        public override DesignerActionItemCollection GetSortedActionItems () {

            // only show simple, yet pertinent, properties...

            DesignerActionItemCollection items = new DesignerActionItemCollection();

            items.Add(new DesignerActionMethodItem(this, "EditNodeStyles", "Edit Node Styles..."));
            items.Add(new DesignerActionPropertyItem("Orientation", "Orientation:"));
            items.Add(new DesignerActionPropertyItem("LinkLineStyle", "Link Line Style:"));
            items.Add(new DesignerActionPropertyItem("ShowRootLines", "Show Root Lines"));
            items.Add(new DesignerActionPropertyItem("ShowPlusMinus", "Show Plus Minus"));

            return items;

        }

    }

}