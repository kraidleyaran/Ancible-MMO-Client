#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Ancible_Tools.Scripts.System
{
    [CreateAssetMenu(fileName = "Build Settings", menuName = "Ancible Tools/Build Settings")]
    public class BuildSettings : ScriptableObject
    {
        public string DevWindowPath;
        public string DevLinuxPath;
        public string DevMacOSPath;

        public void BuildDev()
        {
            BuildWindowsDev();
            BuildMacDev();
            BuildLinuxDev();
        }

        public void BuildWindowsDev()
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Game.unity" };
            buildPlayerOptions.locationPathName = $"{DevWindowPath}";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.Development | BuildOptions.CompressWithLz4HC | BuildOptions.ShowBuiltPlayer;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Windows Build Success! {summary.totalTime.Seconds} seconds - {summary.outputPath}");
            }
            else
            {
                if (summary.result == BuildResult.Failed)
                {
                    Debug.LogError($"Window Build Status - {summary.result}");
                }
                else
                {
                    Debug.LogWarning($"Windows Build Status - {summary.result}");
                }
            }
        }

        public void BuildLinuxDev()
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Game.unity" };
            buildPlayerOptions.locationPathName = $"{DevLinuxPath}";
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.options = BuildOptions.Development | BuildOptions.CompressWithLz4HC | BuildOptions.ShowBuiltPlayer;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Linux Build Success! {summary.totalTime.Seconds} seconds - {summary.outputPath}");
            }
            else
            {
                if (summary.result == BuildResult.Failed)
                {
                    Debug.LogError($"Linux Build Status - {summary.result}");
                }
                else
                {
                    Debug.LogWarning($"Linux Build Status - {summary.result}");
                }
            }
        }

        public void BuildMacDev()
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { "Assets/Scenes/Game.unity" };
            buildPlayerOptions.locationPathName = $"{DevMacOSPath}";
            buildPlayerOptions.target = BuildTarget.StandaloneOSX;
            buildPlayerOptions.options = BuildOptions.Development | BuildOptions.CompressWithLz4HC | BuildOptions.ShowBuiltPlayer;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"MacOS Build Success! {summary.totalTime.Seconds} seconds - {summary.outputPath}");
            }
            else
            {
                if (summary.result == BuildResult.Failed)
                {
                    Debug.LogError($"MacOS Build Status - {summary.result}");
                }
                else
                {
                    Debug.LogWarning($"MacOS Build Status - {summary.result}");
                }
            }
        }
    }
}
#endif