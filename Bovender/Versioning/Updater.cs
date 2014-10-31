﻿using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Bovender.Mvvm;
using Bovender.Mvvm.Messaging;

namespace Bovender.Versioning
{
    /// <summary>
    /// Fetches version information from the internet and raises an UpdateAvailable
    /// event if a new version is available for download.
    /// </summary>
    /// <remarks>
    /// The current version information resides in a simple text file which contains
    /// four lines:              e.g.
    /// 1) Current version       7.0.0-alpha.1
    /// 2) Download URL          http://sourceforge.net/projects/xltoolbox/files/XL_Toolbox_7.0.0-alpha.1.exe
    /// 3) Sha1 of executable    1234abcd...
    /// 4) Version description   This is the first release of the next generation Toolbox
    /// </remarks>
    public abstract class Updater
    {
        #region Public properties

        /// <summary>
        /// An MVVM message whose Send event a view can listen for in
        /// order to have the user indicate the desired download destination
        /// folder.
        /// </summary>
        public Message<StringMessageContent> DownloadDestinationMessage
        {
            get
            {
                if (_downloadDestinationMessage == null)
                {
                    _downloadDestinationMessage = new Message<StringMessageContent>();
                };
                return _downloadDestinationMessage;
            }
        }

        public string DownloadPath { get; set; }
        public bool Downloaded { get; private set; }
        public SemanticVersion NewVersion
        {
            get
            {
                return UpdateArgs.NewVersion;
            }
        }

        public string UpdateDescription
        {
            get
            {
                return UpdateArgs.NewVersionInfo;
            }
        }

        public Uri DownloadUri
        {
            get
            {
                return UpdateArgs.DownloadUrl;
            }
        }

        #endregion

        #region Commands

