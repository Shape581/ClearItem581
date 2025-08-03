using Life;
using Life.InventorySystem;
using Life.Network;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClearItem581
{
    public class Main : Plugin
    {
        public string directoryPath;
        public string configPath;
        public Config config;
        private Coroutine coroutine;

        public Main(IGameAPI api) : base(api) { }

        public override void OnPluginInit()
        {
            base.OnPluginInit();
            directoryPath = Path.Combine(pluginsPath, Assembly.GetExecutingAssembly().GetName().Name);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            configPath = Path.Combine(directoryPath, "config.json");
            if (!File.Exists(configPath))
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            else
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            if (coroutine == null)
                coroutine = Nova.man.StartCoroutine(Loop());
            new SChatCommand("/clearitem", "Enlève les objets au sol", "/clearitem", (player, args) =>
            {
                ClearObjects();
                player.Notify("ClearItem581", "Nettoyage déclanché.");
            }).Register();
            new SChatCommand("/stopclearitem", "Stop le nettoyage", "/stopclearitem", (player, args) =>
            {
                if (coroutine != null)
                {
                    Nova.man.StopCoroutine(coroutine);
                    player.Notify("ClearItem581", "ClearItem stoppé.", NotificationManager.Type.Success);
                }
                else
                    player.Notify("ClearItem581", "Le ClearItem est déjà stoppé.", NotificationManager.Type.Error);
            }).Register();
            new SChatCommand("/startclearitem", "Démare le nettoyage", "/startclearitem", (player, args) =>
            {
                if (coroutine == null)
                {
                    coroutine = Nova.man.StartCoroutine(Loop());
                    player.Notify("ClearItem581", "ClearItem démaré.", NotificationManager.Type.Success);
                }
                else
                    player.Notify("ClearItem581", "Le ClearItem est déja démaré.", NotificationManager.Type.Error);
            }).Register();
            Console.ForegroundColor =  ConsoleColor.Green;
            Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} initialise !");
            Console.ResetColor();
        }

        private IEnumerator Loop()
        {
            while (true)
            {
                ClearObjects();
                yield return new UnityEngine.WaitForSeconds(config.minuteLoopInterval * 60);
            }
        }

        public async void ClearObjects()
        {
            Nova.server.Players.Where(obj => obj.isSpawned).ToList().ForEach(player => player.SendText($"<color={LifeServer.COLOR_BLUE}>Tout les objets posé au sol seront supprimé dans 60sec.</color>"));
            await Task.Delay(TimeSpan.FromSeconds(60));
            foreach (var droppedItem in UnityEngine.Object.FindObjectsOfType<DroppedItem>().ToList())
                NetworkServer.Destroy(droppedItem.gameObject);
            Nova.server.Players.Where(obj => obj.isSpawned).ToList().ForEach(player => player.SendText($"<color={LifeServer.COLOR_BLUE}>Tout les objets posé au sol ont été supprimé.</color>"));
        }
    }
}
