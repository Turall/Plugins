using MyPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PluginOne
{
    public class FilmPlugin : IPlugin
    {
        public string Name { get; set; } = "Films";

        public async Task<string> Do(string info)
        {
            HttpClient httpClient = new HttpClient();
            var Getfilms = await httpClient.GetStringAsync($"http://www.omdbapi.com/?apikey=cb2c41fe&t={info}");
            dynamic film = JsonConvert.DeserializeObject(Getfilms);
            return film.Plot;
        }
    }
}
