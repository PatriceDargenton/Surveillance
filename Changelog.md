# Changelog

All notable changes to the Surveillance project will be documented in this file.

## [Unreleased]

## [1.11] - 2024-02-17
### Updated
- Selenium.WebDriver.4.15.0 -> 4.17.0
- Chrome driver : 119 -> 121
- AngleSharp : 1.06 -> 1.1.0

## [1.10] - 2023-11-12
### Updated
- Selenium.WebDriver.4.14.0 -> 4.15.0
- Chrome driver : 117 -> 119
- noInstallRequired : true -> false (for ChromeDriver, since version 115).

## [1.09] - 2023-11-10
### Updated
- WSAParameter generalized, and Site constructors simlified;
- ByClassName -> ByClassNameLogin, ByClassNameConnection, ByCssSelector -> ByCssSelectorConnection, CheckTextByCssSelector -> ByCssSelectorCheckText.

### Added
- ByIdSelectorCheckText, ByNameSelectorCheckText, ByClassNameSelectorCheckText;
- ByNameLogin;
- ByClassNamePW, ByCssSelectorPW;
- ByNameConnection, ByClassNameConnection, ByCssSelectorConnection;
- ByNameSelect, ByClassNameSelect, ByCssSelectorSelect;
- ByNameInlineFrame, ByClassNameInlineFrame, ByCssSelectorInlineFrame;
- ByIdCheckText, ByNameCheckText, ByClassNameCheckText.

## [1.08] - 2023-10-28
### Added
- Boolean Const.navigate: if false, do not navigate, just test ini parameters in debug mode.
- Possibility to switch to one or more iframes, before searching any web element: ByIdInlineFrame, ByNameInlineFrame, ByClassNameInlineFrame, ByCssSelectorInlineFrame.

### Updated
- Selenium.WebDriver.4.12.4 -> 4.14.0

## [1.07] - 2023-10-07
### Fixed
- AngleSharp: version 1.0.4 was not compatible with Chrome 64 bit (we always had a time out), fixed using 1.0.5.
- Selenium updated from 4.9 to 4.11.

## [1.06] - 2023-09-20
### Added
- Button to display the last report.

## [1.05] - 2023-06-27
### Added
- Sendkeys: option to specify a delay in seconds: {WAIT 10}
- Error handling for Sendkeys.

## [1.04] - 2023-06-20
### Added
- Shortcut mode added, to be able to automatically open one or more sites (but the browser must be closed beforehand).
- Passwords saved in the user's folder, in encrypted mode.
- Configuration mode via a Surveillance.ini file.

### Fixed
- Login and password erased beforehand (for some sites, we could have the login twice).

## [1.03] - 2023-06-14
### Fixed
- Installation: the .cache folder may not exist.

## [1.02] - 2023-06-08
### Added
- Control also applications (.exe) using sendkeys.
- Control without profile.
- Control via Firefox browser.

## [1.01] - 2023-06-06 First version

## [1.00] - 2023-05-24 Start of the Surveillance project