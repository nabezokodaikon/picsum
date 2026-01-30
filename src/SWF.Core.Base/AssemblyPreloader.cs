using System.Reflection;

namespace SWF.Core.Base
{

    public static class AssemblyPreloader
    {
        public static void OptimizeStartup(string[] assemblyNames)
        {
            ArgumentNullException.ThrowIfNull(assemblyNames, nameof(assemblyNames));

            AppConstants.ThrowIfNotUIThread();

            using (Measuring.Time(true, "AssemblyPreloader.OptimizeStartup"))
            {
                Parallel.ForEach(
                    assemblyNames,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = AppConstants.GetHeavyMaxDegreeOfParallelism(assemblyNames)
                    },
                    typeName =>
                    {
                        var asm = Assembly.Load(typeName);
                        foreach (var refAsm in asm.GetReferencedAssemblies())
                        {
                            Assembly.Load(refAsm);
                        }
                    }
                );
            }
        }
    }
}
