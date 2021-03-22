sailingwiththegods
===================

[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](LICENSE) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/ba9a42007847465d8bb80df93ad3dd77)](https://app.codacy.com/manual/kddressel/sailingwiththegods?utm_source=github.com&utm_medium=referral&utm_content=kddressel/sailingwiththegods&utm_campaign=Badge_Grade_Dashboard) ![CI](https://github.com/kddressel/sailingwiththegods/workflows/CI/badge.svg?branch=develop)

![](docs/images/screenshot.png)

This game is designed to:

* maximize immersion in the realities of sailing the ancient sea, closing the gap between our modern perspectives of sea travel and ancient realities
* generate data recorded from players’ behavioral patterns when making choices about how to move about the maritime networks
* integrate ancient voices from history and mythology with geospatial realities of economic resources, political trends and potential for gathering up to date knowledge

[Visit the website](https://scholarblogs.emory.edu/samothraciannetworks) for more information!

# Setup

This Build uses Unity 2019.4.9f1

While the main game code is open source, the game depends on a private repo for assets purchased from the Unity Asset Store. This should go in the ```Assets/_Proprietary``` folder. Access to the proprietary repo is limited, but if it is missing the project will populate with open source fallback assets upon loading for the first time in Unity.

## Git Setup for Windows

* Install [Git for Windows](https://git-scm.com/download/win) using **default settings**
  * Make sure you pick **"Git Credential Manager Core"**
* Check if you have / need access to the proprietary repo
* Use https athentication steps below to clone your fork

## Clone the project using https authentication

```
git clone yourforkurl --branch develop
git remote add upstream https://github.com/kddressel/sailingwiththegods.git

# and if you have access to the proprietary repo...
git submodule update --init
```

* After running git submodule update --init, you should get a github login popup
* Choose "Sign in with your browser"
* Authorize GitCredentialManager to your github account
* Wait for a while. The submodule is large and may not appear to be downloading immediately

Most of the team is using https, but if you would like to use SSH authentication, follow [this guide](docs/ssh-auth.md).

## Unzip the Navmesh

The Main Scene has a NavMesh that's over 100 MB so we have it committed to git as a ZIP file and the actual NavMesh asset is gitignored. After pulling for the first time, you need to manually unzip:

```Assets/_Scenes/Main Scene/NavMesh.zip```

# Documentation

* [Coding Conventions](docs/coding-convention.md)
* [UI System](docs/ui-system.md)
