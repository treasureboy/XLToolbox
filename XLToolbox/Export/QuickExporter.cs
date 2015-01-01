﻿using System;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm.Actions;
using XLToolbox.Export.Models;
using XLToolbox.Export.ViewModels;
using XLToolbox.Excel.Instance;

namespace XLToolbox.Export
{
    /// <summary>
    /// Provides user-entry points for the 'quick' export
    /// functions that re-use previously used settings.
    /// </summary>
    public class QuickExporter
    {
        #region Public methods

        /// <summary>
        /// Exports the current selection using the last settings, if available.
        /// </summary>
        public void ExportSelection()
        {
            Preset p = Preset.FromLastUsed(
                ExcelInstance.Application.ActiveWorkbook);
            if (p == null)
            {
                Dispatcher.Execute(Command.ExportSelection);
            }
            else
            {
                SingleExportSettingsViewModel svm = new SingleExportSettingsViewModel(p);
                svm.ChooseFileNameMessage.Sent += ChooseFileNameMessage_Sent; 
                if (svm.ChooseFileNameCommand.CanExecute(null))
                {
                    svm.ChooseFileNameCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Performs a batch export using the last used settings, if available.
        /// </summary>
        public void ExportBatch()
        {
            BatchExportSettingsViewModel bvm = BatchExportSettingsViewModel.FromLastUsed(
                ExcelInstance.Application.ActiveWorkbook);
            if (bvm == null)
            {
                Dispatcher.Execute(Command.BatchExport);
            }
            else
            {
                bvm.ChooseFolderMessage.Sent += ChooseFolderMessage_Sent;
                if (bvm.ChooseFolderCommand.CanExecute(null))
                {
                    bvm.ChooseFolderCommand.Execute(null);
                }
            }
        }

        #endregion

        #region Private methods

        void ChooseFileNameMessage_Sent(object sender, MessageArgs<FileNameMessageContent> e)
        {
            ChooseFileSaveAction action = new ChooseFileSaveAction();
            action.Invoke(e);
        }

        void ChooseFolderMessage_Sent(object sender, MessageArgs<StringMessageContent> e)
        {
            ChooseFolderAction action = new ChooseFolderAction();
            action.Invoke(e);
        }

        #endregion
    }
}
