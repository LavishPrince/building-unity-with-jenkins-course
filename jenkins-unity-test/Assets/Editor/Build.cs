using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO.Compression;
using System.IO;

public class BuildScript
{
    public static void BuildWindowsCommandLine()
    {
        // Define the path where the build will be placed.
        string buildPath = "Builds/Windows/MyGame.exe";

        // Create the directory if it doesn't exist.
        System.IO.Directory.CreateDirectory("Builds/Windows");

        // Get the scenes included in the build settings.
        string[] scenes = GetScenes();

        // Build player options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
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
        ZipBuild("Builds/Windows");
    }

    private static string[] GetScenes()
    {
        // Collect all enabled scenes in the build settings.
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
        ZipFile.CreateFromDirectory(buildPath, zipPath);
    }
}

