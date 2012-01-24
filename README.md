# UniversalEmoticonPackBuilder
UniversalEmoticonPackBuilder is a tool to build an emoticon packs for multiple clients

For now QIP Infium/2010, Pidgin and Adium support is complete.

## Solution projects
* UniversalEmoticonPackBuilder — console utility
* UniversalEmoticonPackBuilderLib — builder itself
* qip_buildmap — creates UniversalEmoticonPackBuilder map from qip smilepack _define.ini

## Usage
First you need to create a pack to build. Check the *examplePack/* for a simple example.

Basically you need a **mapfile* and a **build config**.

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