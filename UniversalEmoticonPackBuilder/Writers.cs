using QIPSmileBuilder;
using System.IO;
using System;
using System.Text;
using System.Xml;
using System.Linq;

class QipSmileConfig : SmileConfig
{
    private StreamWriter defineFile;
    private int counter = 0;

    public QipSmileConfig(string path, PackInfo pack)
        : base(pack, "QIP")
    {
        this.path = path + String.Format("{0}/", PackFullName);
        Directory.CreateDirectory(this.path);

        defineFile = new StreamWriter(this.path + "_define.ini", false, Encoding.GetEncoding("windows-1251"));
        defineFile.AutoFlush = true;
    }

    protected override void WriteEntry(string file, string[] equivs)
    {
        defineFile.WriteLine(String.Join(",", equivs));
        counter++;
    }

    protected override void EndFile()
    {
        defineFile.Flush();
        var w = new StreamWriter(this.path + "_define_vis.ini", false, Encoding.GetEncoding("windows-1251"));
        w.Write("1-{0}", counter + 1);
        w.Flush();
    }

    protected override string RenameFile(string fname)
    {
        var ext = fname.Split('.').Last();
        return toBase26(counter, 2) + "." + ext;
    }

    static string toBase26(int x, int digits)
    {
        string base26Characters = "abcdefghijklmnopqrstuvwxyz";

        char[] result = new char[digits];
        for (int i = digits - 1; i >= 0; --i)
        {
            result[i] = base26Characters[x % base26Characters.Length];
            x /= base26Characters.Length;
        }
        return new string(result);
    }
}

class PidginSmileConfig : SmileConfig
{
    private StreamWriter pidginTheme;

    public PidginSmileConfig(string path, PackInfo pack)
        : base(pack, "Pidgin")
    {
        this.path = path + String.Format("{0}/{1}/", PackFullName);
        Directory.CreateDirectory(this.path);

        pidginTheme = new StreamWriter(this.path + "theme", false, Encoding.GetEncoding("windows-1251"));
        pidginTheme.AutoFlush = true;
        pidginTheme.WriteLine("Name={0}", pack.name);
        pidginTheme.WriteLine("Description={0}", pack.Description);
        pidginTheme.WriteLine("Icon=ff.gif");
        pidginTheme.WriteLine("Author={0}", pack.author);
        pidginTheme.WriteLine("[default]");
    }

    protected override void WriteEntry(string file, string[] equivs)
    {
        pidginTheme.WriteLine("{0}\t{1}", file, String.Join("\t", equivs));
    }

    protected override void EndFile()
    {
        pidginTheme.Flush();
    }
}

class AdiumSmileConfig : SmileConfig
{
    private XmlTextWriter plistWriter;

    public AdiumSmileConfig(string path, PackInfo pack)
        : base(pack, "Adium")
    {
        this.path = path + String.Format("{0} {1} by {2} for {3}/{0}.AdiumEmoticonSet/", pack.name, pack.version, pack.author, client);
        Directory.CreateDirectory(this.path);

        plistWriter = new XmlTextWriter(this.path + "Emoticons.plist", Encoding.UTF8);

        plistWriter.Formatting = Formatting.Indented;
        plistWriter.WriteStartDocument();
        plistWriter.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);

        plistWriter.WriteStartElement("plist");
        plistWriter.WriteAttributeString("version", "1.0");
        plistWriter.WriteStartElement("dict");
        plistWriter.WriteElementString("key", "AdiumSetVersion");
        plistWriter.WriteElementString("integer", "1");
        plistWriter.WriteElementString("key", "Emoticons");
        plistWriter.WriteStartElement("dict");
    }

    protected override void WriteEntry(string file, string[] equivs)
    {
        plistWriter.WriteElementString("key", file);
        plistWriter.WriteStartElement("dict");
        plistWriter.WriteElementString("key", "Equivalents");
        plistWriter.WriteStartElement("array");
        foreach (var eq in equivs)
            plistWriter.WriteElementString("string", eq);
        plistWriter.WriteEndElement();
        plistWriter.WriteElementString("key", "Name");
        plistWriter.WriteElementString("string", equivs[0]);
        plistWriter.WriteEndElement();
    }

    protected override void EndFile()
    {
        plistWriter.WriteEndElement();
        plistWriter.WriteEndElement();
        plistWriter.WriteEndElement();
        plistWriter.WriteEndDocument();
        plistWriter.Flush();
    }
}

class MirandaSmileConfig : SmileConfig
{
    private StreamWriter mirandaTheme;

    public MirandaSmileConfig(string path, PackInfo pack)
        : base(pack, "Miranda")
    {
        this.path = path + String.Format("{0} {1} by {2} for Miranda/Animated/", pack.name, pack.version, pack.author);
        Directory.CreateDirectory(this.path);

        mirandaTheme = new StreamWriter(this.path + pack.name + ".msl", false, Encoding.GetEncoding("utf-8"));
        mirandaTheme.AutoFlush = true;
        mirandaTheme.WriteLine("Name\t=\t{0}", pack.name);
        mirandaTheme.WriteLine("Author\t=\t{0}", pack.author);
        mirandaTheme.WriteLine("Date\t=\t{0}", DateTime.Now);
        mirandaTheme.WriteLine("Version\t=\t{0}", pack.version);
    }

    protected override void WriteEntry(string file, string[] equivs)
    {
        mirandaTheme.WriteLine("Smiley\t= \"{0}\", 0, \"{1}\"", file, String.Join("\", \"", equivs));
    }

    protected override void EndFile()
    {
        mirandaTheme.Flush();
    }
}

class WIMSkin : SmileConfig
{
    private StreamWriter wimSkin;

    public WIMSkin(string path, PackInfo pack)
        : base(pack, "WIM")
    {
        this.path = path + String.Format("{0} {1} by {2} for WIM/PlurkSmilies/", pack.name, pack.version, pack.author);
        Directory.CreateDirectory(this.path);

        wimSkin = new StreamWriter(this.path + pack.name + ".lua", false, Encoding.GetEncoding("utf-8"));
        wimSkin.AutoFlush = true;
        wimSkin.WriteLine("WIM_ClassicSkin.emoticons.definitions = {");

        this.path += "Emoticons/";
        Directory.CreateDirectory(this.path);
    }

    protected override void WriteEntry(string file, string[] equivs)
    {
        wimSkin.WriteLine("[\"{0}\"] = \"Interface\\\\AddOns\\\\PlurkSmilies\\\\Emoticons\\\\{1}\",", equivs[0], file);
    }

    protected override void EndFile()
    {
        wimSkin.WriteLine("};");
        wimSkin.Flush();
    }
}