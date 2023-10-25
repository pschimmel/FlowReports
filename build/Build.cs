using System;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NUnit;
using Nuke.GitHub;
using Serilog;

[GitHubActions(
  "continuous",
  GitHubActionsImage.WindowsLatest,
  AutoGenerate = true,
  FetchDepth = 0,
  WritePermissions = new[] { GitHubActionsPermissions.Packages },
  OnPushBranches = new[] { MasterBranch, DevelopmentBranch, ReleasesBranch },
  OnPullRequestBranches = new[] { ReleasesBranch },
  InvokedTargets = new[] { nameof(Pack) },
  EnableGitHubToken = true,
  ImportSecrets = new[]
  {
    nameof(NuGetApiKey)
  }
)]

// See examples on:
// https://anktsrkr.github.io/post/getting-started-with-nuke/
// https://anktsrkr.github.io/post/write-your-first-building-block-in-nuke/
// https://anktsrkr.github.io/post/manage-your-package-version-using-nuke/
// https://anktsrkr.github.io/post/manage-your-package-release-using-nuke-in-github/
// https://cfrenzel.com/publishing-nuget-nuke-appveyor/
// https://www.ariank.dev/create-a-github-release-with-nuke-build-automation-tool/
// https://blog.dangl.me/archive/escalating-automation-the-nuclear-option/
class Build : NukeBuild
{
  /// Support plugins are available for:
  ///   - JetBrains ReSharper        https://nuke.build/resharper
  ///   - JetBrains Rider            https://nuke.build/rider
  ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
  ///   - Microsoft VSCode           https://nuke.build/vscode

  public static int Main() => Execute<Build>(x => x.Pack);

  const string MasterBranch = "master";
  const string DevelopmentBranch = "development";
  const string ReleasesBranch = "releases/**";

  [Nuke.Common.Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

  // Gets the API key for the Nuget package. This is defined as GitHub secret on the Repository Level
  [Nuke.Common.Parameter("Nuget API key"), Secret()]
  readonly string NuGetApiKey;

  // URL on which the Nuget package can be published
  [Nuke.Common.Parameter("Public nuget repository")]
  readonly string NuGetApiUrl = "https://api.nuget.org/v3/index.json";

  [Nuke.Common.Parameter("Authors")]
  readonly string Authors;

  [Nuke.Common.Parameter("Copyright Details")]
  readonly string Copyright;

  [Nuke.Common.Parameter("NuGet Artifacts Type")]
  readonly string NuGetArtifactsType;

  [Nuke.Common.Parameter("Zip Artifacts Type")]
  readonly string ZipArtifactsType;

  [GitVersion]
  readonly GitVersion GitVersion;

  [GitRepository]
  readonly GitRepository GitRepository;

  [Solution(GenerateProjects = true)]
  readonly Solution Solution;

  static readonly string ApplicationName = "FlowReports";

  static GitHubActions GitHubActions => GitHubActions.Instance;

  static AbsolutePath ArtifactsDirectory => RootDirectory / ".artifacts";

  static AbsolutePath OutputDirectory => RootDirectory / "Output";

  static string ChangeLogFile => RootDirectory / "CHANGELOG.md";

  static string GithubNugetFeed => GitHubActions != null
     ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
     : null;

  Target Clean => _ => _
    .Description($"Cleaning project.")
    .Before(Restore)
    .Executes(() =>
    {
      DotNetTasks.DotNetClean(c => c.SetProject(Solution.FlowReports_TestApplication));
      DotNetTasks.DotNetClean(c => c.SetProject(Solution.FlowReports_UnitTests));
      ArtifactsDirectory.CreateOrCleanDirectory();
    });

  Target Restore => _ => _
    .Description($"Restoring project dependencies.")
    .DependsOn(Clean)
    .Executes(() =>
    {
      DotNetTasks.DotNetRestore(_ => _.SetProjectFile(Solution.FlowReports_TestApplication));
      DotNetTasks.DotNetRestore(_ => _.SetProjectFile(Solution.FlowReports_UnitTests));
      DotNetTasks.DotNetRestore(_ => _.SetProjectFile(Solution.FlowReports));
    });

  Target Compile => _ => _
    .Description($"Building project with the version.")
    .DependsOn(Restore)
    .Executes(() =>
    {
      DotNetTasks.DotNetBuild(b => b
       .SetProjectFile(Solution.FlowReports_TestApplication)
       .SetConfiguration(Configuration)
       .SetVersion(GitVersion.NuGetVersionV2)
       .SetAssemblyVersion(GitVersion.AssemblySemVer)
       .SetInformationalVersion(GitVersion.InformationalVersion)
       .SetFileVersion(GitVersion.AssemblySemFileVer)
       .EnableNoRestore());

      DotNetTasks.DotNetBuild(b => b
       .SetProjectFile(Solution.FlowReports_UnitTests)
       .SetConfiguration(Configuration)
       .SetVersion(GitVersion.NuGetVersionV2)
       .SetAssemblyVersion(GitVersion.AssemblySemVer)
       .SetInformationalVersion(GitVersion.InformationalVersion)
       .SetFileVersion(GitVersion.AssemblySemFileVer)
       .EnableNoRestore());
    });

  Target Test => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {
      AbsolutePath unitTestPath1 = OutputDirectory / "bin" / Configuration / "net7.0-windows" / "FlowReports.UnitTests.dll";
      AbsolutePath unitTestPath2 = OutputDirectory / "bin" / Configuration / "net48" / "FlowReports.UnitTests.dll";

      NUnitTasks.NUnit3(_ => _
        .SetInputFiles(unitTestPath1, unitTestPath2)
        .SetProcessExitHandler(p => p.ExitCode switch
        {
          -1 => throw new Exception("Invalid args"),
          > 0 => throw new Exception($"{p.ExitCode} tests have failed"),
          _ => null
        }));
    });

