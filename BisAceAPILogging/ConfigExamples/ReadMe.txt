The API's will look for a 'logging.settings' file to configure logging. This file should be located in your configuration directory, which is specified in web.config

i.e.

<add key="SystemConfigFilesFolder" value="C:/Temp/AppConfigs" />

Will indicate that the loggings.setting file would be found in c:\temp\appconfigs

