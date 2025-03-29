using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using iesaver;

var services = new ServiceCollection();
services.AddLogging();

var serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

var inputGam = args[0];
var chitinKeyPath = args[1];
var dialogTlkPath = args[2];

var t = new GamProcessor();
await t.Process(inputGam, serviceProvider, loggerFactory, chitinKeyPath, dialogTlkPath);