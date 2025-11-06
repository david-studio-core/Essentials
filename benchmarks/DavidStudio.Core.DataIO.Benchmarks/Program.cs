using System.Reflection;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);

// Comment the version above and uncomment this for debugging
//
// BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, new DebugInProcessConfig());