﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace XLToolbox.Export
{
    /// <summary>
    /// Repository for export settings, is concerned with storing and
    /// retrieving a collection of <see cref="ExportSettings"/>.
    /// </summary>
    [Serializable]
    public class SettingsRepository : IDisposable
    {
        #region Public properties

        public ObservableCollection<Settings> ExportSettings { get; set; }

        #endregion

        #region Methods

        #endregion

        #region Constructor

        public SettingsRepository()
            : base ()
        {
            // Must initialize the ExportSettings property, lest a null pointer
            // exception is thrown in the LoadSettings() method.
            ExportSettings = new ObservableCollection<Export.Settings>();
            LoadSettings();
        }

        #endregion

        #region Add and remove

        public void Add(Settings exportSettings)
        {
            ExportSettings.Add(exportSettings);
        }

        public void Remove(Settings exportSettings)
        {
            ExportSettings.Remove(exportSettings);
        }

        #endregion

        #region Load and save

        protected virtual void LoadSettings()
        {
            using (IsolatedStorageFile store = GetIsolatedStorageFile())
            {
                try
                {
                    if (store.FileExists(ISOSTOREFILENAME))
                    {
                        IsolatedStorageFileStream stream = store.OpenFile(ISOSTOREFILENAME,
                            System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                        // The line below would fail if ExportSettings is null; however,
                        // the property is initialized in the constructor.
                        XmlSerializer serializer = new XmlSerializer(ExportSettings.GetType());
                        ExportSettings = serializer.Deserialize(stream) as ObservableCollection<Settings>;
                        stream.Close();
                    }
                    else
                    {
                        ExportSettings = new ObservableCollection<Settings>();
                    }
                }
                catch (Exception e)
                {
                    throw new StoreException("Cannot read export settings.", e);
                }
            }
        }

        protected virtual void SaveSettings()
        {
            try
            {
                using (IsolatedStorageFile store = GetIsolatedStorageFile())
                {
                    IsolatedStorageFileStream stream = store.CreateFile(ISOSTOREFILENAME);
                    XmlSerializer serializer = new XmlSerializer(ExportSettings.GetType());
                    serializer.Serialize(stream, ExportSettings);
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                throw new StoreException("Cannot write export settings.", e);
            }
        }

        private IsolatedStorageFile GetIsolatedStorageFile()
        {
            return IsolatedStorageFile.GetStore(
                IsolatedStorageScope.Roaming | IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        }

        #endregion

        #region Disposal

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                SaveSettings();
                _disposed = true;
            }
        }

        ~SettingsRepository()
        {
            Dispose(false);
        }

        #endregion

        #region Private fields

        bool _disposed;

        #endregion

        #region Private constants

        string ISOSTOREFILENAME = "export_settings.xml";
        #endregion
    }
}
