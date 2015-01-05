﻿/* SubmissionCompletedAction.cs
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
using System.Windows;
using Bovender.Mvvm.Actions;

namespace XLToolbox.ExceptionHandler
{
    /// <summary>
    /// WPF action that is invoked when the exception report submission
    /// process is completed.
    /// </summary>
    class SubmissionCompletedAction : ProcessCompletedAction
    {
        protected override Window CreateSuccessWindow()
        {
            return Content.InjectInto<SubmissionSuccessView>();
        }

        protected override Window CreateFailureWindow()
        {
            throw new NotImplementedException();
        }

        protected override Window CreateCancelledWindow()
        {
            throw new NotImplementedException();
        }
    }
}