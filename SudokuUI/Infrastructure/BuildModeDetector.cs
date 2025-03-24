using System.Diagnostics;
using System.Reflection;

namespace SudokuUI.Infrastructure;

/// <summary>
/// Find out if an assembly is build in debug or release mode
/// (Source: https://www.hanselman.com/blog/how-to-programmatically-detect-if-an-assembly-is-compiled-in-debug-or-release-mode)
/// </summary>

public static class BuildModeDetector
{
    public enum BuildMode { Debug, Release, Other }

    public static BuildMode GetBuildMode(Assembly assembly)
    {
        var attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);

        if (attributes.Length == 0)
            return BuildMode.Release;

        foreach (var attr in attributes)
        {
            if (attr is DebuggableAttribute d)
            {
                if (d.IsJITOptimizerDisabled == true)
                    return BuildMode.Debug;
                else
                    return BuildMode.Release;
            }
        }

        return BuildMode.Other;
    }
}