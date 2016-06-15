﻿/* QuickExporter.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2016 Daniel Kraus
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using Bovender.Mvvm.Messaging;
using Bovender.Mvvm.Actions;
using XLToolbox.Export.Models;
using XLToolbox.Export.ViewModels;
using XLToolbox.Excel.ViewModels;
using System.Windows;

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
            Logger.Info("ExportSelection");
            Preset preset = Preset.FromLastUsed(Instance.Default.Application.ActiveWorkbook);
            Logger.Info("Preset.FromLastUsed: {0}", preset);
            if (preset == null)
            {
                Dispatcher.Execute(Command.ExportSelection);
            }
            else
            {
                SingleExportSettings settings = SingleExportSettings.CreateForSelection(preset);
                SingleExportSettingsViewModel svm = new SingleExportSettingsViewModel(settings);
                svm.ChooseFileNameMessage.Sent += ChooseFileNameMessage_Sent;
                svm.ShowProgress.Sent += (sender, args) =>
                {
                    Logger.Info("Creating process view");
                    args.Content.CancelButtonText = Strings.Cancel;
                    args.Content.Caption = Strings.Export;
                    args.Content.CompletedMessage.Sent += (sender2, args2) =>
                    {
                        args.Content.CloseViewCommand.Execute(null);
                    };
                    args.Content.InjectAndShowInThread<Bovender.Mvvm.Views.ProcessView>();
                };
                svm.ProcessFailedMessage.Sent += (sender, args) =>
                {
                    Logger.Info("Received ExportFailedMessage, informing user");
                    MessageBox.Show(args.Content.Value, Strings.Export,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                };
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
            Logger.Info("ExportBatch");
            BatchExportSettingsViewModel bvm = BatchExportSettingsViewModel.FromLastUsed(
                Instance.Default.ActiveWorkbook);
            Logger.Info("BatchExportSettingsViewModel.FromLastUsed: {0}", bvm);
            if ((bvm != null) && bvm.ChooseFolderCommand.CanExecute(null))
            {
                bvm.ChooseFolderMessage.Sent += ChooseFolderMessage_Sent;
                bvm.ExportProcessMessage.Sent +=
                    (sender, args) =>
                    {
                        ProcessAction a = new ProcessAction();
                        a.Caption = Strings.BatchExport;
                        a.CancelButtonText = Strings.Cancel;
                        a.Invoke(args);
                    };
                bvm.ChooseFolderCommand.Execute(null);
            }
            else
            {
                // We did get a view model, but its ChooseFolderCommand is disabled,
                // which means that the selected batch export options are invalid
                // in the current context.
                if (bvm != null)
                {
                    bvm = new BatchExportSettingsViewModel();
                    // Do not 'sanitize' the export options, so that the user
                    // can see the selected, but disabled options.
                    bvm.InjectInto<Views.BatchExportSettingsView>().ShowDialog();
                }
                else
                {
                    Dispatcher.Execute(Command.BatchExport);
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

        void ChooseFolderMessage_Sent(object sender, MessageArgs<FileNameMessageContent> e)
        {
            ChooseFolderAction action = new ChooseFolderAction();
            action.Invoke(e);
        }

        #endregion

        #region Class logger

        private static NLog.Logger Logger { get { return _logger.Value; } }

        private static readonly Lazy<NLog.Logger> _logger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        #endregion
    }
}
