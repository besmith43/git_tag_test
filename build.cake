using System.Diagnostics;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

Task("Clean")
	.Does(() =>
{
	CleanDirectory("./.build");
	CleanDirectory("./.publish");
	DotNetClean("./runner/runner.csproj", new DotNetCleanSettings
	{
		Configuration = configuration,
		OutputDirectory = "./.build/"
	});
	DotNetClean("./runner/runner.csproj", new DotNetCleanSettings
	{
		Configuration = "Release",
		OutputDirectory = "./.publish/"
	});
});

Task("Build")
		.Does(() =>
{
	DotNetBuild("./runner/runner.csproj", new DotNetBuildSettings
	{
		Configuration = configuration,
		NoRestore = false,
		NoLogo = true,
		Verbosity = DotNetVerbosity.Minimal,
		OutputDirectory = "./.build/"
	});
});

Task("Run")
		.Does(() =>
{
	DotNetExecute("./.build/runner.dll");
});

Task("UpVersion")
	.Does(() =>
{
	var propsFile = "./Directory.Build.props";
	var readedVersion = XmlPeek(propsFile, "//Version");
	var currentVersion = new Version(readedVersion);
	var newMinor = currentVersion.Minor;

	if (target == "publish")
	{
		newMinor++;
	}

	var semVersion = new Version(currentVersion.Major, newMinor, currentVersion.Build + 1);
	var version = semVersion.ToString();

	XmlPoke(propsFile, "//Version", version);


});

Task("Major-Release")
	.Does(() =>
{
	var propsFile = "./Directory.Build.props";
	var readedVersion = XmlPeek(propsFile, "//Version");
	var currentVersion = new Version(readedVersion);

	var semVersion = new Version(currentVersion.Major + 1, 0, 0);
	var version = semVersion.ToString();

	XmlPoke(propsFile, "//Version", version);
});

Task("Tag")
	.Does(() =>
{
	var propsFile = "./Directory.Build.props";
	var readedVersion = XmlPeek(propsFile, "//Version");
	var currentVersion = new Version(readedVersion);

	ProcessStartInfo gitTagStartInfo = new ProcessStartInfo();
	gitTagStartInfo.FileName = "git";
	gitTagStartInfo.Arguments = $"tag -a {currentVersion.ToString()} -m {currentVersion.ToString()}";
	gitTagStartInfo.UseShellExecute = false;
	gitTagStartInfo.RedirectStandardOutput = true;
	gitTagStartInfo.RedirectStandardError = true;
	gitTagStartInfo.CreateNoWindow = true;

	Process gitTagProcess = new Process();
	gitTagProcess.StartInfo = gitTagStartInfo;
	gitTagProcess.Start();

	string gitTagStdOutput = gitTagProcess.StandardOutput.ReadToEnd();
	string gitTagStdError = gitTagProcess.StandardError.ReadToEnd();

	gitTagProcess.WaitForExit();

	if (gitTagProcess.ExitCode != 0)
	{
		Information(gitTagStdError);
		throw new Exception("git tag failed");
	}
	else
	{
		Information(gitTagStdOutput);
	}
});

Task("PushTag")
	.Does(() =>
{
	var propsFile = "./Directory.Build.props";
	var readedVersion = XmlPeek(propsFile, "//Version");
	var currentVersion = new Version(readedVersion);

	ProcessStartInfo gitPushStartInfo = new ProcessStartInfo();
	gitPushStartInfo.FileName = "git";
	gitPushStartInfo.Arguments = $"push origin {currentVersion.ToString()}";
	gitPushStartInfo.UseShellExecute = false;
	gitPushStartInfo.RedirectStandardOutput = true;
	gitPushStartInfo.RedirectStandardError = true;
	gitPushStartInfo.CreateNoWindow = true;

	Process gitPushProcess = new Process();
	gitPushProcess.StartInfo = gitPushStartInfo;
	gitPushProcess.Start();

	string gitPushStdOutput = gitPushProcess.StandardOutput.ReadToEnd();
	string gitPushStdError = gitPushProcess.StandardError.ReadToEnd();

	gitPushProcess.WaitForExit();

	if (gitPushProcess.ExitCode != 0)
	{
		Information(gitPushStdError);
		throw new Exception("git push tag failed");
	}
	else
	{
		Information(gitPushStdOutput);
	}
});

Task("Publish")
	.IsDependentOn("Clean")
	.IsDependentOn("UpVersion")
	.IsDependentOn("Tag")
	.IsDependentOn("PushTag")
	.Does(() =>
{
	DotNetPublish("./runner/runner.csproj", new DotNetPublishSettings
	{
		Configuration = "Release",
		NoRestore = false,
		NoLogo = true,
		Runtime = "win-x64",
		SelfContained = true,
		PublishSingleFile = true,
		Verbosity = DotNetVerbosity.Minimal,
		OutputDirectory = "./.publish/"
	});
});


Task("Default")
	.IsDependentOn("Clean")
	.IsDependentOn("Build")
	.IsDependentOn("Run");


RunTarget(target); // this is going to run the default task if you don't pass in a --target argument

