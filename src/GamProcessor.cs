using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using ii.InfinityEngine;
using ii.InfinityEngine.Readers;
using iesaver.Models;

namespace iesaver
{
    public class GamProcessor()
    {
        public async Task Process(string gamFile, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, string chitinKeyPath, string dialogTlkPath)
        {
            var game = new Game(chitinKeyPath, dialogTlkPath);
            game.LoadResources([IEFileType.Ids, IEFileType.Itm]);

            var gamReader = new GamFileBinaryReader();
            gamReader.TlkFile = game.Tlk;
            var gam = gamReader.Read(gamFile);

            await using var htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);

            var index = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var model = new IndexModel { Message = "Hello from the Render Message component!" };

                foreach (var cre in gam.PartyMembers)
                {
                    var name = cre.CreFile.ShortName.Strref == -1 ? cre.Name.ToString() : cre.CreFile.ShortName.Text;
                    Console.WriteLine(name);
                    model.PartyMembers.Add(new CreatureModel() { Name = name });
                }

                foreach (var cre in gam.NonPartyMembers)
                {
                    if (cre.CreFile.Flags.BeenInParty)
                    {
                        var name = cre.CreFile.ShortName.Strref == -1 ? cre.Name.ToString() : cre.CreFile.ShortName.Text;
                        Console.WriteLine(name);
                        model.PreviousPartyMembers.Add(new CreatureModel() { Name = name });
                    }
                }

                var dictionary = new Dictionary<string, object>
                {
                    { "Model", model }
                };

                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await htmlRenderer.RenderComponentAsync<Index>(parameters);

                return output.ToHtmlString();
            });

            File.WriteAllText("index.html", index);




            foreach (var cre in gam.PartyMembers)
            {
                var name = cre.CreFile.ShortName.Strref == -1 ? cre.Name.ToString() : cre.CreFile.ShortName.Text;
                name = name.Replace("\0", "");

                var creature = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var model = new CreatureModel();
                    model.Name = name;
                    model.CreFile = cre.CreFile;
                    model.Identifiers = game.Identifiers;
                    model.Items = game.Items;

                    var dictionary = new Dictionary<string, object>
                    {
                        { "Model", model }
                    };

                    var parameters = ParameterView.FromDictionary(dictionary);
                    var output = await htmlRenderer.RenderComponentAsync<Creature>(parameters);

                    return output.ToHtmlString();
                });

                var filename = string.Concat(name.Split(Path.GetInvalidFileNameChars()));

                File.WriteAllText($"{filename}.html", creature);
            }



            foreach (var cre in gam.NonPartyMembers)
            {
                if (!cre.CreFile.Flags.BeenInParty)
                    continue;

                var name = cre.CreFile.ShortName.Strref == -1 ? cre.Name.ToString() : cre.CreFile.ShortName.Text;
                name = name.Replace("\0", "");

                var creature = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
                {
                    var model = new CreatureModel();
                    model.Name = name;
                    model.CreFile = cre.CreFile;
                    model.Identifiers = game.Identifiers;
                    model.Items = game.Items;

                    var dictionary = new Dictionary<string, object>
                    {
                        { "Model", model }
                    };

                    var parameters = ParameterView.FromDictionary(dictionary);
                    var output = await htmlRenderer.RenderComponentAsync<Creature>(parameters);

                    return output.ToHtmlString();
                });

                var filename = string.Concat(name.Split(Path.GetInvalidFileNameChars()));

                File.WriteAllText($"{filename}.html", creature);
            }



            var journal = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var model = new JournalModel();
                model.Message = "Journal";
                model.JournalEntries = gam.JournalEntries;

                var dictionary = new Dictionary<string, object>
                {
                    { "Model", model }
                };

                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await htmlRenderer.RenderComponentAsync<Journal>(parameters);

                return output.ToHtmlString();
            });

            File.WriteAllText("journal.html", journal);
        }
    }
}