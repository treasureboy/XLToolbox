﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XLToolbox.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://xltoolbox.net")]
        public string WebsiteUrl {
            get {
                return ((string)(this["WebsiteUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://xltoolbox.net/version-ng.txt")]
        public string VersionInfoUrl {
            get {
                return ((string)(this["VersionInfoUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://xltoolbox.net/")]
        public string HelpUrl {
            get {
                return ((string)(this["HelpUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://xltoolbox.net/receive.php")]
        public string ExceptionPostUrl {
            get {
                return ((string)(this["ExceptionPostUrl"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DownloadPath {
            get {
                return ((string)(this["DownloadPath"]));
            }
            set {
                this["DownloadPath"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Daniel\'s XL Toolbox")]
        public string AddinName {
            get {
                return ((string)(this["AddinName"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Point")]
        public global::XLToolbox.Export.Models.Unit ExportUnit {
            get {
                return ((global::XLToolbox.Export.Models.Unit)(this["ExportUnit"]));
            }
            set {
                this["ExportUnit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ExportPath {
            get {
                return ((string)(this["ExportPath"]));
            }
            set {
                this["ExportPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::XLToolbox.Export.Models.Preset ExportPreset {
            get {
                return ((global::XLToolbox.Export.Models.Preset)(this["ExportPreset"]));
            }
            set {
                this["ExportPreset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::XLToolbox.Export.Models.BatchExportSettings BatchExportSettings {
            get {
                return ((global::XLToolbox.Export.Models.BatchExportSettings)(this["BatchExportSettings"]));
            }
            set {
                this["BatchExportSettings"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://xltoolbox.net/donate")]
        public string DonateUrl {
            get {
                return ((string)(this["DonateUrl"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://xltoolbox.net/blog/tags/alpha")]
        public string WhatsNewUrl {
            get {
                return ((string)(this["WhatsNewUrl"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool WindowManagerAlwaysOnTop {
            get {
                return ((bool)(this["WindowManagerAlwaysOnTop"]));
            }
            set {
                this["WindowManagerAlwaysOnTop"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::XLToolbox.Csv.CsvFile CsvImport {
            get {
                return ((global::XLToolbox.Csv.CsvFile)(this["CsvImport"]));
            }
            set {
                this["CsvImport"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::XLToolbox.Csv.CsvFile CsvExport {
            get {
                return ((global::XLToolbox.Csv.CsvFile)(this["CsvExport"]));
            }
            set {
                this["CsvExport"] = value;
            }
        }
    }
}
