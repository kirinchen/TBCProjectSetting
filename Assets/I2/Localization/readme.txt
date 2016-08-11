----------------------------------------------
              I2 Localization
                  2.1.0 f1
        http://www.inter-illusion.com
          inter.illusion@gmail.com
----------------------------------------------

Thank you for buying the I2 Localization!

Documentation can be found here: http://www.inter-illusion.com/assets/I2Localization.pdf

If you have any questions, suggestions, comments or feature requests, please
drop by the I2 forum: http://www.inter-illusion.com/forum/index

----------------------
  Installation
----------------------

1- Import the plugin package into a Unity project.
2- Enable the support for third party plugins installed on the project 
   (Menu: Tools\I2 Localization\Enable Plugins)
3- Open any of the example scenes to see how to setup and localize Components
   (Assets/I2/Localization/Examples)
4- To create your own localizations, open the prefab I2\Localization\Resources\I2Languages   
5- Create the languages you will support.
6- The I2Languages source is a global source accessible by all scenes

The documentation provides further explanaition on each of those steps and some tutorials.
Also its presented how to convert an existing NGUI localization into the I2 Localization system.

-----------------------
  Troubleshooting
-----------------------

This plugins contains several dll to access GoogleAPI funtionality. 
If some other plugin already installed those dlls into the project there will be some warnings.
That could be fixed by removing the duplicated dlls from either of the plugins


-----------------------
  Google Spreadsheets
-----------------------

Google updated the spreadsheet format and every spreadsheet created after april 1st uses 
the new format. While the code gets updated to support the new spreadsheets, only the classic
spreadsheets are supported.
To create a classic spreadsheet, open the following spreadsheet and make a copy into your 
google drive by selecting the menu "File\Make a Copy"
g.co/oldsheets

-----------------------
 Ratings
-----------------------

If you find this plugin to be useful and want to recommend others to use it. 
Please leave a review or rate it on the Asset Store. 
That will help with the sales and allow me to invest more time improving the plugin!!


-----------------------
 Version History
-----------------------
2.2.0
- NEW: Added support for TextMeshPro
- NEW: Terms can now have category and subcategories (e.g. Tutorials/Tutorial1/Startup/Title)
- FIX: NGUI is now detected by looking for the NGUIDebug class instead of UIPanel

2.1.0 f1
- NEW: After importing CSV or Google Spreadsheets, the category filter is set to show every term
- NEW: Terms list is now fully expanded on the Language Source
- NEW: Localize Component now has an Option to convert to (Upper, Lower, DontModify) the translations
- FIX: Validations for when importing Spreadsheets with empty columns/languages

2.1.0 b3
- NEW: The plugin is now compatible with Unity 5 (up to alpha 11)
- NEW: Register a function in the event LocalizationManager.OnLocalizeEvent to get called when the language changes
- FIX: Updated the example scenes to use the new Language Sources
- FIX: Terms are now saved correctly after importing a CSV or a Google Spreadsheet
- FIX: Allowed methods with one argument to be used as Localization CallBacks
- FIX: SelectNoLocalizedLabels was running every frame after executed
- DEL: Removed button to select CSV file. Now the Import and Export buttons display the open/save dialog

2.1.0 b2
- FIX: W8P and Metro compatibility
- FIX: Compiler warnings

