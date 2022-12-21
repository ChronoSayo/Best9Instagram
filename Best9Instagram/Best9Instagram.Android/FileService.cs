using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(Best9Instagram.Droid.FileService))]
namespace Best9Instagram.Droid
{
    internal class FileService : IFileService
    {
        public string Save(byte[] data, string name)
        {
            string path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryPictures, "Best9Instagram");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string filename = $"{name}.jpg";
            string localPath = Path.Combine(path, filename);
            File.WriteAllBytes(localPath, data);

            return path;
        }
    }
}