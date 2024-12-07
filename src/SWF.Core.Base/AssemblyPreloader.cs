using System.Collections.Concurrent;
using System.Reflection;

namespace SWF.Core.Base
{
    public static class AssemblyPreloader
    {
        private static readonly ConcurrentDictionary<string, bool> PreloadedAssemblies = new();

        private static Task PreloadAssemblyForType(Type criticalType)
        {
            return Task.Run(() =>
            {
                try
                {
                    var assemblyName = criticalType.Assembly.GetName().Name;
                    if (assemblyName == null)
                    {
                        return;
                    }

                    if (PreloadedAssemblies.TryAdd(assemblyName, true))
                    {
                        var assembly = criticalType.Assembly;
                        Assembly.Load(assemblyName);
                        LoadCriticalReferences(assembly);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUtil.Write(
                        $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                    );
                }
            });
        }

        private static void LoadCriticalReferences(Assembly assembly)
        {
            foreach (var refAssembly in assembly
                .GetReferencedAssemblies()
                .Where(IsCriticalAssembly))
            {
                try
                {
                    var assemblyName = refAssembly.Name;
                    if (assemblyName == null)
                    {
                        return;
                    }

                    if (PreloadedAssemblies.TryAdd(assemblyName, true))
                    {
                        Assembly.Load(refAssembly);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleUtil.Write(
                        $"依存アセンブリ読み込みエラー: {refAssembly.Name}. Error: {ex.Message}"
                    );
                }
            }
        }

        private static bool IsCriticalAssembly(AssemblyName assemblyName)
        {
            var name = assemblyName.Name;
            if (name == null)
            {
                return false;
            }

            // 重要なアセンブリを判定（カスタマイズ可能）
            string[] criticalPrefixes =
            [
                "Microsoft",
                "System",
            ];

            return criticalPrefixes.Any(prefix =>
                name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        // アプリケーション起動時の最適化メソッド
        public static void OptimizeStartup(params Type[] criticalTypes)
        {
            using (TimeMeasuring.Run(true, "MicrosoftStorePreloader.OptimizeStartup"))
            {
                var tasks = criticalTypes
                    .Select(PreloadAssemblyForType)
                    .ToArray();

                Task.WaitAll(tasks);
            }
        }
    }
}
