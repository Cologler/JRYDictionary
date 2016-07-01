# JRY Dictionary

this is a dictionary tools for help me search name of thing in other language.

however, it need input record by your self.

maybe some day support share the library? :)

**import: this tools use mongodb as db (I want it can be easy to extend).**

## how look like

here is main windos:

![](https://i.imgur.com/7uQKkG9.png)

easy to create pin yin/romaji/abbr and more.

![](https://i.imgur.com/tlnEyCD.png)

contain a viewer for easy to read.

![](https://i.imgur.com/uiD8yBr.png)

and a simple editor to edit data.

![](https://i.imgur.com/UZyXRzP.png)

## how to use

### 1. create a text file

``` json
{
    "LoginAddress": "your login ip and port ",
    "LoginDatabaseName": "your login db name",
    "LoginUserName": "your user name",
    "LoginUserPassword": "your password"
}
```

### 2. modify setting file

open file `JryDictionary.exe.config`, you will find:

``` xml
<setting name="MongoDbConnectionSettingFile" serializeAs="String">
  <value>D:\Software\MyS\common\conn_mongo.json</value>
</setting>
```

replace `D:\Software\MyS\common\conn_mongo.json` to your text file path which create in step 1.

### 3. enjoy it

run `JryDictionary.exe`. 

## download

you can download release binary files in:

* [github release](https://github.com/Cologler/JRYDictionary/releases)
* [Amazon Cloud Drive](http://amzn.to/1s3GeGb)

## description

before first `-` will load as meta.

here are current available metadata:

* `FM` - try add `FM:MD` to use markdown description
* `CV:{url}` - cover
* `BG:{url}` - background
* `GL:{url}` - galleries (allow mulit)

use `BG` and `CV` to set background and cover:

![](https://i.imgur.com/f9pOtI9.png)

and use `GL` to add mulit gallery:

![](https://i.imgur.com/v6vAgX8.png)

then, write description text after `-`:

![](https://i.imgur.com/SdHbhUk.png)

### url

all url supported `http://` or `https://` or `file:///`.
try use local file like `BG: file:///C:\img.jpg`.

you can use `%OneDrive%` to use OneDrive relative directory like
`BG: file:///%OneDrive%\img.jpg`