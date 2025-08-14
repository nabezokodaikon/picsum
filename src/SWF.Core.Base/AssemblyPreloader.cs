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
            catch (ArgumentNullException ex)
            {
                ConsoleUtil.Write(true,
                    $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                );
            }
            catch (ArgumentException ex)
            {
                ConsoleUtil.Write(true,
                    $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                );
            }
            catch (FileNotFoundException ex)
            {
                ConsoleUtil.Write(true,
                    $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                );
            }
            catch (FileLoadException ex)
            {
                ConsoleUtil.Write(true,
                    $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                );
            }
            catch (BadImageFormatException ex)
            {
                ConsoleUtil.Write(true,
                    $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                );
            }
        }

        // アプリケーション起動時の最適化メソッド
        public static void OptimizeStartup(params Type[] criticalTypes)
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
