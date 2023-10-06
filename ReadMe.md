# Surveillance

Surveillance is a tool for monitoring websites or applications by controlling the login process (input of account and password for each web site or application).

## Table of Contents
- [Installation](#installation)
- [Usage](#usage)
- [Shortcuts](#shortcuts)
- [Limitations](#limitations)
- [Projects](#projects)

## Installation

Click on the Installation button if it's enabled (if not enabled, it means everything is already installed).

The new Selenium manager now requires the Chrome driver to be downloaded manually, see here:

https://chromedriver.chromium.org/downloads

https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/117.0.5938.88/win64/chromedriver-win64.zip

chromedriver.exe must be copied to:

```
C:\Users\[YourWindowsAccount]\.cache\selenium\chromedriver\win64\117.0.5938.88\chromedriver.exe
```

https://www.selenium.dev/blog/2023/whats-new-in-selenium-manager-with-selenium-4.11.0/

To help fix installation bugs, you can run, in the \bin\debug\selenium-manager directory:
```
selenium-manager --browser chrome --debug
selenium-manager --browser firefox --debug
selenium-manager --browser edge --debug
pause
```

## Usage

- Sites are configured in an external text file (Surveillance.ini), and when a new site is added, the password is requested and then saved in the user's folder with a minimum level of security (symmetric encryption with an internal key). It is possible to add a list of pre-filled and encrypted passwords before compilation (be careful to exclude them in the source code repository, even encrypted, you can also use another encryption internal key);
- Symmetric encryption gives a different encryption for a same text each time, which avoids revealing that two passwords would be identical on different sites, without this affecting the decryption, of course;
- Since each account is filled in with its password, the saved passwords in cookies are finaly not used, so there is no real need to use the current user's profile. The option allows checking if all sites work, even for a user who uses their machine for the first time (a temporary profile corresponds to this case).
- Here are some examples to be saved in Surveillance.ini :
```
- Site : Example1
. URL : https://www.example1.com // Simple ping

- Site : Example2
. URL : https://www.example2.com
. Certificate : Yes // The website requires a certificate (useful for filtering sites that do not require a certificate)
. Login : LOGIN
. ByIdLogin : login
. ByIdPW : password
. ByIdConnection : submit

- Site : Example3
. URL : https://www.example3.com
. ConnectionByProfile : Yes // Use Windows user profile to connect to the website
. ByIdLogin : login
. ByIdPW : password
. ByCssSelector : input[value='Connection']

- Site : Example4
. URL : https://www.example4.com
. ConnectionByProfile : Yes
. Autoconnection : Yes // Ignore error if login button not present, when user is already logged in
. ByIdLogin : Login
. ByIdPW : Password
. ByClassName : btn

- Site : Example5
. URL : https://www.example5.com
. Activation : No // Shortcut mode only
. ConnectionByProfile : Yes
. Autoconnection : Yes
. ByIdLogin : Login
. ByIdPW : Password
. ByClassName : btn

- Site : Example6
. URL : https://www.example6.com
. ByNameLogin : login
. ByNamePW : password
. ByIdConnection : anchorHome
. CheckTextByCssSelector : #spantitle // How to find the html element that contains the text to find
. CheckText : My text // Text to find in the web site

- Site : Example7
. URL : https://www.example7.com
. Login : Login-example7
. ByIdLogin : LOGIN
. ByIdPW : PWD
. ByIdConnection : submit
. CheckTextByCssSelector : [class*='name of the block'] // How to find the html element that contains the text to find
. CheckText : Hello // Text to find in the web site

- Site : Example8
. URL : C:\Program Files (x86)\Software\Software.exe
. Sendkeys : {TAB 2} // Press the TAB key twice
. Sendkeys : Login
. Sendkeys : {ENTER}

- Site : Example9
. URL : C:\Windows\system32\mstsc.exe
. Sendkeys : MyHost // {Server}
. Sendkeys : {ENTER}
. Sendkeys : {Password} // The password for Example9 will be asked for connection
. Sendkeys : {TAB}{ENTER}

- Site : Example10
. URL : C:\Windows\system32\mstsc.exe
. Sendkeys : {TAB} // Show options with a space:
. Sendkeys :  
. Sendkeys : x.x.x.x // Host
. Sendkeys : {TAB}
. Sendkeys : ^a // Select current Login
. Sendkeys : {DELETE} // Clean Login before
. Sendkeys : MyLogin // Login
. Sendkeys : {ENTER}
. Sendkeys : {WAIT 5} // Wait 5 seconds
. Sendkeys : {Password}
. Sendkeys : {ENTER}

- Site : Example11
. URL : C:\Users\[User]\Desktop\MyConnection.rdp
. Sendkeys : {WAIT 10} // Wait 10 seconds
. Sendkeys : {Password}
. Sendkeys : {ENTER} // Leave two blank lines at the end

- Site : Example12
. URL : https://www.example12.com // Simple ping with a login input html element to find in the web site
. CheckTextByCssSelector : input[name='username']
. CheckTextName : Username // For information only: name to display in the report if the input box is found

```
- Here is [the list of hotkey codes available for SendKeys](https://learn.microsoft.com/fr-fr/dotnet/api/system.windows.forms.sendkeys.send) (like {ENTER}, {TAB}, ...).

## Shortcuts

- If one or more command-line arguments are passed (the site name, or the names of sites separated by a space), the Surveillance application automatically opens the corresponding site(s) and logs in. However, for now, the browser (only Chrome, using the user profile) must be closed beforehand.

## Limitations

- Input dialogs such as JavaScript alerts are not yet supported, but as long as the browser is not closed, the user of Surveillance can still enter his username and password and continue browsing the site (even in pilot mode);
- The MS-Edge browser works normally but only with a temporary profile, by design;
- AngleSharp: not compatible with Chrome 64 bit? (we always get a time out)

## Projects

- Shortcut Mode: Control of an already opened browser (for now, the browser must be closed beforehand);
- Control of sites with a connection via a JavaScript alert dialog box.