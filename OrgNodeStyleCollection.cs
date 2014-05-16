
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    /// <summary>Represents a collection of <see cref="CheckBoxStudio.WinForms.OrgNodeStyle"/> objects.</summary>
    [Editor(typeof(OrgNodeStyleCollectionEditor), typeof(UITypeEditor))]
    public class OrgNodeStyleCollection : IList {

        private OrgPanel panel = null;
        private List<OrgNodeStyle> items = new List<OrgNodeStyle>();

        internal OrgNodeStyleCollection (OrgPanel panel) {

            this.panel = panel;
            
        }
        
        IEnumerator IEnumerable.GetEnumerator () {

            return this.GetEnumerator();

        }

        /// <summary>Returns an enumerator that iterates through the node style collection.</summary>
        public IEnumerator<OrgNodeStyle> GetEnumerator () {

            return items.GetEnumerator();

        }

        /// <summary>Gets a value indicating whether the node style collection is read-only.</summary>
        public bool IsReadOnly {

            get { return ((IList)items).IsReadOnly; }

        }

        /// <summary>Gets the number of node styles contained in the collection.</summary>
        public int Count {

            get { return items.Count; }

        }

        /// <summary>Adds the specified node style to the end of the collection.</summary>
        /// <param name="style">The node style to add to the collection.</param>
        public void Add (OrgNodeStyle style) {

            this.Insert(this.Count, style);

        }

        /// <summary>Inserts the specified node style into the collection at the specified index.</summary>
        /// <param name="index">The location within the collection to insert the node style.</param>
        /// <param name="style">The node style to insert into the collection.</param>
        public void Insert (int index, OrgNodeStyle style) {

            OrgNodeStyleCollection.Validate(panel, style);

            style.panel = panel;
            items.Insert(index, style);

            panel.PerformLayout();

        }

        /// <summary>Returns the index of the specified node style in the collection.</summary>
        /// <param name="style">The node style to locate in the collection.</param>
        public int IndexOf (OrgNodeStyle style) {

            return items.IndexOf(style);

        }

        /// <summary>Gets the node style at the specified index in the collection.</summary>
        /// <param name="index">The location of the node style in the collection.</param>
        public OrgNodeStyle this[int index] {

            get { return items[index]; }
            internal set { items[index] = value; }

        }

        /// <summary>Gets the node style with the specified name in the collection.></summary>
        /// <param name="name">The name of the node style in the collection.</param>
        public OrgNodeStyle this[string name] {

            get {

                foreach (OrgNodeStyle style in items)
                    if (style.Name == name)
                        return style;

                return null;

            }

        }

        /// <summary>Determines whether the collection contains the specified node style.</summary>
        /// <param name="style">The node style to locate in the collection.</param>
        public bool Contains (OrgNodeStyle style) {

            return items.Contains(style);

        }

        /// <summary>Copies the node style collection to the specified array, starting with the specified array index.</summary>
        /// <param name="array">The array that will contain the copied elements.</param>
        /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
        public void CopyTo (OrgNodeStyle[] array, int arrayIndex) {

            items.CopyTo(array, arrayIndex);

        }

        /// <summary>Removes a node style from the collection at the specified index.</summary>
        /// <param name="index">The location of the node style in the collection.</param>
        public void RemoveAt (int index) {

            this.Remove(this[index]);

        }

        /// <summary>Removes the specified node style from the collection.</summary>
        /// <param name="style">The node style to remove from the collection.</param>
        public bool Remove (OrgNodeStyle style) {

            bool value = items.Remove(style);
            style.panel = null;

            panel.PerformLayout();

            return value;

        }

        /// <summary>Removes all node styles from the collection.</summary>
        public void Clear () {

            panel.SuspendLayout();

            while (this.Count > 0)
                this.RemoveAt(0);

            panel.ResumeLayout(true);

        }

        bool IList.IsFixedSize {

            get { return ((IList)items).IsFixedSize; }

        }

        bool IList.IsReadOnly {

            get { return ((IList)items).IsReadOnly; }

        }

        bool ICollection.IsSynchronized {

            get { return ((ICollection)items).IsSynchronized; }

        }

        object ICollection.SyncRoot {

            get { return ((ICollection)items).SyncRoot; }

        }

        object IList.this[int index] {

            get { return this[index]; }
            set { this[index] = (OrgNodeStyle)value; }

        }

        int IList.Add (object value) {

            this.Add((OrgNodeStyle)value);

            return this.Count - 1;

        }

        void IList.Insert (int index, object value) {

            this.Insert(index, (OrgNodeStyle)value);

        }

        bool IList.Contains (object value) {

            return this.Contains((OrgNodeStyle)value);

        }

        int IList.IndexOf (object value) {

            return this.IndexOf((OrgNodeStyle)value);

        }

        void IList.Remove (object value) {

            this.Remove((OrgNodeStyle)value);

        }

        void ICollection.CopyTo (Array array, int index) {

            this.CopyTo((OrgNodeStyle[])array, index);

        }

        private static void Validate (OrgPanel panel, OrgNodeStyle style) {

            if (style == null)
                throw new ArgumentException("Specified node style cannot be null.");

            if (style.Panel != null)
                throw new ArgumentException("Specified node style is already assigned to a panel.");

            if (panel.NodeStyles.Contains(style))
                throw new ArgumentException("Specified node style already exists in the collection.");

        }

    }

    internal class OrgNodeStyleCollectionEditor : CollectionEditor {

        public OrgNodeStyleCollectionEditor () : base (typeof(OrgNodeStyleCollection)) {



        }

        protected override CollectionForm CreateCollectionForm () {

            CollectionForm form = base.CreateCollectionForm();

            form.Size = new Size(form.Width + 100, form.Height + 50);
            ShowHelp(form);

            return form;

        }

        private static void ShowHelp (Control control) {

            PropertyGrid grid = control as PropertyGrid;

            if (grid != null)
                grid.HelpVisible = true;

            foreach (Control child in control.Controls)
                ShowHelp(child);
 
        }

    }

}
