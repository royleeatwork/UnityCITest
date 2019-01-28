using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public static class Builder
{
    public class EditorSetup
    {
        public static string AndroidSdkRoot
        {
            get { return EditorPrefs.GetString("AndroidSdkRoot"); }
            set { EditorPrefs.SetString("AndroidSdkRoot", value); }
        }

        public static string JdkRoot
        {
            get { return EditorPrefs.GetString("JdkPath"); }
            set { EditorPrefs.SetString("JdkPath", value); }
        }

        // For IL2CPP
        public static string AndroidNdkRoot
        {
            get { return EditorPrefs.GetString("AndroidNdkRoot"); }
            set { EditorPrefs.SetString("AndroidNdkRoot", value); }
        }


    }

    public static string GetOutputPath(string defaultPath = null)
    {
        if (!InternalEditorUtility.inBatchMode)
        {
            return defaultPath;
        }

        string[] args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; ++i)
        {
            if (args[i].Equals("-output") && i < args.Length - 1)
            {
                return args[i + 1];
            }
        }
        return defaultPath;
    }

    static BuildPlayerOptions GetBuildOptions(BuildTarget buildTarget, string outputPath)
    {
        var buildScenes = EditorBuildSettings.scenes.Where(v => v.enabled).Select(v => v.path);
        var buildPlayerOptions = new BuildPlayerOptions()
        {
            target = buildTarget,
            scenes = buildScenes.ToArray(),
            options = BuildOptions.None,
            locationPathName = outputPath,
        };

        return buildPlayerOptions;
    }

    #region AndroidBuild
    static void SetupAPKKeys()
    {
        // PlayerSettings.Android.keystoreName = "myKey.keystore";
        // PlayerSettings.Android.keyaliasName = "";
        // PlayerSettings.Android.keystorePass = "";
        // PlayerSettings.Android.keyaliasPass = "";
    }

    static void SetupXDKPath()
    {
        EditorSetup.AndroidSdkRoot = @"C:\Users\Bomding\AppData\Local\Android\Sdk";
        //EditorSetup.AndroidNdkRoot = @"C:\android-ndk-r10e";
        EditorSetup.JdkRoot = @"C:\Program Files\Java\jdk1.8.0_202";
    }

    public static void BuildAndroidAPK()
    {
        SetupAPKKeys();
        SetupXDKPath();

        var outputDir = GetOutputPath();
        var outputPath = Path.GetFullPath(outputDir + Path.DirectorySeparatorChar + Application.productName + ".apk");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        var buildPlayerOptions = GetBuildOptions(BuildTarget.Android, outputPath);

        var result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        foreach (var f in result.files)
        {
            Debug.Log(f);
        }

        if (!InternalEditorUtility.inBatchMode)
        {
            System.Diagnostics.Process.Start(outputDir);
        }
    }
    #endregion

    [MenuItem("Test/Builder Test")]
    public static void TestFunction()
    {
        BuildAndroidAPK();
    }
    public static void BuildPC()
    {
        var outputDir = GetOutputPath();
        var outputPath = Path.GetFullPath(outputDir + Path.DirectorySeparatorChar + Application.productName + ".exe");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        var buildPlayerOptions = GetBuildOptions(BuildTarget.StandaloneWindows, outputPath);

        var result = BuildPipeline.BuildPlayer(buildPlayerOptions);

        foreach (var f in result.files)
        {
            Debug.Log(f);
        }

        if (!InternalEditorUtility.inBatchMode)
        {
            System.Diagnostics.Process.Start(outputDir);
        }
    }

}