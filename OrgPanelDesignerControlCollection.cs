
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    [ListBindable(false)]
    [DesignerSerializer(typeof(OrgPanelDesignerControlCollectionSerializer), typeof(CodeDomSerializer))]
    internal class OrgPanelDesignerControlCollection : OrgPanelControlCollection, IList {

        OrgPanelControlCollection items = null;

        public OrgPanelDesignerControlCollection (OrgPanel owner) : base(owner) {

            items = owner.Controls;

        }

        public override int Count {

            get { return items.Count; }

        }

        object ICollection.SyncRoot {

            get { return this; }

        }

        bool ICollection.IsSynchronized {

            get { return false; }

        }

        bool IList.IsFixedSize {

            get { return false; }

        }

        public new bool IsReadOnly {

            get { return items.IsReadOnly; }

        }

        int IList.Add (object control) {

            return ((IList)items).Add(control);

        }

        public override void Add (Control control) {

            items.Add(control);

        }

        public override void Add (Control control, Control parent) {

            items.Add(control, parent);

        }

        public override void Add (Control control, Control parent, string styleName) {

            items.Add(control, parent, styleName);

        }

        public override void AddRange (Control[] controls) {

            items.AddRange(controls);

        }

        bool IList.Contains (object control) {

            return ((IList)items).Contains(control);

        }

        public new void CopyTo (Array dest, int index) {

            items.CopyTo(dest, index);

        }

        public override bool Equals (object value) {

            return items.Equals(value);

        }

        public new IEnumerator GetEnumerator () {

            return items.GetEnumerator();

        }

        public override int GetHashCode () {

            return items.GetHashCode();

        }

        int IList.IndexOf (object control) {

            return ((IList)items).IndexOf(control);

        }

        void IList.Insert (int index, object value) {

            ((IList)items).Insert(index, value);

        }

        void IList.Remove (object control) {

            ((IList)items).Remove(control);

        }

        void IList.RemoveAt (int index) {

            ((IList)items).RemoveAt(index);

        }

        object IList.this[int index] {

            get { return ((IList)items)[index]; }
            set { throw new NotSupportedException(); }

        }

        public override int GetChildIndex (Control child, bool throwException) {

            return items.GetChildIndex(child, throwException);

        }

        public override void SetChildIndex (Control child, int newIndex) {

            items.SetChildIndex(child, newIndex);

        }

        public override void Clear () {

            for (int i = items.Count - 1; i >= 0; i--) { // only remove the sited non-inherited components

                IComponent comp = items[i];

                if (comp != null && comp.Site != null &&
                    TypeDescriptor.GetAttributes(comp).Contains(InheritanceAttribute.NotInherited))
                    items.RemoveAt(i);

            }

        }

    }

    // Custom code dom serializer for the DesignerControlCollection. We need this so we can filter out controls
    // that aren't sited in the host's container.
    internal class OrgPanelDesignerControlCollectionSerializer : OrgPanelControlCollectionSerializer {

        protected override object SerializeCollection (IDesignerSerializationManager manager, CodeExpression targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize) {

            List<IComponent> components = new List<IComponent>();

            foreach (object value in valuesToSerialize) {

                IComponent comp = (IComponent)value;

                if (comp != null && comp.Site != null && !(comp.Site is INestedSite))
                    components.Add(comp);

            }

            return base.SerializeCollection(manager, targetExpression, targetType, originalCollection, components);

        }

    }

}