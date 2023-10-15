// See examples on:
// https://anktsrkr.github.io/post/getting-started-with-nuke/
// https://anktsrkr.github.io/post/write-your-first-building-block-in-nuke/
// https://anktsrkr.github.io/post/manage-your-package-version-using-nuke/
// https://anktsrkr.github.io/post/manage-your-package-release-using-nuke-in-github/

using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NUnit;
using Serilog;

[GitHubActions(
  "continuous",
  GitHubActionsImage.WindowsLatest,
  AutoGenerate = true,
  FetchDepth = 0,
  OnPushBranches = new[] { MasterBranch, DevelopmentBranch, ReleasesBranch },
  OnPullRequestBranches = new[] { ReleasesBranch },
  InvokedTargets = new[] { nameof(Pack) },
  EnableGitHubToken = true,
  ImportSecrets = new[]
  {
    nameof(NuGetApiKey)
  }
)]

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

  //[Nuke.Common.Parameter("Nuget Feed Url for Public Access of Pre Releases")]
  //readonly string NugetFeed;

  [Nuke.Common.Parameter("Nuget Api Key"), Secret]
  readonly string NuGetApiKey;

  [Nuke.Common.Parameter("Copyright Details")]
  readonly string Copyright;

  [Nuke.Common.Parameter("Artifacts Type")]
  readonly string ArtifactsType;

  [Nuke.Common.Parameter("Excluded Artifacts Type")]
  readonly string ExcludedArtifactsType;

  [GitVersion]
  readonly GitVersion GitVersion;

  [GitRepository]
  readonly GitRepository GitRepository;

  [Solution(GenerateProjects = true)]
  readonly Solution Solution;

  static GitHubActions GitHubActions => GitHubActions.Instance;

  static AbsolutePath ArtifactsDirectory => RootDirectory / ".artifacts";

  static AbsolutePath OutputDirectory => RootDirectory / "Output";

  //static readonly string PackageContentType = "application/octet-stream";

  //static string ChangeLogFile => RootDirectory / "CHANGELOG.md";

  static string GithubNugetFeed => GitHubActions != null
     ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
     : null;

  Target Clean => _ => _
    .Description($"Cleaning project.")
    .Before(Restore)
    .Executes(() =>
    {
      DotNetTasks.DotNetClean(c => c.SetProject(Solution.FlowReports_TestApplication));
      ArtifactsDirectory.CreateOrCleanDirectory();
    });

  Target Restore => _ => _
    .Description($"Restoring project dependencies.")
    .DependsOn(Clean)
    .Executes(() =>
    {
      //DotNetTasks.DotNetToolRestore();
      DotNetTasks.DotNetRestore(_ => _.SetProjectFile(Solution));
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
    .Produces(ArtifactsDirectory / ArtifactsType)
    .DependsOn(Test)
    .Triggers(PublishToGithub, PublishToNuGet)
    .Executes(() =>
    {
      DotNetTasks.DotNetPack(p =>
        p.SetProject(Solution.FlowReports_TestApplication)
         .SetConfiguration(Configuration)
         .SetOutputDirectory(ArtifactsDirectory)
         .EnableNoBuild()
         .EnableNoRestore()
         .SetCopyright(Copyright)
         .SetVersion(GitVersion.NuGetVersionV2)
         .SetAssemblyVersion(GitVersion.AssemblySemVer)
         .SetInformationalVersion(GitVersion.InformationalVersion)
         .SetFileVersion(GitVersion.AssemblySemFileVer));
    });

  Target PublishToGithub => _ => _
    .Description($"Publishing to Github for development only.")
    //.Triggers(CreateRelease)
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnDevelopBranch() || GitHubActions.IsPullRequest)
    .Executes(() =>
    {
      Globbing.GlobFiles(ArtifactsDirectory, ArtifactsType)
        .Where(x => string.IsNullOrWhiteSpace(ExcludedArtifactsType) || !x.Name.EndsWith(ExcludedArtifactsType))
        .ToList()
        .ForEach(x =>
        {
          if (GitHubActions == null)
            Log.Information("GitHub Actions == null");
          else if (GitHubActions.Token == null)
            Log.Information("GitHub Token == null");
          else
            Log.Information("GitHub Token = {Token}", GitHubActions.Token);
          //DotNetTasks.DotNetNuGetPush(s => s
          //           .SetTargetPath(x)
          //           .SetSource(GithubNugetFeed)
          //           .SetApiKey(GitHubActions.Token)
          //           .EnableSkipDuplicate()
          //);
        });
      ;
    });

  Target PublishToNuGet => _ => _
    .Description($"Publishing to NuGet with the version.")
    //.Triggers(CreateRelease)
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch())
    .Executes(() =>
    {
      Globbing.GlobFiles(ArtifactsDirectory, ArtifactsType)
        .Where(x => string.IsNullOrWhiteSpace(ExcludedArtifactsType) || !x.Name.EndsWith(ExcludedArtifactsType))
        .ToList()
        .ForEach(x =>
        {
          DotNetTasks.DotNetNuGetPush(s => s
            .SetTargetPath(x)
            //.SetSource(NugetFeed)
            .SetApiKey(NuGetApiKey)
            .EnableSkipDuplicate()
          );
        });
    });

  //Target CreateRelease => _ => _
  //   .Description($"Creating release for the publishable version.")
  //   .Requires(() => Configuration.Equals(Configuration.Release))
  //   .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
  //   .Executes(async () =>
  //   {
  //     var credentials = new Credentials(GitHubActions.Token);
  //     GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

  //     var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());

  //     var releaseTag = GitVersion.NuGetVersionV2;
  //     var changeLogSectionEntries = ChangelogTasks.ExtractChangelogSectionNotes(ChangeLogFile);
  //     var latestChangeLog = changeLogSectionEntries.Aggregate((c, n) => c + Environment.NewLine + n);

  //     var newRelease = new NewRelease(releaseTag)
  //     {
  //       TargetCommitish = GitVersion.Sha,
  //       Draft = true,
  //       Name = $"v{releaseTag}",
  //       Prerelease = !string.IsNullOrEmpty(GitVersion.PreReleaseTag),
  //       Body = latestChangeLog
  //     };

  //     var createdRelease = await GitHubTasks.GitHubClient
  //                                                 .Repository
  //                                      .Release.Create(owner, name, newRelease);

  //     Globbing.GlobFiles(ArtifactsDirectory, ArtifactsType)
  //                       .Where(x => string.IsNullOrWhiteSpace(ExcludedArtifactsType) || !x.Name.EndsWith(ExcludedArtifactsType))
  //                       .ToList()
  //                       .ForEach(async x => await UploadReleaseAssetToGithub(createdRelease, x));

  //     await GitHubTasks
  //           .GitHubClient
  //           .Repository
  //           .Release
  //           .Edit(owner, name, createdRelease.Id, new ReleaseUpdate { Draft = false });
  //   });

  //private static async Task UploadReleaseAssetToGithub(Release release, string asset)
  //{
  //  await using var artifactStream = File.OpenRead(asset);
  //  var fileName = Path.GetFileName(asset);
  //  var assetUpload = new ReleaseAssetUpload
  //  {
  //    FileName = fileName,
  //    ContentType = PackageContentType,
  //    RawData = artifactStream,
  //  };
  //  await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, assetUpload);
  //}
}
