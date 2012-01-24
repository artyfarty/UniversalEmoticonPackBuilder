# UniversalEmoticonPackBuilder
UniversalEmoticonPackBuilder is a tool to build an emoticon packs for multiple clients

For now QIP Infium/2010, Pidgin and Adium support is complete.

Licence: [Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/)

## Solution projects
* UniversalEmoticonPackBuilder — console utility
* UniversalEmoticonPackBuilderLib — builder itself
* qip_buildmap — creates UniversalEmoticonPackBuilder map from qip smilepack _define.ini

## Usage
First you need to create a pack to build. Check the *examplePack/* for a simple example.

Basically you need a **mapfile** and a **build config**.

### Mapfile
Mapfile defines which codes corresponds to emoticon images

```
imagefile:code[,alternativecode[,alternativecode[,alternativecode ... ]]]
```

### Build config
Build config defines a basic metainfo of the pack and sets the name of the mapfile.

```
{
	"name":"Example",
	"version":"1.0",
	"author":"artyfarty",
	"map":"map.uemap",
	"builders":["qip","adium","pidgin"]
}
```
You can also select which builders (clients) to use.
You can look up more builders in the source or write your own.

### Building
Now place mapfile, config and images in the same dir and point the console utility on the config:

```
uepackbuild "C:/blah blah/fooPack/config.json"
```

Or add **uepackbuild** to PATH and launch it from pack dir without params.

Congrats! Now you have *example_1.0_by_artyfarty_for_adium.zip*, *example_1.0_by_artyfarty_for_pidgin.zip* and *example_1.0_by_artyfarty_for_qip.zip* in *C:/blah blah/fooPack/build/* dir, ready for distribution.