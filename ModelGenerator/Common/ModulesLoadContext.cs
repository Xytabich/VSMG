using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ModelGenerator
{
    internal class ModulesLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver dependencyResolver;

        public ModulesLoadContext(DirectoryInfo modulesPath) : base(true)
        {
            dependencyResolver = new AssemblyDependencyResolver(modulesPath.FullName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if(Default.LoadFromAssemblyName(assemblyName) != null)
            {
                return null;
            }

            var path = dependencyResolver.ResolveAssemblyToPath(assemblyName);
            if(!string.IsNullOrEmpty(path)) return LoadFromAssemblyPath(path);

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string name)
        {
            var path = dependencyResolver.ResolveUnmanagedDllToPath(name);
            if(!string.IsNullOrEmpty(path)) return LoadUnmanagedDllFromPath(path);

            return base.LoadUnmanagedDll(name);
        }
    }
}