        public DelegatingCommand DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                {
                    _downloadCommand = new DelegatingCommand(
                        (param) => DoDownload(),
                        (param) => CanDownload()
                        );
                }
                return _downloadCommand;
            }
        }

        #endregion

        #region Private fields

        private DelegatingCommand _downloadCommand;
        private Message<StringMessageContent> _downloadDestinationMessage;
        private WebClient _client;
        private WebClient _infoClient;
        private UpdateAvailableEventArgs UpdateArgs { get; set; }
        private string Sha1 { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Signals that an updated version is available for download.
        /// </summary>
        public event EventHandler<UpdateAvailableEventArgs> UpdateAvailable;

        /// <summary>
        /// Signals that an update check was successfully performed, but no new
        /// update is available.
        /// </summary>
        public event EventHandler<UpdateAvailableEventArgs> NoUpdateAvailable;

        /// <summary>
        /// Signals that the version information could not be downloaded from the internet.
        /// </summary>
        public event EventHandler<DownloadStringCompletedEventArgs> FetchingVersionFailed;

        /// <summary>
        /// Signals a change in the download process of the executable file. This event is
        /// chained from WebClient's event with the same name.
        /// </summary>
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        /// <summary>
        /// Signals that the new release has been downloaded, verified and is ready to install.
        /// </summary>
        public event EventHandler<UpdateAvailableEventArgs> UpdateInstallable;

        /// <summary>
        /// Signals that the downloaded file could not be verified.
        /// </summary>
        public event EventHandler<UpdateAvailableEventArgs> DownloadFailedVerification;

        #endregion

        #region Abstract methods

        /// <summary>
        /// Returns the URI for the file that provides current version information.
        /// </summary>
        /// <returns>URI for version info file.</returns>
        protected abstract Uri GetVersionInfoUri();

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether the current user is authorized to write to the folder
        /// where the addin files are stored. If the user does not have write permissions,
        /// he/she cannot update the addin by herself/hisself.
        /// </summary>
        public bool IsAuthorized
        {
            get
            {
                string addinPath = AppDomain.CurrentDomain.BaseDirectory;
                /* Todo: compute permissions, rather than try and catch */
                try
                {
                    using (FileStream f = new FileStream(Path.Combine(addinPath, "xltbupd.test"),
                        FileMode.Create, FileAccess.Write))
                    {
                        f.WriteByte(0xff);
                    };
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Downloads the current version information file asynchronously from the project
        /// home page.
        /// </summary>
        /// <remarks>
        /// Eventually triggers the UpdateAvailable or NoUpdateAvailable events if the current version
        /// information was downloaded successfully; and triggers the FetchingVersionFailed
        /// event if the version information could not be downloaded.
        /// </remarks>
        public void FetchVersionInformation()
        {
            _infoClient = new WebClient();
            _infoClient.DownloadStringCompleted += downloadTxt_DownloadStringCompleted;
            _infoClient.DownloadStringAsync(GetVersionInfoUri());
        }

        public void CancelFetchVersionInformation()
        {
            if (_infoClient != null)
            {
                _infoClient.CancelAsync();
            }
        }

        /// <summary>
        /// Downloads the current release from the internet.
        /// </summary>
        public void DownloadUpdate(string targetDir)
        {
            // Extract the file name from the SourceForge URL
            string fn;
            Regex r = new Regex(@"(?<fn>[^/]+?exe)");
            Match m = r.Match(UpdateArgs.DownloadUrl.ToString());
            if (m.Success)
            {
                fn = m.Groups["fn"].Value;
            }
            else
            {
                fn = String.Format("XL_Toolbox_{0}.exe", NewVersion.ToString());
            };
            DownloadPath = Path.Combine(targetDir, fn);

            /* Check if the file exists already. If the Sha1 is identical,
             * do not download it again. If the Sha1 is different, it is a file
             * with the same name, but different content (broken download?).
             */
            if (File.Exists(DownloadPath))
            {
                ComputeSha1();
                if (Sha1 == UpdateArgs.Sha1)
                {
                    OnUpdateInstallable();
                    return;
                }
            }

            _client = new WebClient();
            _client.DownloadProgressChanged += _client_DownloadProgressChanged;
            _client.DownloadFileCompleted += _client_DownloadFileCompleted;
            _client.DownloadFileAsync(UpdateArgs.DownloadUrl, DownloadPath);
        }

        public void CancelDownload()
        {
            _client.CancelAsync();
        }

        public void InstallUpdate()
        {
            // Compute the SHA1 again so we know it's current.
            ComputeSha1();
            if (Sha1 == UpdateArgs.Sha1)
            {
                System.Diagnostics.Process.Start(DownloadPath, "/UPDATE");
            }
            else
            {
                OnDownloadFailedVerification();
            }
        }

        #endregion

        #region Private methods

        void _client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled) {
                ComputeSha1();
                if (Sha1 == UpdateArgs.Sha1)
                {
                    OnUpdateInstallable();
                }
                else
                {
                    OnDownloadFailedVerification();
                    /* throw new DownloadCorruptException(String.Format(
                        "Checksum of downloaded file {0} does not match expected checksum {1}",
                        Sha1, UpdateArgs.Sha1)); */
                };
            }
            else
            {
                System.IO.File.Delete(DownloadPath);
            }
        }

        void _client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(this, e);
            }
        }

        /// <summary>
        /// Inspects the downloaded version information.
        /// </summary>
        /// <param name="sender">System.Net.WebClient instance</param>
        /// <param name="e">Event arguments</param>
        void downloadTxt_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                StringReader r = new StringReader(e.Result);
                SemanticVersion v = new SemanticVersion(r.ReadLine());
                Uri url = new Uri(r.ReadLine());
                string sha1 = r.ReadLine();
                string info = r.ReadLine();

                // If a new version is available, raise the corresponding event.
                if (v > SemanticVersion.CurrentVersion())
                {
                    UpdateArgs = new UpdateAvailableEventArgs(v, info, url, sha1);
                    OnUpdateAvailable();
                }
                else
                {
                    OnNoUpdateAvailable();
                }
            }
            else
            {
                // Raise an event that signals failure.
                OnFetchingVersionFailed(e);
            }
        }

        /// <summary>
        /// Computes the Sha1 hash of the downloaded file.
        /// </summary>
        private void ComputeSha1()
        {
            Sha1 = FileHelpers.Sha1Hash(DownloadPath);
        }

        #endregion

        #region Protected methods

        protected virtual void DoDownload()
        {
            string defaultPath = Properties.Versioning.Default.DownloadPath;
            if (string.IsNullOrEmpty(defaultPath))
            {
                defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            };

        }

        /// <summary>
        /// Performs the actual download once the user has confirmed the download
        /// destination.
        /// </summary>
        protected virtual void ConfirmDownload()
        {

        }

        protected virtual bool CanDownload()
        {
            return IsAuthorized;
        }

        protected virtual void OnUpdateInstallable()
        {
            Downloaded = true;
            if (UpdateInstallable != null)
            {
                UpdateInstallable(this, UpdateArgs);
            }
        }

        protected virtual void OnDownloadFailedVerification()
        {
            if (DownloadFailedVerification != null)
            {
                DownloadFailedVerification(this, UpdateArgs);
            }
        }

        protected virtual void OnUpdateAvailable()
        {
            if (UpdateAvailable != null)
            {
                UpdateAvailable(this, UpdateArgs);
            }
        }

        protected virtual void OnNoUpdateAvailable()
        {
            if (NoUpdateAvailable != null)
            {
                NoUpdateAvailable(this, UpdateArgs);
            }
        }

        protected virtual void OnFetchingVersionFailed(DownloadStringCompletedEventArgs e)
        {
            if (FetchingVersionFailed != null)
            {
                FetchingVersionFailed(this, e);
            }
        }

        #endregion
    }
}
