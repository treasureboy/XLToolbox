﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLToolbox.Export;

namespace XLToolbox.UnitTests.Export
{
    /// <summary>
    /// Overrides the LoadSettings() and SaveSettings() methods of the
    /// base class so the settings storage is not hampered during testing.
    /// </summary>
    class SettingsRepositoryForTesting : SettingsRepository
    {
        protected override void LoadSettings() { }
        protected override void SaveSettings() { }
    }
}
