﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using Bovender.Mvvm;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm.ViewModels;
using XLToolbox.Excel.ViewModels;
using XLToolbox.Excel.Instance;
using XLToolbox.WorkbookStorage;
using XLToolbox.Export.Models;

namespace XLToolbox.Export.ViewModels
{
    /// <summary>
    /// View model for the <see cref="Settings"/> class.
    /// </summary>
    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class BatchExportSettingsViewModel : SettingsViewModelBase
    {
        #region Factory

        public static BatchExportSettingsViewModel FromLastUsed()
        {
            return Properties.Settings.Default.BatchExportSettingsViewModel;
        }

        public static BatchExportSettingsViewModel FromLastUsed(Workbook workbookContext)
        {
            Store store = new Store(workbookContext);
            BatchExportSettingsViewModel vm = store.Get<BatchExportSettingsViewModel>(
                typeof(BatchExportSettingsViewModel).ToString()
                );
            if (vm != null)
            {
                return vm;
            }
            else
            {
                return BatchExportSettingsViewModel.FromLastUsed();
            }
        }

        #endregion

        #region Public properties

        public EnumProvider<BatchExportScope> Scope
        {
            get
            {
                if (_scope == null)
                {
                    _scope = new EnumProvider<BatchExportScope>();
                    _scope.AsEnum = ((BatchExportSettings)Settings).Scope;
                }
                return _scope;
            }
        }

        public EnumProvider<BatchExportLayout> Layout
        {
            get
            {
                if (_layout == null)
                {
                    _layout = new EnumProvider<BatchExportLayout>();
                    _layout.AsEnum = ((BatchExportSettings)Settings).Layout;
                }
                return _layout;
            }
        }
        
        public EnumProvider<BatchExportObjects> Objects
        {
            get
            {
                if (_objects == null)
                {
                    _objects = new EnumProvider<BatchExportObjects>();
                    _objects.AsEnum = ((BatchExportSettings)Settings).Objects;
                }
                return _objects;
            }
        }

        public string Path
        {
            get { return ((BatchExportSettings)Settings).Path; }
            set
            {
                ((BatchExportSettings)Settings).Path = value;
                OnPropertyChanged("Path");
            }
        }
       
        #endregion

        #region Commands

        /// <summary>
        /// Causes the <see cref="ChooseFolderMessage"/> to be sent.
        /// Upon confirmation of the message by a view, the Export
        /// process will be started.
        /// </summary>
        public DelegatingCommand ChooseFolderCommand
        {
            get
            {
                if (_chooseFolderCommand == null)
                {
                    _chooseFolderCommand = new DelegatingCommand(
                        param => DoChooseFolder());
                }
                return _chooseFolderCommand;
            }
        }

        #endregion

        #region Messages

        public Message<StringMessageContent> ChooseFolderMessage
        {
            get
            {
                if (_chooseFolderMessage == null)
                {
                    _chooseFolderMessage = new Message<StringMessageContent>();
                }
                return _chooseFolderMessage;
            }
        }

        #endregion

        #region Constructors

        public BatchExportSettingsViewModel()
            : base()
        {
            Settings = new BatchExportSettings();
        }

        #endregion

        #region Implementation of SettingsViewModelBase

        /// <summary>
        /// Determines the suggested target directory and sends the
        /// ChooseFileNameMessage.
        /// </summary>
        private void DoChooseFolder()
        {
            ChooseFolderMessage.Send(
                new StringMessageContent(GetExportPath()),
                (content) => ConfirmFolder(content)
            );
        }

        protected override void DoExport()
        {
            if (CanExport())
            {
                // TODO: Make export asynchronous
                ProcessMessageContent pcm = new ProcessMessageContent();
                pcm.IsIndeterminate = true;
                ExportProcessMessage.Send(pcm);
                Exporter exporter = new Exporter();
                exporter.ExportBatch(Settings as BatchExportSettings);
                pcm.CompletedMessage.Send(pcm);
            }
        }

        protected override bool CanExport()
        {
            SelectionViewModel svm = new SelectionViewModel(ExcelInstance.Application);
            return svm.Selection != null;
        }

        #endregion

        #region Private methods

        private void ConfirmFolder(StringMessageContent messageContent)
        {
            if (messageContent.Confirmed)
            {
                ((BatchExportSettings)Settings).Path = messageContent.Value;
                DoExport();
            }
        }

        #endregion

        #region Private fields

        private EnumProvider<BatchExportScope> _scope;
        private EnumProvider<BatchExportObjects> _objects;
        private EnumProvider<BatchExportLayout> _layout;
        private DelegatingCommand _chooseFolderCommand;
        private Message<StringMessageContent> _chooseFolderMessage;

        #endregion
    }
}
