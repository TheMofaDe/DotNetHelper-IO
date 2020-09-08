Task("Clean")
.Does<BuildParameters>((context,parameters) => {
      
    var settings = new DeleteDirectorySettings (){
         Recursive = true,
         Force = true
    };

    CleanDirectory(parameters.Paths.Directories.ArtifactsRoot);

    foreach (var project in parameters.SolutionProjects)
    {
        var path = System.IO.Path.GetDirectoryName(project.ProjectFilePath.FullPath);
        var binFolder = System.IO.Path.Combine(path,"bin");
        var objFolder = System.IO.Path.Combine(path,"obj");

        if(context.DirectoryExists(objFolder))
            context.DeleteDirectory(objFolder,settings);
        if(context.DirectoryExists(binFolder))
            context.DeleteDirectory(binFolder,settings);
    }
    
    if(context.DirectoryExists(parameters.Paths.Directories.Artifacts))
            context.DeleteDirectory(parameters.Paths.Directories.Artifacts,settings);


}).ReportError(exception =>
{
    Error(exception.Dump());
});
