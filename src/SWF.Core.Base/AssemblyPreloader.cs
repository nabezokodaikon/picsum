using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Versioning;

namespace SWF.Core.Base
{
    [SupportedOSPlatform("windows10.0.17763.0")]
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

            using (TimeMeasuring.Run(true, "AssemblyPreloader.OptimizeStartup"))
            {
                Parallel.ForEach(
                    criticalTypes,
                    new ParallelOptions { MaxDegreeOfParallelism = Math.Max(criticalTypes.Length, 1) },
                    SyncPreloadAssemblyForType
                );
            }
        }
    }
}
