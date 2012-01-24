using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;
using System.Reflection;

namespace QIPSmileBuilder
{
    abstract class SmileConfig
    {
        protected string packRootPath;
        protected string path;
        protected string imagePath;
        protected string buildPath;

        protected PackInfo pack;
        protected string client;

        public SmileConfig(PackInfo pack, string client, string path)
        {
            this.pack = pack;
            this.client = client;
            this.buildPath = path;
            this.packRootPath = path + String.Format("{0}/", PathPackFullName);
        }

        public void CopySmiley(string orig_file, string[] equivs)
        {
            File.Copy(orig_file, imagePath + RenameFile(orig_file), true);
            WriteEntry(RenameFile(orig_file), equivs);
        }

        public void Finish()
        {
            EndFile();

            using (ZipFile zip = new ZipFile())
            {
                zip.FlattenFoldersOnExtract = false;
                zip.AddDirectory(packRootPath);
                zip.Comment = pack.Description;
                zip.Save(String.Format("{0}{1}.zip", buildPath, PathPackFullName));
            }
            Directory.CreateDirectory(packRootPath).Delete(true);
        }

        protected abstract void WriteEntry(string file, string[] equivs);
        protected abstract void EndFile();
        protected virtual string RenameFile(string fname)
        {
            return fname;
        }

        public string PackFullName
        {
            get { return String.Format("{0} for {1}", pack.FullName, client); }
        }

        public string PathPackFullName
        {
            get { return PackFullName.Replace(' ', '_').ToLower(); }
        }
    }
}
