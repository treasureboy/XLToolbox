﻿/* ScreenshotExporterViewModel.cs
 * part of Daniel's XL Toolbox NG
 * 
 * Copyright 2014-2015 Daniel Kraus
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bovender.Mvvm;
using Bovender.Mvvm.ViewModels;
using Bovender.Mvvm.Messaging;
using XLToolbox.Export;
using Xl = Microsoft.Office.Interop.Excel;

namespace XLToolbox.Export.ViewModels
{
    /// <summary>
    /// View model for the <see cref="ScreenshotExporter"/> class.
    /// Provides commands in accordance with the MVVM pattern used
    /// by Bovender.
    /// </summary>
    public class ScreenshotExporterViewModel : ViewModelBase
    {
        #region Commands

        public DelegatingCommand ExportSelectionCommand
        {
            get
            {
                if (_exportSelectionCommand == null)
                {
                    _exportSelectionCommand = new DelegatingCommand(
                        (param) => DoChooseFileName(),
                        (param) => CanExportSelection()
                        );
                }
                return _exportSelectionCommand;
            }
        }

        #endregion

        #region Messages

        public Message<FileNameMessageContent> ChooseFileNameMessage
        {
            get
            {
                if (_chooseFileNameMessage == null)
                {
                    _chooseFileNameMessage = new Message<FileNameMessageContent>();
                }
                return _chooseFileNameMessage;
            }
        }
        #endregion

        #region Private methods

        private void DoChooseFileName()
        {
            if (CanExportSelection())
            {
                ChooseFileNameMessage.Send(
                    new FileNameMessageContent("Portable Network Graphics (PNG)|*.png"),
                    DoExportSelection);
            }
        }

        private void DoExportSelection(FileNameMessageContent messageContent)
        {
            if (CanExportSelection() && messageContent.Confirmed)
            {
                ScreenshotExporter exporter = new ScreenshotExporter();
                exporter.ExportSelection(messageContent.Value);
            }
        }

        private bool CanExportSelection()
        {
            Xl.Application app = Excel.Instance.ExcelInstance.Application;
            return !(app == null || app.Selection is Xl.Range);
        }

        #endregion

        #region Implementation of ViewModelBase

        public override object RevealModelObject()
        {
            return null;
        }

        #endregion

        #region Private fields

        private DelegatingCommand _exportSelectionCommand;
        private Message<FileNameMessageContent> _chooseFileNameMessage;

        #endregion
    }
}