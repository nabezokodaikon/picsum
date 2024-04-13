![AppIcon](./resource/appicon.png) 

# PicSum
This is an application for viewing image files.
It has the following features.
* Two image files can be displayed in two-page spread.
* The list display of image files can be browsed like Explorer.
* Tab operations and window operations can be operated like a general web browser.
* The address bar can be operated like explorer.
* Image files can be displayed using the following methods.
    * Frequently opened folders are displayed on the home page.
    * Image files can be starred.
    * Add arbitrary tags to image files.
### Supplement
* Browsing history, star, tag and bookmark features are only stored in this application's database.
* This application is intended only for viewing image files and does not have file editing functions.

## How to build
1. Install `.Net 8.0`.
1. Install `Visual Studio 2022`.
1. Run the `build_release.bat` file.

## How to start
Run `picsum.exe` in the `bin_release` directory created by the build.
### Command line arguments
`--home`

Open the home page at startup.

`--cleanup`

Clean up the database.

## Screenshot
![File list page](./screenshot/screenshot-001.png)
![Image view page](./screenshot/screenshot-002.png)