2.1.0 b1
- NEW: Terms database is now saved within the LanguageSource and not a separated Language Files
- NEW: The selected language is now saved to the PlayerPrefs into "I2 Language"
- NEW: On the Localize Component, creating a key shows a list of terms as you type and their usage
- NEW: On the Localize Component, when changing the translation of a term shows a preview in the target (label/etc)
- NEW: When selecting a Term in the Localize Component, the list can be filtered with the Create Term string
- NEW: On the Localize Component, the Terms List is now sorted Case Insensitive
- NEW: The auto-enable Plugins will set the Script Define Symbols for ALL platforms (IOS,Android,Web,etc)
- NEW: In the Localize Component, the textField thats used for create a key now has a clear button to easy editing
- NEW: If a term is not found when localizing an object the object is left untouched (Previously labels got empty)
- NEW: There is now a button in the Localize Component to quickly rename a Term in the current scene
- DEL: Removing the Editor Databased used to cache the Language Files because all the info is now in the LanguageSource
- FIX: Selecting the CSV file to export will now allow you to create a new file
- FIX: Added a message to explain when exporting fails because the file is Read-Only or its open in other program
- FIX: When exporting to a file inside the project, the "Assets/" section was been skipped
- FIX: Import and Export CSV files now also works on when the editor is set to Web Player
- FIX: Exporting CSV now uses UTF8 encoding to keep special characters
- FIX: The "Open Source" button on the Localize Component now selects the Primary or Secondary term based on the selected tab
- FIX: Terms are now trimmed because spaces at the end/beggining can lead to confusions
- FIX: The list of terms was not showing correctly when selecting MISSING but unselecting USED

2.0.3 f1
- NEW: Support for localizing 2D-ToolKit (TextMeshes and Sprites)

2.0.3 b2
- NEW: When more than one localization type is available, the plugin allows you to select which component to localize
- FIX: When localizing secondary elements (Atlas, Fonts) the system checks that they still exist to avoid null exceptions
- FIX: Localization of Prefab now have the lowest priority to easy localizing labels/sprites with childs

2.0.3 b1
- NEW: The plugin will now check and enable by default all Plugins included in the project (NGUI,DFGUI,UGUI)
- NEW: Global Localization Source (I2Languages) its now empty by default to make it easy to start a new project
- FIX: Moved the Terms used in each example scene to a new Language Source inside each scene
- DEL: Removed Resources.UnloadAsset when changing the localization to avoid unloading referenced assets

2.0.2 f1
- NEW: UIFonts fonts can now be localized on NGUI
- FIX: Some example scenes were corrupted
- FIX: Modified the plugin to be compatible with Unity5

2.0.1 f1
- NEW: When an object is set as a translation, the object is also added automatically to the Reference array
- FIX: Importing from Google Spreadsheets will not longer generate 'Description' as a language
- FIX: The editor will show a message if exporting to Google fails
- FIX: The variable is IsLeft2Right was renamed as IsRight2Left to match its behavior
- FIX: Importing Google Spreadsheets no longer duplicate the languages
- FIX: Importing CSV was skipping some languages and not parsing terms after import
- FIX: Converted encoded translations into its ASCII characters ("Il s\x26#39;agit" -> "Il s'agit")
- FIX: Terms Section in the Localize custom editor can be collapsed
- FIX: Localize custom editor becomes more compact and easy to read when several sections are collapsed
- FIX: Expanded Terms in the Terms Tab of the LanguageSource will display an Arrow to make evident that they can be collapsed
- FIX: Terms description is now collapsed automatically when another term is selected
- FIX: The spreadsheet was been opened in the browser even if the Open Spreadsheet after Export flag was disabled

2.0.0 a2
- NEW: Support for languagges using Right To Left (RTL) with correct rendering for Arabic languages.
- NEW: Added a toggle on the Localize component to allow discarding RTL processing for selected objects.
- NEW: Languages can now have a Language code to allow for Language Regions (e.g. English Canada vs English United States)
- NEW: Automatic Translation using Google Services will use the language code instead of the Language Name
- NEW: CSV and Google Spreadsheets will save the language code if needed
- FIX: When adding a language to a source the editor will not switch to the Terms tab. That to allows adding several languages at once.
- FIX: Menu options was moved from "Menu > Assets > I2 Localization" to "Tools > I2 Localization"
- FIX: Localization Manager will not allow changing to a language that doesn't exist

2.0.0 a1
- NEW: Support for Daikon Forge GUI components
- NEW: Support for uGUI as of the Unity 4.6 beta 2 (this is only available for users in the beta test group)
- NEW: Terms can now have a type (Text, Object, Audio, Font, Sprite)
- NEW: Terms can be set to generate the ScriptLocalization.cs for Compile-Time-Checking of used Terms.
- FIX: Changed the Terms preview based on the Term Type
- FIX: Language Sources can now be in the Resources folder, the scene or bundled
- FIX: Component Localize allows to change the target for localizing more than one component in one GameObject

