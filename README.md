# JRY Dictionary

this is a dictionary tools for help me search name of thing in other language.

however, it need input record by your self.

maybe some day support share the library? :)

**import: this tools use mongodb as db (I want it can be easy to extend).**

## how look like

![](https://i.imgur.com/LjAi41P.png)

and easy to create pin yin (by 1 click) for me.

![](https://i.imgur.com/S8jieGs.png)

## how to use

### 1. create a text file

``` json
{
    "LoginAddress": "127.0.0.1:50710",
    "LoginDatabaseName": "admin",
    "LoginUserName": "conanvista",
    "LoginUserPassword": "LVpMQhAt31hli8Uiq2Ir"
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