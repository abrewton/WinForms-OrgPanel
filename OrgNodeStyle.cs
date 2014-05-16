
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;

namespace CheckBoxStudio.WinForms {

    /// <summary>Specifies the link color style for a <see cref="CheckBoxStudio.WinForms.OrgNodeStyle"/>.</summary>
    public enum LinkColorStyle {

        /// <summary>No color style.</summary>
        None = 0,

        /// <summary>Link background color.</summary>
        Background = 1,

        /// <summary>Link foreground color.</summary>
        Foreground = 2

    }

    /// <summary>Represents style information for nodes in a <see cref="CheckBoxStudio.WinForms.OrgPanel"/></summary>
    public class OrgNodeStyle : IDisposable {

        internal OrgPanel panel = null;

        private string name = "";
        private Padding controlMargin = new Padding(7);
        private bool shadowVisible = true;
        private LinkColorStyle linkColorStyle = LinkColorStyle.Foreground;
        private Corners corners = new Corners(CornerStyle.Rounded);
        private Color frameStartColor = SystemColors.Window;
        private Color frameEndColor = SystemColors.Control;
        private LinearGradientMode gradientMode = LinearGradientMode.Vertical;
        private Pen borderPen = new Pen(SystemColors.ControlDark);

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgNodeStyle"/> class.</summary>
        public OrgNodeStyle () : this ("") {

            

        }

        /// <summary>Initializes a new instance of the <see cref="CheckBoxStudio.WinForms.OrgNodeStyle"/> class with the specified name.</summary>
        /// <param name="name">The name of the node style.</param>
        public OrgNodeStyle (string name) {

            this.Name = name;

        }

        /// <summary>Gets the panel that the node style is assigned to.</summary>
        [Browsable(false)]
        public OrgPanel Panel {

            get { return panel; }

        }

        /// <summary>Gets or sets the name of the node style.</summary>
        [Description("The name of the node style.")]
        [Category("Data"), DefaultValue("")]
        [MergableProperty(false)]
        public string Name {

            get {

                if (name == "" && panel != null)
                    name = string.Format("NodeStyle{0}", panel.NodeStyles.IndexOf(this));

                return name;
            
            }

            set { name = value == null ? "" : value.Trim(); }

        }

        /// <summary>Gets or sets the control margin of the node style.</summary>
        [Description("The control margin of the node style.")]
        [Category("Layout"), DefaultValue(typeof(Padding), "7, 7, 7, 7")]
        public Padding ControlMargin {

            get { return controlMargin; }
            set {

                if (value == controlMargin)
                    return;

                controlMargin = value;
                
                if (panel != null)
                    panel.PerformLayout();
            
            }

        }

        /// <summary>Gets or sets a value indicating whether shadows are visible for the node style.</summary>
        [Description("Indicates whether shadows are visible for the node style.")]
        [Category("Appearance"), DefaultValue(true)]
        public bool ShadowVisible {

            get { return shadowVisible; }
            set {

                if (value == shadowVisible)
                    return;

                shadowVisible = value;
            
                if (panel != null)
                    panel.Invalidate();
            
            }

        }

        /// <summary>Gets or sets the link color style of the node style.</summary>
        [Description("The link color style of the node style.")]
        [Category("Appearance"), DefaultValue(typeof(LinkColorStyle), "Foreground")]
        public LinkColorStyle LinkColorStyle {

            get { return linkColorStyle; }
            set {

                if (value == linkColorStyle)
                    return;

                linkColorStyle = value;
            
                if (panel != null)
                    panel.Invalidate();
            
            }

        }

        /// <summary>Gets or sets the corner styles of the node style.</summary>
        [Description("The corner styles of the node style.")]
        [Category("Appearance")]
        public Corners Corners {

            get { return corners; }
            set {

                if (value == corners)
                    return;

                corners = value;
            
                if (panel != null)
                    panel.Invalidate();

            }

        }

        /// <summary>Gets or sets the frame start color of the node style.</summary>
        [Description("The frame start color of the node style.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "Window")]
        public Color FrameStartColor {

            get { return frameStartColor; }
            set {

                if (value == frameStartColor)
                    return;

                frameStartColor = value;

                if (panel != null)
                    panel.PerformLayout();
            
            }

        }

        /// <summary>Gets or sets the frame end color of the node style.</summary>
        [Description("The frame end color of the node style.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "Control")]
        public Color FrameEndColor {

            get { return frameEndColor; }
            set {

                if (value == frameEndColor)
                    return;

                frameEndColor = value;
            
                if (panel != null)
                    panel.PerformLayout();
            
            }

        }

        /// <summary>Gets or sets the frame gradient mode of the node style.</summary>
        [Description("The frame gradient mode of the node style.")]
        [Category("Appearance"), DefaultValue(typeof(LinearGradientMode), "Vertical")]
        public LinearGradientMode GradientMode {

            get { return gradientMode; }
            set {

                if (value == gradientMode)
                    return;

                gradientMode = value;
            
                if (panel != null)
                    panel.PerformLayout();
            
            }

        }

        /// <summary>Gets or sets the border color of the node style.</summary>
        [Description("The border color of the node style.")]
        [Category("Appearance"), DefaultValue(typeof(Color), "ControlDark")]
        public Color BorderColor {

            get { return borderPen.Color; }
            set {

                if (value == borderPen.Color)
                    return;

                borderPen.Color = value;
            
                if (panel != null)
                    panel.Invalidate();
            
            }

        }

        /// <summary>Returns a <see cref="System.String"/> representing the name of the node style.</summary>
        public override string ToString () {

            return this.Name;

        }

        /// <summary>Releases all resources used by the node style.</summary>
        public void Dispose () {

            this.Dispose(true);

        }

        /// <summary>Releases all resources used by the node style.</summary>
        /// <param name="disposing">Indicates whether disposing should occur.</param>
        protected virtual void Dispose (bool disposing) {

            if (!disposing)
                return;

            if (borderPen != null)
                borderPen.Dispose();

        }

        internal Pen BorderPen {

            get { return borderPen; }

        }

        private bool ShouldSerializeCorners () {

            return !(corners.AllStylesEqual && corners.TopLeft == CornerStyle.Rounded);

        }

        /*/// <summary>Specifies the corner styles for a <see cref="CheckBoxStudio.WinForms.OrgNode"/>.</summary>
        [Flags]
        [Editor(typeof(CornerStylesEditor), typeof(UITypeEditor))]
        public enum CornerStyles : byte {
            /// <summary>No corner styles.</summary>
            None = 0,
            /// <summary>Top-left corner is beveled.</summary>
            TopLeftBeveled = 1,
            /// <summary>Top-left corner is rounded.</summary>
            TopLeftRounded = 2,
            /// <summary>Top-right corner is beveled.</summary>
            TopRightBeveled = 4,
            /// <summary>Top-right corner is rounded.</summary>
            TopRightRounded = 8,
            /// <summary>Bottom-right corner is beveled.</summary>
            BottomRightBeveled = 16,
            /// <summary>Bottom-right corner is rounded.</summary>
            BottomRightRounded = 32,
            /// <summary>Bottom-left corner is beveled.</summary>
            BottomLeftBeveled = 64,
            /// <summary>Bottom-left corner is rounded.</summary>
            BottomLeftRounded = 128,
            /// <summary>All corners are beveled.</summary>
            AllBeveled = TopLeftBeveled | TopRightBeveled | BottomRightBeveled | BottomLeftBeveled,
            /// <summary>All corners are rounded.</summary>
            AllRounded = TopLeftRounded | TopRightRounded | BottomRightRounded | BottomLeftRounded
        }*/

    }

}
