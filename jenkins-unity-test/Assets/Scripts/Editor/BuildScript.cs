using UnityEditor;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System;

public class BuildScript
{
    public static void BuildWindows()
    {
        string path = "Builds/Windows";
        CreateDirectory(path);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = $"{path}/TestGame.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        ZipBuild(path);
    }

    public static void BuildWebGL()
    {
        string path = "Builds/WebGL";
        CreateDirectory(path);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = path,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        ZipBuild(path);
    }

    public static void BuildAndroid()
    {
        string[] args = Environment.GetCommandLineArgs();
        string buildType = GetArgument(args, "-buildType");

        string path = (buildType == "APK") ? "Builds/AndroidAPK/TestGame.apk" : (buildType == "AAB") ? "Builds/AndroidAAB/TestGame.aab" : "Unknown";
        
        if (path == "Unknown") return;

        CreateDirectory(path);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)25;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;

        PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable("TEST_PROJECT_KEYSTORE_FILE");
        PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("KEYSTORE_PASS");
        PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("ALIAS_NAME");
        PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("ALIAS_PASS");
        PlayerSettings.Android.bundleVersionCode = Int32.Parse(Environment.GetEnvironmentVariable("BUILD_NUMBER"));
        PlayerSettings.Android.useAPKExpansionFiles = (buildType == "AAB") ? true : false;

        EditorUserBuildSettings.buildAppBundle = (buildType == "AAB") ? true : false;

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    public static void BuildIOS()
    {
        string path = "Builds/iOS";
        CreateDirectory(path);
        PlayerSettings.iOS.buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = path,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        ZipBuild(path);
    }

    public static void CreateDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    private static void ZipBuild(string buildPath)
    {
        string zipPath = buildPath + ".zip";
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }
        ZipFile.CreateFromDirectory(buildPath, zipPath);
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
