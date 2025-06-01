using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Checkers.Services;
using Checkers.Controllers;
using Checkers.Views;
using Checkers.Models;

namespace Checkers
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1) Настроюємо DI
            var services = new ServiceCollection();

            // AI
            services.AddSingleton<IRandomProvider, RandomProvider>();
            services.AddSingleton<IBoardEvaluator, BasicBoardEvaluator>();
            services.AddSingleton<JumpSequenceExplorer>();
            services.AddSingleton<MinimaxSearcher>();
            services.AddSingleton<EasyStrategy>();
            services.AddSingleton<MediumStrategy>();
            services.AddSingleton<HardStrategy>(sp =>
                new HardStrategy(sp.GetRequiredService<MinimaxSearcher>(), 3));
            services.AddSingleton<ProStrategy>();
            services.AddSingleton<AIService>(sp =>
            {
                var dict = new Dictionary<AIService.Difficulty, IAIStrategy>
                {
                    { AIService.Difficulty.Easy,   sp.GetRequiredService<EasyStrategy>() },
                    { AIService.Difficulty.Medium, sp.GetRequiredService<MediumStrategy>() },
                    { AIService.Difficulty.Hard,   sp.GetRequiredService<HardStrategy>() },
                    { AIService.Difficulty.Pro,    sp.GetRequiredService<ProStrategy>() },
                };
                return new AIService(dict, sp.GetRequiredService<JumpSequenceExplorer>());
            });
            services.AddTransient<MoveApplier>();
            services.AddTransient<MinimaxSearcher>();

            // Game services
            services.AddSingleton<GameSaver>();
            services.AddSingleton<GameStatistics>(sp => GameStatistics.LoadFromFile("stats.dat"));
            services.AddSingleton<GameController>();

            // View
            services.AddSingleton<MainForm>();

            // 2) Створюємо провайдер
            var serviceProvider = services.BuildServiceProvider();

            // 3) Беремо форму з DI, а не new MainForm()
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
    }
}
