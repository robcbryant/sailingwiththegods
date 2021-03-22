## Using ssh authentication

```
git clone yourforkurl --branch develop
git remote add upstream git@github.com:kddressel/sailingwiththegods.git

# and if you have access to the proprietary repo...
git submodule update --init
```

Most of the team is using https, so the submodule is stored as https in the .gitmodules to ease that workflow. It's recommended to use https for the submodule and ssh for the main repo if you are using ssh, but if you want to use ssh for the submodule you can do this:

* Edit .git/config
* Point the Assets/_Proprietary submodule url at ```git@github.com:kddressel/sailingwiththegods-proprietary.git```
* cd into Assets/Proprietary
* rm -rf .git
* git init
* git remote add origin ```git@github.com:kddressel/sailingwiththegods-proprietary.git```

