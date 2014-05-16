
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CheckBoxStudio.WinForms {

    internal class EditorServiceContext : ITypeDescriptorContext, IWindowsFormsEditorService {

        private IComponentChangeService componentChangeSvc;
        private ComponentDesigner designer;
        private PropertyDescriptor targetProperty;

        public EditorServiceContext (ComponentDesigner designer, PropertyDescriptor prop) : base() {

            this.designer = designer;
            this.targetProperty = prop;

        }

        public IContainer Container {

            get {

                if (designer.Component.Site == null)
                    return null;

                return designer.Component.Site.Container;

            }

        }

        public object Instance {

            get { return designer.Component; }

        }

        public PropertyDescriptor PropertyDescriptor {

            get { return targetProperty; }

        }

        public object GetService (Type serviceType) {

            if (serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService))
                return this;
            
            if (designer.Component.Site == null)
                return null;
          
            return designer.Component.Site.GetService(serviceType);

        }

        public bool OnComponentChanging () {

            try {

                this.ChangeService.OnComponentChanging(designer.Component, targetProperty);

                return true;

            } catch {

                return false;

            }

        }

        public void OnComponentChanged () {

            this.ChangeService.OnComponentChanged(designer.Component, targetProperty, null, null);

        }

        public DialogResult ShowDialog (Form dialog) {

            IUIService uiService = (IUIService)this.GetService(typeof(IUIService));

            if (uiService != null)
                return uiService.ShowDialog(dialog);
            
            return dialog.ShowDialog(designer.Component as IWin32Window);

        }

        public void DropDownControl (Control control) {

            

        }

        public void CloseDropDown () {



        }

        private IComponentChangeService ChangeService {

            get {

                if (componentChangeSvc == null)
                    componentChangeSvc = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
               
                return componentChangeSvc;

            }

        }

        public static void EditValue (ComponentDesigner designer, object component, string propName) {

            PropertyDescriptor prop = TypeDescriptor.GetProperties(component)[propName];
            EditorServiceContext context = new EditorServiceContext(designer, prop);

            UITypeEditor editor = prop.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
            object oldValue = prop.GetValue(component);
            object newValue = editor.EditValue(context, context, oldValue);

            if (newValue != oldValue)
                prop.SetValue(component, newValue);

        }

    }

}