1.8.0
- NEW: Callbacks can be setup on the editor for correct concatenation according to the language 
- NEW: Event system for callbacks with reflection
- NEW: Moved all localization calls into events for localizing more types of components without much code change
- FIX: Moved NGUI and UnityStandard localization code into separated files to minimize dependencies

1.7.0
- NEW: Localize component has now Primary and Secondary Terms
- NEW: Secondary term allows localizing Fonts on Labels
- NEW: Secondary term allows localizing Atlas on Sprites
- NEW: Support for localizing Prefabs
- NEW: Support for localizing GUITexture

1.6.0
- NEW: Added separated components to localize labels and sprites to remove the dependency with the NGUI localization
- NEW: Support for localizing Audio Clips
- NEW: Support for localizing GUIText
- NEW: Support for localizing TextMesh

1.4.0
- NEW: The filter on the Terms list can now have multiple values (e.g. "Tuto;Beg" will show only the terms containing "Tuto" or "Beg"
- NEW: Added References to the UILocalize component to be able of store not only text but also objects
- FIX: UILocalize will now show the Localization source it references

1.3.0
- NEW: Languages can now be moved up and down to organize them
- NEW: Allowed to filter by category on the Terms list
- FIX: First language in the list becomes now the starting/default language

1.2.0
- NEW: Merged Import and Export tabs to allow for external data sources that could be synchronized
- NEW: Ability to categorize Terms to improve organization (e.g. Tutorial, Main, Game Screen, etc)
- NEW: Each term category exports into a separated sheet when linking to Google Spreadsheets
- NEW: Parsing scenes for changing the category on selected terms

1.0.2
- FIX: Improved performance on the inspector by removing unneeded Layout functions
- FIX: General Code Cleanup

1.0.1 
- NEW: Custom Editors now allow Undo the changes on the keys and startingLanguage
- FIX: Removed testing Log calls

1.0.0 f2
- FIX: Parsing scenes was executed several times in a row or not at all.
- FIX: Importing CSV will now parse the current scene to show Key Usages
- FIX: A message is shown when Selecting All No Localized labels in scene, if there are none
- FIX: Clicking on the usage number of unused keys will not try to select them
- FIX: Merging Keys will save scenes to avoid loosing changes
- FIX: Sometimes exporting without saving made changes to be lost. Now it automatically saves data if needed.


1.0.0 f1
- NEW: The language TextAsset will be shown in the Language list instead of just the name. That allows finding the asset, moving it to another folder, etc
- NEW: Languages can now be also added by dragging a TextAsset into the Add Language bars.
- NEW: Keys that are are missing the translation in any of the languages are highlighted in the Keys List by making them Italic and Darker
- DEL: Removed button Update NGUI in the Key list. All data will be saved automatically when the inspector view changes to another object or the editor is closed
- FIX: Filter for list of keys now is case insensitive.
- FIX: Auto opening google Spreadsheet after export was opening two web pages.
- FIX: Deleting a language will not only unlink the TextAsset from NGUI but will also delete the text file.
- FIX: If a TextAsset is manually deleted, but NGUI still keeps a reference in the language list, that language is now skipped
- FIX: Removed compile warnings when in WebPlayer platform
- FIX: Removed exception when adding keys before creating a language
- FIX: Adding multiple keys to NGUI was only adding the first one and returning an exception

1.0.0 b2
- NEW: Added a TextField to filter the list of keys.
- NEW: Option to auto open the Google Spreadsheet doc after exporting.
- NEW: Added a centralized Error reporting.
- NEW: Option to save or not the google password.
- NEW: Added a menu option to quickly access the help.  (Help\I2 Localization For NGUI).
- NEW: Key list show a warning icon on the keys that are used in the scenes but are not in the NGUI files.
- FIX: An error will show when contacting Google Translation on the WebPlayer Platform as its not yet supported.
- FIX: Google public spreadsheet Key is now remembered when the editor opens.

1.0.0 b1
- NEW: First Version including core features.
