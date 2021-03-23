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
using XRayModsManager.res;

namespace XRayModsManager
{
    public partial class Form1 : Form
    {
        private FilesController ctrl;
        private Thread thread;

        public Form1()
        {
            ctrl = new FilesController();

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

        // при нажатии на ссылку
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/kotleni/xray-modsmanager");
        }

        // при нажатии на кнопку "Запустить игру"
        private void start_Click(object sender, EventArgs e)
        {
            LOG("Запуск игры.");

            string[] paths = new string[] {
                System.Environment.CurrentDirectory + "\\bin\\XR_3DA.exe",
                System.Environment.CurrentDirectory + "\\bin\\xrEngine.exe"
            };

            for(var i = 0; i < paths.Length; i++)
            {
                if (File.Exists(paths[i]))
                {
                    Process.Start(paths[i]);
                    return;
                }
            }

            MessageBox.Show("Не удалось найти файл для запуска игры!", "Ошибка");
        }

        // Удалить папку, со всем содержимым
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

        // при нажатии на кнопку "Обновить моды"
        private void update_Click(object sender, EventArgs e)
        {
            DeleteDirectory("gamedata/");
            Directory.CreateDirectory("gamedata/");

            thread = new Thread(updateMods);
            thread.Start();
        }

        // функция логирования
        private void LOG(string msg)
        {
            File.AppendAllText("xraymm.log", msg + "\n");

            Console.WriteLine(msg);
        }
       
        bool first = true; // если первая итерация
        string currentMod = "";
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

                if (!File.Exists("gamedata\\" + clearFilePath))
                {
                    LOG(" Копирование файла " + clearFilePath);
                    File.Copy(file, "gamedata\\" + clearFilePath);

                    ctrl.addFile(currentMod, clearFilePath);
                } else
                { // если файл уже есть
                    LOG("Ошибка, файл уже есть! " + clearFilePath);

                    var modfile = ctrl.findFile(clearFilePath);

                    this.BeginInvoke((MethodInvoker) delegate {
                        MessageBox.Show("Конфликт модов, " + modfile.plugin + " и " + currentMod + ".", "Ошибка");
                    });

                    thread.Abort();
                }
            }
        }

        // загрузить мод
        private void loadMod(string name)
        {
            first = true;

            iterDir("mods\\" +  name + "\\gamedata\\");
        }

        // обновить моды
        private void updateMods()
        {
            DateTime dt = DateTime.Now;
            LOG("[" + String.Format("{0:s}", dt) + "]");

            var i = 0;
            foreach (string mod in modsList.CheckedItems)
            {
                LOG("Установка мода " + mod);
                currentMod = mod;

                loadMod(mod);
                
                this.BeginInvoke((MethodInvoker)delegate {
                    modsList.SetItemChecked(i, false);
                });

                i++;
            }

        
        }
    }
}
