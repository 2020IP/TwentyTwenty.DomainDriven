#tool nuget:?package=GitVersion.CommandLine

GitVersion versionInfo = null;
var target = Argument("target", "Default");
var outputDir = "./artifacts/";
var configuration   = Argument("configuration", "Release");

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
        CreateDirectory(outputDir);
    });

Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    });

Task("Restore")
    .IsDependentOn("Version")
    .Does(() => {        
        DotNetCoreRestore(new DotNetCoreRestoreSettings
        {
            ArgumentCustomization = args => args.Append("/p:SemVer=" + versionInfo.NuGetVersion)
        });
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(".", new DotNetCoreBuildSettings
        {
            ArgumentCustomization = args => args.Append("/p:SemVer=" + versionInfo.NuGetVersion)
        });
    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = outputDir,
            NoBuild = true,
            Configuration = configuration,
            ArgumentCustomization = args => args.Append("/p:SemVer=" + versionInfo.NuGetVersion)
        };

        DotNetCorePack("src/TwentyTwenty.DomainDriven/", settings);
        DotNetCorePack("src/TwentyTwenty.DomainDriven.Marten/", settings);
        DotNetCorePack("src/TwentyTwenty.DomainDriven.MassTransit/", settings);

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*"))
                AppVeyor.UploadArtifact(file.FullPath);
        }
    });

Task("Default")
    .IsDependentOn("Package");

RunTarget(target);