# FF1 Randomizer

This is a randomizer for Final Fantasy 1 for the NES. It currently operates on the US version of the ROM only.

## Project Structure

The general project structure is as follows:

- UI: FF1Blazorizer
- Actual logic: FF1Lib
- Desktop Application (Obsolete): FF1Randomizer
- Command line FFR: FF1R
- Shared Desktop and Command line stuff: FFR.Common
- deployment scripts: .circleci




## Setting up a Development Environment

1. [Download and install Visual Studio](https://visualstudio.microsoft.com/)
2. In the Visual Studio Installer, select the following optional pieces:

   ASP.NET and web development
   .NET desktop development

3. Clone the randomizer (git specific information below)
4. All done!

## Git workflow

The randomizer is hosted on github and therefore some level of git knowledge is required in order to contribute. In order to help get anyone ready to contribute, this will assume no prior git knowledge. Please skip past any steps you have already done, or this entire section if you are already familiar with git and able to rebase, merge, deal with multiple repos, bisect, and handle PRs on github.

### Basic Description
#### Git
 A version control system. This will control a folder or directory for you, managing the files inside it so you can swap between versions of those files.

#### Git Terms
 - commit: A discrete piece of history in the version control system
 - branch: A distinct history of the files in the git directory. This can have a history that shares a common ancestor with another branch (the usual case) or be fully separate. Usually features/bugfixes start with a new branch off the main branch.
 - repo, repository, or remote: a remote server that facilitates distributed development through being a synchronized source of truth for the files in the git directory. There can be multiple repos.
 - rebase or merge: Methods of taking changes that are in a repo and combining them with your local work
 - pull and push: transferring files from the repo to your local stuff or the other way around.
 - clone: The action of setting up a local git directory that is a clone of a specific repo.


#### Github
A company that provides git hosting services.

#### Github Terms
   - Fork: A separate repo of a project on github, copied at a specific point in its history. You will likely have a fork of the FFR repo.
   - PR or Pull Request: A way of asking someone to merge your changes from a branch into the main branch. This can be done from a separate fork without issue.
   - Issues: This is a bug/feature system built into github, the FFR project does not make much use of these.


### Getting started

1. Make a github account.
2. Head over to [the git main page](https://git-scm.com/downloads) and install git, there will be instructions for linux and mac here too
3. Follow the instructions [here](https://docs.github.com/en/github/using-git/setting-your-username-in-git) to set your username in git
4. Follow the instructions [here](https://docs.github.com/en/github/setting-up-and-managing-your-github-user-account/setting-your-commit-email-address#setting-your-commit-email-address-in-git) to set your email that will be attached to your git commits. You may also want to follow the earlier parts of this page to set your github email.
5. Follow the instructions [here](https://docs.github.com/en/github/getting-started-with-github/set-up-git#next-steps-authenticating-with-github-from-git) to authenticate with Github.
6. Fork the FFR repo. Click on the "Fork" button at the top right, and follow any instructions.
7. Now in your forked repo, clone it. Click on the "Code" button at the top of this page and select your relevant clone type (https or ssh) and copy that url. Then in your terminal/command line, navigate to the directory where you want the FFR directory to reside in, then type: `git clone <url you copied, without the angled brackets here>`
8. Go back to the main FFR repo, [link here](https://github.com/FiendsOfTheElements/FF1Randomizer) and copy the url used to clone it. Add the main FFR repo as a remote using this command: `git remote add <some name you can remember like "main" or whatever> <the url you copied>`
9. Get yourself a drink and celebrate having setup everything you need to start contributing to FFR!

### Working on a feature
