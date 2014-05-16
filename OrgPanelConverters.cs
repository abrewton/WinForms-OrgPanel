
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace CheckBoxStudio.WinForms {

    internal class ParentControlConverter : TypeConverter {

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            if (destinationType == typeof(string)) {

                List<Control> controls = ParentControlConverter.GetSelectedControls(context);
                OrgPanel panel = (OrgPanel)controls[0].Parent;
                Control parent = panel.GetNodeParentControl(controls[0]);

                foreach (Control ctrl in controls)
                    if (parent != panel.GetNodeParentControl(ctrl))
                        return "";

                if (parent == panel)
                    return "(root)";

                return parent.Name;

            }

            return base.ConvertTo(context, culture, value, destinationType);

        }

        public static List<Control> GetSelectedControls (ITypeDescriptorContext context) {

            List<Control> list = new List<Control>();

            if (context.Instance is Control)
                list.Add((Control)context.Instance);

            if (context.Instance is object[])
                foreach (object item in (object[])context.Instance)
                    list.Add((Control)item);

            return list;

        }

    }

    internal class NodeIndexConverter : Int32Converter {

        public override bool GetStandardValuesSupported (ITypeDescriptorContext context) {

            return true;

        }

        public override bool GetStandardValuesExclusive (ITypeDescriptorContext context) {

            return true;

        }

        public override StandardValuesCollection GetStandardValues (ITypeDescriptorContext context) {

            List<Control> controls = ParentControlConverter.GetSelectedControls(context);
            OrgPanel panel = (OrgPanel)controls[0].Parent;
            OrgNode node = panel.GetNode(controls[0]);
            List<int> values = new List<int>();

            for (int i = 0; i < node.parent.Nodes.Count; i++)
                values.Add(i);

            return new StandardValuesCollection(values);

        }

    }

    internal class NodeStyleNameConverter : StringConverter {

        public override bool GetStandardValuesSupported (ITypeDescriptorContext context) {

            return true;

        }

        public override StandardValuesCollection GetStandardValues (ITypeDescriptorContext context) {

            List<Control> controls = ParentControlConverter.GetSelectedControls(context);
            OrgPanel panel = (OrgPanel)controls[0].Parent;
            List<string> values = new List<string>();

            foreach (OrgNodeStyle style in panel.NodeStyles)
                values.Add(style.Name);

            return new StandardValuesCollection(values);

        }

        /*public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                int index = (int)value;
                if (index < 0)
                    return "(none)";
                List<Control> controls = ParentControlConverter.GetSelectedControls(context);
                OrgPanel panel = (OrgPanel)controls[0].Parent;
                return string.Format("{0} - {1}", index, panel.NodeStyles[index].Name);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                string[] values = value.ToString().Split('-');
                if (values.Length == 0)
                    return -1;
                int index = -1;
                if (int.TryParse(values[0], out index))
                    return index;
                return -1;
            }
            return base.ConvertFrom(context, culture, value);
        }*/

        /*public override bool GetStandardValuesExclusive (ITypeDescriptorContext context) {
            return true;
        }*/

        /*public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                if (value == OrgNode.defaultStyle)
                    return "(default)";
                return value.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                List<Control> controls = ParentControlConverter.GetSelectedControls(context);
                OrgPanel panel = (OrgPanel)controls[0].Parent;
                OrgNodeStyle style = panel.NodeStyles[(string)value];
                if (style == null)
                    return OrgNode.defaultStyle;
                return style;
            }
            return base.ConvertFrom(context, culture, value);
        }*/

    }

    /*internal class OrgNodeArrayConverter : ArrayConverter {
        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                OrgNode[] nodes = (OrgNode[])value;
                return string.Format("{0:N0}", nodes.Length);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }*/

}
