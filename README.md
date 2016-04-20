# dotnet-web-authnz

An ASP.NET module library that provides an extensible interface for integration of various authentication and authorization integration providers.

This module was meant to be used within the [ArcGIS Web Adaptor](http://server.arcgis.com/en/server/latest/install/windows/about-the-arcgis-web-adaptor.htm) (IIS), in order to provide a custom principal to the Web GIS platform ([ArcGIS for Server](http://server.arcgis.com/en/server/) and [Portal for ArcGIS](http://server.arcgis.com/en/portal/)) in caases where standard configurations are not possible.

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

Modify the <tt>web.config</tt> file resemble the following:

```xml
<?xml version="1.0"?>
<configuration>
  <configSections>
    <section name="Esri.Services.WebAuthnz" 
             type="Esri.Services.WebAuthnz.Config.AuthnzConfigSection, Esri.Services.WebAuthnz" />
    <section name="log4net" 
             type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
  </configSections>
  
  <Esri.Services.WebAuthnz requireHTTPS="false" 
                           providerType="Esri.Services.WebAuthnz.Providers.Impl.MyProvider, Esri.Services.WebAuthnz">
    <accessControl>
      <!-- 
          see documentation below
      -->
    </accessControl>
  </Esri.Services.WebAuthnz>
  
  <log4net>
    <!-- 
        your log4net configuration here
        see https://logging.apache.org/log4net/release/manual/configuration.html for examples
    -->
  </log4net>
  
  <system.web>
    <httpModules>
      <!-- Esri's authentication module should come first -->
      <add name="EsriWebAuthnzModule" 
           type="Esri.Services.WebAuthnz.Modules.EsriWebAuthnzModule, Esri.Services.WebAuthnz"/>
    </httpModules>
  </system.web>
  
  <system.webServer>
    <validation validateIntegratedModeConfiguration="false"/>
    <modules>
      <!-- Esri's authentication module should come first -->
      <add name="EsriWebAuthnzModule-integrated" 
           type="Esri.Services.WebAuthnz.Modules.EsriWebAuthnzModule, Esri.Services.WebAuthnz"/>
    </modules>
  </system.webServer>
</configuration>
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

```xml
<accessControl />
```

In this example, access is granted to users whose attributes include 'ORIOLES' or 'RAVENS' in a property named 'team', excluding any users who have 'NATIONALS' in the same property.

```xml
<accessControl>
  <or>
    <prop name="team" value="ORIOLES" />
    <prop name="team" value="RAVENS" />
  </or>
  <prop name="team" value="NATIONALS" negate="true" />
</accessControl>
```
