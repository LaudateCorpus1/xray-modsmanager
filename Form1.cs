using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XRayModsManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            string[] filePaths = Directory.GetDirectories("mods/");
            for(var i = 0; i < filePaths.Length; i++)
            {
                var sp = filePaths[i].Split('/');
                modsList.Items.Add(sp[sp.Length - 1]);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/kotleni/xray-modsmanager");
        }

        private void start_Click(object sender, EventArgs e)
        {
            LOG("Запуск игры.");

            string[] paths = new string[] {
                System.Environment.CurrentDirectory + "\\bin\\XR_3DA.exe"
            };

            for(var i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i]))
                    Process.Start(paths[i]);
            }
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        private void update_Click(object sender, EventArgs e)
        {
            DeleteDirectory("gamedata/");
            Directory.CreateDirectory("gamedata/");

            Thread thread = new Thread(updateMods);
            thread.Start();
        }

        private void LOG(string msg)
        {
            File.AppendAllText("xraymm.log", msg + "\n");

            Console.WriteLine(msg);
        }
       
        bool first = true;
        private void iterDir(string path)
        {
            if (!first) {
                var clearPath = path.Split(new string[] { "\\" }, 4, StringSplitOptions.None)[3];
                Directory.CreateDirectory("gamedata\\" + clearPath);

                LOG(" Новая папка " + clearPath);
            }

            first = false;

            LOG(" Переход в папку " + path);

            foreach(string dir in Directory.GetDirectories(path))
            {
                iterDir(dir);
            }

            foreach (string file in Directory.GetFiles(path))
            {
                var clearFilePath = file.Split(new string[] { "\\" }, 4, StringSplitOptions.None)[3];

                LOG(" Копирование файла " + clearFilePath);
                File.Copy(file, "gamedata\\" + clearFilePath);
            }
        }

        private void loadMod(string name)
        {
            first = true;

            iterDir("mods\\" +  name + "\\gamedata\\");
        }

        private void updateMods()
        {
            DateTime dt = DateTime.Now;
            LOG("[" + String.Format("{0:s}", dt) + "]");

            foreach (string mod in modsList.CheckedItems)
            {
                LOG("Установка мода " + mod);
                loadMod(mod);
            }
        }
    }
}
