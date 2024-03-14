﻿// Copyright 2023 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using JetBrains.Annotations;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
#pragma warning disable SYSLIB0011

namespace Nuke.Common.Tooling;

[PublicAPI]
public static partial class SettingsEntityExtensions
{
    public static T NewInstance<T>(this T settingsEntity)
        where T : ISettingsEntity
    {
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);

        var binaryFormatter = new BinaryFormatter();

        using var memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, settingsEntity);
        memoryStream.Seek(offset: 0, loc: SeekOrigin.Begin);

        var newInstance = (T)binaryFormatter.Deserialize(memoryStream);
        if (newInstance is ToolSettings toolSettings)
        {
            toolSettings.ProcessArgumentConfigurator = ((ToolSettings)(object)settingsEntity).ProcessArgumentConfigurator;
            toolSettings.ProcessLogger = ((ToolSettings)(object)settingsEntity).ProcessLogger;
            toolSettings.ProcessExitHandler = ((ToolSettings)(object)settingsEntity).ProcessExitHandler;
        }

        return newInstance;
    }
}
