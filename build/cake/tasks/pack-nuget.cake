
Task("Pack-Nuget")
    .IsDependentOnWhen("Generate-Docs", isSingleStageRun)
    .Does<BuildParameters>((context,parameters) =>  {

    var buildSettings = new DotNetCoreMSBuildSettings (){};
    buildSettings.WithProperty("GeneratePackageOnBuild", "true");
    buildSettings.WithProperty("PackageVersion", parameters.Version.SemVersion);
    buildSettings.WithProperty("Version", parameters.Version.SemVersion);
    buildSettings.WithProperty("PackageProjectUrl", parameters.Version.PackageProjectUrl);
    buildSettings.WithProperty("RepositoryUrl", parameters.Version.RepositoryUrl);
    buildSettings.WithProperty("PackageReleaseNotes", parameters.Version.PackageReleaseNotes.Replace(".git/",""));
    buildSettings.WithProperty("RepositoryCommit", parameters.Version.RepositoryCommit);
    buildSettings.WithProperty("RepositoryBranch", parameters.Version.RepositoryBranch);
        
     foreach(var project in parameters.SolutionProjects)
     {

        var isABoolean = bool.TryParse(project.GetProjectProperty("IsPackable"),out var canPackAndPublish);

        if(canPackAndPublish){
        DotNetCorePack(project.ProjectFilePath.FullPath, new DotNetCorePackSettings { 
            ArgumentCustomization = args => args
                               .Append($"-p:Version={parameters.Version.SemVersion}"),
            Configuration = parameters.Configuration,
            OutputDirectory =  parameters.Paths.Directories.NugetRoot,
            NoBuild = true, 
            NoRestore = true,
            MSBuildSettings = buildSettings,
        });
        }

        
      }
       
});

