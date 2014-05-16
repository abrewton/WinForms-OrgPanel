
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;

namespace CheckBoxStudio.WinForms {

    internal class OrgPanelLicenseProvider : LicenseProvider {

        public override License GetLicense (LicenseContext context, Type type, object instance, bool allowExceptions) {

            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Acme\\HostKeys");
            // get license stored in registry
            //string key = context.GetSavedLicenseKey(type, Assembly.GetAssembly(type));
            //Assembly assembly = Assembly.GetAssembly(type);
            //System.Windows.Forms.MessageBox.Show(assembly.Location);
            //License license = base.GetLicense(context, type, instance, false);
            //System.Windows.Forms.MessageBox.Show(license.LicenseKey);

            bool valid = false;

            if (!valid)
                return null;

            return new OrgPanelLicense("");

        }

    }

    internal class OrgPanelLicense : License {

        private string key = null;

        public OrgPanelLicense (string key) {

            this.key = key;

        }

        public override string LicenseKey {

            get { return key; }

        }

        public override void Dispose () {

            key = null;

        }

    }

}
