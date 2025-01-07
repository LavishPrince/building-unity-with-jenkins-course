using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO.Compression;
using System.IO;
using System;

public class BuildScript
{
    public static void BuildWindowsCommandLine()
    {
        // Define the path where the build will be placed
        string buildPath = "Builds/Windows/MyGame.exe";

        // Create the directory if it doesn't exist
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(buildPath));

        // Get the scenes included in the build settings
        string[] scenes = GetScenes();

        // Build player options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CompressWithLz4
        };

        // Start the build process
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // Handle the result of the build process
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize / (1024 * 1024)} MB");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed!");
        }

        // Zip the build folder
        ZipBuild("Builds/Windows");
    }

    private static string[] GetScenes()
    {
        // Collect all enabled scenes in the build settings
        int sceneCount = EditorBuildSettings.scenes.Length;
        string[] scenes = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    private static void ZipBuild(string buildPath)
    {
        string zipPath = buildPath + ".zip";
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }

        // Create a zip archive of the build folder
        ZipFile.CreateFromDirectory(buildPath, zipPath);
    }

    public static void BuildAndroid()
    {
        string[] args = Environment.GetCommandLineArgs();
        string buildType = GetArgument(args, "-buildType");

        string path = buildType switch
        {
            "APK" => "Builds/AndroidAPK/TestGame.apk",
            "AAB" => "Builds/AndroidAAB/TestGame.aab",
            _ => null
        };

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Invalid or missing build type specified.");
            return;
        }

        // Ensure the directory exists
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));

        // Build player options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        // Configure Android-specific settings
        PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)25;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;
        PlayerSettings.Android.useAPKExpansionFiles = buildType == "AAB";
        PlayerSettings.Android.useCustomKeystore = false;
        // PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("TEST_PROJECT_KEYSTORE_FILE");
        // PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        // PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("ALIAS_NAME");
        // PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("ALIAS_PASS");
        EditorUserBuildSettings.buildAppBundle = buildType == "AAB";

        // Start the build process
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

    }

    public static void BuildIOS()
    {
        string path = "Builds/iOS";
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
        PlayerSettings.iOS.buildNumber = "1";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = path,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    private static string GetArgument(string[] args, string name)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }

        return null;
    }
}

