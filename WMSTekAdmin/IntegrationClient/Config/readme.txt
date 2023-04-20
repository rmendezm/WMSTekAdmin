WMSTek 30 - Configuración ambiente Web 
======================================

PREREQUISITOS
-------------
 SERVER:
  - Microsoft .NET Framework 3.5 SP1
  - IIS 6.0 o superior
 
 CLIENTE:
  - Internet Explorer 7 o superior.
  
INSTALACION
-----------
1) Ejecutar instalador WMSTek3Setup.msi

2) Dar permisos de escritura a la carpeta WMSTek3/Log al usuario IIS_WPG

3) Modificar archivo Web.config para apuntar la key 'remotingFile' a la ruta de acceso al servicio OperationService
   (ej: <add key="remotingFile" value="tcp://localhost:10002/RemotingManager" />)

4) Modificar archivo Web.config para quitar modo 'debug'
	(<compilation debug="false">)
	
5) Configurar el acceso a la base de datos WMSTek3 en WebResources\Config\databaseAdmin.config
	(ej: <data_source_name>Database=WMSTEK30_01;Server=10.12.1.116;User ID=usuarioweb;Password=usuarioweb;</data_source_name>)

				
NOTAS
-----
 - Si se utiliza Internet Explorer 8 para acceder al sitio, se debe desactivar la Vista de Compatibilidad. 
	(en menú Herramientas -> Configuración de Vista de Compatibilidad, desmarcar la opción 'Mostrar sitios de la intranet en Vista de compatibilidad')
				
