﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApplicationTeacher.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("15")]
        public int TimeBetweenDemand {
            get {
                return ((int)(this["TimeBetweenDemand"]));
            }
            set {
                this["TimeBetweenDemand"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public int ScreenToShareId {
            get {
                return ((int)(this["ScreenToShareId"]));
            }
            set {
                this["ScreenToShareId"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string PathToSaveFolder {
            get {
                return ((string)(this["PathToSaveFolder"]));
            }
            set {
                this["PathToSaveFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsd=\"http://www.w3." +
            "org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <s" +
            "tring>chrome</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection IgnoredProcesses {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["IgnoredProcesses"]));
            }
            set {
                this["IgnoredProcesses"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsd=\"http://www.w3." +
            "org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <s" +
            "tring>steam</string>\r\n  <string>discord</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AlertedProcesses {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AlertedProcesses"]));
            }
            set {
                this["AlertedProcesses"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <string>porn</string>
  <string>xxx</string>
  <string>game</string>
  <string>jeux</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AlertedUrls {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AlertedUrls"]));
            }
            set {
                this["AlertedUrls"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsd=\"http://www.w3." +
            "org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n  <s" +
            "tring>stackoverflow.com</string>\r\n  <string>github.com</string>\r\n</ArrayOfString" +
            ">")]
        public global::System.Collections.Specialized.StringCollection AutorisedWebsite {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AutorisedWebsite"]));
            }
            set {
                this["AutorisedWebsite"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2000")]
        public int DefaultTimeout {
            get {
                return ((int)(this["DefaultTimeout"]));
            }
            set {
                this["DefaultTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string AllFocus {
            get {
                return ((string)(this["AllFocus"]));
            }
            set {
                this["AllFocus"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string StreamOptions {
            get {
                return ((string)(this["StreamOptions"]));
            }
            set {
                this["StreamOptions"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string StudentToShareScreen {
            get {
                return ((string)(this["StudentToShareScreen"]));
            }
            set {
                this["StudentToShareScreen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool FilterEnabled {
            get {
                return ((bool)(this["FilterEnabled"]));
            }
            set {
                this["FilterEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Specialized.StringCollection IgnoredUrls {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["IgnoredUrls"]));
            }
            set {
                this["IgnoredUrls"] = value;
            }
        }
    }
}
