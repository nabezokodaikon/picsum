using System.Collections.Concurrent;
using System.Reflection;

namespace SWF.Core.Base
{

    public static class AssemblyPreloader
    {
        private static readonly ConcurrentDictionary<string, bool> PRELOADED_ASSEMBLIES = new();

        private static void SyncPreloadAssemblyForType(Type criticalType)
        {
            try
            {
                var assemblyName = criticalType.Assembly.GetName().Name;
                if (assemblyName == null)
                {
                    return;
                }

                if (PRELOADED_ASSEMBLIES.TryAdd(assemblyName, true))
                {
                    var assembly = criticalType.Assembly;
                    Assembly.Load(assemblyName);
                }
            }
            catch (Exception ex) when (
                ex is ArgumentNullException ||
                ex is ArgumentException ||
                ex is FileNotFoundException ||
                ex is FileLoadException ||
                ex is BadImageFormatException)
            {
                ConsoleUtil.Write(true,
                    $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                );
            }
        }

        // アプリケーション起動時の最適化メソッド
        public static void OptimizeStartup(Type[] criticalTypes)
        {
            ArgumentNullException.ThrowIfNull(criticalTypes, nameof(criticalTypes));

            using (Measuring.Time(true, "AssemblyPreloader.OptimizeStartup"))
            {
                Parallel.ForEach(
                    criticalTypes,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Math.Min(criticalTypes.Length, AppConstants.MAX_DEGREE_OF_PARALLELISM)
                    },
                    SyncPreloadAssemblyForType
                );
            }
        }

        public static void PreJIT(Type[] types)
        {
            ArgumentNullException.ThrowIfNull(types, nameof(types));

            using (Measuring.Time(true, "AssemblyPreloader.PreJIT"))
            {
                Parallel.ForEach(
                    types,
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Math.Min(types.Length, AppConstants.MAX_DEGREE_OF_PARALLELISM)
                    },
                    type =>
                    {
                        foreach (var method in type.GetMethods(System.Reflection.BindingFlags.DeclaredOnly |
                                                               System.Reflection.BindingFlags.NonPublic |
                                                               System.Reflection.BindingFlags.Public |
                                                               System.Reflection.BindingFlags.Instance |
                                                               System.Reflection.BindingFlags.Static))
                        {
                            if (!method.IsAbstract && !method.ContainsGenericParameters)
                            {
                                System.Runtime.CompilerServices.RuntimeHelpers.PrepareMethod(method.MethodHandle);
                            }
                        }
                    }
                );
            }
        }
    }
}
