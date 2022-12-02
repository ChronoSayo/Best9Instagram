using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Best9Instagram.Droid.FileService))]
namespace Best9Instagram.Droid
{
    internal class FileService : IFileService
    {
        public string Save(byte[] data, string name)
        {
            if (MainActivity.Instance.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Android.Content.PM.Permission.Granted)
                return "";

            string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryPictures, "Best9Instagram");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filename = $"{name}.jpg";
            string localPath = Path.Combine(path, filename);
            System.IO.File.WriteAllBytes(localPath, data);

            return path;
        }
    }
}