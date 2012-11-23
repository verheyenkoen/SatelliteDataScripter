#Satellite Data Scripter

A tool for generating INSERT/UPDATE scripts for satellite data. Only supports MS Sql Server.

### Remarks

Only tables with an _ID_ column can be scripted.

### Requires

.NET Client Profile 3.5

### Configuration

Add your Sql Server connection strings to SatelliteDataScripter.exe.config, like this:

    <?xml version="1.0"?>
    <configuration>
      <connectionStrings>
        <clear/>
        <add name="Connection name" connectionString="Data Source={server};Initial Catalog={database};Integrated Security=SSPI;"/>
      </connectionStrings>
    </configuration>

### Development Notes

Uses project [SlowCheetah](http://visualstudiogallery.msdn.microsoft.com/69023d00-a4f9-4a34-a6cd-7e854ba318b5) for app.config transformations. Transformation files should not be commited.