#!/bin/bash

# sh for macOS
# install pwsh: https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-macos?view=powershell-7.4

../node_modules/.bin/nswag run && pwsh ./refresh.ps1
