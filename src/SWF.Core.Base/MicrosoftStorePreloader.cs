using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SWF.Core.Base
{
    public static class MicrosoftStorePreloader
    {
        private static readonly ConcurrentDictionary<string, bool> PreloadedAssemblies = new();

        public static void PreloadCriticalAssemblies(params Type[] criticalTypes)
        {
            var tasks = criticalTypes
                .Select(PreloadAssemblyForType)
                .ToArray();

            Task.WaitAll(tasks);
        }

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

                        // モジュールコンストラクターの実行
                        RuntimeHelpers.RunModuleConstructor(assembly.GetType().Module.ModuleHandle);

                        // 依存アセンブリの読み込み（最小限）
                        LoadCriticalReferences(assembly);

                        ConsoleUtil.Write($"アセンブリ読み込み完了: {assemblyName}");
                    }
                }
                catch (Exception ex)
                {
                    // Microsoft Storeアプリでのロギング
                    ConsoleUtil.Write(
                        $"アセンブリ読み込みエラー: {criticalType.Assembly.GetName().Name}. Error: {ex.Message}"
                    );
                }
            });
        }

        private static void LoadCriticalReferences(Assembly assembly)
        {
            var criticalReferences = assembly.GetReferencedAssemblies()
                .Where(IsCriticalAssembly)
                .ToList();

            foreach (var refAssembly in criticalReferences)
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
                "System",
                "Microsoft",
            ];

            return criticalPrefixes.Any(prefix =>
                name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        // アプリケーション起動時の最適化メソッド
        public static void OptimizeStartup(params Type[] criticalTypes)
        {
            // プリロード
            PreloadCriticalAssemblies(criticalTypes);

            // その他の起動最適化
            ConfigureAppDomainOptimizations();
        }

        private static void ConfigureAppDomainOptimizations()
        {
            // アプリドメインの最適化設定
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                // アセンブリ読み込み時のカスタムロジック
                ConsoleUtil.Write(
                    $"アセンブリ動的読み込み: {args.LoadedAssembly.GetName().Name}"
                );
            };
        }
    }
}
