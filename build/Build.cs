// See examples on:
// https://anktsrkr.github.io/post/getting-started-with-nuke/
// https://anktsrkr.github.io/post/write-your-first-building-block-in-nuke/
// https://anktsrkr.github.io/post/manage-your-package-version-using-nuke/
// https://anktsrkr.github.io/post/manage-your-package-release-using-nuke-in-github/
// https://cfrenzel.com/publishing-nuget-nuke-appveyor/

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.NUnit;
using Octokit;
using Octokit.Internal;
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

  [Nuke.Common.Parameter("Nuget Feed Url for Public Access of Pre Releases")]
  readonly string NugetFeed;

  [Nuke.Common.Parameter("Nuget Api Key"), Secret]
  readonly string NuGetApiKey;

  [Nuke.Common.Parameter("Authors")]
  readonly string Authors;

  [Nuke.Common.Parameter("Copyright Details")]
  readonly string Copyright;

  [Nuke.Common.Parameter("NuGet Artifacts Type")]
  readonly string NuGetType;

  [Nuke.Common.Parameter("Zip Artifacts Type")]
  readonly string ZipType;

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

  static readonly string PackageContentType = "application/octet-stream";

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
    .Description($"Building project version {GitVersion.NuGetVersionV2}.")
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
    .Description($"Packing project version {GitVersion.NuGetVersionV2}.")
    .Requires(() => Configuration.Equals(Configuration.Release))
    .Produces(ArtifactsDirectory / NuGetType, ArtifactsDirectory / ZipType)
    .DependsOn(Test)
    .Triggers(PublishToGithub, PublishToNuGet)
    .Executes(() =>
    {
      DotNetTasks.DotNetPack(p =>
        p.SetProject(Solution.FlowReports)
         .SetConfiguration(Configuration)
         .SetOutputDirectory(ArtifactsDirectory)
         .EnableNoBuild()
         .EnableNoRestore()
         .SetAuthors(Authors)
         .SetCopyright(Copyright)
         .SetRepositoryUrl(@"https://github.com/pschimmel/FlowReports")
         .SetPackageProjectUrl(@"https://github.com/pschimmel/FlowReports")
         .SetPackageLicenseUrl(@"https://github.com/pschimmel/FlowReports/blob/master/LICENSE.txt")
         .SetVersion(GitVersion.NuGetVersionV2)
         .SetAssemblyVersion(GitVersion.AssemblySemVer)
         .SetInformationalVersion(GitVersion.InformationalVersion)
         .SetFileVersion(GitVersion.AssemblySemFileVer));

      RootDirectory.ZipTo(ArtifactsDirectory / (ApplicationName + "_" + GitVersion.NuGetVersionV2 + "_src.zip"),
                          x => !x.ToFileInfo().FullName.Contains(".artifacts") &&
                               !x.ToFileInfo().FullName.Contains(@"\.vs") &&
                               !x.ToFileInfo().FullName.Contains(@"\temp") &&
                               !x.ToFileInfo().FullName.Contains(@"\Output") &&
                               !x.ToFileInfo().FullName.Contains(@"\bin\") &&
                               !x.ToFileInfo().FullName.Contains(@"\obj\"),
                          CompressionLevel.SmallestSize,
                          System.IO.FileMode.Create);
    });

  Target PublishToGithub => _ => _
    .Description($"Publishing to Github for development only.")
    .Triggers(CreateRelease)
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnDevelopBranch() || GitHubActions.IsPullRequest)
    .Executes(() =>
    {
      Globbing.GlobFiles(ArtifactsDirectory, NuGetType, ZipType)
        .ToList()
        .ForEach(x =>
        {
          if (GitHubActions == null)
          {
            Log.Information("GitHub Actions == null");
            return;
          }
          else if (GitHubActions.Token == null)
          {
            Log.Information("GitHub Token == null");
            return;
          }
          else
          {
            Log.Information("GitHub Token = {Token}", GitHubActions.Token);
          }

          DotNetTasks.DotNetNuGetPush(s => s
                     .SetTargetPath(x)
                     .SetSource(GithubNugetFeed)
                     .SetApiKey(GitHubActions.Token)
                     .EnableSkipDuplicate()
          );
        });
      ;
    });

  Target PublishToNuGet => _ => _
    .Description($"Publishing to NuGet with the version.")
    .Triggers(CreateRelease)
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch())
    .Executes(() =>
    {
      Globbing.GlobFiles(ArtifactsDirectory, NuGetType)
        .ToList()
        .ForEach(x =>
        {
          DotNetTasks.DotNetNuGetPush(s => s
            .SetTargetPath(x)
            .SetSource(NugetFeed)
            .SetApiKey(NuGetApiKey)
            .EnableSkipDuplicate()
          );
        });
    });

  Target CreateRelease => _ => _
    .Description($"Creating release for the publishable version.")
    .Requires(() => Configuration.Equals(Configuration.Release))
    .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
    .Executes(async () =>
    {
      var credentials = new Credentials(GitHubActions.Token);
      GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

      var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());

      var releaseTag = GitVersion.NuGetVersionV2;
      var changeLogSectionEntries = ChangelogTasks.ExtractChangelogSectionNotes(ChangeLogFile);
      var latestChangeLog = changeLogSectionEntries.Aggregate((c, n) => c + Environment.NewLine + n);

      var newRelease = new NewRelease(releaseTag)
      {
        TargetCommitish = GitVersion.Sha,
        Draft = true,
        Name = $"v{releaseTag}",
        Prerelease = !string.IsNullOrEmpty(GitVersion.PreReleaseTag),
        Body = latestChangeLog
      };

      var createdRelease = await GitHubTasks.GitHubClient
                                            .Repository
                                            .Release.Create(owner, name, newRelease);

      Globbing.GlobFiles(ArtifactsDirectory, NuGetType, ZipType)
              .ToList()
              .ForEach(async x => await UploadReleaseAssetToGithub(createdRelease, x));

      await GitHubTasks.GitHubClient
                       .Repository
                       .Release
                       .Edit(owner, name, createdRelease.Id, new ReleaseUpdate { Draft = false });
    });

  private static async Task UploadReleaseAssetToGithub(Release release, string asset)
  {
    await using var artifactStream = File.OpenRead(asset);
    var fileName = Path.GetFileName(asset);
    var assetUpload = new ReleaseAssetUpload
    {
      FileName = fileName,
      ContentType = PackageContentType,
      RawData = artifactStream,
    };
    await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, assetUpload);
  }
}
