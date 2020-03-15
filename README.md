![Rundeck](https://cdn.worldvectorlogo.com/logos/rundeck.svg "Rundeck")
# Rundeck Tools

There are many ways to run the tool.  Either:
1.  configure appsettings.json in full, and run with no command line arguments;
2.  configure appsettings.json just with credentials and provide other arguments at the command line
3.  provide all configuration, including credentials at the command line.
---
## Configure appsettings.json in full

### Example 1: Create a new project

appsettings.json:

    {
        "Configuration": {
            "RundeckCredentials": {
                "Uri": "http://rundeck.panoramicdata.com/",
                "ApiToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            },
            "Class": "Project",
            "Action": "Create",
            "Object": "{\"name\": \"NewOne\"}"
        }
    }

Run:

    ./Rundeck.Tools.exe

### Example 2: List a project's resources

appsettings.json:

    {
        "Configuration": {
            "RundeckCredentials": {
                "Uri": "http://rundeck.panoramicdata.com/",
                "ApiToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            },
            "Class": "ProjectResource",
            "Action": "List",
            "Parent": "NewOne"
        }
    }

Run:

    ./Rundeck.Tools.exe

### Example 3: Delete a project

appsettings.json:

    {
        "Configuration": {
            "RundeckCredentials": {
                "Uri": "http://rundeck.panoramicdata.com/",
                "ApiToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            },
            "Class": "Project",
            "Action": "Delete",
            "Parent": "NewOne"
        }
    }

Run:

    ./Rundeck.Tools.exe

---
## Configure credentials in appsettings.json

### Example 4: List projects

appsettings.json:

    {
        "Configuration": {
            "RundeckCredentials": {
                "Uri": "http://rundeck.panoramicdata.com/",
                "ApiToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            }
        }
    }

Run:

    ./Rundeck.Tools.exe --Configuration:Action=List --Configuration:Action=Project

---
## Configure at the command line

### Example 5: List projects

Run:

    ./Rundeck.Tools.exe --Configuration:RundeckCredentials:Uri="http://rundeck.panoramicdata.com/" --Configuration:RundeckCredentials:ApiToken="xxxxxxxxxxxxxxxxxxxxxxxxxxxx" --Configuration:Action=List --Configuration:Action=Project

