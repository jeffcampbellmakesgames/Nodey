[![GitHub issues](https://img.shields.io/github/issues/jeffcampbellmakesgames/nodey.svg)](https://github.com/jeffcampbellmakesgames/nodey/issues)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/jeffcampbellmakesgames/nodey/master/LICENSE.md)
[![openupm](https://img.shields.io/npm/v/com.jeffcampbellmakesgames.nodey?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.jeffcampbellmakesgames.nodey/)

# Nodey
A library to enable developers to easily make node-graph enhanced editor and runtime tools and systems.

Support Nodey on [KO-FI](https://ko-fi.com/stampyturtle)

![Example Nodey Graph](/Images/ExampleNodeyGraph.png)

## Overview
Thinking of developing a node-based plugin? Then this is for you.

Nodey is super userfriendly, intuitive and will help you reap the benefits of node graphs in no time.
With a minimal footprint, it is ideal as a base for custom state machines, dialogue systems, decision makers etc.

### Key features
* Lightweight in runtime
* Very little boilerplate code
* Strong separation of editor and runtime code
* No runtime reflection (unless you need to edit/build node graphs at runtime. In this case, all reflection is cached.)
* Does not rely on any 3rd party plugins
* Custom node inspector code is very similar to regular custom inspector code
* Support for Unity versions follows LTS Tech Cycle, two years of patch support and compatibility (see [here](https://blogs.unity3d.com/2018/04/09/new-plans-for-unity-releases-introducing-the-tech-and-long-term-support-lts-streams/) for more information).

## Installing Nodey
Using this library in your project can be done in three ways:

### Install via OpenUPM
The package is available on the [openupm registry](https://openupm.com/). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.jeffcampbellmakesgames.nodey
```

### Install via GIT URL
Using the native Unity Package Manager introduced in 2017.2, you can add this library as a package by modifying your `manifest.json` file found at `/ProjectName/Packages/manifest.json` to include it as a dependency. See the example below on how to reference it.

```
{
	"dependencies": {
		...
		"com.jeffcampbellmakesgames.nodey" : "https://github.com/jeffcampbellmakesgames/nodey.git#release/stable",
		...
	}
}
```

### Install via classic `.UnityPackage`
The latest release can be found [here](https://github.com/jeffcampbellmakesgames/nodey/releases) as a UnityPackage file that can be downloaded and imported directly into your project's Assets folder.

You will need to have Git installed and available in your system's PATH.

If you are using [Assembly Definitions](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html) in your project, you will need to add `JCMG.Nodey` and/or `JCMG.Nodey.Editor` as Assembly Definition References.

## Support
If this is useful to you and/or you’d like to see future development and more tools in the future, please consider supporting it either by contributing to the Github projects (submitting bug reports or features and/or creating pull requests) or by buying me coffee using any of the links below. Every little bit helps!

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I2W7GX)

## Contributors
If you are interested in contributing, found a bug, or want to request a new feature, please see [here](./contributors.md) for more information.

### Hey, isn't this a fork of xNode?
You are very astute; this is a fork of xNode that significantly changes the project structure and code style for several reasons:

#### Clean Code
It aims to clean up the code style and functionality by making it more readable, less-complex, and easier to update in the future by following some of the principles of [Clean Code](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882) by Robert C. Martin of Microsoft. You can read more about the code style in use [here](./contributors.md).

I'd also like to refactor the existing logic to be unit-testable such that bugs and regressions are more easily discoverable as well as potentially add integration-tests for users of the library so that they are better informed when there are issues with how its used.

#### Ease of Development and Distribution
The original xNode project is setup as a submodule in contributors and user's Unity projects; this can make it more difficult to update as a dedicated library in isolation from other user's projects that may alter it for their own use-case and serialization of assets between various contributors on different Unity versions can cause hard-to-troubleshoot bugs. From my experience it also makes it more difficult for potential users of the library to clone and get started as submodules are not always easy to work with in general (definitely not designer/artist friendly and not always easily understood by programmers either). By using several different distribution options of the library including legacy `.UnityPackage` files, native Unity Package Manager, and OpenUPM there are many ways for users to keep up-to-date with new developments on the project.

#### Use Example Content to Dogfood Development
The example content in general has been orphaned on a side branch of the submodule and isn't easily available to test or debug how the library works (or for contributors to use for testing new features and bug fixes). By reorganizing the content of the library on the `develop` branch to have a dedicated Unity project and a well-defined distribution path its easy to develop, test, and update the example content as new features and bug fixes are implemented.

#### Contributing back to xNode
During the time that I am refactoring this library to be more stable I am contributing back to the original xNode library for any issues that I find in the form of [pull-requests](https://github.com/Siccity/xNode/pulls?utf8=%E2%9C%93&q=is%3Apr+author%3Ajeffcampbellmakesgames+). Its personally important to me to do this as any changes I make to diverge this library is built on years of hard-work put in by others and I would like to contribute back to that same font of knowledge. However, at some point once this library has diverged enough in its code structure and implementation I will likely PR less as it will be less-possible to accomplish.

### Node example:
```csharp
// public classes deriving from Node are registered as nodes for use within a graph
public class MathNode : Node 
{
	// Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node 
	[Input] 
	public float a;

	[Input] 
	public float b;

	// The value of an output node field is not used for anything, but could be used for caching output results
	[Output] 
	public float result;

	[Output] 
	public float sum;

	// The value of 'mathType' will be displayed on the node in an editable format, similar to the inspector
	public MathType mathType;

	private enum MathType 
	{ 
		Add, 
		Subtract, 
		Multiply, 
		Divide
	}
	
	// GetValue should be overridden to return a value for any specified output port
	public override object GetValue(NodePort port) 
	{
		// Get new a and b values from input connections. Fallback to field values if input is not connected
		float a = GetInputValue<float>("a", this.a);
		float b = GetInputValue<float>("b", this.b);

		// After you've gotten your input values, you can perform your calculations and return a value
		if (port.fieldName == "result")
		{

			switch(mathType) 
			{
				case MathType.Add: default: return a + b;
				case MathType.Subtract: return a - b;
				case MathType.Multiply: return a * b;
				case MathType.Divide: return a / b;
			}
		}
		else if (port.fieldName == "sum") 
		{
			return a + b;
		}
		
		return 0f;
	}
}
```
