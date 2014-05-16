
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace CheckBoxStudio.WinForms {

    /// <summary>Specifies a corner style.</summary>
    public enum CornerStyle {

        /// <summary>No corner style.</summary>
        None = 0,

        /// <summary>Corner is beveled.</summary>
        Beveled = 1,

        /// <summary>Corner is rounded.</summary>
        Rounded = 2

    }

    /// <summary>Represents style information for how corners will be displayed.</summary>
    [TypeConverter(typeof(CornersConverter))]
    public struct Corners {

        private CornerStyle topLeft;
        private CornerStyle topRight;
        private CornerStyle bottomRight;
        private CornerStyle bottomLeft;

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.Corners"/> structure with the specified style for all corners.</summary>
        /// <param name="all">The style for all corners.</param>
        public Corners (CornerStyle all) : this (all, all, all, all) {
            


        }

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.Corners"/> structure with the specified corner styles.</summary>
        /// <param name="topLeft">The style for the top-left corner.</param>
        /// <param name="topRight">The style for the top-right corner.</param>
        /// <param name="bottomRight">The style for the bottom-right corner.</param>
        /// <param name="bottomLeft">The style for the bottom-left corner.</param>
        public Corners (CornerStyle topLeft, CornerStyle topRight, CornerStyle bottomRight, CornerStyle bottomLeft) {

            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

        }

        /// <summary>Gets a value indicating whether all corner styles are equal.</summary>
        [Browsable(false)]
        public bool AllStylesEqual {

            get { return topLeft == topRight && topRight == bottomRight && bottomRight == bottomLeft; }

        }

        /// <summary>Gets the style of the top-left corner.</summary>
        [Description("The style of the top-left corner.")]
        [DefaultValue(typeof(CornerStyle), "Rounded"), RefreshProperties(RefreshProperties.All)]
        public CornerStyle TopLeft {

            get { return topLeft; }
            set { topLeft = value; }

        }

        /// <summary>Gets the style of the top-right corner.</summary>
        [Description("The style of the top-right corner.")]
        [DefaultValue(typeof(CornerStyle), "Rounded"), RefreshProperties(RefreshProperties.All)]
        public CornerStyle TopRight {

            get { return topRight; }
            set { topRight = value; }

        }

        /// <summary>Gets the style of the bottom-right corner.</summary>
        [Description("The style of the bottom-right corner.")]
        [DefaultValue(typeof(CornerStyle), "Rounded"), RefreshProperties(RefreshProperties.All)]
        public CornerStyle BottomRight {

            get { return bottomRight; }
            set { bottomRight = value; }

        }

        /// <summary>Gets the style of the bottom-left corner.</summary>
        [Description("The style of the bottom-left corner.")]
        [DefaultValue(typeof(CornerStyle), "Rounded"), RefreshProperties(RefreshProperties.All)]
        public CornerStyle BottomLeft {

            get { return bottomLeft; }
            set { bottomLeft = value; }

        }

        /// <summary>Returns a <see cref="System.String"/> representing the styles of the corners.</summary>
        public override string ToString () {

            if (this.AllStylesEqual)
                return topLeft == CornerStyle.None ? "None" : string.Format("All {0}", topLeft);

            return string.Format("{0}, {1}, {2}, {3}", topLeft, topRight, bottomRight, bottomLeft);

        }

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode () {

            return (int)topLeft | (int)topRight << 2 | (int)bottomRight << 4 | (int)bottomLeft << 6;

        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">Another object to compare to.</param>
        public override bool Equals (object obj) {

            return base.Equals(obj);

        }

        /// <summary></summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        public static bool operator == (Corners c1, Corners c2) {

            return c1.Equals(c2);

        }

        /// <summary></summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        public static bool operator != (Corners c1, Corners c2) {

            return !(c1 == c2);

        }

    }

    internal class CornersConverter : TypeConverter {

        public override bool GetPropertiesSupported (ITypeDescriptorContext context) {

            return true;

        }

        public override PropertyDescriptorCollection GetProperties (ITypeDescriptorContext context, object value, Attribute[] attributes) {

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(Corners), attributes);

            return props.Sort(new string[] { "TopLeft", "TopRight", "BottomRight", "BottomLeft" });

        }

        public override bool GetCreateInstanceSupported (ITypeDescriptorContext context) {

            return true;

        } 

        public override object CreateInstance (ITypeDescriptorContext context, IDictionary propertyValues) {
           
            return new Corners(
                (CornerStyle)propertyValues["TopLeft"],
                (CornerStyle)propertyValues["TopRight"],
                (CornerStyle)propertyValues["BottomRight"],
                (CornerStyle)propertyValues["BottomLeft"]
            );

        }

        public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType) {

            if (destinationType == typeof(InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);

        }

        public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            if (value is Corners) {

                if (destinationType == typeof(InstanceDescriptor)) {

                    Corners corners = (Corners)value;
                    Type type = typeof(CornerStyle);

                    if (corners.AllStylesEqual) {

                        return new InstanceDescriptor(
                            typeof(Corners).GetConstructor(new Type[] { type }),
                            new object[] { corners.TopLeft }
                        );

                    } else {

                        return new InstanceDescriptor(
                            typeof(Corners).GetConstructor(new Type[] { type, type, type, type }),
                            new object[] { corners.TopLeft, corners.TopRight, corners.BottomRight, corners.BottomLeft }
                        );

                    }

                }

            }

            return base.ConvertTo(context, culture, value, destinationType);

        } 

    }

}
