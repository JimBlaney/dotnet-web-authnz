# dotnet-web-authnz

An ASP.NET module library that provides an extensible interface for integration of various authentication and authorization integration providers.

This module was meant to be used within the [ArcGIS Web Adaptor](http://server.arcgis.com/en/server/latest/install/windows/about-the-arcgis-web-adaptor.htm) (IIS), in order to provide a custom principal to the Web GIS platform ([ArcGIS for Server](http://server.arcgis.com/en/server/) and [Portal for ArcGIS](http://server.arcgis.com/en/portal/)) in cases where standard configurations are not possible.

## Dependencies

* ArcGIS Web Adaptor (IIS)
* Microsoft .NET Framework 3.5+

## Installation

1. [Install](http://server.arcgis.com/en/server/latest/install/windows/about-the-arcgis-web-adaptor.htm) and [configure](http://server.arcgis.com/en/server/latest/install/windows/configure-arcgis-web-adaptor-after-installation.htm) the ArcGIS Web Adaptor (IIS)
1. Create a folder named <tt>bin</tt> within the web adaptor folder (typically <tt>c:\inetpub\wwwroot\arcgis</tt>)
1. Build the solution
1. Place all of the binaries from the output folder into the <tt>bin</tt> folder

## Configuration

### Web.config

Modify the <tt>web.config</tt> file (within the Web Adaptor folder) to resemble the following (add content, do not remove existing configuration items):

```xml
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="Esri.Services.WebAuthnz" 
             type="Esri.Services.WebAuthnz.Config.AuthnzConfigSection, Esri.Services.WebAuthnz,Version=1.0.0.0,Culture=neutral,PublicKeyToken=5357ddc79ef593b5"  requirePermisson="false" />
    <section name="log4net" 
             type="log4net.Config.Log4NetConfigurationSectionHandler, log4net,Version=2.0.8.0,Culture=neutral,PublicKeyToken=669e0ddf0bb1aa2a" requirePermisson="false" />
  </configSections>
  
  <Esri.Services.WebAuthnz requireHTTPS="true" 
                           providerType="Esri.Services.WebAuthnz.Providers.Impl.CommonNameIdentityProvider, Esri.Services.WebAuthnz,Version=1.0.0.0,Culture=neutral,PublicKeyToken=5357ddc79ef593b5"
                           setPrincipal="true">
                           <!-- setPrincipal should be true for Portal and UNFEDERATED ArcGIS Servers only -->
    <accessControl>
      <!-- 
          see documentation below
      -->
    </accessControl>

    <!-- IP addresses to allow without client certificate -->
    <whitelistedIPs>
      <add key="localhost" value="127.0.0.1" />
    </whitelistedIPs>
    
    <!-- properties are provider-specific -->
    <providerSettings>
      <!--
      <add key="property1" value="value1" />
      <add key="property2" value="value2" />
      <add key="propertyn" value="valuen" />
      -->
    </providerSettings>
  </Esri.Services.WebAuthnz>
  
  <log4net>
    <!-- 
        your log4net configuration here
        see https://logging.apache.org/log4net/release/manual/configuration.html for examples
    -->
    <appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
      <layout type="log4net.Layout.PatternLayout">
        <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
      </layout>
    </appender>
    <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
      <file value="d:\logs\web_adaptor" />
      <appendToFile value="true" />
      <rollingStyle value="Composite" />
      <datePattern value="yyyyMMdd" />
      <maxSizeRollBackups value="-1" />
      <countDirection value="1" />
      <maximumFileSize value="1MB" />
      <layout type="log4net.Layout.PatternLayout">
          <conversionPattern value="%date [%thread] %-5level %logger [%property{NDC}] - %message%newline" />
      </layout>
    </appender>
    <root>
      <level value="INFO" />
      <appender-ref ref="ConsoleAppender" />
      <appender-ref ref="RollingFileAppender" />
    </root>
  </log4net>
  
  <system.web>
    <httpModules>
      <!-- The other Esri authentication module should come first -->
      <add name="EsriWebAuthnzModule" 
           type="Esri.Services.WebAuthnz.Modules.EsriWebAuthnzModule, Esri.Services.WebAuthnz,Version=1.0.0.0,Culture=neutral,PublicKeyToken=5357ddc79ef593b5"/>
    </httpModules>
  </system.web>
  
  <system.webServer>
    <!-- add this tag if it does not already exist (it should) -->
    <validation validateIntegratedModeConfiguration="false"/>
    <modules>
      <!-- The other Esri authentication module should come first -->
      <add name="EsriWebAuthnzModule-integrated" 
           type="Esri.Services.WebAuthnz.Modules.EsriWebAuthnzModule, Esri.Services.WebAuthnz,Version=1.0.0.0,Culture=neutral,PublicKeyToken=5357ddc79ef593b5"/>
    </modules>
  </system.webServer>
</configuration>
```

### WebAdaptor.config

Modify the <tt>webadaptor.config</tt> file (found in the web adaptor folder (typically <tt>c:\inetpub\wwwroot\arcgis</tt>).

* Locate the <tt>EnableGetRolesForUser</tt> property and change the value to false.

```
<EnableGetRolesForUser>false</EnableGetRolesForUser>
```

### Access Control

The <tt>EsriWebIdentityProvider</tt> concrete implementation produces an instance of <tt>EsriWebIdentity</tt>, which contains a <tt>Dictionary&lt;string, string[]&gt;</tt> of user attributes that are used to determine if the user has the authorization required to view the requested resource. At this time, the module can only be configured to act globally on an ASP.NET web application -- custom authorization ruels for separate <tt>location</tt> directives are not supported.

#### Defining Rules in Web.config

There are three nodes permitted in the <tt>&lt;accessControl&gt;</tt> element:

* <tt>and</tt> (Conjunction) -- all child element logic must evaluate to <tt>true</tt> in order to grant access.
* <tt>or</tt> (Disjunction) -- any child element logic must evaluate to <tt>true</tt> in order to grant access.
* <tt>prop</tt> (Proposition) -- the property <u>must exist</u> and <u>contain the value</u> within the <tt>EsriWebIdentity</tt> in order to grant access. If the property does not exist, this node will <u>always</u> evaluate to <tt>false</tt>. Comparison is <u>case-sensitive</u>.
  * <tt>@name</tt> -- The name of the property to evaluate.
  * <tt>@value</tt> -- The value of the property to evaluate. The dictionary of properties may contain multiple values for a given key.
  
All three nodes have a <tt>@negate</tt> attribute (default value: <tt>false</tt>), which will invert the logical output of the node's boolean evaluation. 

The <tt>accessControl</tt> node behaves as a conjunction.

#### Examples

The following example illustrates a universal pass-through for all users.

```
<accessControl />
```

In this example, access is granted to users whose attributes include 'ORIOLES' or 'RAVENS' in a property named 'team', excluding any users who have 'NATIONALS' in the same property.

```
<accessControl>
  <or>
    <prop name="team" value="ORIOLES" />
    <prop name="team" value="RAVENS" />
  </or>
  <prop name="team" value="NATIONALS" negate="true" />
</accessControl>
```

## Other Configuration

### Configuring IIS to supply a list of allowable client certificate-issuing CAs

Use the command prompt as an Administrator for the steps listed in tis section.

#### Configure the IIS binding to use the correct certificate store

```
> netsh show sslcert ipport=0.0.0.0:443

SSL Certificate bindings:
-------------------------

    IP:port                      : 0.0.0.0:443
    Certificate Hash             : CERTIFICATE_HASH
    Application ID               : {4dc3e181-e14b-4a21-b022-59fc669b0914}
    Certificate Store Name       : My
    Verify Client Certificate Revocation : Enabled
    Verify Revocation Using Cached Client Certificate Only : Disabled
    Usage Check                  : Enabled
    Revocation Freshness Time    : 0
    URL Retrieval Timeout        : 0
    Ctl Identifier               : (null)
    Ctl Store Name               : (null)
    DS Mapper Usage              : Enabled
    Negotiate Client Certificate : Disabled
    Reject Connections           : Disabled
```

Make note of the certificate hash in the resposne above (it will be specific to each implementation).

```
> netsh http delete sslcert ipport=0.0.0.0:443

SSL Certificate successfully deleted
```

```
> netsh http add sslcert ipport=0.0.0.0:443 ^
                         certhash=CERTIFICATE_HASH ^
                         appid={4dc3e181-e14b-4a21-b022-59fc669b0914} ^
                         certstorename=My ^
                         sslctlstorename=ClientAuthIssuer ^
                         clientcertnegotiation=enable

SSL Certificate successfully added
```

#### Configure the system to provide the list of certificates from the configured store

```
reg add HKLM\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL ^
        /v SendTrusterIssuerList ^
        /t REG_DWORD ^
        /d 0 ^
        /f
```

#### Add CA certificates to the configured store

```
cd path\to\directory\containing\CA\certs\
for %f in (*.cer) do (
  certutil -enterprise -addstore -F ClientAuthIssuer %f
)
```

NOTE: if using in a <tt>.bat</tt> file, <tt>%f</tt> should be replaced with <tt>%%f</tt> for all occurrences.