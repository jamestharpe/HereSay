HereSay is a content management system (CMS) based on the open source N2 CMS framework 
(http://n2cms.com/) and extensions to that framework developed by Rollins, Inc. 
(http://www.rollins.com/).

Installation, Easy as 1..2..3:

	1. Copy HereSay to your webserver. You should have a database ready for HereSay to use as a 
        content source.

	2. Visit your website and navigate to /N2/installation/. For example: 
	   http://yoursite/N2/installation/.

	3. Follow the on-screen instructions. Here are some helpful hints:

		Welcome: This screen just tells you where you need to go next, based on your current installation
			status.

		Database connection: If you are seening an error, you will need to adjust the <connectionString>
			section of your website's web.config file.

		Database tables: This is where you can really mess things up. If you click "Create tables" you 
			will destroy any existing data. You have been warned.

		Content package: Here you can import existing content from another HereSay website, or manually
			create an empty home page and root page.

		Finishing touches: You're pretty much done - just restart and begin managing your content!

Directories, a Tour:

	Root: /

		The root directory contains all the files that make up your website. This directory includes the
		web.config file which handles the low-level, technical settings of your website.

	HereSay directory: /_hs

		The HereSay directory contains the actual HereSay installation including themes, templates, 
		addons, plug-ins, and any other widgets, gadgets, and thingamabobs used to extend HereSay.

	Themes directory: /_hs/Themes

		The Themes directory contains the themes for your website. By default, the "Boring" theme is 
		included in /_hs/Themes/Boring so that you have something to work with before creating or 
		installing a new theme.

	Parts directory: /_hs/Parts

	...

	Application Data directory: /App_Data

		By Microsoft convention, this directory is where the data for your website is stored, such as the
		content. You might not be using this directory if your database is running seperatly; if that's 
		the case then you can delete it. For more information on Microsoft's website folder structure 
		conventions, visit http://msdn.microsoft.com/en-us/library/ex526337.aspx.

	Media directory: /media

		This is the default location for uploading multimedia content such as images, videos, and flash.
		You don't have to use this folder if you don't want to. To allow uploading of content to a
		different directory or additional directories, you will need to modify the 
		/configuration/n2/edit/uploadFolders section of your websites web.config file.
	
	N2 directory: /N2

		By N2 CMS convention, this directory is where the management portion of your website is. This is 
		where you will be editing content, customizing most of your settings (except those found in the 
		web.config file in the root), and generally having a good time.

Glossary:

	AddOn: An file or set of files that extends the functionality of HereSay and/or N2.
	
	ASP.NET: Microsoft's website and web application framework, ASP.NET is the foundation of HereSay 
		and N2.

	Theme: A set of templates, images, and other assets that give your site its look and feel. Themes 
		are stored in the /_hs/themes directory.