  Target Pack => _ => _
    .Description($"Packing project with the version.")
    .Requires(() => Configuration.Equals(Configuration.Release))
    .Produces(ArtifactsDirectory / NuGetArtifactsType)
    .DependsOn(Test)
    .Triggers(PublishToGithub, PublishToNuGet, Zip)
    .Executes(() =>
    {
      // For more definitions see Directory.Build.props file in solution folder
      DotNetTasks.DotNetPack(p =>
        p.SetProject(Solution.FlowReports)
         .SetConfiguration(Configuration)
         .SetOutputDirectory(ArtifactsDirectory)
         .EnableNoBuild()
         .EnableNoRestore()
         .SetAuthors(Authors)
         .SetCopyright(Copyright)
         .SetVersion(GitVersion.NuGetVersionV2)
         .SetAssemblyVersion(GitVersion.AssemblySemVer)
         .SetInformationalVersion(GitVersion.InformationalVersion)
         .SetFileVersion(GitVersion.AssemblySemFileVer));
    });

  Target PublishToGithub => _ => _
    .Description($"Publishing to Github for development only.")
    .Triggers(CreateRelease)
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnDevelopBranch() || (GitHubActions?.IsPullRequest ?? false))
    .Executes(() =>
    {
      Globbing.GlobFiles(ArtifactsDirectory, NuGetArtifactsType)
        .ToList()
        .ForEach(x =>
        {
          if (GitHubActions?.Token == null)
          {
            Log.Information("Not online. PublishToGithub skipped.");
            return;
          }

          DotNetTasks.DotNetNuGetPush(s => s
                     .SetTargetPath(x)
                     .SetSource(GithubNugetFeed)
                     .SetApiKey(GitHubActions.Token)
                     .EnableSkipDuplicate()
          );
        });
    });

  Target PublishToNuGet => _ => _
    .Description($"Publishing to NuGet with the version.")
    .Triggers(CreateRelease)
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch())
    .Executes(() =>
    {
      Globbing.GlobFiles(ArtifactsDirectory, NuGetArtifactsType)
        .ToList()
        .ForEach(x =>
        {
          DotNetTasks.DotNetNuGetPush(s => s
            .SetTargetPath(x)
            .SetSource(NuGetApiUrl)
            .SetApiKey(NuGetApiKey)
            .EnableSkipDuplicate());
        });
    });

  Target Zip => _ => _
    .Description($"Packing project with the version.")
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
    .Produces(ArtifactsDirectory / NuGetArtifactsType)
    .Triggers(CreateRelease)
    .Executes(() =>
    {
      Log.Information("Zipping...");
      RootDirectory.ZipTo(ArtifactsDirectory / "Source Code " + (ApplicationName + "_" + GitVersion.NuGetVersionV2 + ".zip"),
                          x => !x.ToFileInfo().FullName.Contains(".artifacts") &&
                               !x.ToFileInfo().FullName.Contains(@"\.vs") &&
                               !x.ToFileInfo().FullName.Contains(@"\.github") &&
                               !x.ToFileInfo().FullName.Contains(@"\.git") &&
                               !x.ToFileInfo().FullName.Contains(@"\temp") &&
                               !x.ToFileInfo().FullName.Contains(@"\Output") &&
                               !x.ToFileInfo().FullName.Contains(@"\bin\") &&
                               !x.ToFileInfo().FullName.Contains(@"\obj\"),
                          CompressionLevel.SmallestSize,
                          System.IO.FileMode.Create);
    });

  Target CreateRelease => _ => _
    .Description($"Creating release for the publishable version.")
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
    .Executes(async () =>
    {
      var releaseTag = $"v{GitVersion.MajorMinorPatch}";
      var changeLogSectionEntries = ChangelogTasks.ExtractChangelogSectionNotes(ChangeLogFile);
      var latestChangeLog = changeLogSectionEntries.Aggregate((c, n) => c + Environment.NewLine + n);
      var completeChangeLog = $"## {releaseTag}" + Environment.NewLine + latestChangeLog;
      var (gitHubOwner, repositoryName) = GitHubTasks.GetGitHubRepositoryInfo(GitRepository);
      //var nuGetPackages = Globbing.GlobFiles(ArtifactsDirectory, NuGetArtifactsType)
      //                            .Select(x => x.ToString())
      //                            .ToArray();

      Log.Information($"Github Owner: {gitHubOwner}.");
      Log.Information($"Repository Name: {repositoryName}.");
      Log.Information($"Release: {releaseTag}.");

      foreach (var entry in changeLogSectionEntries)
      {
        Log.Information(entry);
      }

      var s = Globbing.GlobFiles(ArtifactsDirectory, NuGetArtifactsType, ZipArtifactsType)
                      .Select(x => x.ToFileInfo().FullName)
                      .ToArray();

      await GitHubTasks.PublishRelease(x =>
        x.SetArtifactPaths(s)
         .SetCommitSha(GitVersion.Sha)
         .SetReleaseNotes(completeChangeLog)
         .SetRepositoryName(repositoryName)
         .SetRepositoryOwner(gitHubOwner)
         .SetTag(releaseTag)
         .SetToken(GitHubActions.Token));

      //var credentials = new Credentials(GitHubActions.Token);
      //GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

      //var owner = GitRepository.GetGitHubOwner();
      //var name = GitRepository.GetGitHubName();
      //var releaseTag = GitVersion.NuGetVersionV2;

      //Log.Information($"Owner: {owner}.");
      //Log.Information($"Name: {name}.");
      //Log.Information($"Release: {releaseTag}.");

      //var changeLogSectionEntries = ChangelogTasks.ExtractChangelogSectionNotes(ChangeLogFile);
      //var latestChangeLog = changeLogSectionEntries.Aggregate((c, n) => c + Environment.NewLine + n);

      //var newRelease = new NewRelease(releaseTag)
      //{
      //  TargetCommitish = GitVersion.Sha,
      //  Draft = true,
      //  Name = $"v{releaseTag}",
      //  Prerelease = !string.IsNullOrEmpty(GitVersion.PreReleaseTag),
      //  Body = latestChangeLog
      //};

      //var createdRelease = await GitHubTasks.GitHubClient
      //                                      .Repository
      //                                      .Release
      //                                      .Create(owner, name, newRelease);

      //Globbing.GlobFiles(ArtifactsDirectory, NuGetArtifactsType)
      //        .ToList()
      //        .ForEach(async x => await UploadReleaseFileToGithub(createdRelease, x));

      //Globbing.GlobFiles(ArtifactsDirectory, ZipArtifactsType)
      //        .ToList()
      //        .ForEach(async x => await UploadReleaseFileToGithub(createdRelease, x));

      //await GitHubTasks.GitHubClient
      //                 .Repository
      //                 .Release
      //                 .Edit(owner, name, createdRelease.Id, new ReleaseUpdate { Draft = false });
    });

  //private static async Task UploadReleaseFileToGithub(Release release, string asset)
  //{
  //  await using var artifactStream = File.OpenRead(asset);
  //  var fileName = Path.GetFileName(asset);

  //  var assetUpload = new ReleaseAssetUpload
  //  {
  //    FileName = fileName,
  //    ContentType = "application/octet-stream",
  //    RawData = artifactStream,
  //  };

  //  await Nuke.Common.Tools.GitHub.GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, assetUpload);
  //}
}
